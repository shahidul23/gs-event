using System;

namespace GSEvent.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }
}
