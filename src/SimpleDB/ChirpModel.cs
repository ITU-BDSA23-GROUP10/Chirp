using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;

namespace SimpleDB;
public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }

    private string dbPath;
    private string sqlDBFilePath;

    public ChirpDBContext()
    {
        string dbPath;
        if(Environment.GetEnvironmentVariable("CHIRPDBPATH") != null) 
            dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH"); 
        else 
            dbPath = Path.GetTempPath() + "/chirp.db";
        }

        sqlDBFilePath = dbPath;
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={dbPath}");
}