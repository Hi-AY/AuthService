using AuthService.Application.DTOs.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using MediatR;

namespace AuthService.Application.Commands.LoginUser;

public class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwt;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwt)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Request.Email);

        if (user == null)
            throw new ApplicationException("Invalid credentials");

        var passwordValid = _passwordHasher.VerifyPassword(
            request.Request.Password,
            user.PasswordHash);

        if (!passwordValid)
            throw new ApplicationException("Invalid credentials");

        // Generate access token
        var accessToken = _jwt.GenerateToken(user);

        // Generate refresh token
        var refreshTokenValue = _jwt.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        await _userRepository.AddRefreshTokenAsync(refreshToken);
        await _userRepository.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue
        };
    }
}