using System.Data;
using Dapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Infrastructure.Persistence.Repositories;

public class UserRepository(IDbConnection connection) : IUserRepository
{
  public async Task<User?> GetByEmail(string email)
  {
    const string sql = "SELECT * FROM \"Users\" WHERE \"Email\" = @Email";
    return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
  }

  public async Task AddAsync(User user)
  {
    const string sql = @"INSERT INTO ""Users"" (""Id"", ""Email"", ""PasswordHash"", ""Created"", ""IsActive"", ""Role"", ""IsEmailConfirmed"", ""EmailConfirmationToken"", ""EmailConfirmationTokenExpiry"")
                             VALUES (@Id, @Email, @PasswordHash, @Created, @IsActive, @Role, @IsEmailConfirmed, @EmailConfirmationToken, @EmailConfirmationTokenExpiry)";

    await connection.ExecuteAsync(sql, user);
  }

  public async Task UpdateAsync(User user)
  {
    const string sql = @"UPDATE ""Users"" SET 
                             ""Email"" = @Email, 
                             ""PasswordHash"" = @PasswordHash, 
                             ""IsActive"" = @IsActive, 
                             ""Role"" = @Role, 
                             ""IsEmailConfirmed"" = @IsEmailConfirmed, 
                             ""EmailConfirmationToken"" = @EmailConfirmationToken, 
                             ""EmailConfirmationTokenExpiry"" = @EmailConfirmationTokenExpiry
                           WHERE ""Id"" = @Id";

    await connection.ExecuteAsync(sql, user);
  }

  public async Task<User?> GetUserByTokenAsync(string token)
  {
    const string sql = "SELECT * FROM \"Users\" WHERE \"EmailConfirmationToken\" = @Token";
    
    return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Token = token });
  }
}