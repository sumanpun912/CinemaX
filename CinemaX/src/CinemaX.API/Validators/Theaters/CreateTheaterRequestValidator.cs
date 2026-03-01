using CinemaX.Application.Theaters;
using FluentValidation;

namespace CinemaX.API.Validators.Theaters;

public sealed class CreateTheaterRequestValidator : AbstractValidator<CreateTheaterRequest>
{
    public CreateTheaterRequestValidator()
    {
        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("CityId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}