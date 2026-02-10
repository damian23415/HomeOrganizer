using HomeOrganizer.Domain.Entities.Users;

namespace HomeOrganizer.Domain.Interfaces;

public interface IUserRepository
{
  Task<User?> GetByEmailAsync(string email);
  Task AddAsync(User user);
  Task UpdateAsync(User user);
  Task<User?> GetByEmailConfirmationTokenAsync(string token);
}