using System;
using AutoMapper;
using GSEvent.DTOs.RefreshToken;
using GSEvent.Models;

namespace GSEvent.Mappings;

public class RefreshTokenProfile : Profile
{
    public RefreshTokenProfile()
    {
        CreateMap<RefreshTokenCreateDto, RefreshToken>();
    }
}
