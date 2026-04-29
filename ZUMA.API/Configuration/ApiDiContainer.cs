using ZUMA.API.Attributes;
using ZUMA.API.REST.Mappers;

namespace ZUMA.API.Configuration;

public class ApiDiContainer
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ValidationFilterAttribute>();
        services.AddSingleton<MessageMapper>();
    }
}

