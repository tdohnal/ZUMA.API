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
        // ✅ Přidej logging support
        services.AddLogging();

        // Zaregistruj DbContext
        services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CustomerDb")
                ?? throw new InvalidOperationException("ConnectionString 'CustomerDb' not found.")));

        // Zaregistruj Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Zaregistruj Services
        services.AddScoped<IUserService, UserService>();
    }
}
