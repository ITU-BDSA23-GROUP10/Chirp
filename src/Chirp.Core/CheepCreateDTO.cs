namespace Chirp.Core;
// The CheepCreateDTO is used to create a new Cheep in the database. 
// This is therefore sent to the cheep repository, 
// which then creates a new Cheep object in the database.
public record CheepCreateDTO(string text, string author);