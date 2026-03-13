using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Application.Interfaces;
using MediatR;

namespace AuthService.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Request.Email);

        if (existingUser != null)
            throw new Exception("User already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Request.Password),
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user.Id;
    }
}