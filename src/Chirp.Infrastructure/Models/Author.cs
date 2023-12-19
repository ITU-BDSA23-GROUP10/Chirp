using System.ComponentModel.DataAnnotations.Schema;
namespace Chirp.Infrastructure.Models;
// The Author class is used to represent an Author in our system and the Author table in the database.
// An Author is defined as a User that has written a Cheep.
public record Author
{
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    
    // Foreign key that refers to UserId in User.cs
    [ForeignKey("User")]
    public int AuthorId { get; set; } 
    public required User User { get; set; }
}