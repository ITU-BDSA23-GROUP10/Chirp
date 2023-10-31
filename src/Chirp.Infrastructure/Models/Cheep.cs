using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure.Models;

public record Cheep
{
    public int CheepId { get; set; }
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }

    [ForeignKey("Author")]
    public int AuthorId { get; set; }
    public required Author Author { get; set; } = null!;
}