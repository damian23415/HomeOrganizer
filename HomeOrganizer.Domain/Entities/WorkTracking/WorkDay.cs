using HomeOrganizer.Domain.Exceptions;
using HomeOrganizer.Domain.ValueObjects;

namespace HomeOrganizer.Domain.Entities.WorkTracking;

public class WorkDay : BaseEntity
{
  public Guid UserId { get; private set; }
  public DateTime Date { get; private set; }
  public DateTime StartTime { get; private set; }
  public DateTime EndTime { get; private set; }
  public decimal HourlyRateUsed { get; private set; }
  public decimal TotalEarnings => new Money(HourlyRateUsed * TotalHours);
  public int TotalHours => (int)(EndTime - StartTime).TotalHours;
  
  private WorkDay() { } // EF
  
  public WorkDay(Guid userId, DateTime date, DateTime startTime, DateTime endTime, decimal hourlyRate)
  {
    if (userId == Guid.Empty) 
      throw new ArgumentException("UserId required.", nameof(userId));
    
    if (endTime <= startTime) 
      throw new WorkDayDomainException("End time must be after start time.");
    
    if (hourlyRate < 0) 
      throw new WorkDayDomainException("Hourly rate cannot be negative.");
        
    UserId = userId;
    Date = date.Date;
    StartTime = startTime;
    EndTime = endTime;
    HourlyRateUsed = hourlyRate;
  }
  
  public void UpdateTimes(DateTime startTime, DateTime endTime)
  {
    if (endTime <= startTime) 
      throw new WorkDayDomainException("End time must be after start time.");
    
    StartTime = startTime;
    EndTime = endTime;
  }
  
  public void UpdateHourlyRate(decimal rate)
  {
    if (rate < 0) 
      throw new WorkDayDomainException("Rate cannot be negative.");
    
    HourlyRateUsed = rate;
  }
}