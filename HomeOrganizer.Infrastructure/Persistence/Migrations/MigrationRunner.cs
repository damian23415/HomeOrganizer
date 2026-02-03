using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace HomeOrganizer.Infrastructure.Persistence.Migrations;

public static class MigrationRunner
{
  public static void AddMigrations(this IServiceCollection services, string connectionString)
  {
    services
      .AddFluentMigratorCore()
      .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations())
      .AddLogging(lb => lb.AddFluentMigratorConsole());
  }

  public static void RunMigrations(this IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
  }
}