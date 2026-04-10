using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.CommunicationService.Infrastructure.Configuration;

namespace ZUMA.Infrastructure.ExternalServices;

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

        using var client = new SmtpClient();
        try
        {
            // Připojení na Brevo (port 587 vyžaduje StartTls)
            await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, ct);

            // Autentizace (Username je tvůj email, Password je API klíč z Breva)
            await client.AuthenticateAsync(_options.Username, _options.Password, ct);

            await client.SendAsync(email, ct);
            await client.DisconnectAsync(true, ct);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CHYBA ODESILANI: {ex.Message}");
            return false;
        }
    }
}