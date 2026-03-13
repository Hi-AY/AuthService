using System.Net;
using System.Text.Json;

using FluentValidation;

namespace AuthService.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await HandleException(context, ex);
        }
    }

    private static Task HandleValidationException(
        HttpContext context,
        ValidationException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = ex.Errors.Select(e => e.ErrorMessage);

        var result = JsonSerializer.Serialize(new
        {
            error = "Validation failed",
            details = errors
        });

        return context.Response.WriteAsync(result);
    }

    private static Task HandleException(
        HttpContext context,
        Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = JsonSerializer.Serialize(new
        {
            error = ex.Message
        });

        return context.Response.WriteAsync(result);
    }
}