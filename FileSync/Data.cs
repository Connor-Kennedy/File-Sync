using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Collections.Specialized;

namespace FileSync
{
    static class Data
    {
        private static string connectionString = "sqlitedatabase.db";

        // create file table
        public static string CreateFilesTableCommandString()
        {
            string command = 
                @"
                CREATE TABLE IF NOT EXISTS Files (
                FileID INTEGER PRIMARY KEY AUTOINCREMENT,
                FullFilePath TEXT NOT NULL UNIQUE,
                DateModified TEXT NOT NULL
                );
                ";
            
            return command;
        }

        // read file table
        public static string GetFilesCommandString()
        {
            string command =
                @"
                SELECT * FROM Files;
                ";

            return command;
        }

        // delete file table
        public static string DeleteFilesTableCommand() 
        {
            string command = 
                @"
                DROP TABLE IF EXISTS Files;
                ";
            
            return command;
        }

        // create file entry
        public static string AddFile(string fullFilePath, string dateModified)
        {
            using (var connection = new SQLiteConnection($"Data Source={Data.connectionString}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"
                        INSERT INTO Files (FullFilePath, DateModified)
                        VALUES ('{fullFilePath}','{dateModified}');
                    ";

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
                return null;
            }
        }
        
        // read file entry        
        public static string GetFileDateModified(string fullFilePath)
        {
            using (var connection = new SQLiteConnection($"Data Source={Data.connectionString}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = 
                    $@"
                        SELECT DateModified
                        FROM Files
                        WHERE FullFilePath = '{fullFilePath}';
                    ";

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //there will only be 1 index for above query...
                            return reader.GetString(0);                            
                        }
                    }
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;

                }
                
                return null;
            
            }
            
            
            
            

            


        }
        
        // update file entry
        public static string UpdateFileDateModified(string fullFilePath, string dateModified)
        {
            using (var connection = new SQLiteConnection($"Data Source={Data.connectionString}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"
                        UPDATE Files
                        SET DateModified = '{dateModified}'
                        WHERE FullFilePath = '{fullFilePath}';
                    ";
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
                return null;
            }
        }

        // delete file entry
        public static string DeleteFile(string fullFilePath)
        {
            using (var connection = new SQLiteConnection($"Data Source={Data.connectionString}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"
                        DELETE FROM Files
                        WHERE FullFilePath = '{fullFilePath}';
                        ";
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
                return null;
            }
        }

        public static void ExecuteSQL(string commandString)
        {
            using (var connection = new SQLiteConnection($"Data Source={Data.connectionString}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = commandString;

                try 
                {
                    using (var reader = command.ExecuteReader())
                    {
                        //todo: make this better! return collection to caller??
                        
                        string keysString = String.Empty;
                        string valuesString = String.Empty;

                        while (reader.Read())
                        {
                            var DBResponse = reader.GetValues();//(0);//GetString(0);

                            foreach (string s in DBResponse.AllKeys)
                            {
                                //Console.WriteLine($"DB Response: {DBResponse[s]}");
                                if (!keysString.Contains(s))
                                {
                                    keysString += $"{s}, ";
                                }

                                valuesString += $"{DBResponse[s]}, ";
                            }
                            valuesString += "\n";

                        }
                        Console.WriteLine(keysString);
                        Console.WriteLine(valuesString);
                    }
                }
                catch (System.Data.SQLite.SQLiteException e)
                {
                    Console.WriteLine($"Database Error: {e}");
                }

                
            }
        }
    }
}
