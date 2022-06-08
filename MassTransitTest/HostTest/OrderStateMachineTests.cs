using Host.Contracts;
using Host.StateMachines;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace HostTest;

public class OrderStateMachineTests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task OrderStateMachine_Saga_Enter_AwaitingPacking_State()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine(typeof(OrderStateMachine));
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var sagaId = Guid.NewGuid();

        await harness.Bus.Publish(new OrderCreated(sagaId));

        Assert.That(await harness.Consumed.Any<OrderCreated>());

        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderSaga>();

        Assert.That(await sagaHarness.Consumed.Any<OrderCreated>());
        
        Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == sagaId));

        var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, x=>x.AwaitingPacking);
        
        Assert.IsNotNull(instance, "Saga instance not found");
        
        Assert.That(instance.OrderStatus, Is.EqualTo(OrderStatus.AwaitingPacking));
    }
}