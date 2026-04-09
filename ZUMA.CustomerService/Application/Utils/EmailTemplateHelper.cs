namespace ZUMA.CustomerService.Application.Utils;

public static class EmailTemplateHelper
{
    private static string _templatePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

    public static async Task<string> GetRenderedTemplateAsync(EmailTemplateType templateType, Dictionary<string, string> placeholders)
    {
        string fileName = templateType switch
        {
            EmailTemplateType.RegistrationVerify => "RegistrationVerify.html",
            EmailTemplateType.Authorization => "Authorization.html",
            EmailTemplateType.WelcomeMessage => "Welcome.html",
            _ => throw new ArgumentOutOfRangeException(nameof(templateType), "Unknown template type")
        };

        string fullPath = Path.Combine(_templatePath, fileName);

        if (!File.Exists(fullPath))
        {
            return placeholders.TryGetValue("Body", out var b) ? b : "Empty Content";
        }

        string content = await File.ReadAllTextAsync(fullPath);

        foreach (var item in placeholders)
        {
            content = content.Replace($"{{{{{item.Key}}}}}", item.Value);
        }

        return content;
    }
}
