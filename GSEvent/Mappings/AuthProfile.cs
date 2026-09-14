using System;
using AutoMapper;
using GSEvent.DTOs.Auth;
using GSEvent.DTOs.RefreshToken;
using GSEvent.Models;

namespace GSEvent.Mappings;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterDto, ApplicationUser>();
         CreateMap<RegisterDto, ApplicationUser>();

        CreateMap<ApplicationUser, UserReadDto>();

        CreateMap<RefreshToken, RefreshTokenCreateDto>();

        CreateMap<AuthResponseDto, AuthResponseDto>();

    }
}
