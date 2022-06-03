using Host.CommunicationProtocols.Order;
using Host.Contracts;
using Host.Data;
using Host.Services;
using MassTransit;
using Models;

namespace Host.Consumers;

public class AddOrderConsumer :
    IConsumer<AddOrder>
{
    private ILogger<AddOrderConsumer> _logger;
    private IDbSession _db;
    private ITimeService _time;

    public AddOrderConsumer(ILogger<AddOrderConsumer> logger, IDbSession db, ITimeService time)
    {
        _logger = logger;
        _db = db;
        _time = time;
    }

    public async Task Consume(ConsumeContext<AddOrder> context)
    {
        var newOrder = new OrderSaga
        {
            OrderDate = _time.Now,
            UpdatedDate = _time.Now,
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
        
        using var db = _db.GetSession();
        using var session = db.OpenSession();
        using var transaction = session.BeginTransaction();
        
        await session.SaveOrUpdateAsync(newOrder);
        await transaction.CommitAsync();
    }
}
