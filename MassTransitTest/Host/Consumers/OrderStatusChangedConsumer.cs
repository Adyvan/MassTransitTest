using Host.Contracts;
using MassTransit;
using Models;

namespace Host.Consumers;

public class OrderStatusChangedConsumer:
    IConsumer<OrderStatusChange>
{
    readonly ILogger<OrderStatusChangedConsumer> _logger;
    readonly IBus _bus;

    public OrderStatusChangedConsumer(
        ILogger<OrderStatusChangedConsumer> logger,
        IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<OrderStatusChange> context)
    {
        switch (context.Message.NextStatus)
        {
            case OrderStatus.Packed:
                await _bus.Publish(new OrderPacked(context.Message.CorrelationId)).ConfigureAwait(false);
                break; 
            case OrderStatus.Shipped:
                await _bus.Publish(new OrderShipped(context.Message.CorrelationId)).ConfigureAwait(false);
                break;
            case OrderStatus.Cancelled:
                await _bus.Publish(new OrderCancelled(context.Message.CorrelationId)).ConfigureAwait(false);
                break;
            default:
                throw new Exception("Not Support Next Status");
        }
        _logger.LogInformation($"Create move to {context.Message.NextStatus}");
    }
}