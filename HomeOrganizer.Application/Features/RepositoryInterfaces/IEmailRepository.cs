using System.Data;
using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.Messages;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IEmailRepository
{
  Task SaveOutgoingMessageAsync(OutgoingMessage message, IDbTransaction transaction);
  Task UpdateOutgoingMessageAsync(OutgoingMessage message);
}