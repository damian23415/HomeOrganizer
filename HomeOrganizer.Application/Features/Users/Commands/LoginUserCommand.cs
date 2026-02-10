using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.Users.DTOs;
using MediatR;

namespace HomeOrganizer.Application.Features.Users.Commands;

public class LoginUserCommand : IRequest<Result<LoginUserResponse>>
{
  public string Email { get; set; } = null!;
  public string Password { get; set; } = null!;
  
  public LoginUserCommand(string email, string password)
  {
    Email = email;
    Password = password;
  }
}