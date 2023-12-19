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
    public DbSet<Follows> Follows { get; set; }
    public DbSet<Reaction> Reactions { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
        : base(options)
    {
    }
    // This is used to set model options for the database.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cheeps
        modelBuilder.Entity<Cheep>(cheep =>
        {
            cheep.Property(ch => ch.CheepId).ValueGeneratedOnAdd();
            cheep.HasKey(ch => ch.CheepId);
            cheep.Property(ch => ch.Text).HasMaxLength(160);
            cheep.HasOne(ch => ch.Author)
                .WithMany(au => au.Cheeps)
                .HasForeignKey(ch => ch.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        // Users
        modelBuilder.Entity<User>( user =>
        {
            user.Property(us => us.UserId).ValueGeneratedOnAdd();
            user.HasKey(us => us.UserId);
            user.HasIndex(us => us.Name).IsUnique();
            user.Property(us => us.Email).HasMaxLength(50);
        });

        // Follows
        modelBuilder.Entity<Follows>( follows => 
        {
            // code from https://stackoverflow.com/a/2912896
            follows.HasKey(_follow => new { _follow.FollowerId, _follow.FollowingId });
            follows.HasIndex(_follow => new { _follow.FollowerId, _follow.FollowingId }).IsUnique();
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

        //Reactions
        modelBuilder.Entity<Reaction>( reactions =>
        {
            reactions.HasKey(_react => new {_react.cheepId, _react.userId});
            reactions.HasIndex(_react => new {_react.cheepId, _react.userId}).IsUnique();
        });
    }
}