using System;

namespace GSEvent.DTOs.Auth;

public class ReadRoleDto
{
    public string Id {get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}
