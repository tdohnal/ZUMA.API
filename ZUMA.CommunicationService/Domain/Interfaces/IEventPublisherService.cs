using ZUMA.SharedKernel.Messagges.Events;

namespace ZUMA.CommunicationService.Domain.Interfaces;

public interface IEventPublisherService
{
    Task PublishFireEmailsAsync(FireEmailEvent fireEmail, CancellationToken cancellationToken = default);
}
