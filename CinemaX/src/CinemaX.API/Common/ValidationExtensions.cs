using CinemaX.Domain.Common;
using FluentValidation.Results;

namespace CinemaX.API.Common;

public static class ValidationExtensions
{
    public static Error? ToValidationError(this ValidationResult result)
    {
        if (result.IsValid)
            return null;

        var message = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
        return Error.Validation(message);
    }
}