using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.CommunicationService.Infrastructure.Configuration;

namespace ZUMA.CommunicationService.Infrastructure.ExternalServices;

public class SmtpEmailClient(IOptions<SmtpOptions> options) : IEmailClient
{
    private readonly SmtpOptions _options = options.Value;

    public async Task<bool> SendAsync(EmailEntity message, CancellationToken ct)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        email.To.Add(new MailboxAddress(message.Recipient, message.Recipient));
        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = message.Body };
        email.Body = bodyBuilder.ToMessageBody();

        email.Headers.Add("Priority", "Urgent");
        email.Headers.Add("Importance", "high");

        email.Headers.Add("X-Entity-Ref-ID", Guid.NewGuid().ToString());
        email.Headers.Add("X-Mailgun-Tag", "transactional");
        email.Headers.Add("X-Report-Abuse-To", "abuse@zumalab.site");
        // Odstraní podezření na hromadný mailing u některých filtrů
        email.Headers.Add("Precedence", "bulk");

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, ct);

            await client.AuthenticateAsync(_options.Username, _options.Password, ct);

            await client.SendAsync(email, ct);
            await client.DisconnectAsync(true, ct);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return false;
        }
    }
}