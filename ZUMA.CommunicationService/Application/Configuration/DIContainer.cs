using ZUMA.CommunicationService.Application.Services;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.Application.Configuration;

namespace ZUMA.CommunicationService.Application.Configuration;

public static class DIContainer
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureApplicationBaseServices(configuration);

        #region Email 

        services.AddScoped<IEmailService, EmailService>();

        #endregion

        services.AddSingleton<IEventPublisherService, EventPublisherService>();
    }
}
