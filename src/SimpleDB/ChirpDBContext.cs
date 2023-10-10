using Microsoft.EntityFrameworkCore;
using SimpleDB.Models;

namespace SimpleDB;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    // Parameterless from Reddit:
    //https://www.reddit.com/r/dotnet/comments/rp90n2/unable_to_create_an_object_of_type_mycontext_for/
    /*public ChirpDBContext()
    {
    }*/

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
        :base(options)
    {
    }
}