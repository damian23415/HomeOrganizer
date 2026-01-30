namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class HourlyRateResponse
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public decimal Rate { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
}