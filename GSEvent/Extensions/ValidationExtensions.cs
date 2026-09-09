using System;
using FluentValidation;
using GSEvent.Common;
using GSEvent.Validators.Auth;
using GSEvent.Validators.Category;
using Microsoft.AspNetCore.Mvc;

namespace GSEvent.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddApiValidation(
        this IServiceCollection services
    )
    {
        services.Configure<ApiBehaviorOptions>(option =>
        {
            option.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors
                            .SelectMany(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                ? "Invalid Value."
                                : e.ErrorMessage)
                            .ToArray()
                    );
                return new BadRequestObjectResult(
                    ApiResponse<object>.ErrorResponse(
                        "Validation failed.",
                        StatusCodes.Status422UnprocessableEntity,
                        errors
                    )
                );
            };
        });
        // Authentication
        services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateCategoryValidation>();

        return services;
    }
}
