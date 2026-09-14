using System;
using GSEvent.Data;
using GSEvent.DTOs.Auth;
using GSEvent.Models;
using GSEvent.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UserRepository(
        AppDbContext appDbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        _appDbContext = appDbContext;               
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ApplicationUser?> CreateAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return null;
        }
        return user;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) is not null;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username) is not null;
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> GetByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }
}
