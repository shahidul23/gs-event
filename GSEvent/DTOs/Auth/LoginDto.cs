using System;

namespace GSEvent.DTOs.Auth;

public class LoginDto
{
    public string UsernameOrEmailOrPhone {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
}
