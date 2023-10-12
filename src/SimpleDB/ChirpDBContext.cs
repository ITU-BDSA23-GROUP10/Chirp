using Microsoft.EntityFrameworkCore;
using SimpleDB.Models;

namespace SimpleDB;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public string DbPath { get; } = Environment.GetEnvironmentVariable("CHIRPDBPATH") ??
    Path.Combine(Path.GetTempPath(), "chirp.db");

    // Parameterless from Reddit:
    //https://www.reddit.com/r/dotnet/comments/rp90n2/unable_to_create_an_object_of_type_mycontext_for/
    public ChirpDBContext()
    {
    }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
        :base(options)
    {
    }

    // Reverted to using Getting Started instead of Razor Page EF Core:
    // https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli#create-the-model
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");
}