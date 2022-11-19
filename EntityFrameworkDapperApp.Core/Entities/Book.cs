namespace EntityFrameworkDapperApp.Core.Entities;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime PublishedOn { get; set; }

    // One-to-many relationship (Review)
    public ICollection<Review> Reviews { get; set; }
}