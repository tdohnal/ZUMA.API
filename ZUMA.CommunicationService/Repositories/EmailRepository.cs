using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Repositories;

namespace ZUMA.CommunicationService.Repositories;

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

    //public async Task<IList<EmailEntity>> GetPendingEmailsAsync(CancellationToken cancellationToken = default) => await _dbSet.Include(x => x.Recipient).Where(x => !x.Sent.HasValue).ToListAsync(cancellationToken);
}
