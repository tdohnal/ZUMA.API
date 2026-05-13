using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Infrastructure.Repositories;

namespace ZUMA.CustomerService.Infrastructure.Repositories;

internal class ControlsElementsItemRepository : RepositoryBase<ControlsElementsItemEntity>, IControlsElementsItemRepository
{
    private readonly ILogger<ControlsElementsItemRepository> _logger;

    public ControlsElementsItemRepository
        (
        CustomerDbContext dbContext,
        IDistributedCache cache,
        ILogger<ControlsElementsItemRepository> logger
        )
      : base(dbContext, cache, logger)
    {
        _logger = logger;
    }

    protected override bool IsCacheEnabled => true;

    protected override IQueryable<ControlsElementsItemEntity> ApplyIncludes(IQueryable<ControlsElementsItemEntity> query)
    {
        return query.Include(x => x.ControlElement);
    }
}
