namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class CreateHourlyRateRequest
{
  public decimal Rate { get; set; }
  public DateTime EffectiveFrom { get; set; }
  public DateTime? EffectiveTo { get; set; }
  public bool IsHistorical { get; set; }
}