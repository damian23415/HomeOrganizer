using HomeOrganizer.Api.Extensions;
using HomeOrganizer.Api.Middleware;
using HomeOrganizer.Api.User;
using HomeOrganizer.Application;
using HomeOrganizer.Infrastructure;
using HomeOrganizer.Infrastructure.Persistence.Migrations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi();

var app = builder.Build();

app.Services.RunMigrations(builder.Configuration);
app.UseErrorHandling();
app.MapOpenApi();

app.MapUserEndpoints();
app.MapScalarApiReference();

app.Run();


public partial class Program
{
}