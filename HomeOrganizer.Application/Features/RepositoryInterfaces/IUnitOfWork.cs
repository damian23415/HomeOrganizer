using System.Data;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IUnitOfWork
{
  IDbTransaction BeginTransaction();
  void Commit(IDbTransaction transaction);
  void Rollback(IDbTransaction transaction);
}