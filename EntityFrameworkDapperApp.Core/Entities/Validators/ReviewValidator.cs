using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkDapperApp.Core.Entities.Validators;

public static class ReviewValidator
{
    public static IEnumerable<ValidationResult> ValidateRating(int value, string fieldName)
    {
        if (value is < 1 or > 5)
        {
            yield return new ValidationResult(
                $"The {fieldName} field must be between 1 and 5.", new[] {fieldName});
        }
    }

    public static IEnumerable<ValidationResult> ValidateBookId(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
        {
            yield return new ValidationResult(
                $"The {nameof(fieldName)} field is not a valid ID.", new[] {fieldName});
        }
    }
}