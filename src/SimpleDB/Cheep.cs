namespace SimpleDB;
public record Cheep
{
    public int CheepId { get; set; }
    public new Author author { get; set; }
    public string Message { get; set; }

    public long Timestamp { get; set;}
}