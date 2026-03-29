using MassTransit;
using ZUMA.CommunicationService.Services.Email;

namespace ZUMA.CommunicationService;

public class EmailConsumer : IConsumer/*<VerificaitonResponse>*/
{
    private readonly ILogger<EmailConsumer> _logger;
    private readonly IEmailService _emailService;

    public EmailConsumer(IEmailService emailService, ILogger<EmailConsumer> logger)
    {
        //    _logger = logger;
        //    _emailService = emailService;
    }

    public async Task Consume(ConsumeContext/*<VerificaitonResponse> */context)
    {
        //var emailData = context.Message;
        //_logger.LogInformation("Zpracovávám odeslání e-mailu pro: {Recipient}", emailData.Recipient);
        //await _emailService.ProcessQueueAsync();
    }
}