using AutoMapper;
using HomeOrganizer.Application.Features.WorkTracking.Commands;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities.WorkTracking;

namespace HomeOrganizer.Application.Common.Mappings;

public class WorkDayMappingProfile : Profile
{
  public WorkDayMappingProfile()
  {
    CreateMap<WorkDay, WorkDayResponse>()
        .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
        .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.TotalEarnings));
  }
}