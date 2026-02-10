using HomeOrganizer.Domain.Entities.Users;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Infrastructure.Repositories.Users;

public class UserRepository(HomeOrganizerDbContext context) : IUserRepository
{
  public async Task<User?> GetByEmailAsync(string email) => await context.Users.SingleOrDefaultAsync(u => u.Email.Value == email);
  
  public async Task<User?> GetByEmailConfirmationTokenAsync(string token) => await context.Users.SingleOrDefaultAsync(u => u.EmailConfirmationToken == token);
  
  public async Task AddAsync(User user)
  {
    context.Users.Add(user);
    await context.SaveChangesAsync();
  }

  public async Task UpdateAsync(User user)
  {
    context.Users.Update(user);
    await context.SaveChangesAsync();
  }
}