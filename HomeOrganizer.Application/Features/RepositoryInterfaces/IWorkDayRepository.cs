using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.WorkTracking;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IWorkDayRepository
{
  Task<List<WorkDay>> GetAllFromMonthAsync(int year, int month, Guid userId);
  Task<List<WorkDay>> GetAllForUserAsync(Guid userId);
  Task AddAsync(WorkDay workDay);
}