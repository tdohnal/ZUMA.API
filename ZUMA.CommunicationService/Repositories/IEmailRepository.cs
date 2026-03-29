using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Repositories;

namespace ZUMA.CommunicationService.Repositories;

public interface IEmailRepository : IRepositoryBase<EmailEntity>
{
    Task<IList<EmailEntity>> GetPendingEmailsAsync(CancellationToken cancellationToken = default);
}
