using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Application.Services;

namespace ZUMA.CustomerService.Application.Services;

internal class ControlsElementsItemService : ServiceBase<ControlsElementsItemEntity>, IControlsElementsItemService
{
    private readonly ILogger<ControlsElementsItemService> _logger;
    private readonly IControlsElementsItemRepository _controlsElementItemRepository;
    private readonly IEventPublisherService _eventPublisherService;
    private readonly IConfiguration _config;

    public ControlsElementsItemService(
        IControlsElementsItemRepository controlsElementItemRepository,
        IEventPublisherService eventPublisherService,
        ILogger<ControlsElementsItemService> logger,
        IConfiguration config
        ) : base(controlsElementItemRepository)
    {
        _controlsElementItemRepository = controlsElementItemRepository;
        _eventPublisherService = eventPublisherService;
        _logger = logger;
        _config = config;
    }

}