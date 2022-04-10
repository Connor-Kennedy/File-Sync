using System;
using System.Collections.Generic;
using System.IO;

namespace FileSync
{
    class Program
    {
        static void Main(string[] args)
        {
            string offlineFilesRootLocation = $@"{System.Environment.GetEnvironmentVariable("USERPROFILE")}\Desktop\OfflineFiles";
            
            while (true)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Main Menu");
                Console.WriteLine();
                Console.WriteLine("1. Download Offline Files from CSV + Sync CSV Files");
                Console.WriteLine("2. Sync Existing Offline Files");
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();

                switch (Console.ReadLine())
                {
                    case "1":

                        Console.WriteLine(@"Paste CSV Path without quotes eg. M:\12625\01 - Swan Neck Thermodrive Conveyor\12625-01GA-001.csv");

                        string csvPath = Console.ReadLine();

                        FileInfo csvFile = new FileInfo(csvPath);

                        if (csvFile.Exists && csvFile.Extension.Contains(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("CSV File found!");
                            Console.WriteLine("Syncing Files...");

                            CreateDirectory(offlineFilesRootLocation);
                            List<string> csvFiles = GetServerFilesList(csvPath);
                            Sync.syncFiles(csvFiles, offlineFilesRootLocation, true);

                        } else
                        {
                            Console.WriteLine("CSV File not found, try again or complain to Connor!");
                        }

                        break;

                    case "2":

                        CreateDirectory(offlineFilesRootLocation);
                        List<string> offlineFiles = new List<string>(Directory.GetFiles(offlineFilesRootLocation,"*.*", SearchOption.AllDirectories));
                        Sync.syncFiles(offlineFiles, offlineFilesRootLocation, false);

                        break;
                    
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

            }

        }

        private static void CreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine("An OfflineFiles directory has been created on your Desktop.");
            }
            else
            {
                Console.WriteLine($"Directory already exists: {directoryPath}");
            }
        }

        public static List<string> GetServerFilesList(string csvPath)
        {
            List<string> serverFiles = new List<string>();
            
            using (var reader = new StreamReader(csvPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    serverFiles.Add(values[0]);
                }
            }
            return serverFiles;
        }

      
    }

    
}

