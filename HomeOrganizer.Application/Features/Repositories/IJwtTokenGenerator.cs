using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.Repositories;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}