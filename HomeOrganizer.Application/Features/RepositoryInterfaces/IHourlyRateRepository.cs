using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IHourlyRateRepository
{
  Task<HourlyRatePeriod?> GetAsync(Guid userId, DateTime currentDate);
  Task AddAsync(HourlyRatePeriod hourlyRatePeriod);
}