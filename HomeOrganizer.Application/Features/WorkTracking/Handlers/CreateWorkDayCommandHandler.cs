using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Commands;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities.WorkTracking;
using HomeOrganizer.Domain.Interfaces;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class CreateWorkDayCommandHandler(
    IHourlyRateRepository hourlyRateRepository,
    IWorkDayRepository workDayRepository,
    IMapper mapper) : IRequestHandler<CreateWorkDayCommand, Result<WorkDayResponse>>
{
  public async Task<Result<WorkDayResponse>> Handle(CreateWorkDayCommand request, CancellationToken cancellationToken)
  {
    var hourlyRate = await hourlyRateRepository.GetAsync(request.UserId, request.Date);
    
    if (hourlyRate == null)
      return Result<WorkDayResponse>.Failure("No hourly rate found for the specified date. Please set an hourly rate before logging work hours.");

    
    var workDay = new WorkDay(request.UserId, request.Date, request.StartTime, request.EndTime, hourlyRate.Rate);
    await workDayRepository.AddAsync(workDay);
    
    var response = mapper.Map<WorkDayResponse>(workDay);
    
    return Result<WorkDayResponse>.Success(response);
  }
}