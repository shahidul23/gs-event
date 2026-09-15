using System;
using GSEvent.Models;

namespace GSEvent.Repositories.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> CreateAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<ApplicationUser?> GetByUsernameAsync(string username);
    Task<ApplicationUser?> GetByPhoneAsync(string phone);
    Task<ApplicationUser?> GetByIdAsync(string userId);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<bool> ExistsByPhoneAsync(string phone);

}
