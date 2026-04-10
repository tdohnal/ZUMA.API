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

        // Globální doporučení: Nepoužívej v předmětu hranaté závorky []
        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = message.Body,
            // KLÍČOVÉ PRO DORUČITELNOST: Přidání čistě textové verze
            TextBody = $"Váš ověřovací kód pro ZUMA"
        };
        email.Body = bodyBuilder.ToMessageBody();

        // Vysoce prioritní transakční hlavičky
        email.Headers.Add("X-Priority", "1");
        email.Headers.Add("Importance", "High");

        // Tyto hlavičky pomáhají obejít filtry hromadné pošty
        email.Headers.Add("X-Auto-Response-Suppress", "All");

        // POZOR: List-Unsubscribe nesmí být prázdný, raději ho úplně vynechej 
        // nebo v Brevu nastav, aby se transakčním mailům nepřidával.
        // email.Headers.Remove("List-Unsubscribe"); 

        // !!! TUTO ŘÁDKU SMAŽ - 'bulk' tě posílá do Hromadných !!!
        // email.Headers.Add("Precedence", "bulk"); 

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