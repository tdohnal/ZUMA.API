using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.Customer.Infrastructure.Persistance;
using ZUMA.Customer.Infrastructure.Repositories;

namespace ZUMA.Customer.Infrastructure.Configuration;

public static class DIContainer
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration);

        #region Contexts

        services.AddDbContext<CustomerDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DbConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DbConnection' not found.")));

        #endregion

        #region Registration 

        services.AddScoped<IRegistrationRepository, RegistrationRepository>();

        #endregion

        #region User 

        services.AddScoped<IUserRepository, UserRepository>();

        #endregion
    }
}
