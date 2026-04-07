using ZUMA.SharedKernel.Messagges.Events;

namespace Zuma.Customer.Domain.Interfaces;

public interface IEventPublisherService
{
    Task PublishCreateEmailEventAsync(CreateEmailEvent createEmailEvent, CancellationToken cancellationToken = default);
}
