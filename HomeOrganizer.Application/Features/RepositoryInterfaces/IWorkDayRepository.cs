using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Features.RepositoryInterfaces;

public interface IWorkDayRepository
{
  Task<List<WorkDay>> GetAllFromMonth(int year, int month, Guid userId);
  Task AddAsync(WorkDay workDay);
}