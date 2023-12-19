namespace Chirp.Core;

// The ReactionDTO is used to send Reaction data to the frontend for displaying.
public record ReactionDTO ( int cheepId, int userId, string reactionType );