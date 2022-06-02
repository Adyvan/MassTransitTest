using MassTransit;

namespace Host.Consumers;

public class BackgroundServicesConsumerDefinition :
    ConsumerDefinition<AddOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<AddOrderConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}
