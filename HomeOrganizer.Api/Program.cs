using System.Text;
using HomeOrganizer.Api.Extensions;
using HomeOrganizer.Api.Middleware;
using HomeOrganizer.Api.User;
using HomeOrganizer.Api.WorkTracking;
using HomeOrganizer.Application;
using HomeOrganizer.Infrastructure;
using HomeOrganizer.Infrastructure.Persistence.Migrations;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.Services.RunMigrations();
app.UseErrorHandling();
app.MapOpenApi();

app.UseCors("AllowVueApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapWorkTrackingEndpoints();
app.MapScalarApiReference();

app.Run();

public partial class Program
{
}