namespace EntityFrameworkDapperApp.Core.Entities.DTOs;

public record BookDto 
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public DateTime PublishedOn { get; init; }
    
    public ICollection<ListDto> Reviews { get; init; } = new List<ListDto>();
};