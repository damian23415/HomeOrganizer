using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.DTOs.Users;
using MediatR;

namespace HomeOrganizer.Application.Features.Users.Commands;

public class RegisterUserCommand : IRequest<Result<RegisterUserResponse>>
{
  public string Email { get; set; } = null!;
  public string Password { get; set; } = null!;
    
  public RegisterUserCommand(string email, string password)
  {
    Email = email;
    Password = password;
  }
}