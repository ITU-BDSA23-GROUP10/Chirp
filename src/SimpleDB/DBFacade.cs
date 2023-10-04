using Microsoft.Data.Sqlite;

namespace SimpleDB;


public class DBFacade 
{
    //private string sqlDBFilePath = Path.GetTempPath() + "/chirp.db";
    private string sqlDBFilePath;

    private string sqlQuery;

    public DBFacade() {
      
        // This shit don't work:
        // // Change the directory to %WINDIR%
        //Environment.SetEnvironmentVariable("CHIRPDBPATH", "");
        //Environment.CurrentDirectory = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        string dbPath = "";
        string test = Environment.CurrentDirectory;
        if(Environment.GetEnvironmentVariable("CHIRPDBPATH") != null) 
        {
          dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH"); 
          //string orignalWorkDir = Environment.CurrentDirectory;
          using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            { 
                
                connection.Open();

                // Code from: https://stackoverflow.com/a/1728859
                string script = File.ReadAllText(@"../Chirp.Razor/data/schema.sql");
                var command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();

                script = File.ReadAllText(@"../Chirp.Razor/data/dump.sql");
                command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();
            }
        } else 
        {
            dbPath = Path.GetTempPath() + "/chirp.db";
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                // Code from: https://stackoverflow.com/a/1728859
                string script = File.ReadAllText(@"../Chirp.Razor/data/schema.sql");
                var command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();

                script = File.ReadAllText(@"../Chirp.Razor/data/dump.sql");
                command = connection.CreateCommand();
                command.CommandText = script;
                command.ExecuteNonQuery();
        }

        }
        // DirectoryInfo info = new DirectoryInfo(".");

        sqlDBFilePath = dbPath;
    }


    public List<Cheep> GetCheeps(int offset, int limit) 
    {
        //limit and offset are for pagination
        sqlQuery = @"SELECT U.username, M.text, M.pub_date FROM message M JOIN user U ON U.user_id = M.author_id ORDER by M.pub_date desc LIMIT @limit OFFSET @offset";
        List<Cheep> cheeps; 

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
    
            //for pagination
            command.Parameters.AddWithValue("@limit", limit);
            command.Parameters.AddWithValue("@offset", offset);
            //end pagination

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

