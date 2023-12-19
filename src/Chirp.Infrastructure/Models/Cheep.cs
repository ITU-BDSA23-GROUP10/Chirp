using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure.Models;
// The Cheep class is used to represent Cheeps in our system and the Cheep table in the database.
public record Cheep
{   
    // This attribute is used to make CheepId an auto-incrementing primary key.
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CheepId { get; set; }
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    // Foreign key that refers to AuthorId in Author.cs
    [ForeignKey("Author")]
    public int AuthorId { get; set; }
    public required Author Author { get; set; } = null!;
}