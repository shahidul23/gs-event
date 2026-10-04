using System;
using GSEvent.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<bool> RoleExistsAsync(string roleName);

    Task<IdentityRole?> GetByNameAsync(string roleName);
    Task<List<IdentityRole>> GetAllRolesAsync();
}
