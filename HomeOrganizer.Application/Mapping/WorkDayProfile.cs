using AutoMapper;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.WorkTracking;

namespace HomeOrganizer.Application.Mapping;

public class WorkDayProfile : Profile
{
  public WorkDayProfile()
  {
    CreateMap<CreateWorkDayRequest, WorkDay>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
      .ForMember(dest => dest.UserId, opt => opt.Ignore())
      .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => 
        (int)(src.EndTime - src.StartTime).TotalHours))
      .ForMember(dest => dest.HourlyRateUsed, opt => opt.Ignore())
      .ForMember(dest => dest.TotalEarnings, opt => opt.Ignore());
    
    CreateMap<WorkDay, WorkDayResponse>();
  }
}