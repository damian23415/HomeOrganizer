using HomeOrganizer.Domain.Entities.Billings;
using HomeOrganizer.Domain.Interfaces;
using HomeOrganizer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Infrastructure.Repositories.WorkTracking;

public class HourlyRateRepository(HomeOrganizerDbContext context) : IHourlyRateRepository
{
  public async Task<HourlyRate?> GetActiveRateOnHistoricalDateAsync(Guid userId, DateTime givenEffectiveFrom)
  {
    return await context.HourlyRates
        .FirstOrDefaultAsync(x => x.UserId == userId 
                                  && x.EffectiveFrom <= givenEffectiveFrom 
                                  &&  x.EffectiveTo >= givenEffectiveFrom);
  }

  public async Task<HourlyRate?> GetActiveHourlyRateForGivenDateAsync(Guid userId, DateTime now)
  {
    return await context.HourlyRates
        .SingleOrDefaultAsync(x => x.UserId == userId
                                   && x.EffectiveFrom <= now
                                  && (x.EffectiveTo == null || x.EffectiveTo >= now));
  }

  public async Task<List<HourlyRate>> GetHourlyRatesByUserIdAsync(Guid userId) => 
      await context.HourlyRates
          .Where(x => x.UserId == userId)
          .OrderByDescending(x => x.EffectiveFrom)
          .ToListAsync();

  public async Task<HourlyRate?> GetAsync(Guid userId, DateTime givenDate)
  {
    return await context.HourlyRates
        .SingleOrDefaultAsync(x => x.UserId == userId
                                  && x.EffectiveFrom <= givenDate
                                  && (x.EffectiveTo == null || x.EffectiveTo >= givenDate));
  }

  public async Task<bool> HasOverlappingRateAsync(Guid userId, DateTime from, DateTime to)
  {
    return await context.HourlyRates
        .AnyAsync(r => r.UserId == userId &&
                       ((r.EffectiveFrom < to && (r.EffectiveTo == null || r.EffectiveTo > from)) ||
                        (r.EffectiveFrom <= to && r.EffectiveTo >= from)));
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