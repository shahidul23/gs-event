using System;

namespace GSEvent.DTOs.Auth;

public class UserReadDto
{
    public string Id {get; set;} = string.Empty;
    public string UserName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
}
