using CinemaX.Application.Movies;
using FluentValidation;

namespace CinemaX.API.Validators.Movies;

public sealed class UpdateMovieRequestValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(300).WithMessage("Title must not exceed 300 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.PosterUrl)
            .MaximumLength(500).WithMessage("PosterUrl must not exceed 500 characters.")
            .When(x => x.PosterUrl is not null);

        RuleFor(x => x.GenreId)
            .NotEmpty().WithMessage("GenreId is required.");

        RuleFor(x => x.DurationMins)
            .GreaterThan(0).WithMessage("DurationMins must be greater than 0.");
    }
}