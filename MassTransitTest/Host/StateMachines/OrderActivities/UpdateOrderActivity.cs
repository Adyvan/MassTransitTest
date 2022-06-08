using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;

namespace Host.StateMachines.OrderActivities;

public class UpdateOrderActivity<T> : IStateMachineActivity<OrderSaga, T> where T: OrderStatusChanged
{
    readonly ILogger<UpdateOrderActivity<T>> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public UpdateOrderActivity(
        ILogger<UpdateOrderActivity<T>> logger,
        IOrderRepositoryService orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("order-status-changed");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderSaga, T> context, IBehavior<OrderSaga, T> next)
    {
        _logger.LogInformation($"Execute from {context.Saga.OrderStatus} to {context.Message.NextStatus}");
        context.Saga.OrderStatus = context.Message.NextStatus;

        await _orderRepository.UpdateOrderStatus(context.Saga.CorrelationId.ToId(), context.Message.NextStatus).ConfigureAwait(false);
        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderSaga, T, TException> context, IBehavior<OrderSaga, T> next) where TException : Exception
    {
        return next.Faulted(context);

    }
}