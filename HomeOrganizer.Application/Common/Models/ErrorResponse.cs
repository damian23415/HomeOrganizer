namespace HomeOrganizer.Application.Common.Models;

public class ErrorResponse
{
  public List<string> Errors { get; set; } = new();
  public string Message { get; set; } = string.Empty;
}