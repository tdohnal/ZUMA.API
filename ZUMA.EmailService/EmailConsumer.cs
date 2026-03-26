using MassTransit;
using ZUMA.BussinessLogic.Messagges.Requests;
using ZUMA.BussinessLogic.Services.Email;

namespace ZUMA.EmailService;

public class EmailConsumer : IConsumer<ISendEmailRequest>
{
    private readonly ILogger<EmailConsumer> _logger;
    private readonly IEmailService _emailService;

    public EmailConsumer(IEmailService emailService, ILogger<EmailConsumer> logger)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<ISendEmailRequest> context)
    {
        var emailData = context.Message;
        _logger.LogInformation("Zpracovávám odeslání e-mailu pro: {Recipient}", emailData.Recipient);
        await _emailService.ProcessQueueAsync();
    }
}