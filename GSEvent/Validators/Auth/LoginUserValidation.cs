using System;
using FluentValidation;
using GSEvent.DTOs.Auth;

namespace GSEvent.Validators.Auth;

public class LoginUserValidation : AbstractValidator<LoginDto>
{
    public LoginUserValidation()
    {
        RuleFor(x => x.UsernameOrEmailOrPhone)
            .NotEmpty()
            .WithMessage("Username and Email and Phone is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
            
    }
}
