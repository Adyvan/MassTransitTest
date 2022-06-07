using Host.Contracts;
using Host.Helpers;
using Host.Services;
using MassTransit;
using Models;
using OrderSaga = Models.OrderSaga;

namespace Host.Consumers;

public class AddOrderConsumer :
    IConsumer<AddOrder>
{
    readonly ILogger<AddOrderConsumer> _logger;
    readonly IOrderRepositoryService _orderRepository;
    readonly IBus _bus;

    public AddOrderConsumer(
        ILogger<AddOrderConsumer> logger,
        IOrderRepositoryService orderRepository, 
        IBus bus)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<AddOrder> context)
    {
        var newOrder = new OrderSaga
        {
            CustomerName = context.Message.CustomerName,
            CustomerSurname = context.Message.CustomerSurname,
            ShippedDate = context.Message.ShippedDate,
            Items = context.Message.Items.Select(item => new OrderSagaItem()
            {
                Sku = item.Sku,
                Price = item.Price,
                Quantity = item.Quantity,
            }).ToList(),
            OrderStatus = OrderStatus.Initial,
        };
        
        await _orderRepository.AddOrderAsync(newOrder).ConfigureAwait(false);
        await _bus.Publish(new OrderCreated(newOrder.Id, newOrder.Id.ToGuid())).ConfigureAwait(false);
        
        _logger.LogInformation($"Added order {newOrder.Id}");
    }
}
