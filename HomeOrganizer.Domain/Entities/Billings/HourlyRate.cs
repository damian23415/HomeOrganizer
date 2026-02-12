using HomeOrganizer.Domain.Exceptions;
using HomeOrganizer.Domain.ValueObjects;

namespace HomeOrganizer.Domain.Entities.Billings;

public class HourlyRate : BaseEntity
{
  public Guid UserId { get; private set; }
  public Money Rate { get; private set; }
  public DateTime EffectiveFrom { get; private set; }
  public DateTime? EffectiveTo { get; private set; }
  
  private HourlyRate() { } // EF
  
  public HourlyRate(Guid userId, Money hourlyRate, DateTime effectiveFrom, DateTime? effectiveTo = null)
  {
    if (userId == Guid.Empty) 
      throw new ArgumentException("UserId required.", nameof(userId));
    
    if (effectiveTo.HasValue && effectiveTo <= EffectiveFrom) 
      throw new BillingDomainException("EffectiveTo must be after EffectiveFrom.");
    
    UserId = userId;
    Rate = hourlyRate ?? throw new ArgumentNullException(nameof(hourlyRate));
    EffectiveFrom = effectiveFrom;
    EffectiveTo = effectiveTo;
  }
  
  public void SetEffectiveTo(DateTime effectiveTo)
  {
    if (effectiveTo.Date < EffectiveFrom) 
      throw new BillingDomainException("EffectiveTo must be after EffectiveFrom.");
    
    EffectiveTo = effectiveTo;
  }
  
  public bool IsActive() => EffectiveTo == null || EffectiveTo > DateTime.UtcNow;
}