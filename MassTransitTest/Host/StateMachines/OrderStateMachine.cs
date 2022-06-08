using Host.Contracts;
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
            x.CorrelateById(context => context.Message.CorrelationId);
            x.InsertOnInitial = true;
            x.SetSagaFactory(context => new OrderSaga
            {
                CorrelationId = context.Message.CorrelationId,
                OrderStatus = OrderStatus.Initial,
            });
        });
        Event(() => OnChange, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
        });
        
        InstanceState(x => x.CurrentState, 
            AwaitingPacking,
            Packed,
            Shipped,
            Cancelled);

        Initially(
            When(OnInitiated)
                .Activity(s => s.OfType<InitiatedOrderActivity>())
                .TransitionTo(AwaitingPacking),
            When(OnChange)
                .IfElse(
                    x => (int)x.Saga.OrderStatus < (int)x.Message.NextStatus,
                    x =>
                        x.Activity(s => s.OfType<UpdateOrderActivity>())
                            .If(c => c.Message.NextStatus == OrderStatus.Packed, c => c.TransitionTo(Packed))
                            .If(c => c.Message.NextStatus == OrderStatus.Shipped, c => c.TransitionTo(Shipped).Finalize())
                            .If(c => c.Message.NextStatus == OrderStatus.Cancelled, c => c.TransitionTo(Cancelled).Finalize()),
                    x=> x.(c=> c)
                )

        );
        During(AwaitingPacking, Ignore(OnInitiated));

        // DuringAny(
        //     When(OnChange)
        //         .If(x => x.Message.NextStatus == OrderStatus.Cancelled && OrderStatus.CanCancel.HasFlag(x.Saga.OrderStatus),
        //             x => x.Finalize()));

        SetCompletedWhenFinalized();
    }

    private EventActivityBinder<OrderSaga, OrderStatusChanged> ActivityCallback(EventActivityBinder<OrderSaga, OrderStatusChanged> arg)
    {
        throw new NotImplementedException();
    }

    public State AwaitingPacking { get; set; }
    public State Packed { get; set; }
    public State Shipped { get; set; }
    public State Cancelled { get; set; }
    
    public Event<OrderCreated> OnInitiated { get; private set; }
    public Event<OrderStatusChanged> OnChange { get; private set; }
}