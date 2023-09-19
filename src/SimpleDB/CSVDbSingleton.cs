
// Code taken and refactored from link, section Sixth version:
// https://csharpindepth.com/Articles/Singleton

using SimpleDB;

public sealed class CSVDbSingleton
{
    private static readonly Lazy<CSVDbSingleton> lazy =
        new Lazy<CSVDbSingleton>(() => new CSVDbSingleton());

    public static CSVDbSingleton Instance { get { return lazy.Value; } }

    public IDatabaseRepository<Cheep> Database { get; set; }

    private CSVDbSingleton()
    {
        Database = new CSVDatabase<Cheep>();
    }
}