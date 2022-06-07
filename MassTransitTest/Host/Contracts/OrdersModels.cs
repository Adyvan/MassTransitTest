using MassTransit;
using Models;

namespace Host.Contracts;

public record OrderSaga :
    SagaStateMachineInstance
{
    public long OrderSagaId { get; set; }
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public OrderStatus OrderStatus { get; set; }
}
public record OrderCreated(long OrderSagaId, Guid CorrelationId);
public record OrderStatusChanged (long OrderSagaId, Guid CorrelationId, OrderStatus NextStatus);
