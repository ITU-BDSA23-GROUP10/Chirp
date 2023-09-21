
// Code taken and refactored from link, section Sixth version:
// https://csharpindepth.com/Articles/Singleton

using SimpleDB;

public sealed class SingletonDB
{
    private static readonly Lazy<SingletonDB> lazy =
        new Lazy<SingletonDB>(() => new SingletonDB());

    public static SingletonDB Instance { get { return lazy.Value; } }

    // Declared public Database outside the method below to allow...
    // reading (get) and writing (set) to the database.
    public IDatabaseRepository<Cheep> Database { get; set; }

    private SingletonDB()
    {
        Database = new CSVDatabase<Cheep>();
    }
}