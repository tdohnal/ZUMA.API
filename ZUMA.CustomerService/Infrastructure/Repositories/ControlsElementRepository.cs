using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Infrastructure.Repositories;

namespace ZUMA.CustomerService.Infrastructure.Repositories;

internal class ControlsElementRepository : RepositoryBase<ControlsElementEntity>, IControlsElementRepository
{
    private readonly ILogger<ControlsElementRepository> _logger;

    public ControlsElementRepository
        (
        CustomerDbContext dbContext,
        IDistributedCache cache,
        ILogger<ControlsElementRepository> logger

        )
      : base(dbContext, cache, logger)
    {
        _logger = logger;
    }

    protected override bool IsCacheEnabled => true;

    protected override IQueryable<ControlsElementEntity> ApplyIncludes(IQueryable<ControlsElementEntity> query)
    {
        return query.Include(x => x.OwnerUser)
                    .Include(x => x.Items);
    }
}
