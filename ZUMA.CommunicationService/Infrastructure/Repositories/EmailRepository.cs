using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.Infrastructure.Repositories;

namespace ZUMA.CommunicationService.Infrastructure.Repositories;

internal class EmailRepository : RepositoryBase<EmailEntity>, IEmailRepository
{
    private readonly ILogger<EmailRepository> _logger;

    public EmailRepository
        (
        CommunicationDbContext dbContext,
        IDistributedCache cache,
        ILogger<EmailRepository> logger
        )
      : base(dbContext, cache, logger)
    {
        _logger = logger;
    }

    protected override bool IsCacheEnabled => false;

    public async Task<IList<EmailEntity>> GetPendingEmailsAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(x => !x.Sent.HasValue).ToListAsync(cancellationToken);
}
