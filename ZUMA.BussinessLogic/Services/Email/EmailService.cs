using MassTransit;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net;
using System.Net.Mail;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Messagges.Requests;
using ZUMA.BussinessLogic.Repositories.Email;

namespace ZUMA.BussinessLogic.Services.Email;

internal class EmailService : ServiceBase<EmailEntity>, IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public EmailService
        (
        IEmailRepository emailRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<EmailService> logger
        ) : base(emailRepository)
    {
        _emailRepository = emailRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    protected override Task BeforeCreateAsync(EmailEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new Email with id: {id}", entity.InternalId);
        return base.BeforeCreateAsync(entity, cancellationToken);
    }

    protected override async Task AfterCreateAsync(EmailEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing to RabbitMQ for ID: {id}", entity.InternalId);

        // MUSÍ tam být await, jinak se zpráva neodešle!
        await _publishEndpoint.Publish<ISendEmailRequest>(new
        {
            EmailId = entity.InternalId,
            Recipient = entity.Recipient.Email,
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
                _logger.LogInformation("Processing email to {Recipient}", email.Recipient.Email);
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ZUMA System", "noreply@zuma.com"));
                message.To.Add(new MailboxAddress("", email.Recipient.Email));
                message.Subject = email.Subject;

                message.Body = new TextPart("html")
                {
                    Text = email.Body
                };

                using var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential("09e2883d96546f", "25555dae1a36f2"),
                    EnableSsl = true
                };
                client.Send("noreply@zuma.com", email.Recipient.Email, email.Subject, email.Body);

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
