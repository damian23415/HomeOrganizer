using AutoMapper;
using HomeOrganizer.Application.Features.Users.Dtos;
using HomeOrganizer.Domain.Entities;

namespace HomeOrganizer.Application.Mapping;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<User, UserDto>();
    CreateMap<RegisterUserRequest, User>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(x => Guid.NewGuid()))
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
  }
}