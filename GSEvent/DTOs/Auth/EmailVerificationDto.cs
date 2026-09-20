using System;

namespace GSEvent.DTOs.Auth;

public class EmailVerificationDto
{
    public string Username {get; set;} = string.Empty;
    public string Token {get; set;} = string.Empty;
}
