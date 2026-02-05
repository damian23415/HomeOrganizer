using HomeOrganizer.Application.Features.WorkTracking.Dtos;

namespace HomeOrganizer.Application.Features.WorkTracking.Interfaces;

public interface IHourlyRateService
{
  Task<HourlyRateResponse> CreateHourlyRateAsync(CreateHourlyRateRequest request, Guid userId);
  Task<HourlyRateResponse?> GetCurrentRateAsync(Guid userId, DateTime date);
  Task<IList<HourlyRatesResponse>> GetHourlyRates(Guid userId);
}