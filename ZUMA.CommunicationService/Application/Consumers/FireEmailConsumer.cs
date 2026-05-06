using MassTransit;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Events;

namespace ZUMA.CommunicationService.Application.Consumers;

public class FireEmailConsumer : BaseConsumer<FireEmailEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<FireEmailConsumer> _logger;

    public FireEmailConsumer(
        IEmailService emailService,
        ILogger<FireEmailConsumer> logger) : base(logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    protected override async Task OnConsumeAsync(ConsumeContext<FireEmailEvent> context)
    {
        await _emailService.ProcessQueueAsync();
    }
}