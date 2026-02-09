using System.Data;
using HomeOrganizer.Application.Features.RepositoryInterfaces;

namespace HomeOrganizer.Infrastructure;

public class UnitOfWork(IDbConnection connection) : IUnitOfWork
{
  public IDbTransaction BeginTransaction()
  {
    if (connection.State != ConnectionState.Open)
      connection.Open();

    return connection.BeginTransaction();
  }
  
  public void Commit(IDbTransaction transaction)
  {
    transaction.Commit();
    transaction.Dispose();
  }

  public void Rollback(IDbTransaction transaction)
  {
    try
    {
      transaction.Rollback();
    }
    finally
    {
      transaction?.Dispose();
    }
  }
}