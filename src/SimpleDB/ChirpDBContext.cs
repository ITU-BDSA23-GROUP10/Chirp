using System.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleDB.Models;
using SQLitePCL;

namespace SimpleDB;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public string DbPath { get; }

    public ChirpDBContext()
    {
        DbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ??
        Path.Combine(Path.GetTempPath(), "chirp.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source={DbPath}");

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options, ModelBuilder modelBuilder)
        :base(options)
    {
        // modelBuilder.Entity<Cheep>()
        // .Property(c => c.CheepId)
        // .IsRequired();
        // modelBuilder.Entity<Cheep>()
        // .HasIndex(c => c.CheepId)
        // .IsUnique();

        //Cheep model
        modelBuilder.Entity<Cheep>()
        .Property(c => c.Author)
        .IsRequired();
        modelBuilder.Entity<Cheep>()
        .HasIndex(c => c.Author)
        .IsUnique();
        modelBuilder.Entity<Cheep>()
        .Property(c => c.Author)
        .HasMaxLength(30);
        modelBuilder.Entity<Cheep>()
        .Property(c => c.Text)
        .HasMaxLength(160);

        //Author model
        modelBuilder.Entity<Author>()
        .HasIndex(au => au.Name)
        .IsUnique();
        modelBuilder.Entity<Author>()
        .Property(au => au.Name)
        .HasMaxLength(30);
        modelBuilder.Entity<Author>()
        .Property(au => au.Email)
        .HasMaxLength(30);
        modelBuilder.Entity<Author>()
        .HasIndex(au => au.Email)
        .IsUnique();

// https://stackoverflow.com/a/64726342 <- ClassValidator o.O

/*mb.Entity<SomeObject>()
            .Property(so => so.Type)
            .IsUnicode(false)
            .HasColumnName("Type")
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired()
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);*/

    
        //modelBuilder.Entity<Followers>().HasKey(k => new k (k.followers, k.follower)
    }

    // Reverted to using Getting Started instead of Razor Page EF Core:
    // https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli#create-the-model
    // now set to allow in-memory database configuration
}