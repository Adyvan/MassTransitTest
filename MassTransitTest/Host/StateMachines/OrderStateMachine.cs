using Host.Contracts;
using Host.Helpers;
using Host.Services;
using Host.StateMachines.OrderActivities;
using MassTransit;
using Models;
using OrderSaga = Host.Contracts.OrderSaga;

namespace Host.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderSaga>
{
    public State AwaitingPacking { get; set; }
    public State Packed { get; set; }
    public State Shipped { get; set; }
    public State Cancelled { get; set; }
    
    public Event<OrderCreated> OnInitiated { get; private set; }
    public Event<OrderPacked> OnPacked { get; private set; }
    public Event<OrderShipped> OnShipped { get; private set; }
    public Event<OrderCancelled> OnCancelled { get; private set; }

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
        Event(() => OnPacked, x => { x.CorrelateById(context => context.Message.CorrelationId); });

        Event(() => OnShipped, x => { x.CorrelateById(context => context.Message.CorrelationId); });

        Event(() => OnCancelled, x => { x.CorrelateById(context => context.Message.CorrelationId); });


        InstanceState(x => x.CurrentState,
            AwaitingPacking,
            Packed,
            Shipped,
            Cancelled);

        During(AwaitingPacking,
            Ignore(OnInitiated), 
            Ignore(OnShipped), 
            Ignore(OnCancelled));
        
        During(Packed, 
            Ignore(OnInitiated), 
            Ignore(OnPacked));
        
        During(Cancelled,
            Ignore(OnInitiated),
            Ignore(OnPacked),
            Ignore(OnShipped),
            Ignore(OnCancelled));
        
        During(Shipped,
            Ignore(OnInitiated),
            Ignore(OnPacked),
            Ignore(OnShipped),
            Ignore(OnCancelled));

        Initially(
            When(OnInitiated)
                .Activity(x=> x.OfType<InitiatedOrderActivity>())
                .TransitionTo(AwaitingPacking),
            When(OnPacked)
                .Activity(x=> x.OfType<UpdateOrderActivity<OrderPacked>>())
                .TransitionTo(Packed),
            When(OnShipped)
                .Activity(x=> x.OfType<UpdateOrderActivity<OrderShipped>>())
                .TransitionTo(Shipped)
                .Finalize(),
            When(OnCancelled)
                .Activity(x=> x.OfType<UpdateOrderActivity<OrderCancelled>>())
                .TransitionTo(Cancelled)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}