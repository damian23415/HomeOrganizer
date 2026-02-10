using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Commands;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities.Billings;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Domain.ValueObjects;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class CreateHourlyRateCommandHandler(
    IHourlyRateRepository hourlyRateRepository,
    IMapper mapper) : IRequestHandler<CreateHourlyRateCommand, Result<CreateHourlyRateResponse>>
{
  public async Task<Result<CreateHourlyRateResponse>> Handle(CreateHourlyRateCommand request, CancellationToken cancellationToken)
  {
    var existing = await hourlyRateRepository.GetActiveRateOnDate(request.UserId, request.EffectiveFrom);
    
    if (existing != null)
      return Result<CreateHourlyRateResponse>.Failure("Can not set a new hourly rate to backdate.");

    var currentRate = await hourlyRateRepository.GetActiveHourlyRateAsync(request.UserId);
    
    if (currentRate != null)
    {
      currentRate.SetEffectiveTo(request.EffectiveFrom.AddDays(-1));
      await hourlyRateRepository.UpdateAsync(currentRate);
    }
    
    var entity = new HourlyRate(request.UserId, new Money(request.HourlyRate), request.EffectiveFrom);
    
    if (request.EffectiveTo.HasValue)
      entity.SetEffectiveTo(request.EffectiveTo.Value);
    
    await hourlyRateRepository.AddAsync(entity);
    var response = mapper.Map<CreateHourlyRateResponse>(entity);
    
    return Result<CreateHourlyRateResponse>.Success(response);
  }
}