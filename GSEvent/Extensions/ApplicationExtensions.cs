using System;
using GSEvent.Data;
using GSEvent.Exceptions;
using GSEvent.Repositories;
using GSEvent.Repositories.Interfaces;
using GSEvent.Services;
using GSEvent.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace GSEvent.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationService(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AppDbContext> (options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                 throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' is not configured."
                );
            }
            options.UseNpgsql(connectionString);
        });

        // Repositore 
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        


        //Service 
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        // Controllers
        services.AddControllers();
        // AutoMapper

        // Swagger
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen( options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "GSEvent API",
                    Version = "v1",
                    Description = "Gulshan Society Event Management API"
                });
        });

        // Exception handler
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();


        
        return services;
    }
}
