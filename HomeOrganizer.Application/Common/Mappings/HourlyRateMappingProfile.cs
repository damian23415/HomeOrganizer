using AutoMapper;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Application.Common.Mappings;

public class HourlyRateMappingProfile : Profile
{
  public HourlyRateMappingProfile()
  {
    CreateMap<HourlyRate, CreateHourlyRateResponse>()
        .ForMember(dest => dest.EffectiveFrom, opt => opt.MapFrom(src => src.EffectiveFrom))
        .ForMember(dest => dest.EffectiveTo, opt => opt.MapFrom(src => src.EffectiveTo))
        .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate));
    
    CreateMap<HourlyRate, GetHourlyRateResponse>()
        .ForMember(dest => dest.EffectiveFrom, opt => opt.MapFrom(src => src.EffectiveFrom))
        .ForMember(dest => dest.EffectiveTo, opt => opt.MapFrom(src => src.EffectiveTo))
        .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate));
  }
}