using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Commands;

public interface ICreateHourlyRateCommand : IRequest<Result<CreateHourlyRateResponse>>
{
}