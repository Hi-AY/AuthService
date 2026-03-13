using AuthService.Application.DTOs.Auth;

using MediatR;

namespace AuthService.Application.Commands.LoginUser;

public record LoginUserCommand(LoginRequest Request) : IRequest<string>;