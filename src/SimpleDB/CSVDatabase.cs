﻿namespace SimpleDB;

using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null) 
    {
        // Read datafile with CsvHelper
        // https://joshclose.github.io/CsvHelper/examples/writing/appending-to-an-existing-file/
        using (var reader = new StreamReader("Data/chirp_cli_db.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<T>();
            return records.ToList();
        }
        
        
    }
    
    public void Store(List<T> records)
    {
        // Write to new cheep to csv file using CsvHelper
        // https://joshclose.github.io/CsvHelper/examples/writing/appending-to-an-existing-file/
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };

        using (var stream = File.Open("Data/chirp_cli_db.csv", FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(records);
        }
    }

}

