using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Commands;

public record CreateHourlyRateCommand(
    Guid UserId,
    decimal HourlyRate,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo) : IRequest<Result<CreateHourlyRateResponse>>;