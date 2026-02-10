namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class CreateHourlyRateResponse
{
  public decimal Rate { get; set; }
  public DateTime EffectiveFrom { get; set; }
  public DateTime? EffectiveTo { get; set; }
}