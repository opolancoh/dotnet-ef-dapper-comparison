using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkDapperApp.Core.Entities.DTOs;

public record BookForUpdatingDto : BookForCreatingDto
{
    [Required] public Guid? Id { get; init; }
}
