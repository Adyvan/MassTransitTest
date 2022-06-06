using FluentMigrator;
using Models;

namespace Database.Migrations;

[Migration(0)]
public class OrderTablesMigration : Migration
{
    public override void Up()
    {
        Create.Table(nameof(OrderSaga))
            .WithColumn(nameof(OrderSaga.Id)).AsInt64().PrimaryKey().Identity()
            .WithColumn(nameof(OrderSaga.OrderNumber)).AsInt64().NotNullable()
            .WithColumn(nameof(OrderSaga.OrderDate)).AsDateTime().NotNullable()
            .WithColumn(nameof(OrderSaga.UpdatedDate)).AsDateTime().NotNullable()
            .WithColumn(nameof(OrderSaga.CustomerName)).AsString()
            .WithColumn(nameof(OrderSaga.CustomerSurname)).AsString()
            .WithColumn(nameof(OrderSaga.ShippedDate)).AsDateTime().Nullable();

        Create.Table(nameof(OrderSagaItem))
            .WithColumn(nameof(OrderSagaItem.Id)).AsInt64().NotNullable()
            .WithColumn(nameof(OrderSagaItem.OrderId)).AsInt64().NotNullable()
            .WithColumn(nameof(OrderSagaItem.Sku)).AsInt32().NotNullable()
            .WithColumn(nameof(OrderSagaItem.Price)).AsDecimal().NotNullable()
            .WithColumn(nameof(OrderSagaItem.Quantity)).AsByte().NotNullable();
        
        Create.ForeignKey($"FK_{nameof(OrderSagaItem)}_{nameof(OrderSagaItem.Id)}_{nameof(OrderSaga)}_{nameof(OrderSaga.Id)}")
            .FromTable(nameof(OrderSagaItem)).ForeignColumn(nameof(OrderSagaItem.Id))
            .ToTable(nameof(OrderSaga)).PrimaryColumn(nameof(OrderSaga.Id));
    }

    public override void Down()
    {
        Delete.ForeignKey(
            $"FK_{nameof(OrderSagaItem)}_{nameof(OrderSagaItem.Id)}_{nameof(OrderSaga)}_{nameof(OrderSaga.Id)}");
        Delete.Table(nameof(OrderSaga));
        Delete.Table(nameof(OrderSagaItem));
    }
}
