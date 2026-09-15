using System;
using System.ComponentModel.DataAnnotations;

namespace GSEvent.DTOs.Auth;

public class TokenResetDto
{
    [Required]
    public string Token {get; set;} = string.Empty;
    [Required]
    public string RefreshToken {get; set;} =string.Empty;

}
