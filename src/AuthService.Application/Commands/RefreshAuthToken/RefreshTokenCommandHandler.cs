using AuthService.Application.DTOs.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using MediatR;

namespace AuthService.Application.Commands.RefreshAuthToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwt;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwt)
    {
        _userRepository = userRepository;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _userRepository
            .GetRefreshTokenAsync(request.Request.RefreshToken);

        if (token == null)
            throw new ApplicationException("Invalid refresh token");

        if (token.Revoked)
            throw new ApplicationException("Token revoked");

        if (token.ExpiresAt < DateTime.UtcNow)
            throw new ApplicationException("Token expired");

        var user = token.User;

        // revoke old token
        token.Revoked = true;

        // generate new tokens
        var newAccessToken = _jwt.GenerateToken(user);
        var newRefreshTokenValue = _jwt.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenValue,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        await _userRepository.AddRefreshTokenAsync(newRefreshToken);

        await _userRepository.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue
        };
    }
}