using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZUMA.BussinessLogic.Repositories.User;
using ZUMA.BussinessLogic.Services.User;

namespace ZUMA.BusinessLogic.Configuration;

public static class DIContainer
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        #region User

        services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CustomerDb")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        #endregion
    }
}
