using System;
using GSEvent.DTOs.Auth;
using GSEvent.Models;
using GSEvent.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Repositories;

public class PasswordRepository : IPasswordRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    public PasswordRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, ChangePasswordDto request)
    {
        return await _userManager.ChangePasswordAsync(
            user,
            request.OldPassword,
            request.NewPassword
        );
    }
}
