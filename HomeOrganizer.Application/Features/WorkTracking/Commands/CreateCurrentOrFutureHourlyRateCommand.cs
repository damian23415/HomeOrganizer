namespace HomeOrganizer.Application.Features.WorkTracking.Commands;

public record CreateCurrentOrFutureHourlyRateCommand(
    Guid UserId,
    decimal HourlyRate,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo) : ICreateHourlyRateCommand;