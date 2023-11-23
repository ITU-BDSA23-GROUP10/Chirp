namespace Chirp.Infrastructure.Models;

using System.ComponentModel.DataAnnotations.Schema;

public record User {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }

}

public record Follows {
    [ForeignKey("User")]
    public required int FollowerId { get; set; }
    [ForeignKey("User")]
    public required int FollowingId { get; set; }

    
}