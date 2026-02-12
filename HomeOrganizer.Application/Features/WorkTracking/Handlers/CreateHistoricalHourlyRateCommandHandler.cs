using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Commands;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities.Billings;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Domain.ValueObjects;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class CreateHistoricalHourlyRateCommandHandler(
    IHourlyRateRepository hourlyRateRepository,
    IMapper mapper) : IRequestHandler<CreateHistoricalHourlyRateCommand, Result<CreateHourlyRateResponse>>
{
  public async Task<Result<CreateHourlyRateResponse>> Handle(CreateHistoricalHourlyRateCommand request, CancellationToken cancellationToken)
  {
    if (!request.EffectiveTo.HasValue)
    {
      throw new InvalidOperationException("EffectiveTo is required for historical rates.");
    }
    
    var currentRate = await hourlyRateRepository.GetActiveHourlyRateForGivenDateAsync(request.UserId, DateTime.UtcNow.Date);

    if (request.EffectiveTo >= currentRate?.EffectiveFrom)
    {
      throw new InvalidOperationException("EffectiveTo must be before the current rate's EffectiveFrom.");
    }
    
    var hasOverlap = await hourlyRateRepository.HasOverlappingRateAsync(request.UserId, request.EffectiveFrom, request.EffectiveTo.Value);
    
    if (hasOverlap)
    {
      throw new InvalidOperationException("A rate already exists for this period.");
    }

    var entity = new HourlyRate(request.UserId, new Money(request.HourlyRate), request.EffectiveFrom, request.EffectiveTo.Value);
    await hourlyRateRepository.AddAsync(entity);
    
    var response = mapper.Map<CreateHourlyRateResponse>(entity);

    return Result<CreateHourlyRateResponse>.Success(response);
  }
}