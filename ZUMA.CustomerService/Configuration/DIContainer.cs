using Microsoft.EntityFrameworkCore;
using ZUMA.CustomerService.Repositories.Registration;
using ZUMA.CustomerService.Repositories.User;
using ZUMA.CustomerService.Services.Messaging;
using ZUMA.CustomerService.Services.Registration;
using ZUMA.CustomerService.Services.User;
using ZUMA.SharedKernel.Configuration;
using ZUMA.SharedKernel.Infrastructure.Contexts.Customer;

namespace ZUMA.CustomerService.Configurration;

public static class DIContainer
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureBaseServices(configuration);

        #region Contexts

        services.AddDbContext<CustomerDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DbConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DbConnection' not found.")));

        #endregion

        #region Registration 

        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IRegistrationService, RegistrationService>();

        #endregion

        #region User 

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        #endregion

        services.AddSingleton<IEventPublisherService, EventPublisherService>();
    }
}
