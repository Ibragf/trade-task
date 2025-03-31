using FluentMigrator;

namespace Migrator.Migrations;

[TimestampedMigration(2025, 3, 30, 21, 10)]
public class AddOutboxTable : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.Table("outbox")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("topic").AsString(255).NotNullable()
            .WithColumn("key").AsInt64().NotNullable()
            .WithColumn("payload").AsString().NotNullable()
            .WithColumn("ts").AsDateTimeOffset().NotNullable()
            .WithColumn("processed").AsBoolean().Nullable();
    }
}