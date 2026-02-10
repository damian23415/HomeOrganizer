using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HomeOrganizer.Api.Extensions;

public static class ApiExtensions
{
  public const string CorsPolicyName = "AllowedOrigins";
  public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddOpenApi();
    services.AddCorsPolicy(configuration);
    services.AddJwtAuthentication(configuration);
    services.AddAuthorization();
    
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

    
    return services;
  }

  private static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
  {
    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

    services.AddCors(options =>
    {
      options.AddPolicy("AllowedOrigins", policy =>
      {
        if (allowedOrigins.Length > 0)
        {
          policy.WithOrigins(allowedOrigins);
        }
        else
        {
          policy.WithOrigins("http://localhost:5087");
        }

        policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
      });
    });
  }
  
  private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
  {
    var jwtSettings = configuration.GetSection("Jwt");
    var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Secret is not configured");

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
        options.RequireHttpsMetadata = false; // TODO: Enable in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
          ValidateIssuer = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidateAudience = true,
          ValidAudience = jwtSettings["Audience"],
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        };
      });
  }
}