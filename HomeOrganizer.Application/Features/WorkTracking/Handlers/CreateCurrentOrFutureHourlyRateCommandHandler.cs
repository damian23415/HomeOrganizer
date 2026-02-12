using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Commands;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities.Billings;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Domain.ValueObjects;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class CreateCurrentOrFutureHourlyRateCommandHandler(
    IHourlyRateRepository hourlyRateRepository,
    IMapper mapper) : IRequestHandler<CreateCurrentOrFutureHourlyRateCommand, Result<CreateHourlyRateResponse>>
{
  public async Task<Result<CreateHourlyRateResponse>> Handle(CreateCurrentOrFutureHourlyRateCommand request, CancellationToken cancellationToken)
  {
    var currentRate = await hourlyRateRepository.GetActiveHourlyRateForGivenDateAsync(request.UserId, DateTime.UtcNow.Date);

    if (currentRate != null)
    {
      currentRate.SetEffectiveTo(request.EffectiveFrom.Date.AddDays(-1));
      await hourlyRateRepository.UpdateAsync(currentRate);
    }

    if (request.EffectiveTo.HasValue && request.EffectiveTo <= request.EffectiveFrom)
    {
      throw new InvalidOperationException("EffectiveTo must be after EffectiveFrom.");
    }

    var entity = request.EffectiveTo.HasValue
        ? new HourlyRate(request.UserId, new Money(request.HourlyRate), request.EffectiveFrom, request.EffectiveTo.Value)
        : new HourlyRate(request.UserId, new Money(request.HourlyRate), request.EffectiveFrom);
    
    await hourlyRateRepository.AddAsync(entity);

    var response = mapper.Map<CreateHourlyRateResponse>(entity);
    
    return Result<CreateHourlyRateResponse>.Success(response);
  }
}