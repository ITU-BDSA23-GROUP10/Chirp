namespace Chirp.Infrastructure.Models;

using System.ComponentModel.DataAnnotations.Schema;

public record Reaction {   
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int cheepid { get; set; }
    public int userId { get; set; }
}

// public record ReactionType {
//     // upvote, downvote
//     public string reactionType { get; set; }

//     [ForeignKey("Reaction")]
//     public int ReactionId
//     public Reaction Reaction { get; set; }
// }

public record UpVote : Reaction {
    public bool upVote { get; } = false;
}

public record DownVote : Reaction {
    public bool downVote { get; } = false;
}