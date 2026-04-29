using MassTransit;
using ZUMA.CustomerService.Application.Utils;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.SharedKernel.Domain.MessagingContracts.Events;

namespace ZUMA.CustomerService.Application.Services;

internal class EventPublisherService : IEventPublisherService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventPublisherService> _logger;

    public EventPublisherService(IPublishEndpoint publishEndpoint, ILogger<EventPublisherService> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishCreateEmailEventAsync(CreateEmailEvent createEmailEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing UserRegistered event for email: {Email}", createEmailEvent.Email);

        Dictionary<string, string> placeholders = new()
        {
        { "Name", createEmailEvent.FullName },
        { "Subject", createEmailEvent.Subject },
        { "Body", createEmailEvent.Body ?? "" },
        { "Code", createEmailEvent.Code ??  "" }
    };

        var body = await EmailTemplateHelper.GetRenderedTemplateAsync(createEmailEvent.EmailTemplateType, placeholders);

        await _publishEndpoint.Publish<CreateEmailEvent>(new
        {
            createEmailEvent.UserId,
            createEmailEvent.Email,
            Body = body,
            createEmailEvent.Subject,
            createEmailEvent.FullName,
            createEmailEvent.EmailTemplateType,
        }, cancellationToken);
    }
}
