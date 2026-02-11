using AutoMapper;
using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Application.Features.WorkTracking.Queries;
using HomeOrganizer.Domain.Interfaces;
using MediatR;

namespace HomeOrganizer.Application.Features.WorkTracking.Handlers;

public class GetCurrentHourlyRateHandler(
    IHourlyRateRepository repository, 
    IMapper mapper) : IRequestHandler<GetCurrentHourlyRateQuery, Result<GetHourlyRateResponse>>
{
  public async Task<Result<GetHourlyRateResponse>> Handle(GetCurrentHourlyRateQuery request, CancellationToken cancellationToken)
  {
    var entity = await repository.GetAsync(request.UserId, request.Date);
    
    if (entity == null)
      return Result<GetHourlyRateResponse>.Failure("No hourly rate found for the specified date.");
    
    var response = mapper.Map<GetHourlyRateResponse>(entity);
    
    return Result<GetHourlyRateResponse>.Success(response);
  }
}