using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Domain.Interfaces;

public interface IHourlyRateRepository
{
  Task<HourlyRate?> GetActiveRateOnDate(Guid userId, DateTime givenEffectiveFrom);
  Task<HourlyRate?> GetActiveHourlyRateAsync(Guid userId);
  Task UpdateAsync(HourlyRate hourlyRate);
  Task AddAsync(HourlyRate hourlyRate);
}