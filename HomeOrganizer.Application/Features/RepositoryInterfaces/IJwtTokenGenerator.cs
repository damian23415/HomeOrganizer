using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}