using ZUMA.Communication.Domain.Entities;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.CommunicationService.Repositories;

public interface IEmailRepository : IRepositoryBase<EmailEntity>
{
    Task<IList<EmailEntity>> GetPendingEmailsAsync(CancellationToken cancellationToken = default);
}
