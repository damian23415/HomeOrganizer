namespace HomeOrganizer.Domain.Entities;

public class HourlyRatePeriod
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public decimal HourlyRate { get; set; }
  public DateTime EffectiveFrom { get; set; }
  public DateTime? EffectiveTo { get; set; }
  
  public void UpdateEffectiveTo(DateTime newEffectiveTo)
  {
    EffectiveTo = newEffectiveTo;
  }
}