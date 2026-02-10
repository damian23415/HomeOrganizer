using AutoMapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Interfaces;
using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class HourlyRateService(IHourlyRateRepository hourlyRateRepository, IWorkDayRepository workDayRepository, IMapper mapper) : IHourlyRateService
{
  public async Task<CreateHourlyRateResponse?> GetCurrentRateAsync(Guid userId, DateTime date)
  {
    var entity = await hourlyRateRepository.GetAsync(userId, date);
    return entity == null ? null : mapper.Map<CreateHourlyRateResponse>(entity);
  }

  public async Task<IList<HourlyRatesResponse>> GetHourlyRates(Guid userId)
  {
    var allHourlyRates = await hourlyRateRepository.GetHourlyRates(userId);

    if (!allHourlyRates.Any())
      return [];
    
    var currentRate = allHourlyRates.First();
    
    if (currentRate.EffectiveTo != null)
      throw new InvalidOperationException("The current hourly rate should not have an end date");
    
    var workDays = await workDayRepository.GetAllForUserAsync(userId);

    var rates = allHourlyRates.Select(entity => 
    {
      var matchingDays = workDays.Where(wd => 
          wd.Date >= entity.EffectiveFrom &&
          (entity.EffectiveTo == null || wd.Date <= entity.EffectiveTo)).ToList();

      return new HourlyRatesResponse
      {
          RatePerHour = entity.Rate,
          EffectiveFrom = entity.EffectiveFrom,
          EffectiveTo = entity.EffectiveTo,
          IsCurrentRate = entity.EffectiveTo == null,
          TotalHours = matchingDays.Sum(wd => wd.TotalHours),
          TotalEarnings = matchingDays.Sum(wd => wd.TotalEarnings)
      };
    }).ToList();
    
    return rates;
  }
}