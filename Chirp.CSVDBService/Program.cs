using SimpleDB;
using Chirp.CLI;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.MapPost("/cheep", () => new Cheep("me", "Hej!", 1684229348));
//ReadCheeps
app.MapGet("/cheeps", () => Program.ReadCheeps());

app.Run();

public record Cheep(string Author, string Message, long Timestamp);