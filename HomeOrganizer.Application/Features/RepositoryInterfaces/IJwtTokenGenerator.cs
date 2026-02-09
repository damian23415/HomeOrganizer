using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.Users;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IJwtTokenGenerator
{
  string GenerateToken(User user);
}