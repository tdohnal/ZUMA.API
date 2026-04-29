using ZUMA.SharedKernel.Domain.MessagingContracts.Events;

namespace ZUMA.CustomerService.Domain.Interfaces;

public interface IEventPublisherService
{
    Task PublishCreateEmailEventAsync(CreateEmailEvent createEmailEvent, CancellationToken cancellationToken = default);
}
