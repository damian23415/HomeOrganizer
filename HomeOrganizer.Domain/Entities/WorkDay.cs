namespace HomeOrganizer.Domain.Entities;

public class WorkDay
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public DateTime Date { get; set; }
  public DateTime StartTime { get; set; }
  public DateTime EndTime { get; set; }
  public int TotalHours { get; set; }
  public decimal HourlyRateUsed { get; set; }
  public decimal TotalEarnings { get; set; }
}