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
public record OrderStatusChanged (Guid CorrelationId, OrderStatus NextStatus);
