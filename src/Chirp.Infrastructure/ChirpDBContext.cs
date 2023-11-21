using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Chirp.Infrastructure.Models;

namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<User> Users { get; set; }
    // public string DbPath { get; }

    /*public ChirpDBContext()
    {
        DbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ??
        Path.Combine(Path.GetTempPath(), "chirp.db");
    }*/

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
        : base(options)
    {
    }

    /*protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cheeps
        modelBuilder.Entity<Cheep>( cheep =>
        {
            cheep.Property(ch => ch.CheepId).ValueGeneratedOnAdd();
            cheep.HasKey(ch => ch.CheepId);
            cheep.Property(ch => ch.Text).HasMaxLength(160);
        });

        // Users
        modelBuilder.Entity<User>( user =>
        {
            user.Property(us => us.UserId).ValueGeneratedOnAdd();
            user.HasKey(us => us.UserId);
            user.HasIndex(us => us.Name).IsUnique();
            user.Property(us => us.Email).HasMaxLength(50);

            user.HasMany(us => us.Following)
            .WithOne(us => us.Followers)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // Authors
        modelBuilder.Entity<Author>( author =>
        {
            // General Author properties
            // author.Property(au => au.AuthorId).ValueGeneratedOnAdd();
            author.HasKey(au => au.AuthorId);

            // Establish relationship between Author and Cheeps
            author.HasMany(au => au.Cheeps)
            .WithOne(ch => ch.Author)
            .HasForeignKey(ch => ch.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        });
    }
}