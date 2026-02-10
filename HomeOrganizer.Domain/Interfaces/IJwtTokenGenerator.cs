using HomeOrganizer.Domain.Entities.Users;

namespace HomeOrganizer.Domain.Interfaces;

public interface IJwtTokenGenerator
{
  string GenerateToken(User user);
}