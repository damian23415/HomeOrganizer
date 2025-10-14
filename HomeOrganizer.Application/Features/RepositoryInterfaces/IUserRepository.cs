using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task AddAsync(User user);
}