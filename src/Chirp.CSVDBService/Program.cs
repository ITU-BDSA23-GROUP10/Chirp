using SimpleDB;
using Chirp;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

SingletonDB dbSingleton = SingletonDB.Instance;
IDatabaseRepository<SimpleDB.Cheep> db = dbSingleton.Database;



var newCheep = new SimpleDB.Cheep
        {
            Message = "test",
            Author = "hejehj",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

//app.MapPost("/cheep", () => new Chirp.Program.PostCheep(message));
//ReadCheeps
//app.MapGet("/cheeps", () => {});

//This shows all the cheeps from the db onto the localhost as a JSON file
app.MapGet("/cheeps", () => db.Read());

//This works to create a new cheep in the CSVfile but it uses Get so it clearly is wrong
//app.MapGet("/cheep", () => db.Store(newCheep));

//This resuls in a 405 method not allowed on the network tab when you try it in a webbrowser and doesnt actually save the cheep
app.MapPost("/cheep", (Cheep cheep) => db.Store(cheep));

app.Run();



public record Cheep(string Author, string Message, long Timestamp);