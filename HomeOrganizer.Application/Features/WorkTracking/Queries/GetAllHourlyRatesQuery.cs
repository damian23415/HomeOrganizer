using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Queries;

public record GetAllHourlyRatesQuery(Guid UserId) : IRequest<Result<IList<GetAllHourlyRatesResponse>>>;