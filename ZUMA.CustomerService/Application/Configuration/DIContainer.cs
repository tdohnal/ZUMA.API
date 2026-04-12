using ZUMA.CustomerService.Application.Services;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Configurration;

namespace ZUMA.CustomerService.Application.Configuration;

public static class DIContainer
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureBaseServices(configuration);

        #region Registration 

        services.AddSingleton<IRegistrationService, RegistrationService>();

        #endregion

        #region User 

        services.AddSingleton<IUserService, UserService>();

        #endregion

        #region ControlsElement


        services.AddSingleton<IControlsElementService, ControlsElementService>();

        services.AddSingleton<IControlsElementsItemService, ControlsElementsItemService>();

        #endregion

        services.AddSingleton<IEventPublisherService, EventPublisherService>();
    }
}
