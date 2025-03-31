using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 3, 30, 20, 50)]
public class AddFuturePricesTable : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("future_prices")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("contract").AsString(255).NotNullable()
            .WithColumn("exchange_name").AsString(255).NotNullable()
            .WithColumn("price").AsString(255).NotNullable()
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable();;

        Create.Index("future_prices_exchanges_idx")
            .OnTable("future_prices")
            .OnColumn("updated_at").Ascending()
            .OnColumn("contract").Ascending();
    }
}