using System;
using GSEvent.Enums;
using GSEvent.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Data;

public static class AppDbInitializer
{
    public static async Task SeedRole(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder
            .ApplicationServices
            .CreateAsyncScope();

        var roleManager = serviceScope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in Enum.GetNames<Role>())
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new IdentityRole(role)
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description)
                );

                throw new BadRequestException(
                    $"Failed to create role '{role}': {errors}"
                );
            }
        }
    }
}
