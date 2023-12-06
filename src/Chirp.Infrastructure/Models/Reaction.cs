using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure.Models;

public record Reaction {   
    public int cheepId { get; set; }
    public int userId { get; set; }
    public string reactionType { get; set; } = "";
}
