using System;
using FluentValidation;
using GSEvent.DTOs.CategoryDto;

namespace GSEvent.Validators.Category;

public class CreateCategoryValidation : AbstractValidator<CategoryCreateDto>
{
    public CreateCategoryValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters.");
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters.");
    }

}
