using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Domain.Interfaces;

public interface IHourlyRateRepository
{
  Task<HourlyRate?> GetActiveRateOnHistoricalDateAsync(Guid userId, DateTime givenEffectiveFrom);
  Task<HourlyRate?> GetActiveHourlyRateForGivenDateAsync(Guid userId, DateTime now);
  Task<List<HourlyRate>> GetHourlyRatesByUserIdAsync(Guid userId);
  Task<HourlyRate?> GetAsync(Guid userId, DateTime givenDate);
  Task<bool> HasOverlappingRateAsync(Guid userId, DateTime from, DateTime to);
  Task UpdateAsync(HourlyRate hourlyRate);
  Task AddAsync(HourlyRate hourlyRate);
}