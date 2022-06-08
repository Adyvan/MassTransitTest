using FluentMigrator;
using Models;

namespace Database.Migrations;

[Migration(0)]
public class OrderTablesMigration : Migration
{
    public override void Up()
    {
        Create.Table(nameof(Order))
            .WithColumn(nameof(Order.Id)).AsInt64().Identity().PrimaryKey()
            .WithColumn(nameof(Order.OrderNumber)).AsInt64().NotNullable()
            .WithColumn(nameof(Order.OrderDate)).AsDateTime().NotNullable()
            .WithColumn(nameof(Order.UpdatedDate)).AsDateTime().NotNullable()
            .WithColumn(nameof(Order.CustomerName)).AsString()
            .WithColumn(nameof(Order.CustomerSurname)).AsString()
            .WithColumn(nameof(Order.ShippedDate)).AsDateTime().Nullable();

        Create.Table(nameof(OrderItem))
            .WithColumn(nameof(OrderItem.Id)).AsInt64().Identity().PrimaryKey()
            .WithColumn(nameof(OrderItem.OrderId)).AsInt64().NotNullable()
            .WithColumn(nameof(OrderItem.Sku)).AsInt32().NotNullable()
            .WithColumn(nameof(OrderItem.Price)).AsDecimal().NotNullable()
            .WithColumn(nameof(OrderItem.Quantity)).AsByte().NotNullable();
        
        Create.ForeignKey($"FK_{nameof(OrderItem)}_{nameof(OrderItem.Id)}_{nameof(Order)}_{nameof(Order.Id)}")
            .FromTable(nameof(OrderItem)).ForeignColumn(nameof(OrderItem.Id))
            .ToTable(nameof(Order)).PrimaryColumn(nameof(Order.Id));
    }

    public override void Down()
    {
        Delete.ForeignKey(
            $"FK_{nameof(OrderItem)}_{nameof(OrderItem.Id)}_{nameof(Order)}_{nameof(Order.Id)}");
        Delete.Table(nameof(Order));
        Delete.Table(nameof(OrderItem));
    }
}
