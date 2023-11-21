using System.ComponentModel.DataAnnotations.Schema;
namespace Chirp.Infrastructure.Models;

public record Author
{
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();
    
    [ForeignKey("User")]
    public int AuthorId { get; set; } // Foreign key that refers to UserId in User.cs
    public required User User { get; set; }
}