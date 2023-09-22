using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.MapGet("/cheeps", () => new Cheep("me", "Hej!", 1684229348));

app.Run();

public record Cheep(string Author, string Message, long Timestamp);