using Host.Contracts;
using Host.Helpers;
using Host.StateMachines.OrderActivities;
using MassTransit;
using Models;
using OrderSaga = Host.Contracts.OrderSaga;

namespace Host.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderSaga>
{
    public OrderStateMachine()
    {
        Event(() => OnInitiated, x =>
        {
            x.CorrelateById(context => context.Message.OrderSagaId.ToGuid());
            x.InsertOnInitial = true;
            x.SetSagaFactory(context => new OrderSaga
            {
                CorrelationId = context.Message.OrderSagaId.ToGuid()
            });
        });
        Event(() => OnChange, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
        });
        
        InstanceState(x => x.CurrentState, Inicial, // OnInitiated 
            AwaitingPacking, // OnPacked 
            Packed, //OnShipped
            Shipped,
            Cancelled);

        Initially(
            When(OnInitiated)
                .TransitionTo(AwaitingPacking),
            When(OnChange)
                .If(
                    x => (int)x.Saga.OrderStatus < (int)x.Message.NextStatus,
                    x =>
                        x.Activity(s => s.OfType<UpdateOrderActivity>())
                )
        );

        DuringAny(
            When(OnChange)
                .If(x => x.Message.NextStatus == OrderStatus.Cancelled && OrderStatus.CanCancel.HasFlag(x.Saga.OrderStatus),
                    x => x.Finalize()));

        SetCompletedWhenFinalized();
    }

    private EventActivityBinder<OrderSaga, OrderStatusChanged> ActivityCallback(EventActivityBinder<OrderSaga, OrderStatusChanged> arg)
    {
        throw new NotImplementedException();
    }

    public State Inicial { get; set; }
    public State AwaitingPacking { get; set; }
    public State Packed { get; set; }
    public State Shipped { get; set; }
    public State Cancelled { get; set; }
    
    public Event<OrderCreated> OnInitiated { get; private set; }
    public Event<OrderStatusChanged> OnChange { get; private set; }
}