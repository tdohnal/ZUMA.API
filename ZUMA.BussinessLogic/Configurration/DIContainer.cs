using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;
using ZUMA.BussinessLogic.Repositories.User;
using ZUMA.BussinessLogic.Services.User;

namespace ZUMA.BusinessLogic.Configuration;

public static class DIContainer
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging();

        #region Contexts

        services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CustomerDb")
                ?? throw new InvalidOperationException("ConnectionString 'CustomerDb' not found.")));

        #endregion

        #region User 

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        #endregion

        #region Registration 

        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<IRegistrationService, RegistrationService>();

        #endregion

    }
}
