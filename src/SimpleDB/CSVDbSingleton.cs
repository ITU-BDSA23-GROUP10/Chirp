
// Code taken and refactored from link, section Sixth version:
// https://csharpindepth.com/Articles/Singleton

using SimpleDB;

public sealed class CSVDbSingleton
{
    private static readonly Lazy<CSVDbSingleton> lazy =
        new Lazy<CSVDbSingleton>(() => new CSVDbSingleton());

    public static CSVDbSingleton Instance { get { return lazy.Value; } }

    // IsCreated method by StackOverflow user: https://stackoverflow.com/a/30671363
    public bool IsCreated => lazy.IsValueCreated;

    // Declared public Database outside the method below to allow...
    // reading (get) and writing (set) to the database.
    public IDatabaseRepository<Cheep> Database { get; set; }

    private CSVDbSingleton()
    {
        Database = new CSVDatabase<Cheep>();
    }
}