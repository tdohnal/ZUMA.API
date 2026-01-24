using ZUMA.API.REST.Filters;

namespace ZUMA.API.Configuration;

public class ApiDiContainer
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ValidationFilterAttribute>();
    }
}

