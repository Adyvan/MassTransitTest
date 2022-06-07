using FluentMigrator;
using Models;

namespace Database.Migrations;

[Migration(1)]
public class OrderAddStatePropertiesMigration : Migration
{
    public override void Up()
    {
        Alter.Table(nameof(OrderSaga))
            .AddColumn(nameof(OrderSaga.CurrentState)).AsInt16().Nullable();
    }

    public override void Down()
    {
        Delete.Column(nameof(OrderSaga.CurrentState)).FromTable(nameof(OrderSaga));
    }
}