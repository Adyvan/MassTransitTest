using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;

namespace Host.StateMachines.OrderActivities;

public class UpdateOrderActivity : IStateMachineActivity<OrderSaga, OrderStatusChanged>
{
    readonly ConsumeContext _context; 
    readonly ILogger<UpdateOrderActivity> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public UpdateOrderActivity(ConsumeContext context,
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

    public async Task Execute(BehaviorContext<OrderSaga, OrderStatusChanged> context, IBehavior<OrderSaga, OrderStatusChanged> next)
    {
        _logger.LogInformation($"Execute from {context.Saga.OrderStatus} to {context.Message.NextStatus}");
        context.Saga.OrderStatus = context.Message.NextStatus;

        await _orderRepository.UpdateOrderStatus(context.Saga.CorrelationId.ToId(), context.Message.NextStatus).ConfigureAwait(false);
        
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<OrderSaga, OrderStatusChanged, TException> context, IBehavior<OrderSaga, OrderStatusChanged> next) where TException : Exception
    {
        return next.Faulted(context);
    }
}