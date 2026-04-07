using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZUMA.Communication.Application.Configuration;
using ZUMA.Communication.Infrastructure.Repositories;
using ZUMA.CommunicationService.Repositories;

namespace ZUMA.Communication.Infrastructure.Configuration;

public static class DIContainer
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration);

        #region Contexts

        services.AddDbContext<CommunicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DbConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DbConnection' not found.")));

        #endregion

        #region Email 

        services.AddScoped<IEmailRepository, EmailRepository>();

        #endregion
    }
}
