using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeOrganizer.Domain.Entities.Users;
using HomeOrganizer.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HomeOrganizer.Infrastructure.Services;

public class JwtTokenGenerator(IConfiguration config) : IJwtTokenGenerator
{
  public string GenerateToken(User user)
  {
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var issuer = config["Jwt:Issuer"];
    var audience = config["Jwt:Audience"];

    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.Now.AddMinutes(int.Parse(config["Jwt:ExpiryMinutes"]!)),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}