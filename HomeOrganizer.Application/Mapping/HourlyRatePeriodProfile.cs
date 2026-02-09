using AutoMapper;
using HomeOrganizer.Application.Features.WorkTracking.Dtos;
using HomeOrganizer.Domain.Entities;
using HomeOrganizer.Domain.Entities.Billings;

namespace HomeOrganizer.Application.Mapping;

public class HourlyRatePeriodProfile : Profile
{
  public HourlyRatePeriodProfile()
  {
    CreateMap<CreateHourlyRateRequest, HourlyRatePeriod>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
      .ForMember(dest => dest.HourlyRate, opt => opt.MapFrom(src => src.HourlyRate))
      .ForMember(dest => dest.EffectiveFrom, opt => opt.MapFrom(src => src.StartDate))
      .ForMember(dest => dest.EffectiveTo, opt => opt.MapFrom(src => src.EndDate));

    CreateMap<HourlyRatePeriod, HourlyRateResponse>()
      .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.HourlyRate))
      .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.EffectiveFrom))
      .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EffectiveTo));
  }
}