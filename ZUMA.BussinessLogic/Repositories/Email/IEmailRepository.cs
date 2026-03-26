using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.BussinessLogic.Repositories.Email;

public interface IEmailRepository : IRepositoryBase<EmailEntity>
{
    Task<IList<EmailEntity>> GetPendingEmailsAsync(CancellationToken cancellationToken = default);
}
