using HomeOrganizer.Domain.Entities.WorkTracking;

namespace HomeOrganizer.Domain.Interfaces;

public interface IWorkDayRepository
{
  Task<IList<WorkDay>> GetAllForUserAsync(Guid userId);
  Task<List<WorkDay>> GetAllFromMonthAsync(int year, int month, Guid userId);
  Task AddAsync(WorkDay workDay);
}