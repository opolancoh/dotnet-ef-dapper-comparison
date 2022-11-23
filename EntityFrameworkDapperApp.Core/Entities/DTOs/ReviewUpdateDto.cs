using System.ComponentModel.DataAnnotations;
using EntityFrameworkDapperApp.Core.Entities.Validators;

namespace EntityFrameworkDapperApp.Core.Entities.DTOs;

public record ReviewForUpdatingDto : IValidatableObject
{
    [Required] public Guid? Id { get; init; }
    [Required] public string? Comment { get; init; }
    [Required] public int? Rating { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        validationResult.AddRange(ReviewValidator.ValidateRating(Rating!.Value, nameof(Rating)));

        return validationResult;
    }
};