using System;
using GSEvent.Data;
using GSEvent.Exceptions;
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
        // Controllers
        services.AddControllers();
        // AutoMapper
        services.AddAutoMapper(map => {}, typeof(Program).Assembly);

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
