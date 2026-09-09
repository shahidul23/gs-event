using System;
using AutoMapper;
using GSEvent.DTOs.Auth;
using GSEvent.Models;

namespace GSEvent.Mappings;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterDto, ApplicationUser>();
    }
}
