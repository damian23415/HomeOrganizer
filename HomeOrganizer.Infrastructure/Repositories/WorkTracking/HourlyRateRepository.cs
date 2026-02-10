using HomeOrganizer.Domain.Entities.Billings;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Infrastructure.Repositories.WorkTracking;

public class HourlyRateRepository(HomeOrganizerDbContext context) : IHourlyRateRepository
{
  public async Task<HourlyRate?> GetActiveRateOnDate(Guid userId, DateTime givenEffectiveFrom)
  {
    return await context.HourlyRates
        .FirstOrDefaultAsync(x => x.UserId == userId 
                                  && x.EffectiveFrom <= givenEffectiveFrom 
                                  && (x.EffectiveTo == null || x.EffectiveTo >= givenEffectiveFrom));
  }

  public async Task<HourlyRate?> GetActiveHourlyRateAsync(Guid userId)
  {
    return await context.HourlyRates
        .SingleOrDefaultAsync(x => x.UserId == userId
                                  && x.EffectiveTo == null);
  }

  public async Task UpdateAsync(HourlyRate hourlyRate)
  {
    context.HourlyRates.Update(hourlyRate);
    await context.SaveChangesAsync();
  }

  public async Task AddAsync(HourlyRate hourlyRate)
  {
    context.HourlyRates.Add(hourlyRate);
    await context.SaveChangesAsync();
  }
}