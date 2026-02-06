using FluentMigrator;

namespace HomeOrganizer.Infrastructure.Persistence.Migrations;

[Migration(2026013002)]
public class AddColumnsToConfirmEmail : Migration
{
  public override void Up()
  {
    Alter.Table("Users")
        .AddColumn("IsEmailConfirmed").AsBoolean().WithDefaultValue(false)
        .AddColumn("EmailConfirmationToken").AsString().Nullable()
        .AddColumn("EmailConfirmationTokenExpiry").AsDateTime2().Nullable();
  }

  public override void Down()
  {
    Delete.Column("EmailConfirmationTokenExpiry").FromTable("Users");
    Delete.Column("EmailConfirmationToken").FromTable("Users");
    Delete.Column("IsEmailConfirmed").FromTable("Users");
  }
}