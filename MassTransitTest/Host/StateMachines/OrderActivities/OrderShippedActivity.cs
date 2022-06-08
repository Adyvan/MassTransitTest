using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;
using Models;
using OrderSaga = Host.Contracts.OrderSaga;

namespace Host.StateMachines.OrderActivities;

public class OrderShippedActivity: IStateMachineActivity<OrderSaga, OrderShipped>
{
    readonly ILogger<OrderShippedActivity> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public OrderShippedActivity(
        ILogger<OrderShippedActivity> logger,
        IOrderRepositoryService orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("order-shipped");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderSaga, OrderShipped> context, IBehavior<OrderSaga, OrderShipped> next)
    {
        _logger.LogInformation($"Execute from {context.Saga.OrderStatus} to Shipped");
        context.Saga.OrderStatus = OrderStatus.Shipped;

        await _orderRepository.UpdateOrderStatus(context.Saga.CorrelationId.ToId(), context.Saga.OrderStatus).ConfigureAwait(false);
        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderSaga, OrderShipped, TException> context, IBehavior<OrderSaga, OrderShipped> next) where TException : Exception
    {
        return next.Faulted(context);

    }
}