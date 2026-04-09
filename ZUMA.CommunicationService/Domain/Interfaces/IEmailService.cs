using ZUMA.CommunicationService.Domain.Entities;
using ZUMA.SharedKernel.Services;

namespace ZUMA.CommunicationService.Domain.Interfaces;

public interface IEmailService : IServiceBase<EmailEntity>
{
    Task ProcessQueueAsync(CancellationToken cancellationToken = default);

}
