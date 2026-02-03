using HomeOrganizer.Api.Extensions;
using HomeOrganizer.Application;
using HomeOrganizer.Infrastructure;
using HomeOrganizer.Infrastructure.Persistence.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

app.Services.RunMigrations();

app.MapApiEndpoints();

app.Run();

public partial class Program
{
}