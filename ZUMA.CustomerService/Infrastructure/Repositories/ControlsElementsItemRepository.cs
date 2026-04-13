using Microsoft.EntityFrameworkCore;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.CustomerService.Infrastructure.Repositories;

internal class ControlsElementsItemRepository : RepositoryBase<ControlsElementsItemEntity>, IControlsElementsItemRepository
{
    private readonly ILogger<ControlsElementsItemRepository> _logger;

    public ControlsElementsItemRepository
        (
        ILogger<ControlsElementsItemRepository> logger,
        CustomerDbContext dbContext
        )
      : base(dbContext, logger)
    {
        _logger = logger;
    }

    protected override IQueryable<ControlsElementsItemEntity> ApplyIncludes(IQueryable<ControlsElementsItemEntity> query)
    {
        return query.Include(x => x.ControlElement);
    }
}
