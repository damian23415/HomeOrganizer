using AutoMapper;
using HomeOrganizer.Application.DTOs.Users;
using HomeOrganizer.Application.Features.Users.DTOs;
using HomeOrganizer.Domain.Entities.Users;

namespace HomeOrganizer.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
  public UserMappingProfile()
  {
    CreateMap<User, UserDto>()
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
        .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.IsEmailConfirmed))
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
  }
}