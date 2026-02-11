using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Queries;

public record GetCurrentHourlyRateQuery(Guid UserId, DateTime Date) : IRequest<Result<GetHourlyRateResponse>>;