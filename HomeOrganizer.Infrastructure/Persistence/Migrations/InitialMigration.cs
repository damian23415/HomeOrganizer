using FluentMigrator;

namespace HomeOrganizer.Infrastructure.Persistence.Migrations;

[Migration(2025101401)]
public class InitialMigration : Migration
{
  public override void Up()
  {
    Create.Table("Users")
      .WithColumn("Id").AsGuid().PrimaryKey()
      .WithColumn("Email").AsString(255).NotNullable().Unique()
      .WithColumn("PasswordHash").AsString(1000).NotNullable()
      .WithColumn("Created").AsDateTime().NotNullable()
      .WithColumn("IsActive").AsBoolean().NotNullable()
      .WithColumn("Role").AsString(50).NotNullable();
  }

  public override void Down()
  {
    Delete.Table("Users");
  }
}