using HomeOrganizer.Domain.Entities.WorkTracking;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Infrastructure.Repositories.WorkTracking;

public class WorkDayRepository(HomeOrganizerDbContext context) : IWorkDayRepository
{
  public async Task<IList<WorkDay>> GetAllForUserAsync(Guid userId) =>
      await context.WorkDays.Where(x => x.UserId == userId).ToListAsync();

  public async Task<List<WorkDay>> GetAllFromMonthAsync(int year, int month, Guid userId)
  {
    return await context.WorkDays
        .Where(x => x.UserId == userId
                    && x.Date.Year == year
                    && x.Date.Month == month)
        .ToListAsync();
  }

  public async Task AddAsync(WorkDay workDay)
  {
    await context.WorkDays.AddAsync(workDay);
    await context.SaveChangesAsync();
  }
}