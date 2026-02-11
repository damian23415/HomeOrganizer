using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Commands;

public record CreateWorkDayCommand(
    Guid UserId, 
    DateTime StartTime, 
    DateTime EndTime, 
    DateTime Date) : IRequest<Result<WorkDayResponse>>;