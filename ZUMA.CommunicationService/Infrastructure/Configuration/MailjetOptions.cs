namespace ZUMA.CommunicationService.Infrastructure.Configuration;

public class MailjetOptions
{
    public string SmtpServer { get; set; } = "in-v3.mailjet.com";
    public int Port { get; set; } = 587;
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
}
