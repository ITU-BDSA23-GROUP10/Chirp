namespace Chirp.Infrastructure.Models;

using System.ComponentModel.DataAnnotations.Schema;

public record User {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public HashSet<User> Following { get; set; } = new HashSet<User>();
    public HashSet<User> Followers { get; set; } = new HashSet<User>();
}