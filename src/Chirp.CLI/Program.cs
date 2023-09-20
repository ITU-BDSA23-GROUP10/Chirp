﻿using DocoptNet;
using SimpleDB;

public class Program
{
    //documentation from https://docopt.github.io/docopt.net/dev/
    private const string Help =
            @"Chirp

            Usage:
            Chirp (--read | --cheep <message>)
            Chirp -h | --help

            Options:
            -h --help         Show this screen.
            --read            Print all the cheeps.
            --cheep <message> Post a cheep.

            (with dotnet run)
            dotnet run -- -h (help)
            dotnet run -- --read
            dotnet run -- --cheep <message>
            ";

    private static IDatabaseRepository<Cheep> db = new CSVDatabase<Cheep>();
    private static UserInterface ui = new UserInterface();

    public static void Main(string[] args)
    {
        // Docopt code documentation reworked from https://csharp.hotexamples.com/site/file?hash=0xff1fe91471d0685c57d186e7951993e3fbf382bccde2e3daaaa6c22982498b4b
        // and https://docopt.github.io/docopt.net/dev/#api
        var arguments = new Docopt().Apply(Help, args, exit: true);

        if (arguments["--read"].IsTrue)
        {
            ReadCheeps();
        }
        else if (!arguments["--cheep"].IsNullOrEmpty)
        {
            PostCheep(arguments["--cheep"].Value.ToString());
        }
    }

    public static void ReadCheeps()
    {
        var cheeps = db.Read();
        ui.PrintCheeps(cheeps);
    }

    public static void PostCheep(string message)
    {
        var newCheep = new Cheep
        {
            Message = message,
            Author = Environment.UserName,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        var records = new List<Cheep> { newCheep };
        db.Store(records);
    }
}