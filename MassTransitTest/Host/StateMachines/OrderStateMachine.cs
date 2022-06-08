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
                .Send(x=> new OrderStatusChanged(x.Saga.CorrelationId, OrderStatus.AwaitingPacking)),
            When(OnChange)
                .If(
                    x => (int)x.Saga.OrderStatus < (int)x.Message.NextStatus,
                    x =>
                        x.Activity(s => s.OfType<UpdateOrderActivity>())
                            .If(c => c.Message.NextStatus == OrderStatus.AwaitingPacking, c => c.TransitionTo(AwaitingPacking))
                            .If(c => c.Message.NextStatus == OrderStatus.Packed, c => c.TransitionTo(Packed))
                            .If(c => c.Message.NextStatus == OrderStatus.Shipped, c => c.TransitionTo(Shipped).Finalize())
                            .If(c => c.Message.NextStatus == OrderStatus.Cancelled, c => c.TransitionTo(Cancelled).Finalize())
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