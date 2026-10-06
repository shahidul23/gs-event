using System;
using GSEvent.Common.Pagination;
using GSEvent.DTOs.Auth;
using GSEvent.Models;

namespace GSEvent.Repositories.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<ApplicationUser?> GetByUsernameAsync(string username);
    Task<ApplicationUser?> GetByPhoneAsync(string phone);
    Task<ApplicationUser?> GetByIdAsync(string userId);
    Task<ApplicationUser> AddRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetRoleAsync(ApplicationUser user);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<bool> ExistsByPhoneAsync(string phone);
    Task <string> UserConfirmationAsync(ApplicationUser user);
    Task<bool> UserConfirmedAsync(ApplicationUser user, string token);
    Task<string> GenerateUsernameAsync(string email);
    Task<PaginationResponse<UserReadDto>> getAllUsers(PaginationRequest request);
    Task<string> getUserRole(ApplicationUser user);
    Task<UserReadDto> GetUser(Guid id);
    Task <ApplicationUser> UpdateUserAsync(ApplicationUser user, string password);

}
