using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace HomeOrganizer.Infrastructure.Security;

public class JwtTokenGenerator(JwtSettings settings) : IJwtTokenGenerator
{
  public string GenerateToken(User user)
  {
    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim(ClaimTypes.Role, user.Role)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      settings.Issuer,
      settings.Audience,
      claims,
      expires: DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}