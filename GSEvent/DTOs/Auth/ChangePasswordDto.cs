using System;
using System.ComponentModel.DataAnnotations;

namespace GSEvent.DTOs.Auth;

public class ChangePasswordDto
{
    [Required]
    public string OldPassword {get; set;} = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
