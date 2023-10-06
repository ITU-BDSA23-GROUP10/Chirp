using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;

namespace SimpleDB;
public class ChirpModel : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }

    private string dbPath;
    private string sqlDBFilePath;

    public ChirpModel()
    {
        string dbPath;
        if(Environment.GetEnvironmentVariable("CHIRPDBPATH") != null) 
            dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH"); 
        else 
            dbPath = Path.GetTempPath() + "/chirp.db";

        if (!File.Exists(dbPath))
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            { 
                connection.Open();

                // Code from: https://stackoverflow.com/a/1728859
                string script = File.ReadAllText(@"data/schema.sql");
                var command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();

                script = File.ReadAllText(@"data/dump.sql");
                command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();
            }
        }

        sqlDBFilePath = dbPath;
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={dbPath}");
}