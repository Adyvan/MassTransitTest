using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;
using Models;
using OrderSaga = Host.Contracts.OrderSaga;

namespace Host.StateMachines.OrderActivities;

public class OrderPackedActivity : IStateMachineActivity<OrderSaga, OrderPacked>
{
    readonly ILogger<OrderPackedActivity> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public OrderPackedActivity(
        ILogger<OrderPackedActivity> logger,
        IOrderRepositoryService orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("order-packed");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderSaga, OrderPacked> context, IBehavior<OrderSaga, OrderPacked> next)
    {
        _logger.LogInformation($"Execute from {context.Saga.OrderStatus} to Packed");
        context.Saga.OrderStatus = OrderStatus.Packed;

        await _orderRepository.UpdateOrderStatus(context.Saga.CorrelationId.ToId(), context.Saga.OrderStatus).ConfigureAwait(false);

        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderSaga, OrderPacked, TException> context, IBehavior<OrderSaga, OrderPacked> next) where TException : Exception
    {
        return next.Faulted(context);

    }
}