using System;
using System.ComponentModel.DataAnnotations;

namespace GSEvent.DTOs.CategoryDto;

public class CategoryCreateDto
{
    [StringLength(100, MinimumLength =2, ErrorMessage ="Name is Required, and Length 2 to 100 Cherecteur")]
    public string Name {get; set;} = string.Empty;
    [Required]
    public string Description {get; set;} = string.Empty;
}
