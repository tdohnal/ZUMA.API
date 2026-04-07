using MassTransit;
using Microsoft.Extensions.Logging;
using ZUMA.CommunicationService.Services.EventPublisher;
using ZUMA.SharedKernel.Messagges.Events;

namespace ZUMA.CommunicationService.Services
{
    internal class EventPublisherService : IEventPublisherService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<EventPublisherService> _logger;

        public EventPublisherService(IPublishEndpoint publishEndpoint, ILogger<EventPublisherService> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishFireEmailsAsync(FireEmailEvent fireEmail, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Publishing FireEmail event for email: {Email}", fireEmail.Email);

            await _publishEndpoint.Publish<FireEmailEvent>(new
            {
                fireEmail.EmailId,
                fireEmail.Email,

            }, cancellationToken);
        }
    }
}
