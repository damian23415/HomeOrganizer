using System.Data;
using Dapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Infrastructure.Persistence.Repositories.WorkTracking;

public class WorkDayRepository(IDbConnection connection) : IWorkDayRepository
{
  public async Task<List<WorkDay>> GetAllFromMonth(int year, int month, Guid userId)
  {
    const string sql = @"SELECT * FROM ""WorkDays"" 
        WHERE EXTRACT(YEAR FROM ""Date"") = @Year 
          AND EXTRACT(MONTH FROM ""Date"") = @Month
          AND ""UserId"" = @UserId";
    
    return (await connection.QueryAsync<WorkDay>(sql, new
    {
      Year = year,
      Month = month,
      UserId = userId
    })).ToList();
  }

  public async Task AddAsync(WorkDay workDay)
  {
    const string sql = @"
      INSERT INTO ""WorkDays"" (""Id"", ""UserId"", ""Date"", ""StartTime"", ""EndTime"", ""TotalHours"", ""HourlyRateUsed"", ""TotalEarnings"")
      VALUES (@Id, @UserId, @Date, @StartTime, @EndTime, @TotalHours, @HourlyRateUsed, @TotalEarnings);";
    
    await connection.ExecuteAsync(sql, workDay);
  }
}