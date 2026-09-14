using System;
using FluentValidation;
using GSEvent.DTOs.Auth;

namespace GSEvent.Validators.Auth;

public class LoginUserValidation : AbstractValidator<LoginDto>
{
    public LoginUserValidation()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty()
            .WithMessage("Username and Email is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
            
    }
}
