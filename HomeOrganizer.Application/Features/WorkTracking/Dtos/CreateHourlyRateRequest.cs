namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class CreateHourlyRateRequest
{
  public decimal HourlyRate { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime? EndDate { get; set; }
}