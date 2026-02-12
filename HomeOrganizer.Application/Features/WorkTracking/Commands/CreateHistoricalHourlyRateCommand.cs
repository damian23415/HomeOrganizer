namespace HomeOrganizer.Application.Features.WorkTracking.Commands;

public record CreateHistoricalHourlyRateCommand(
    Guid UserId,
    decimal HourlyRate,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo) : ICreateHourlyRateCommand;