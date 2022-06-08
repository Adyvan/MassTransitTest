using FluentMigrator;
using Models;

namespace Database.Migrations;

[Migration(1)]
public class OrderAddStatePropertiesMigration : Migration
{
    public override void Up()
    {
        Alter.Table(nameof(Order))
            .AddColumn(nameof(Order.CurrentState)).AsInt16().Nullable();
    }

    public override void Down()
    {
        Delete.Column(nameof(Order.CurrentState)).FromTable(nameof(Order));
    }
}