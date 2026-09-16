using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GSEvent.DTOs.Auth;
using GSEvent.Exceptions;
using GSEvent.Models;
using GSEvent.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GSEvent.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public JwtTokenResult GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles)
    {
        var jwtKey = _configuration["Jwt:Secret"]
            ?? throw new UnauthorizedException("JWT Key is not configured.");

        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new UnauthorizedException("JWT Issuer is not configured.");

        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new UnauthorizedException("JWT Audience is not configured.");

        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"],
            out var minutes ) 
                ? minutes 
                : throw new UnauthorizedException( "JWT expiration minutes is not configured or invalid.");

        var jwtId = Guid.NewGuid().ToString();

        var expiresAt = DateTime.UtcNow.AddMinutes(
            expirationMinutes
        );
        var authClaims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Jti,
                jwtId
            ),
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id
            ),
            new Claim(
                ClaimTypes.Name,
                user.UserName ?? string.Empty
            ),
            new Claim(
                ClaimTypes.Email,
                user.Email ?? string.Empty
            ),
        };
        foreach(var role in roles)
        {
            authClaims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role
                )
            );
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = expiresAt,
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = credentials
        };
        var tokenhandler = new JwtSecurityTokenHandler();
        var securityToken = tokenhandler.CreateToken(tokenDescription);
        var token = tokenhandler.WriteToken(securityToken);
        
        return new JwtTokenResult
        {
            Token = token,
            ExpiresAt = expiresAt,
            JwtId = jwtId
        };

    }
}
