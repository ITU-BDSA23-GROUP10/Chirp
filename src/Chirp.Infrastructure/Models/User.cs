namespace Chirp.Infrastructure.Models;

using System.ComponentModel.DataAnnotations.Schema;
// The User class is used to represent a User in our system and the User table in the database.
public record User {
    // This attribute is used to make UserId an auto-incrementing primary key.
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
}