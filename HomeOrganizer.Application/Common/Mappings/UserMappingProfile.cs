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
        .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value))
        .ForMember(d => d.IsEmailConfirmed, opt => opt.MapFrom(s => s.IsEmailConfirmed))
        .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.IsActive));
  }
}