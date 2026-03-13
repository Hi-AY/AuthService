using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthService.Infrastructure.Persistence;
using AuthService.Domain.Interfaces;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Infrastructure.Repositories;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        return services;
    }
}