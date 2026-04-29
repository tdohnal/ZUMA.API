using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.SharedKernel.Domain.Interfaces;

namespace ZUMA.CommunicationService.Domain.Interfaces;

public interface IEmailService : IServiceBase<EmailEntity>
{
    Task ProcessQueueAsync(CancellationToken cancellationToken = default);

}
