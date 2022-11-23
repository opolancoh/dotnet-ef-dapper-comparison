using System.ComponentModel.DataAnnotations;
using EntityFrameworkDapperApp.Core.Entities.Validators;

namespace EntityFrameworkDapperApp.Core.Entities.DTOs;

public record ReviewForCreatingDto : IValidatableObject
{
    [Required] public string? Comment { get; init; }
    [Required] public int? Rating { get; init; }

    [Required] public Guid? BookId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        validationResult.AddRange(ReviewValidator.ValidateRating(Rating!.Value, nameof(Rating)));

        return validationResult;
    }
};