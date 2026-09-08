using System;
using System.ComponentModel.DataAnnotations;

namespace GSEvent.DTOs.CategoryDto;

public class CategoryReadDto
{
    public Guid CategoryId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;}
}
