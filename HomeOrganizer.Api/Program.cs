using HomeOrganizer.Application;
using HomeOrganizer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

app.Run();