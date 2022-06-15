using Host.CommunicationProtocols.Order;
using Host.Data;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Host.Tests.Helpers;
using NHibernate.Event;
using NHibernate.Linq;
using OrderItem = Host.CommunicationProtocols.Order.OrderItem;

namespace Host.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        await using var app = new HostApplication();
        var client = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var provider = scope.ServiceProvider;
        var dbSession = provider.GetRequiredService<IDbSession>();

        //get empty Orders
        var firstOrders = await client.GetFromJsonAsync<OrderResponse[]>("/Order/Orders");
        Assert.That(firstOrders, Is.Not.Null, "Get orders can return not null");
        Assert.That(firstOrders!.Length, Is.EqualTo(0));

        //create Order
        var createOrder = new CreateOrderRequest()
        {
            CustomerName = "TestName",
            CustomerSurname = "TestSurname",
            ShippedDate = DateTime.UtcNow.AddDays(5),
            Items = new List<OrderItem>()
            {
                new OrderItem()
                {
                    Price = 10,
                    Quantity = 23,
                    Sku = 15,
                }
            },
        };

        var addedOrderResponseMessage = await client.PostAsJsonAsync("/Order/Create", createOrder);
        Assert.That(addedOrderResponseMessage.IsSuccessStatusCode, Is.True);

        await WaitAsync(5000, 20);  // wait for event bus

        Order? dbOrder= null;
        using (var db = dbSession.GetSession())
        using (var session = db.OpenSession())
        {
            var dbOrders = session
                .Query<Order>()
                .Fetch(x => x.Items).ToArray();
            
            dbOrder = session
                .Query<Order>()
                .Fetch(x => x.Items) 
                .FirstOrDefault(x => x.Id == 1);// because it is the first

            Assert.That(dbOrder, Is.Not.Null);

            Assert.That(dbOrder!.OrderDate, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            Assert.That(dbOrder!.UpdatedDate, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
            Assert.That(dbOrder!.ShippedDate, Is.EqualTo(createOrder.ShippedDate));
            Assert.That(dbOrder!.CustomerName, Is.EqualTo(createOrder.CustomerName));
            Assert.That(dbOrder!.CustomerSurname, Is.EqualTo(createOrder.CustomerSurname));

            Assert.That(dbOrder!.OrderStatus, Is.EqualTo(OrderStatus.AwaitingPacking));

            Assert.That(dbOrder!.Items, Has.Count.EqualTo(1));
            var item = dbOrder!.Items.First();
            Assert.That(item.Price, Is.EqualTo(createOrder.Items.First().Price));
            Assert.That(item.Sku, Is.EqualTo(createOrder.Items.First().Sku));
            Assert.That(item.Quantity, Is.EqualTo(createOrder.Items.First().Quantity));
        }

        //get not empty Orders
        var secondOrders = await client.GetFromJsonAsync<OrderResponse[]>("/Order/Orders");
        Assert.That(secondOrders, Is.Not.Null, "Get orders can return not null");
        Assert.That(secondOrders!, Has.Length.EqualTo(1));
        var apiOrder1 = secondOrders.FirstOrDefault();
        Assert.That(apiOrder1, Is.Not.Null);

        Assert.That(apiOrder1!.OrderDate, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        Assert.That(apiOrder1!.UpdatedDate, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        Assert.That(apiOrder1!.ShippedDate, Is.EqualTo(createOrder.ShippedDate));
        Assert.That(apiOrder1!.CustomerName, Is.EqualTo(createOrder.CustomerName));
        Assert.That(apiOrder1!.CustomerSurname, Is.EqualTo(createOrder.CustomerSurname));

        Assert.That(apiOrder1!.OrderStatus, Is.EqualTo(OrderStatus.AwaitingPacking));

        Assert.That(apiOrder1!.Items, Has.Count.EqualTo(1));
        var apiItem = apiOrder1!.Items.First();
        Assert.That(apiItem.Price, Is.EqualTo(createOrder.Items.First().Price));
        Assert.That(apiItem.Sku, Is.EqualTo(createOrder.Items.First().Sku));
        Assert.That(apiItem.Quantity, Is.EqualTo(createOrder.Items.First().Quantity));
        
        
        //await app.Server.Host.StopAsync();
    }

    private static async Task WaitAsync(int milisecond, int step)
    {
        var time = milisecond;
        while (time > 0)
        {
            await Task.Yield();
            await Task.Delay(TimeSpan.FromMilliseconds(step));
            time -= step;
        }
    }
}