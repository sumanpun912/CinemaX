using CinemaX.Application.Cities;
using FluentValidation;

namespace CinemaX.API.Validators.Cities;

public sealed class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
{
    public CreateCityRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("TimeZoneId is required.")
            .MaximumLength(100).WithMessage("TimeZoneId must not exceed 100 characters.");
    }
}