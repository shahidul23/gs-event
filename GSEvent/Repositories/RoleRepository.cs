using System;
using GSEvent.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public RoleRepository(
        RoleManager<IdentityRole> roleManager
    )
    {
        _roleManager = roleManager;
    }

    public async Task<IdentityRole?> GetByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }
}
