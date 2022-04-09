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
        public static string AddFileCommandString(string fullFilePath, string dateModified)
        {
            string command = 
                $@"
                INSERT INTO Files (FullFilePath, DateModified)
                VALUES ('{fullFilePath}','{dateModified}');
                ";
            
            return command;
        }
        
        // read file entry        
        public static string GetFileDateModifiedCommandString(string fullFilePath)
        {
            string command =
                $@"
                    SELECT DateModified
                    FROM Files
                    WHERE FullFilePath = '{fullFilePath}';
                ";

            return command;
        }
        
        // update file entry
        public static string UpdateFileDateModifiedCommandString(string fullFilePath, string dateModified)
        {
            string command = 
                $@"
                UPDATE Files
                SET DateModified = '{dateModified}'
                WHERE FullFilePath = '{fullFilePath}';
                ";

            return command;
        }

        // delete file entry
        public static string DeleteFileCommandString(string fullFilePath)
        {
            string command = 
                $@"
                DELETE FROM Files
                WHERE FullFilePath = '{fullFilePath}';
                ";
            
            return command;
        }

        public static void ExecuteSQL(string connectionString, string commandString)
        {
            using (var connection = new SQLiteConnection($"Data Source={connectionString}"))
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
