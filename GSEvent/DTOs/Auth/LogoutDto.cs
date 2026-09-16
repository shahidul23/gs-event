using System;
using System.ComponentModel.DataAnnotations;

namespace GSEvent.DTOs.Auth;

public class LogoutDto
{
    [Required]
    public string RefreshToken {get; set;} = string.Empty;
}
