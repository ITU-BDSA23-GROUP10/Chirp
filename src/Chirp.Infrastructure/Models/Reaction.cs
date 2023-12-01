namespace Chirp.Infrastructure.Models;

using System.ComponentModel.DataAnnotations.Schema;

public record Reaction {   
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int cheepId { get; set; }
    public int userId { get; set; }
    public bool upVote { get; set; } = false;
    public bool downVote { get; set; } = false;
}

// public record ReactionType {
//     // upvote, downvote
//     public string reactionType { get; set; }

//     [ForeignKey("Reaction")]
//     public int ReactionId
//     public Reaction Reaction { get; set; }
// }

// public record UpVote : Reaction {
//     public bool upVote { get; } = false;
// }

// public record DownVote : Reaction {
//     public bool downVote { get; } = false;
// }