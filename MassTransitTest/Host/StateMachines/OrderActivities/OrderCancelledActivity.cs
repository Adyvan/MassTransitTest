using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;
using Models;
using OrderSaga = Host.Contracts.OrderSaga;

namespace Host.StateMachines.OrderActivities;

public class OrderCancelledActivity: IStateMachineActivity<OrderSaga, OrderCancelled>
{
    readonly ILogger<OrderCancelledActivity> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public OrderCancelledActivity(
        ILogger<OrderCancelledActivity> logger,
        IOrderRepositoryService orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("order-cancelled");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderSaga, OrderCancelled> context, IBehavior<OrderSaga, OrderCancelled> next)
    {
        _logger.LogInformation($"Execute from {context.Saga.OrderStatus} to Cancelled");
        context.Saga.OrderStatus = OrderStatus.Cancelled;

        await _orderRepository.UpdateOrderStatus(context.Saga.CorrelationId.ToId(), context.Saga.OrderStatus).ConfigureAwait(false);
        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderSaga, OrderCancelled, TException> context, IBehavior<OrderSaga, OrderCancelled> next) where TException : Exception
    {
        return next.Faulted(context);

    }
}