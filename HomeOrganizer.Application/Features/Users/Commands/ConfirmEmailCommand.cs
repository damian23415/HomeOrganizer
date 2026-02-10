using HomeOrganizer.Application.Common.Models;
using MediatR;

namespace HomeOrganizer.Application.Features.Users.Commands;

public record ConfirmEmailCommand(string Token) : IRequest<Result<Unit>>;
