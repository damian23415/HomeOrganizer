using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task AddAsync(User user);
}