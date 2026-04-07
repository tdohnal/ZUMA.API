using MassTransit;
using Microsoft.Extensions.Logging;
using ZUMA.CommunicationService.Services.Email;
using ZUMA.SharedKernel.Messagges.Events;

namespace ZUMA.CommunicationService.Consumers;

public class FireEmailConsumer : IConsumer<FireEmailEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<FireEmailConsumer> _logger;

    public FireEmailConsumer(
        IEmailService emailService,
        ILogger<FireEmailConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FireEmailEvent> context)
    {
        _logger.LogInformation("Email processing triggered by event. Starting bulk send process...");

        try
        {
            await _emailService.ProcessQueueAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during bulk email processing. Some emails might not have been sent.");
            throw;
        }
    }
}