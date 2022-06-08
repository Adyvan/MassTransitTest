using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;
using Models;
using OrderSaga = Host.Contracts.OrderSaga;

namespace Host.StateMachines.OrderActivities;

public class InitiatedOrderActivity: IStateMachineActivity<OrderSaga, OrderCreated>
{
    readonly ILogger<InitiatedOrderActivity> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public InitiatedOrderActivity(ConsumeContext context,
        ILogger<InitiatedOrderActivity> logger,
        IOrderRepositoryService orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("order-init");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<OrderSaga, OrderCreated> context, IBehavior<OrderSaga, OrderCreated> next)
    {
        _logger.LogInformation($"Execute from {context.Saga.OrderStatus} to {OrderStatus.AwaitingPacking}");
        context.Saga.OrderStatus = OrderStatus.AwaitingPacking;
        
        await _orderRepository.UpdateOrderStatus(context.Saga.CorrelationId.ToId(), OrderStatus.AwaitingPacking).ConfigureAwait(false);
        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderSaga, OrderCreated, TException> context, IBehavior<OrderSaga, OrderCreated> next) where TException : Exception
    {
        return next.Faulted(context);
    }
}