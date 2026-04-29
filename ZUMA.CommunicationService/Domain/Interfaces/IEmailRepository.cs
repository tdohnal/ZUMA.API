using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.SharedKernel.Domain.Interfaces;

namespace ZUMA.CommunicationService.Domain.Interfaces;

public interface IEmailRepository : IRepositoryBase<EmailEntity>
{
    Task<IList<EmailEntity>> GetPendingEmailsAsync(CancellationToken cancellationToken = default);
}
