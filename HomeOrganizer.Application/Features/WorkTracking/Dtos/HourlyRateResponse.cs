namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class HourlyRateResponse
{
  public decimal Rate { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
}