using Host.Contracts;
using Host.Services;
using MassTransit;
using Models;

namespace Host.Consumers;

public class AddOrderConsumer :
    IConsumer<AddOrder>
{
    readonly ILogger<AddOrderConsumer> _logger;
    readonly IOrderRepositoryService _orderRepository;

    public AddOrderConsumer(
        ILogger<AddOrderConsumer> logger,
        IOrderRepositoryService orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public async Task Consume(ConsumeContext<AddOrder> context)
    {
        var newOrder = new OrderSaga
        {
            CustomerName = context.Message.CustomerName,
            CustomerSurname = context.Message.CustomerSurname,
            ShippedDate = context.Message.ShippedDate,
            Items =  context.Message.Items.Select(item => new OrderSagaItem()
            {
                Sku =item.Sku,
                Price = item.Price,
                Quantity = item.Quantity,
            }).ToList(),
        };
        await _orderRepository.AddOrderAsync(newOrder);
        _logger.LogInformation("Added order");
    }
}
