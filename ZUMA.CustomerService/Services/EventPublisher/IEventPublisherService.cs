using ZUMA.SharedKernel.Messagges.Events;

namespace ZUMA.CustomerService.Services.Messaging;

public interface IEventPublisherService
{
    Task PublishCreateEmailEventAsync(CreateEmailEvent createEmailEvent, CancellationToken cancellationToken = default);
}
