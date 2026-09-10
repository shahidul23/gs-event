using System;
using GSEvent.DTOs.Auth;
using GSEvent.Models;

namespace GSEvent.Services.Interfaces;

public interface IJwtService
{
    JwtTokenResult GenerateJwtTokenAsync(ApplicationUser user);
}
