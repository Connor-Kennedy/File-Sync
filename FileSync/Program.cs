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
                Console.WriteLine("10... Date Time Testing");
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

                    case "10":
                        // do something with date time


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

        public static void GetFiles(List<string> files, string destinationRootDirectory)
        {
            foreach (string s in files)
            {
                var fi1 = new FileInfo(s);

                try
                {
                    string offlinePath = convertSourceFilePathToOfflineFilePath(s, destinationRootDirectory);

                    if (offlinePath != null)
                    {
                        string directory = Path.GetDirectoryName(offlinePath);
                        
                        if (Directory.Exists(directory))
                        {
                            if (!File.Exists(offlinePath))
                            {
                                downloadFile(fi1, offlinePath);
                            }
                        } 
                        else
                        {
                            Directory.CreateDirectory(directory);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The process failed: {e.ToString()}");
                }
            }
        }
        
        private static void downloadFile(FileInfo fi, string destinationPath)
        {
            fi.CopyTo(destinationPath);
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

        


        private static string convertSourceFilePathToOfflineFilePath(string sourcePath, string destinationRootDirectory)
        {
            string mappedDrive = sourcePath.Substring(0, 3);
            string offlinePath;
            
            switch (mappedDrive)
            {
                case @"W:\":
                    offlinePath = sourcePath.Replace(mappedDrive, destinationRootDirectory + @"\7plus\");
                    break;
                
                case @"X:\":
                    offlinePath = sourcePath.Replace(mappedDrive, destinationRootDirectory + @"\8plus\");
                    break;
                
                case @"Y:\":
                    offlinePath = sourcePath.Replace(mappedDrive, destinationRootDirectory + @"\9plus\");
                    break;
                
                case @"Z:\":
                    offlinePath = sourcePath.Replace(mappedDrive, destinationRootDirectory + @"\10plus\");
                    break;
                
                case @"L:\":
                    offlinePath = sourcePath.Replace(mappedDrive, destinationRootDirectory + @"\11plus\");
                    break;
                
                case @"M:\":
                    offlinePath = sourcePath.Replace(mappedDrive, destinationRootDirectory + @"\12plus\");
                    break;
                
                default:
                    // ignore files that aren't mapped to one of the above drives
                    return null;
            }

            return offlinePath;
        }
        
    }

    
}

