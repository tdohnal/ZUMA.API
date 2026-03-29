using Microsoft.EntityFrameworkCore;
using ZUMA.CommunicationService.Repositories;
using ZUMA.CommunicationService.Services.Email;
using ZUMA.CommunicationService.Services.EventPublisher;

namespace ZUMA.BusinessLogic.Configuration;

public static class DIContainer
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureBaseServices(configuration);

        #region Contexts

        services.AddDbContext<CommunicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DbConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DbConnection' not found.")));

        #endregion

        #region Email 

        services.AddScoped<IEmailRepository, EmailRepository>();
        services.AddScoped<IEmailService, EmailService>();

        #endregion

        services.AddSingleton<IEventPublisherService, EventPublisherService>();
    }
}
