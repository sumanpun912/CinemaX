using CinemaX.Application.Theaters;
using FluentValidation;

namespace CinemaX.API.Validators.Theaters;

public sealed class UpdateTheaterRequestValidator : AbstractValidator<UpdateTheaterRequest>
{
    public UpdateTheaterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}