using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure.Models;
// The Reaction class is used to represent a Reaction in our system and the Reaction table in the database.
public record Reaction {   
    public int cheepId { get; set; }
    public int userId { get; set; }
    public string reactionType { get; set; } = null!;
}
