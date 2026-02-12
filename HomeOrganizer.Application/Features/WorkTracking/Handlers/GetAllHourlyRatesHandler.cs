using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Queries;
using HomeOrganizer.Domain.Interfaces;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class GetAllHourlyRatesHandler(
    IHourlyRateRepository hourlyRateRepository,
    IWorkDayRepository workDayRepository) : IRequestHandler<GetAllHourlyRatesQuery, Result<IList<GetAllHourlyRatesResponse>>>
{
  public async Task<Result<IList<GetAllHourlyRatesResponse>>> Handle(GetAllHourlyRatesQuery request, CancellationToken cancellationToken)
  {
    var allHourlyRates = await hourlyRateRepository.GetHourlyRatesByUserIdAsync(request.UserId);
    
    if (!allHourlyRates.Any())
      return Result<IList<GetAllHourlyRatesResponse>>.Failure("No hourly rates found for the user.");

    var currentRate = allHourlyRates.First();
    
    var workDays = await workDayRepository.GetAllForUserAsync(request.UserId);

    var rates = allHourlyRates.Select(entity =>
    {
      var matchingDays = workDays.Where(wd =>
          wd.Date >= entity.EffectiveFrom &&
          (entity.EffectiveTo == null || wd.Date <= entity.EffectiveTo)).ToList();

      return new GetAllHourlyRatesResponse
      {
          Rate = entity.Rate,
          EffectiveFrom = entity.EffectiveFrom,
          EffectiveTo = entity.EffectiveTo,
          IsCurrentRate = entity == currentRate,
          TotalHours = matchingDays.Sum(wd => wd.TotalHours),
          TotalEarnings = matchingDays.Sum(wd => wd.TotalEarnings)
      };
    }).ToList();
    
    return Result<IList<GetAllHourlyRatesResponse>>.Success(rates);
  }
}