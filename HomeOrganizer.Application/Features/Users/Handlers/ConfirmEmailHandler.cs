using HomeOrganizer.Application.Common.Models;
using HomeOrganizer.Application.Features.Users.Commands;
using HomeOrganizer.Domain.Interfaces;
using MediatR;

namespace HomeOrganizer.Application.Features.Users.Handlers;

public class ConfirmEmailHandler(IUserRepository userRepository) : IRequestHandler<ConfirmEmailCommand, Result<Unit>>
{
  public async Task<Result<Unit>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
  {
    var user = await userRepository.GetByEmailConfirmationTokenAsync(request.Token);
    
    if (user == null)
      return Result<Unit>.Failure("Invalid or expired token.");
    
    user.ConfirmEmail(request.Token);
    await userRepository.UpdateAsync(user);
    
    return Result<Unit>.Success(Unit.Value);
  }
}