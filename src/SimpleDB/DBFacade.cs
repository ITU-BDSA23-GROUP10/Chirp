using Microsoft.Data.Sqlite;

namespace SimpleDB;


public class DBFacade 
{
    private string sqlDBFilePath = Path.GetTempPath() + "/chirp.db";

    private string sqlQuery;


    public List<Cheep> GetCheeps() 
    {
        sqlQuery = @"SELECT U.username, M.text, M.pub_date FROM message M JOIN user U ON U.user_id = M.author_id ORDER by M.pub_date desc";
        List<Cheep> cheeps; 

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

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

    private static void SQLPrepareStatement(params (string, string)[] values, SQLiteCommand command)
    {
        foreach(string value in values) 
        {
           sqlQuery.Parameters.AddWithValue(value.Item1, value.Item2); 
        }
        sqlQuery.Prepare();
    }

    GetCheepsAuthorSQL(string author)
    {
        


        List<Cheep> cheeps; 

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            using var command = new SQLiteCommand(con);
            command.CommandText = "SELECT U.username, M.text, M.pub_date FROM message M JOIN user U ON U.user_id = M.author_id WHERE U.username = @author ORDER by M.pub_date desc";
            

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

