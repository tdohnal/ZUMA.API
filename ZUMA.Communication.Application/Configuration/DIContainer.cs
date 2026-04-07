using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZUMA.CommunicationService.Services;
using ZUMA.CommunicationService.Services.Email;
using ZUMA.CommunicationService.Services.EventPublisher;
using ZUMA.SharedKernel.Configuration;

namespace ZUMA.Communication.Application.Configuration;

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
