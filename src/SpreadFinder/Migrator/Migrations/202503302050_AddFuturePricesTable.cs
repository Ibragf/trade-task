using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 3, 30, 20, 50)]
public class AddFuturePricesTable : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("future_future_spread")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("contract").AsString(255).NotNullable()
            .WithColumn("first_exchange").AsString(255).NotNullable()
            .WithColumn("second_exchange").AsString(255).NotNullable()
            .WithColumn("spread").AsDouble().NotNullable()
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable();;

        Create.UniqueConstraint()
            .OnTable("future_future_spread")
            .Columns("contract", "first_exchange", "second_exchange");

        Create.Index("future_future_spread_exchanges_idx")
            .OnTable("future_future_spread")
            .OnColumn("first_exchange").Ascending()
            .OnColumn("second_exchange").Ascending();
    }
}