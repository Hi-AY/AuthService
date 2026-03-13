using MediatR;
using AuthService.Application.DTOs.Auth;

namespace AuthService.Application.Commands.RegisterUser;

public record RegisterUserCommand(RegisterRequest Request) : IRequest<Guid>;