using FluentMigrator;

namespace HomeOrganizer.Infrastructure.Persistence.Migrations;

[Migration(2026013003)]
public class AddOutgoingMessageTable : Migration
{
  public override void Up()
  {
    Create.Table("OutgoingMessage")
        .WithColumn("Id").AsGuid().PrimaryKey()
        .WithColumn("RecipientEmail").AsString().NotNullable()
        .WithColumn("Subject").AsString().NotNullable()
        .WithColumn("Body").AsString().NotNullable()
        .WithColumn("Created").AsDateTime().NotNullable()
        .WithColumn("HandledAt").AsDateTime().Nullable();
  }

  public override void Down()
  {
    Delete.Table("OutgoingMessage");
  }
}