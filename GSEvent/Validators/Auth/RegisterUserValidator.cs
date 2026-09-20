using System;
using System.Data;
using FluentValidation;
using GSEvent.DTOs.Auth;

namespace GSEvent.Validators.Auth;

public class RegisterUserValidator : AbstractValidator<RegisterDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.")
            .MinimumLength(2)
            .WithMessage("Name atlest 2 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please provide a valid email address.")
            .MaximumLength(150)
            .WithMessage("Email cannot exceed 150 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^(?:\+8801|01)[3-9]\d{8}$")
            .WithMessage("Please provide a valid Bangladesh phone number.");
        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters.");
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required.")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
}
