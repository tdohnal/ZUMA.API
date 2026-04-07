using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.Customer.Application.Services;
using ZUMA.SharedKernel.Configuration;

namespace ZUMA.Customer.Infrastructure.Configuration;

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
