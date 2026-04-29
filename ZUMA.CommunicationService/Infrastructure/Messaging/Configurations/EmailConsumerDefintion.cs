using MassTransit;
using ZUMA.CommunicationService.Application.Consumers;

namespace ZUMA.CommunicationService.Infrastructure.Messaging.Configurations;

public class EmailConsumerDefintion : ConsumerDefinition<EmailConsumer>
{
    public EmailConsumerDefintion()
    {
        EndpointName = "zuma.communication.email";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EmailConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromMicroseconds(500)));
        endpointConfigurator.UseTimeout(t => t.Timeout = TimeSpan.FromSeconds(15));
    }
}
