using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.WorkTracking.Interfaces;

public interface IWorkDayService
{
  Task<WorkDayResponse> CreateWorkDayAsync(CreateWorkDayRequest request, Guid userId);
  Task<IEnumerable<WorkDayResponse>> GetWorkDaysByMonthAsync(int year, int month, Guid userId);
}