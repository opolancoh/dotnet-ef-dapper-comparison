using System.ComponentModel.DataAnnotations;
using EntityFrameworkDapperApp.Core.Entities.Validators;

namespace EntityFrameworkDapperApp.Core.Entities.DTOs;

public record BookCreateOrUpdateDto : IValidatableObject
{
    [Required] public string? Title { get; init; }
    [Required] public DateTime? PublishedOn { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResult = new List<ValidationResult>();
        validationResult.AddRange(BookValidator.ValidateTitle(Title!, nameof(Title)));

        return validationResult;
    }
};