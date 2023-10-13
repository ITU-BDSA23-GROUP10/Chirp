using Microsoft.EntityFrameworkCore;
using SimpleDB.Models;

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
    
        //modelBuilder.Entity<Followers>().HasKey(k => new k (k.followers, k.follower)
    }
}