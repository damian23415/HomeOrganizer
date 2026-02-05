namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class HourlyRatesResponse
{
  public decimal RatePerHour { get; set; }
  public decimal TotalEarnings { get; set; }
  public int TotalHours { get; set; }
  public bool IsCurrentRate { get; set; }
  public DateTime EffectiveFrom { get; set; }
  public DateTime? EffectiveTo { get; set; }
}