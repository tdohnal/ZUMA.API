using ZUMA.CustomerService.Application.Services;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Configuration;

namespace ZUMA.CustomerService.Application.Configuration;

public static class DIContainer
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureBaseServices(configuration);

        #region Registration 

        services.AddScoped<IRegistrationService, RegistrationService>();

        #endregion

        #region User 

        services.AddScoped<IUserService, UserService>();

        #endregion

        services.AddSingleton<IEventPublisherService, EventPublisherService>();
    }
}
