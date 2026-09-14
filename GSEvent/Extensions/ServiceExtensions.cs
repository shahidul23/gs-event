using System;
using System.Text;
using GSEvent.Data;
using GSEvent.Exceptions;
using GSEvent.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace GSEvent.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtKey = configuration["Jwt:Secret"]
            ?? throw new BadRequestException("JWT Key is missing from configuration.");
        var jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new BadRequestException("JWT Issuer is missing from configuration.");
        var jwtAudience = configuration["Jwt:Audience"]
            ?? throw new BadRequestException("JWT Audience is missing from configuration.");
        
        var tokenValidationParameter = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        services.AddSingleton(tokenValidationParameter);
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(option =>
        {
            option.TokenValidationParameters = tokenValidationParameter;
            option.SaveToken = true;
            option.RequireHttpsMetadata = false;
        });
        services.AddAuthorization();
        return services;
    }
}
