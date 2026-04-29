using MassTransit;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.MessagingContracts.Events;
using ZUMA.SharedKernel.Utils;

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
        ArgumentNullException.ThrowIfNull(context);

        if (context.MessageId is null) throw new NullReferenceException(nameof(context.MessageId));

        using var scope = _logger.BeginMessageScope(messageId: context.MessageId.ToString()!,
                                                    identificationData: context.Message.Email);

        _logger.LogInformation("Email processing triggered by event. Starting bulk send process...");

        var msg = context.Message;
        var sentCount = await _emailService.CreateAsync(new EmailEntity
        {
            EmailTemplateType = msg.EmailTemplateType,
            Body = msg.Body,
            Subject = msg.Subject,
            Recipient = msg.Email
        });
    }
}