using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    //public string DbPath { get; }

    public ChirpDBContext()
    {
        /*DbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ??
        Path.Combine(Path.GetTempPath(), "chirp.db");*/
    }

    /*protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cheeps
        modelBuilder.Entity<Cheep>().Property(ch => ch.Text).HasMaxLength(160);

        // Author
        modelBuilder.Entity<Author>().HasIndex(au => au.Name).IsUnique();
        modelBuilder.Entity<Author>().HasIndex(au => au.Email).IsUnique();
        modelBuilder.Entity<Author>().Property(au => au.Email).HasMaxLength(50);
        
        modelBuilder.Entity<Author>()
            .HasMany(au => au.Cheep).WithOne(ch => ch.Author).
            HasForeignKey<Cheep>(ch => ch.AuthorId).
            IsReQuired(false);
    }
}