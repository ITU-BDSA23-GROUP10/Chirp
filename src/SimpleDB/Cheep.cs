namespace SimpleDB;
using System.DateTime;
public record Cheep
{
    public int CheepId { get; set; }
    public Author Author { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set;}
}