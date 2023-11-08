using System.ComponentModel.DataAnnotations.Schema;
namespace Chirp.Infrastructure.Models;

public record Author
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public List<Cheep> Cheeps { get; set; } = new List<Cheep>();

}