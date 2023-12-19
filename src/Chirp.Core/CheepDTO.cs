namespace Chirp.Core;
// The CheepDTO is used to send Cheep data to the frontend for displaying.
public record CheepDTO(int id,  string Author, string Message, DateTime Timestamp);