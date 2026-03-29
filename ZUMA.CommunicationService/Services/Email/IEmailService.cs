using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Services;

namespace ZUMA.CommunicationService.Services.Email;

public interface IEmailService : IServiceBase<EmailEntity>
{
    Task ProcessQueueAsync(CancellationToken cancellationToken = default);

}
