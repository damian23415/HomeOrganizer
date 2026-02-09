using System.Data;
using Dapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.Messages;

namespace HomeOrganizer.Infrastructure.Persistence.Repositories;

public class EmailRepository(IDbConnection connection) : IEmailRepository
{
  public async Task SaveOutgoingMessageAsync(OutgoingMessage message, IDbTransaction transaction)
  {
    const string query = @"INSERT INTO ""OutgoingMessages"" (""RecipientEmail"", ""Subject"", ""Body"", ""Created"", ""HandledAt"") 
                VALUES (@RecipientEmail, @Subject, @Body, @Created, @HandledAt)";
    
    await connection.ExecuteAsync(query, new
    {
      RecipientEmail = message.RecipientEmail,
      Subject = message.Subject,
      Body = message.Body,
      Created = message.Created,
      HandledAt = message.HandledAt,
    }
    , transaction);
  }

  public async Task UpdateOutgoingMessageAsync(OutgoingMessage message)
  {
    const string query = @"UPDATE ""OutgoingMessages"" 
                SET ""RecipientEmail"" = @RecipientEmail, ""Subject"" = @Subject, ""Body"" = @Body, ""Created"" = @Created, ""HandledAt"" = @HandledAt
                WHERE ""Id"" = @Id";
    
    await connection.ExecuteAsync(query, new
    {
      Id = message.Id,
      RecipientEmail = message.RecipientEmail,
      Subject = message.Subject,
      Body = message.Body,
      Created = message.Created,
      HandledAt = message.HandledAt,
    });
  }
}