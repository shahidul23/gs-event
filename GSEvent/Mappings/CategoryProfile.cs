using System;
using AutoMapper;
using GSEvent.DTOs.CategoryDto;
using GSEvent.Models;

namespace GSEvent.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryReadDto>();
        CreateMap<CategoryCreateDto, Category>();
    }
}
