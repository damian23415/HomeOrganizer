using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Domain.Interfaces;

public interface IHourlyRateRepository
{
  Task<HourlyRate?> GetActiveRateOnDate(Guid userId, DateTime givenEffectiveFrom);
  Task<HourlyRate?> GetActiveHourlyRateAsync(Guid userId);
  Task<List<HourlyRate>> GetAllHourlyRatesAsync(Guid userId);
  Task<HourlyRate?> GetAsync(Guid userId, DateTime givenDate);
  Task UpdateAsync(HourlyRate hourlyRate);
  Task AddAsync(HourlyRate hourlyRate);
}