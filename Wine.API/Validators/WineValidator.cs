using FluentValidation;
using Wine.Application.Dtos;

namespace Wine.API.Validators;

public class WineValidator : AbstractValidator<WineDto>
{
    public WineValidator()
    {
        RuleFor(w => w.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(w => w.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .WithMessage("Year must be between 1900 and the current year.");

        RuleFor(w => w.Brand)
            .NotEmpty().WithMessage("Brand is required.");

        RuleFor(w => w.Type)
            .NotEmpty().WithMessage("Type is required.");
    }
}