using AutoMapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Interfaces;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class HourlyRateService(IHourlyRateRepository hourlyRateRepository, IMapper mapper) : IHourlyRateService
{
  public async Task<HourlyRateResponse> CreateHourlyRateAsync(CreateHourlyRateRequest request, Guid userId)
  {
    var actualDate = await hourlyRateRepository.GetAsync(userId, request.StartDate);
    
    if (actualDate != null)
      throw new InvalidOperationException("An hourly rate already exists for the given date");
    
    var entity = mapper.Map<Domain.Entities.HourlyRatePeriod>(request);
    entity.UserId = userId;
    
    
    await hourlyRateRepository.AddAsync(entity);
    return mapper.Map<HourlyRateResponse>(entity);
  }

  public async Task<HourlyRateResponse?> GetCurrentRateAsync(Guid userId, DateTime date)
  {
    var entity = await hourlyRateRepository.GetAsync(userId, date);
    return entity == null ? null : mapper.Map<HourlyRateResponse>(entity);
  }
}