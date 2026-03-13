using AuthService.Application.DTOs.Auth;

using MediatR;

namespace AuthService.Application.Commands.RefreshAuthToken;

public record RefreshTokenCommand(RefreshTokenRequest Request)
    : IRequest<AuthResponse>;