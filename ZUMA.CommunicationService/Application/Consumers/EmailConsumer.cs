using MassTransit;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Events;

namespace ZUMA.CommunicationService.Application.Consumers;

public class EmailConsumer : IConsumer<CreateEmailEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailConsumer> _logger;

    public EmailConsumer(
        IEmailService emailService,
        ILogger<EmailConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateEmailEvent> context)
    {
        _logger.LogInformation("Email processing triggered by event. Starting bulk send process...");

        var msg = context.Message;

        try
        {
            var sentCount = await _emailService.CreateAsync(new EmailEntity
            {
                EmailTemplateType = msg.EmailTemplateType,
                Body = msg.Body,
                Subject = msg.Subject,
                Recipient = msg.Email
            });

            _logger.LogInformation("Bulk email processing completed. Successfully sent {Count} emails.", sentCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during bulk email processing. Some emails might not have been sent.");
            throw;
        }
    }
}