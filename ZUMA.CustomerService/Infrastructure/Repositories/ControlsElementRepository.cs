using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.CustomerService.Infrastructure.Repositories;

internal class ControlsElementRepository : RepositoryBase<ControlsElementEntity>, IControlsElementRepository
{
    private readonly ILogger<ControlsElementRepository> _logger;

    public ControlsElementRepository
        (
        ILogger<ControlsElementRepository> logger,
        CustomerDbContext dbContext
        )
      : base(dbContext, logger)
    {
        _logger = logger;
    }


}
