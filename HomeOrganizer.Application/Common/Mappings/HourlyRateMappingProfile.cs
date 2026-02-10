using AutoMapper;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Application.Common.Mappings;

public class HourlyRateMappingProfile : Profile
{
  public HourlyRateMappingProfile()
  {
    CreateMap<HourlyRate, CreateHourlyRateResponse>()
        .ForMember(d => d.EffectiveFrom, opt => opt.MapFrom(s => s.EffectiveFrom))
        .ForMember(d => d.EffectiveTo, opt => opt.MapFrom(s => s.EffectiveTo))
        .ForMember(d => d.Rate, opt => opt.MapFrom(s => s.Rate));
  }
}