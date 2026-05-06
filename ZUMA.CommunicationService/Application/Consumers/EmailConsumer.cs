using MassTransit;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Events;

namespace ZUMA.CommunicationService.Application.Consumers;

public class EmailConsumer : BaseConsumer<CreateEmailEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailConsumer> _logger;

    public EmailConsumer(
        IEmailService emailService,
        ILogger<EmailConsumer> logger) : base(logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<CreateEmailEvent> context)
    {
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