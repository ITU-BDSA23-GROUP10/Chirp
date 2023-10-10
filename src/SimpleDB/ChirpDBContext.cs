using Microsoft.EntityFrameworkCore;
using SimpleDB.Models;

namespace SimpleDB;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public string? DbPath { get; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
            : base(options)
        {
        }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? (Path.GetTempPath() + "/chirp.db");
            options.UseSqlite($"Data Source={dbPath}");
        }
}