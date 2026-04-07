using ZUMA.SharedKernel.Entities.Customer;
using ZUMA.SharedKernel.Services;

namespace ZUMA.CommunicationService.Services.Email;

public interface IEmailService : IServiceBase<EmailEntity>
{
    Task ProcessQueueAsync(CancellationToken cancellationToken = default);

}
