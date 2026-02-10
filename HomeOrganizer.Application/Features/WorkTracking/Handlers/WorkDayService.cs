using AutoMapper;
using HomeOrganizer.Application.Features.RepositoryInterfaces;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Interfaces;
using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.WorkTracking;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class WorkDayService(IWorkDayRepository workDayRepository, IHourlyRateRepository hourlyRateRepository, IMapper mapper) : IWorkDayService
{
  public async Task<WorkDayResponse> CreateWorkDayAsync(CreateWorkDayRequest request, Guid userId)
  {
    var hourlyRate = await hourlyRateRepository.GetAsync(userId, request.Date);
    
    if (hourlyRate == null)
      throw new InvalidOperationException("Nie znaleziono stawki godzinowej dla podanej daty.");
    
    var workDay = mapper.Map<WorkDay>(request);
    workDay.UserId = userId;
    workDay.HourlyRateUsed = hourlyRate.Rate;
    workDay.TotalEarnings = workDay.TotalHours * hourlyRate.Rate;
    
    await workDayRepository.AddAsync(workDay);
    
    return mapper.Map<WorkDayResponse>(workDay);
  }

  public async Task<IEnumerable<WorkDayResponse>> GetWorkDaysByMonthAsync(int year, int month, Guid userId)
  {
    var response = await workDayRepository.GetAllFromMonthAsync(year, month, userId);
    return mapper.Map<IEnumerable<WorkDayResponse>>(response);
  }
}