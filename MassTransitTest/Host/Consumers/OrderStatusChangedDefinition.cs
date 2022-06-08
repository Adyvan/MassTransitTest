using Host.Contracts;
using MassTransit;

namespace Host.Consumers;

public class OrderStatusChangedDefinition:
    ConsumerDefinition<OrderStatusChange>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderStatusChange> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
    }
}