namespace HomeOrganizer.Application.Features.WorkTracking.Dtos;

public class CreateWorkDayRequest
{
  public DateTime Date { get; set; }
  public DateTime StartTime { get; set; }
  public DateTime EndTime { get; set; }
}