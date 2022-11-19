namespace EntityFrameworkDapperApp.Core.Entities.DTOs;

public record ReviewBaseDto
{
    public Guid Id { get; init; }
    public string Comment { get; init; }
    public int Rating { get; init; }
};

public record ReviewDto : ReviewBaseDto
{
    public Guid BookId { get; init; }
};