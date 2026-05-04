using ZUMA.API.Attributes;
using ZUMA.API.Mappers;

namespace ZUMA.API.Configuration;

public class DIContainer
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ValidationFilterAttribute>();

        services.AddSingleton<UserMapper>();
        services.AddSingleton<ControlsElementMapper>();
        services.AddSingleton<AuthMapper>();
    }
}

