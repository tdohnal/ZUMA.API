using MailKit.Net.Smtp;
using MassTransit;
using MimeKit;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Messagges.Events;
using ZUMA.BussinessLogic.Services;
using ZUMA.CommunicationService.Repositories;

namespace ZUMA.CommunicationService.Services.Email;

internal class EmailService : ServiceBase<EmailEntity>, IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly string _templatePath;

    public EmailService(
        IEmailRepository emailRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<EmailService> logger) : base(emailRepository)
    {
        _emailRepository = emailRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    protected override async Task AfterCreateAsync(EmailEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing to RabbitMQ for ID: {id}", entity.Id);

        await _publishEndpoint.Publish<FireEmailEvent>(new
        {
            EmailId = entity.Id,
            Recipient = entity.Recipient,
            Subject = entity.Subject,
            Body = entity.Body,
        }, cancellationToken);

        _logger.LogInformation("Message was sent into queue.");
    }

    public async Task ProcessQueueAsync(CancellationToken cancellationToken = default)
    {
        var emailsToSend = await _emailRepository.GetPendingEmailsAsync(cancellationToken);

        foreach (var email in emailsToSend)
        {
            try
            {
                _logger.LogInformation("Processing email to {Recipient}", email.Recipient);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ZUMA System", "noreply@zuma.com"));
                message.To.Add(new MailboxAddress("", email.Recipient));
                message.Subject = email.Subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = email.Body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync("09e2883d96546f", "25555dae1a36f2", cancellationToken);

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation("Email sent successfully to {Recipient}", email.Recipient);

                email.Sent = DateTime.UtcNow;
                await _emailRepository.UpdateAsync(email, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", email.Recipient);
            }
        }
    }
}