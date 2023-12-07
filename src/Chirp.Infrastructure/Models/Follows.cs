using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Infrastructure.Models;

public record Follows { 
    public required int FollowerId { get; set; }
    public required int FollowingId { get; set; }
}