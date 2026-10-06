using System;
using GSEvent.Common.Pagination;
using GSEvent.Data;
using GSEvent.DTOs;
using GSEvent.DTOs.Auth;
using GSEvent.Exceptions;
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

    public async Task<string> GenerateUsernameAsync(string email)
    {
        var username = email.Split('@')[0];
        var originalUsername = username;
        var counter = 1;
        while (await _userManager.FindByNameAsync(username) != null)
        {
            username = $"{originalUsername}{counter}";

            counter++;
        }
        return username;

    }

    public async Task<IList<ApplicationUser>> getAllUsers()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<PaginationResponse<UserReadDto>> getAllUsers(PaginationRequest request)
    {
        var query = _userManager.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x => 
            x.FullName!.Contains(search) ||
            x.UserName!.Contains(search) ||
            x.Email!.Contains(search) ||
            x.PhoneNumber!.Contains(search)
            );
        }
        query = request.SortBy!.ToLower() switch
        {
           "fullname" => request.SortDirection?.ToLower() == "desc"
                ? query.OrderByDescending(x => x.FullName)
                : query.OrderBy(x => x.FullName),

            "username" => request.SortDirection?.ToLower() == "desc"
                ? query.OrderByDescending(x => x.UserName)
                : query.OrderBy(x => x.UserName),

            "email" => request.SortDirection?.ToLower() == "desc"
                ? query.OrderByDescending(x => x.Email)
                : query.OrderBy(x => x.Email),

            "phone" => request.SortDirection?.ToLower() == "desc"
                ? query.OrderByDescending(x => x.PhoneNumber)
                : query.OrderBy(x => x.PhoneNumber),
            _ => query.OrderBy(x => x.FullName)
        };
        var totalItems = await query.CountAsync();
        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        var result = new List<UserReadDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserReadDto
            {
                Id = user.Id,
                FullName = user.FullName ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty 
            });
        }
        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);
        return new PaginationResponse<UserReadDto>
        {
            Data = result,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasPreviousPage = request.Page > 1,
            HasNextPage = request.Page < totalPages
        };
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

    public async Task<UserReadDto> GetUser(Guid id)
    {
        var data = await GetByIdAsync(id.ToString());
        if (data == null)
        {
            throw new NotFoundException("User Not Found");
        }
        var roles = await _userManager.GetRolesAsync(data);
        return new UserReadDto
        {
            Id = data.Id,
            FullName = data.FullName ?? string.Empty,
            UserName = data.UserName ?? string.Empty,
            Email = data.Email ?? string.Empty,
            Phone = data.PhoneNumber ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty
        };
    }

    public async Task<string> getUserRole(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault() ?? string.Empty;
    }

    public async Task<ApplicationUser> UpdateUserAsync(ApplicationUser user,string password)
    {
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        if (!string.IsNullOrWhiteSpace(password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!passwordResult.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
            }
        }
        return user;
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
