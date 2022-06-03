using FluentNHibernate.Mapping;
using Models;

namespace Host.Data.Mapping;

public class OrderSagaItemMap : ClassMap<OrderSagaItem>
{
    public OrderSagaItemMap()
    {
        Id(x => x.Id);
        Map(x => x.Price);
        Map(x => x.Quantity);
        Map(x => x.Sku);
        Map(x => x.OrderId);
        References(x => x.Order);
    }
}