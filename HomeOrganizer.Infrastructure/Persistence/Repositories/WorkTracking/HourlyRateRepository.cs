using System.Data;
using Dapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Infrastructure.Persistence.Repositories.WorkTracking;

public class HourlyRateRepository(IDbConnection dbConnection) : IHourlyRateRepository
{
  public async Task<HourlyRate?> GetAsync(Guid userId, DateTime date)
  {
    const string sql = @"
      SELECT *
      FROM ""HourlyRatePeriods""
      WHERE ""UserId"" = @UserId
        AND ""EffectiveFrom"" <= @CurrentDate
        AND (""EffectiveTo"" IS NULL OR ""EffectiveTo"" >= @CurrentDate)
      ORDER BY ""EffectiveFrom"" DESC
      LIMIT 1";

    return await dbConnection.QuerySingleOrDefaultAsync<HourlyRate>(sql,
      new { UserId = userId, CurrentDate = date });
  }

  public async Task<HourlyRate?> GetGreaterThanAsync(Guid userId, DateTime date)
  {
    const string sql = @"
      SELECT *
      FROM ""HourlyRatePeriods""
      WHERE ""UserId"" = @UserId
        AND ""EffectiveFrom"" >= @Date
      ORDER BY ""EffectiveFrom"" ASC
      LIMIT 1";
    
    return await dbConnection.QuerySingleOrDefaultAsync<HourlyRate>(sql,
        new { UserId = userId, Date = date });
  }

  public async Task<IList<HourlyRate>> GetHourlyRates(Guid userId)
  {
    const string sql = @"
      SELECT *
      FROM ""HourlyRatePeriods""
      WHERE ""UserId"" = @UserId
      ORDER BY ""EffectiveFrom"" DESC";
    
    return (await dbConnection.QueryAsync<HourlyRate>(sql, new { UserId = userId })).ToList();
  }

  public async Task<HourlyRate?> GetCurrentHourlyRateAsync(Guid userId)
  {
    const string sql = @"
      SELECT *
      FROM ""HourlyRatePeriods""
      WHERE ""UserId"" = @UserId
      AND ""EffectiveTo"" IS NULL";
    
    return await dbConnection.QuerySingleOrDefaultAsync<HourlyRate>(sql, new { UserId = userId });
  }

  public async Task AddAsync(HourlyRate hourlyRate)
  {
    const string sql = @"
      INSERT INTO ""HourlyRatePeriods"" (""Id"", ""UserId"", ""HourlyRate"", ""EffectiveFrom"", ""EffectiveTo"")
      VALUES (@Id, @UserId, @HourlyRate, @EffectiveFrom, @EffectiveTo);";
    
    await dbConnection.ExecuteAsync(sql, hourlyRate);
  }

  public async Task UpdateAsync(HourlyRate hourlyRate)
  {
    const string sql = @"
      UPDATE ""HourlyRatePeriods""
      SET ""HourlyRate"" = @HourlyRate,
          ""EffectiveFrom"" = @EffectiveFrom,
          ""EffectiveTo"" = @EffectiveTo
      WHERE ""Id"" = @Id;";
    
    await dbConnection.ExecuteAsync(sql, hourlyRate);
  }
}