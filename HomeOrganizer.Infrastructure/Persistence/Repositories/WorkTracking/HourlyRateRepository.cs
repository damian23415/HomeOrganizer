using System.Data;
using Dapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Infrastructure.Persistence.Repositories.WorkTracking;

public class HourlyRateRepository(IDbConnection dbConnection) : IHourlyRateRepository
{
  public async Task<HourlyRatePeriod?> GetAsync(Guid userId, DateTime currentDate)
  {
    const string sql = @"
      SELECT *
      FROM HourlyRatePeriods
      WHERE UserId = @UserId
        AND StartDate <= @CurrentDate
        AND (EndDate IS NULL OR EndDate >= @CurrentDate)
      ORDER BY StartDate DESC
      LIMIT 1;";

    return await dbConnection.QuerySingleOrDefaultAsync<HourlyRatePeriod>(sql,
      new { UserId = userId, CurrentDate = currentDate });
  }

  public async Task AddAsync(HourlyRatePeriod hourlyRatePeriod)
  {
    const string sql = @"
      INSERT INTO HourlyRatePeriods (Id, UserId, HourlyRate, EffectiveFrom, EffectiveTo)
      VALUES (@Id, @UserId, @HourlyRate, @EffectiveFrom, @EffectiveTo);";
    
    await dbConnection.ExecuteAsync(sql, hourlyRatePeriod);
  }
}