using MailKit.Net.Smtp;
using MassTransit;
using Microsoft.Extensions.Logging;
using MimeKit;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Messagges.Requests;
using ZUMA.BussinessLogic.Repositories.Email;

namespace ZUMA.BussinessLogic.Services.Email;

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

        _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
    }

    protected override async Task BeforeCreateAsync(EmailEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new Email with id: {id}", entity.InternalId);

        _logger.LogInformation("Processing template for Email ID: {id}", entity.InternalId);

        var placeholders = new Dictionary<string, string>
        {
            { "Name", entity.Recipient.FullName ?? "User" },
            { "Subject", entity.Subject },
            { "Body", entity.Body },
            { "Code", entity.Recipient.AuthCode ??  "" }
        };

        var renderedHtml = await GetRenderedTemplateAsync(entity.EmailTemplateType, placeholders);

        entity.Body = renderedHtml;
    }

    protected override async Task AfterCreateAsync(EmailEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing to RabbitMQ for ID: {id}", entity.InternalId);

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
                message.To.Add(new MailboxAddress(email.Recipient.FullName, email.Recipient.Email));
                message.Subject = email.Subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = email.Body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync("09e2883d96546f", "25555dae1a36f2", cancellationToken);

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation("Email sent successfully to {Recipient}", email.Recipient.Email);

                email.Sent = DateTime.UtcNow;
                await _emailRepository.UpdateAsync(email, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", email.Recipient.Email);
            }
        }
    }

    private async Task<string> GetRenderedTemplateAsync(EmailTemplateType templateType, Dictionary<string, string> placeholders)
    {
        string fileName = templateType switch
        {
            EmailTemplateType.RegistrationVerify => "RegistrationVerify.html",
            EmailTemplateType.Authorization => "Authorization.html",
            EmailTemplateType.WelcomeMessage => "Welcome.html",
            _ => throw new ArgumentOutOfRangeException(nameof(templateType), "Unknown template type")
        };

        string fullPath = Path.Combine(_templatePath, fileName);

        if (!File.Exists(fullPath))
        {
            _logger.LogError("Template file not found: {path}", fullPath);
            return placeholders.TryGetValue("Body", out var b) ? b : "Empty Content";
        }

        string content = await File.ReadAllTextAsync(fullPath);

        foreach (var item in placeholders)
        {
            content = content.Replace($"{{{{{item.Key}}}}}", item.Value);
        }

        return content;
    }
}