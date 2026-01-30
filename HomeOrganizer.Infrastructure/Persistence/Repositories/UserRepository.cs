using System.Data;
using Dapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Infrastructure.Persistence.Repositories;

public class UserRepository(IDbConnection connection) : IUserRepository
{
  public async Task<User?> GetByEmail(string email)
  {
    const string sql = "SELECT * FROM Users WHERE Email = @Email";
    return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
  }

  public async Task AddAsync(User user)
  {
    const string sql = @"INSERT INTO Users (Id, Email, PasswordHash, Created, IsActive, Role)
                             VALUES (@Id, @Email, @PasswordHash, @Created, @IsActive, @Role)";

    user.Id = Guid.NewGuid();
    await connection.ExecuteAsync(sql, user);
  }
}