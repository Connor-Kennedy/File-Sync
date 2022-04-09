using System;
using System.Collections.Generic;
using System.IO;



namespace FileSync
{
    class Program
    {
        static void Main(string[] args)
        {
            string offlineFilesLocation = @"C:\Users\conno\Desktop\MyOfflineLocation";
            string sourceFilesLocation = @"C:\Users\conno\Desktop\WatchedFiles";
            string csvPath = @"M:\12625\01 - Swan Neck Thermodrive Conveyor\12625-01GA-001.csv";

            CreateDirectory(offlineFilesLocation);
            CreateDirectory(sourceFilesLocation);


            while (true)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Main Menu");
                Console.WriteLine();
                Console.WriteLine("1... Print Hello World!");
                Console.WriteLine("2... Get Server Files");
                Console.WriteLine("3... Database Operation: Create Files Table");
                Console.WriteLine("4... Database Operation: Read Files DB Table");
                Console.WriteLine("5... Database Operation: Delete Files DB Table");
                Console.WriteLine("6... Database Operation: Create test file");
                Console.WriteLine("7... Database Operation: Read date modified of a file");
                Console.WriteLine("8... Database Operation: Update file");
                Console.WriteLine("9... Database Operation: Delete file");
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("Hello World");
                        break;

                    case "2":
                        Console.WriteLine("Get CSV Files");
                        List<string> requiredFiles;
                        try
                        {
                            requiredFiles = GetRequiredFilesList(csvPath);
                            GetFiles(requiredFiles, offlineFilesLocation);

                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine($"Failure: Check VPN connection");
                            Console.WriteLine(e);
                            Console.WriteLine();
                        }
                        break;

                    case "3":
                        Data.ExecuteSQL("sqlitedatabase.db", Data.CreateFilesTableCommandString());
                        break;
                    case "4":
                        Data.ExecuteSQL("sqlitedatabase.db", Data.GetFilesCommandString());
                        break;
                    case "5":
                        Data.ExecuteSQL("sqlitedatabase.db", Data.DeleteFilesTableCommand());
                        break;
                    case "6":
                        Console.Write("File:");
                        string file6 = Console.ReadLine();
                        Console.Write("DateModified:");
                        string file6Date = Console.ReadLine();
                        Data.ExecuteSQL("sqlitedatabase.db", Data.AddFileCommandString(file6, file6Date));
                        break;
                    case "7":
                        Console.Write("File:");
                        string file7 = Console.ReadLine();
                        Data.ExecuteSQL("sqlitedatabase.db", Data.GetFileDateModifiedCommandString(file7));
                        break;
                    case "8":
                        Console.Write("File:");
                        string file8 = Console.ReadLine();
                        Console.Write("New DateModified:");
                        string date8 = Console.ReadLine();
                        Data.ExecuteSQL("sqlitedatabase.db", Data.UpdateFileDateModifiedCommandString(file8, date8));
                        break;
                    case "9":
                        Console.Write("File:");
                        string file9 = Console.ReadLine();
                        Data.ExecuteSQL("sqlitedatabase.db", Data.DeleteFileCommandString(file9));
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

            }

        }

        public static void CreateDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            try
            {
                if (di.Exists)
                {
                    Console.WriteLine("Directory already exists!");
                    return;
                }

                di.Create();
                Console.WriteLine("Directory created.");
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }
        }

        public static void GetFiles(List<string> files, string destinationFileLocation)
        {
            foreach (string s in files)
            {
                var fi1 = new FileInfo(s);

                try
                {

                    if (s.Contains(@"W:\"))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(s.Replace(@"W:\", destinationFileLocation + @"\7plus\"))))
                        {
                            fi1.CopyTo(s.Replace(@"W:\", destinationFileLocation + @"\7plus\"));
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(s.Replace(@"W:\", destinationFileLocation + @"\7plus\")));
                            fi1.CopyTo(s.Replace(@"W:\", destinationFileLocation + @"\7plus\"));
                        }
                    }
                    else if (s.Contains(@"X:\"))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(s.Replace(@"X:\", destinationFileLocation + @"\8plus\"))))
                        {
                            fi1.CopyTo(s.Replace(@"X:\", destinationFileLocation + @"\8plus\"));
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(s.Replace(@"X:\", destinationFileLocation + @"\8plus\")));
                            fi1.CopyTo(s.Replace(@"X:\", destinationFileLocation + @"\8plus\"));
                        }
                    }
                    else if (s.Contains(@"Y:\"))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(s.Replace(@"Y:\", destinationFileLocation + @"\9plus\"))))
                        {
                            fi1.CopyTo(s.Replace(@"Y:\", destinationFileLocation + @"\9plus\"));
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(s.Replace(@"Y:\", destinationFileLocation + @"\9plus\")));
                            fi1.CopyTo(s.Replace(@"Y:\", destinationFileLocation + @"\9plus\"));
                        }
                    }
                    else if (s.Contains(@"Z:\"))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(s.Replace(@"Z:\", destinationFileLocation + @"\10plus\"))))
                        {
                            fi1.CopyTo(s.Replace(@"Z:\", destinationFileLocation + @"\10plus\"));
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(s.Replace(@"Z:\", destinationFileLocation + @"\10plus\")));
                            fi1.CopyTo(s.Replace(@"Z:\", destinationFileLocation + @"\10plus\"));
                        }
                    }
                    else if (s.Contains(@"L:\"))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(s.Replace(@"L:\", destinationFileLocation + @"\11plus\"))))
                        {
                            fi1.CopyTo(s.Replace(@"L:\", destinationFileLocation + @"\11plus\"));
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(s.Replace(@"L:\", destinationFileLocation + @"\11plus\")));
                            fi1.CopyTo(s.Replace(@"L:\", destinationFileLocation + @"\11plus\"));
                        }
                    }
                    else if (s.Contains(@"M:\"))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(s.Replace(@"M:\", destinationFileLocation + @"\12plus\"))))
                        {
                            fi1.CopyTo(s.Replace(@"M:\", destinationFileLocation + @"\12plus\"));
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(s.Replace(@"M:\", destinationFileLocation + @"\12plus\")));
                            fi1.CopyTo(s.Replace(@"M:\", destinationFileLocation + @"\12plus\"));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The process failed: {e.ToString()}");
                }
            }
        }
        
        public static List<string> GetRequiredFilesList(string csvPath)
        {
            List<string> requiredFiles = new List<string>();
            
            using (var reader = new StreamReader(csvPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (values[0].Contains(@"W:\"))
                    {
                        requiredFiles.Add(values[0]);
                    }
                    else if (values[0].Contains(@"X:\"))
                    {
                        requiredFiles.Add(values[0]);
                    }
                    else if (values[0].Contains(@"Y:\"))
                    {
                        requiredFiles.Add(values[0]);
                    }
                    else if (values[0].Contains(@"Z:\"))
                    {
                        requiredFiles.Add(values[0]);
                    }
                    else if (values[0].Contains(@"L:\"))
                    {
                        requiredFiles.Add(values[0]);
                    }
                    else if (values[0].Contains(@"M:\"))
                    {
                        requiredFiles.Add(values[0]);
                    }
                }
            }
            return requiredFiles;
        }

        
    }
}
