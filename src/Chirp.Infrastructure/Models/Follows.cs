using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure.Models;
// The Follows class is used to represent a Follows in our system and the Follows table in the database.
public record Follows { 
    public required int FollowerId { get; set; }
    public required int FollowingId { get; set; }
}