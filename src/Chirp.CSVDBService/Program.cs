using SimpleDB;
using Chirp;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

SingletonDB dbSingleton = SingletonDB.Instance;
IDatabaseRepository<SimpleDB.Cheep> db = dbSingleton.Database;

//This shows all the cheeps from the db onto the localhost as a JSON file
app.MapGet("/cheeps", () => db.Read());

// to see the post through curl (make sure your csv data file has the actual data in it)
// curl -X POST -H "Content-Type: application/json" -d "{\"Author\":\"Alexa\", \"Message\":\"measse\"}" http://localhost:5076/cheep
app.MapPost("/cheep", (Cheep newCheep) => 
{
    var cheepWithTimestamp = new SimpleDB.Cheep 
    { 
        Author = newCheep.Author, 
        Message = newCheep.Message, 
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() 
    };
    db.Store(cheepWithTimestamp);
    return Results.Created($"/cheep/{cheepWithTimestamp.Timestamp}", cheepWithTimestamp);
});
app.Run();

/*
to see the post through curl (make sure your csv data file has the actual data in it)
// curl -X POST -H "Content-Type: application/json" -d "{\"Author\":\"Alexa\", \"Message\":\"measse\"}" http://localhost:5076/cheep
*/


public record Cheep(string Author, string Message, long Timestamp);