using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ZUMA.CommunicationService.Domain.Entities; // Tvoje entita
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.CommunicationService.Infrastructure.Configuration;

namespace ZUMA.Infrastructure.ExternalServices;

public class MailjetSmtpClient(IOptions<MailjetOptions> options, HttpClient httpClient) : IEmailClient
{
    private readonly MailjetOptions _options = options.Value;

    public async Task<bool> SendAsync(EmailEntity message, CancellationToken ct)
    {
        // Příprava dat podle Mailjet API v3.1 struktury
        var requestPayload = new
        {
            Messages = new[]
            {
                new
                {
                    From = new { Email = _options.SenderEmail, Name = _options.SenderName },
                    To = new[] { new { Email = message.Recipient, Name = message.Recipient } },
                    Subject = message.Subject,
                    HTMLPart = message.Body,
                    TextPart = "Zobrazte si tento e-mail v prohlížeči podporujícím HTML."
                }
            }
        };

        var json = JsonSerializer.Serialize(requestPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Autentizace (Base64 API Key : Secret Key)
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ApiKey}:{_options.SecretKey}"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mailjet.com/v3.1/send");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        request.Content = content;

        try
        {
            var response = await httpClient.SendAsync(request, ct);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // Pokud to selže, přečteme proč (pro ladění)
            var errorDetail = await response.Content.ReadAsStringAsync(ct);
            // Zde bys měl zalogovat errorDetail
            return false;
        }
        catch
        {
            return false;
        }
    }
}