using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeOrganizer.Infrastructure.Persistence.Migrations;

public static class MigrationRunner
{
    public static IServiceCollection AddMigrations(this IServiceCollection services, string connectionString)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(MigrationRunner).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }
    
    public static void RunMigrations(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        using var scope = serviceProvider.CreateScope();
        
        EnsureDatabaseExists(connectionString!);
        
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
    
    private static void EnsureDatabaseExists(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
    
        builder.InitialCatalog = "master";
    
        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();
    
        var sql = $@"
        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'{databaseName}')
        BEGIN
            CREATE DATABASE [{databaseName}]
        END";
    
        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }
}