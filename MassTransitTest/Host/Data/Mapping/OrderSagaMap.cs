using FluentNHibernate.Mapping;
using Models;

namespace Host.Data.Mapping;

public class OrderSagaMap : ClassMap<OrderSaga>
{
    public OrderSagaMap()
    {
        Id(x => x.Id);
        Map(x => x.CustomerName);
        Map(x => x.CustomerSurname);
        Map(x => x.OrderDate);
        Map(x => x.UpdatedDate);
        Map(x => x.ShippedDate);
        Map(x => x.OrderNumber);
        Map(x => x.CurrentState);
        HasMany(x => x.Items)
            .Cascade.All();
    }
}