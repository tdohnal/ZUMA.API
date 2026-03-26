using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.BussinessLogic.Services.Email;

public interface IEmailService : IServiceBase<EmailEntity>
{
    Task ProcessQueueAsync(CancellationToken cancellationToken = default);

}
