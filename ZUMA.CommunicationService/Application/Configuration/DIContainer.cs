using ZUMA.CommunicationService.Application.Services;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.Configurration;

namespace ZUMA.CommunicationService.Application.Configuration;

public static class DIContainer
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureBaseServices(configuration);

        #region Email 

        services.AddScoped<IEmailService, EmailService>();

        #endregion

        services.AddSingleton<IEventPublisherService, EventPublisherService>();
    }
}
