using System;
using GSEvent.Data;
using GSEvent.DTOs.Auth;
using GSEvent.Models;
using GSEvent.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ApplicationUser> AddRoleAsync(ApplicationUser user, string role)
    {
        await _userManager.AddToRoleAsync(user, role);
        return user;
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<ApplicationUser?> CreateAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(x => x.Description)
            );

            Console.WriteLine($"User creation failed: {errors}");
            return null;
        }
        return user;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) is not null;
    }

    public async Task<bool> ExistsByPhoneAsync(string phone)
    {
        return await _userManager.Users.AnyAsync(x => x.PhoneNumber == phone);
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

    public async Task<ApplicationUser?> GetByPhoneAsync(string phone)
    {
        return await _userManager.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phone);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<IList<string>> GetRoleAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<string> UserConfirmationAsync(ApplicationUser user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<bool> UserConfirmedAsync(ApplicationUser user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }
}
