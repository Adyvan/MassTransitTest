using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;
using Models;
using OrderSaga = Host.Contracts.OrderSaga;

namespace Host.StateMachines.OrderActivities;

public class InitiatedOrderActivity: IStateMachineActivity<OrderSaga, OrderCreated>
{
    readonly ConsumeContext _context; 
    readonly ILogger<UpdateOrderActivity> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public InitiatedOrderActivity(ConsumeContext context,
        ILogger<UpdateOrderActivity> logger,
        IOrderRepositoryService orderRepository)
    {
        _context = context;
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