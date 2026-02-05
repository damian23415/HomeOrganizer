using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IHourlyRateRepository
{
  Task<HourlyRatePeriod?> GetAsync(Guid userId, DateTime date);
  Task<HourlyRatePeriod?> GetGreaterThanAsync(Guid userId, DateTime date);
  Task<IList<HourlyRatePeriod>> GetHourlyRates(Guid userId);
  Task<HourlyRatePeriod?> GetCurrentHourlyRateAsync(Guid userId);
  Task AddAsync(HourlyRatePeriod hourlyRatePeriod);
  Task UpdateAsync(HourlyRatePeriod hourlyRatePeriod);
}