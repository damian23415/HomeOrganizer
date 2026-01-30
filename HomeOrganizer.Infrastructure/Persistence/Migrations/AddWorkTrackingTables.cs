using FluentMigrator;

namespace HomeOrganizer.Infrastructure.Persistence.Migrations;

[Migration(20260130)]
public class AddWorkTrackingTables : Migration
{
  public const string HourlyRatePeriods = nameof(HourlyRatePeriods);
  public const string WorkDays = nameof(WorkDays);

  public override void Up()
  {
    Create.Table(HourlyRatePeriods)
      .WithColumn("Id").AsGuid().PrimaryKey()
      .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("Users", "Id")
      .WithColumn("HourlyRate").AsDecimal().NotNullable()
      .WithColumn("EffectiveFrom").AsDateTime().NotNullable()
      .WithColumn("EffectiveTo").AsDateTime().Nullable();

    Create.Table(WorkDays)
      .WithColumn("Id").AsGuid().PrimaryKey()
      .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("Users", "Id")
      .WithColumn("Date").AsDateTime().NotNullable()
      .WithColumn("StartTime").AsDateTime().NotNullable()
      .WithColumn("EndTime").AsDateTime().NotNullable()
      .WithColumn("TotalHours").AsInt32().NotNullable()
      .WithColumn("HourlyRateUsed").AsDecimal().NotNullable()
      .WithColumn("TotalEarnings").AsDecimal().NotNullable();
  }

  public override void Down()
  {
    Delete.Table(HourlyRatePeriods);
    Delete.Table(WorkDays);
  }
}