namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class GetAllHourlyRatesResponse
{
  public decimal Rate { get; set; }
  public DateTime EffectiveFrom { get; set; }
  public DateTime? EffectiveTo { get; set; }
  public bool IsCurrentRate { get; set; }
  public int TotalHours { get; set; }
  public decimal TotalEarnings { get; set; }
}