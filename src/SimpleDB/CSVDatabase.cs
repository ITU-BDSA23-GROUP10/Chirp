namespace SimpleDB;

using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Text;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null) 
    {
        // Read datafile with CsvHelper
        // https://joshclose.github.io/CsvHelper/examples/writing/appending-to-an-existing-file/
        using (var reader = new StreamReader("../Data/chirp_cli_db.csv", Encoding.UTF8))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<T>();
            var recordsList = records.ToList();
            if (limit == null || limit >= recordsList.Count)
                return recordsList;
            else
                return recordsList.GetRange(recordsList.Count() - (int)limit - 1, (int)limit);
        }
    }
    
    public void Store(T record)
    {
        // Write to new cheep to csv file using CsvHelper
        // https://joshclose.github.io/CsvHelper/examples/writing/appending-to-an-existing-file/
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
            // Ensures quotation-encapsulation, from StackOverflow:
            // https://stackoverflow.com/a/69581108
            ShouldQuote = args => true,
        };

        using (var stream = File.Open("../Data/chirp_cli_db.csv", FileMode.Append))
        using (var writer = new StreamWriter(stream, Encoding.UTF8))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }

}


