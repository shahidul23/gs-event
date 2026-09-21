using System;
using GSEvent.DTOs.Auth;
using GSEvent.Models;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Repositories.Interfaces;

public interface IPasswordRepository
{
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user,ChangePasswordDto request);
    Task<bool> GeneratePasswordResetTokenAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordDto reset);
}
