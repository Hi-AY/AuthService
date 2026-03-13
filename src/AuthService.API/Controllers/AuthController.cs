using AuthService.Application.Commands.LoginUser;
using AuthService.Application.Commands.RefreshAuthToken;
using AuthService.Application.Commands.RegisterUser;
using AuthService.Application.DTOs.Auth;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var userId = await _mediator.Send(new RegisterUserCommand(request));

        return Ok(new { userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _mediator.Send(new LoginUserCommand(request));

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var response = await _mediator.Send(new RefreshTokenCommand(request));

        return Ok(response);
    }
}