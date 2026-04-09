using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.CommunicationService.Infrastructure.Repositories;

internal class EmailRepository : RepositoryBase<EmailEntity>, IEmailRepository
{
    private readonly ILogger<EmailRepository> _logger;

    public EmailRepository
        (
        ILogger<EmailRepository> logger,
        CommunicationDbContext dbContext
        )
      : base(dbContext, logger)
    {
        _logger = logger;
    }

    public async Task<IList<EmailEntity>> GetPendingEmailsAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(x => !x.Sent.HasValue).ToListAsync(cancellationToken);
}
