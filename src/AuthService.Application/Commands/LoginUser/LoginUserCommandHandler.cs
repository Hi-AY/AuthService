using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;

using MediatR;

namespace AuthService.Application.Commands.LoginUser;

public class LoginUserCommandHandler
    : IRequestHandler<LoginUserCommand, string>
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

    public async Task<string> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository
            .GetByEmailAsync(request.Request.Email);

        if (user == null)
            throw new ApplicationException("Invalid credentials");

        var validPassword = _passwordHasher.VerifyPassword(
            request.Request.Password,
            user.PasswordHash);

        if (!validPassword)
            throw new ApplicationException("Invalid credentials");

        return _jwt.GenerateToken(user);
    }
}