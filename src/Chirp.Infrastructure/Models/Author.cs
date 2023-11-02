namespace Chirp.Infrastructure.Models;

public record Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();

}