using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Queries;
using HomeOrganizer.Domain.Interfaces;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class GetWorkDaysByMonthHandler(
    IWorkDayRepository workDayRepository,
    IMapper mapper) : IRequestHandler<GetWorkDaysByMonthQuery, Result<IList<WorkDayResponse>>>
{
  public async Task<Result<IList<WorkDayResponse>>> Handle(GetWorkDaysByMonthQuery request, CancellationToken cancellationToken)
  {
    var entities = await workDayRepository.GetAllFromMonthAsync(request.Year,request.Month, request.UserId);

    var response = mapper.Map<IList<WorkDayResponse>>(entities);
    
    return Result<IList<WorkDayResponse>>.Success(response);
  }
}