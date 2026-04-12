using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Services;

namespace ZUMA.CustomerService.Application.Services;

internal class ControlsElementService : ServiceBase<ControlsElementEntity>, IControlsElementService
{
    private readonly ILogger<ControlsElementService> _logger;
    private readonly IControlsElementRepository _controlsElementRepository;
    private readonly IEventPublisherService _eventPublisherService;
    private readonly IConfiguration _config;

    public ControlsElementService(
        IControlsElementRepository controlsElementRepository,
        IEventPublisherService eventPublisherService,
        ILogger<ControlsElementService> logger,
        IConfiguration config
        ) : base(controlsElementRepository)
    {
        _controlsElementRepository = controlsElementRepository;
        _eventPublisherService = eventPublisherService;
        _logger = logger;
        _config = config;
    }

}