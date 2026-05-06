using MassTransit;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.Application.Services;
using ZUMA.SharedKernel.Domain.MessagingContracts.Events;

namespace ZUMA.CommunicationService.Application.Services;

internal class EmailService : ServiceBase<EmailEntity>, IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailRepository _emailRepository;
    private readonly IEmailClient _emailClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public EmailService(
        IEmailRepository emailRepository,
        IEmailClient emailClient,
        IPublishEndpoint publishEndpoint,
        ILogger<EmailService> logger) : base(emailRepository)
    {
        _emailRepository = emailRepository;
        _emailClient = emailClient;
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
        _logger.LogInformation("Email processing triggered by event. Starting bulk send process...");

        var emailsToSend = await _emailRepository.GetPendingEmailsAsync(cancellationToken);

        foreach (var email in emailsToSend)
        {
            try
            {
                _logger.LogInformation("Processing email to {Recipient}", email.Recipient);

                await _emailClient.SendAsync(email, cancellationToken);

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