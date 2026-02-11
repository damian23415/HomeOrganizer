using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Queries;

public record GetWorkDaysByMonthQuery(Guid UserId, int Year, int Month) : IRequest<Result<IList<WorkDayResponse>>>;