using ZUMA.BussinessLogic.Messagges.Events;

namespace ZUMA.CommunicationService.Services.EventPublisher;

public interface IEventPublisherService
{
    Task PublishFireEmailsAsync(FireEmailEvent fireEmail, CancellationToken cancellationToken = default);
}
