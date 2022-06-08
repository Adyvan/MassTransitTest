using MassTransit;
using Models;

namespace Host.Contracts;

public record OrderSaga :
    SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public OrderStatus OrderStatus { get; set; }
}
public record OrderCreated(Guid CorrelationId);
public record OrderStatusChange (Guid CorrelationId, OrderStatus NextStatus) : IConsumer;

public record OrderStatusChanged(Guid CorrelationId, OrderStatus NextStatus);
public record OrderCancelled(Guid CorrelationId) : OrderStatusChanged(CorrelationId, OrderStatus.Cancelled);
public record OrderPacked(Guid CorrelationId) : OrderStatusChanged(CorrelationId, OrderStatus.Packed);
public record OrderShipped(Guid CorrelationId) : OrderStatusChanged(CorrelationId, OrderStatus.Shipped);
