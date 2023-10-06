using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.Sqlite;

namespace SimpleDB;


public class DBFacade 
{
    //private string sqlDBFilePath = Path.GetTempPath() + "/chirp.db";
    private string sqlDBFilePath;

    private string sqlQuery;
    public ChirpModel context;


    public DBFacade() {
        context = new ChirpModel();
        DBInitializer(context);
        
    }

    public async Task<int> CountCheeps(string? author = null)
    {
        string countWithAuthor = @"SELECT COUNT(M.text) FROM message M JOIN user U ON U.user_id = M.author_id WHERE U.username = @author";
        string countAll = @"SELECT COUNT(text) FROM message";

        sqlQuery = (author != null) ? countWithAuthor : countAll;

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            if (author != null)
            {
                (string, string) values = ("@author", author);
                SQLPrepareStatement(command, values);
            }

            //Code from: https://stackoverflow.com/a/75859283
            var result = await command.ExecuteScalarAsync();
            // Inspired by comment: https://stackoverflow.com/questions/4958379/what-is-the-difference-between-null-and-system-dbnull-value#comment20987621_4958408
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }

            return 1;
        }
    }

    public List<Cheep> GetCheeps(int offset, int limit) 
    {
       
    }

    private static void SQLPrepareStatement(SqliteCommand command, params (string, string)[] values)
    {
        foreach((string, string) value in values) 
        {
           command.Parameters.AddWithValue(value.Item1, value.Item2); 
        }
        command.Prepare();
    }

    public List<Cheep> GetCheepsAuthorSQL(string author, int offset, int limit)
    {
        List<Cheep> cheeps; 

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            
            //limit and offset are for pagination
            command.CommandText = "SELECT U.username, M.text, M.pub_date FROM message M JOIN user U ON U.user_id = M.author_id WHERE U.username = @author ORDER by M.pub_date desc LIMIT @limit OFFSET @offset";
            
            //for pagination
            command.Parameters.AddWithValue("@limit", limit);
            command.Parameters.AddWithValue("@offset", offset);
            //end pagination
            
            (string, string) values = ("@author", author);
            SQLPrepareStatement(command, values);

            using var reader = command.ExecuteReader();
            cheeps = new List<Cheep>();
            while (reader.Read())
            { 
                cheeps.Add(new Cheep() 
                {
                    Author = reader.GetString(0), 
                    Message = reader.GetString(1), 
                    Timestamp = reader.GetInt64(2)
                });  
            }
            return cheeps;
        } 
    }
   

}
