using System;
using System.Collections.Generic;
using System.IO;

// sync cases:

// file exists only in DB
    // delete from DB?
// file exists only locally
    // create DB entry and copy to server
// file exists only on server
    // create DB entry and copy to local
// file exists in db and locally - but not on server
    // file has been deleted from server...... delete locally and from db.
// file exists in db and server - but not locally
    // file has been deleted locally.......... delete from server and db.
// file exists locally and on server - but not on DB
    // add file to DB
    // SYNC.. Latest modified file as correct????????
// file exists in all 3
    // SYNC.. 5 cases based on lastModified date for each copy:
        // CASE 1 OF 5: db date == local date == server date
        // no sync required
        // CASE 2 OF 5: db date != local date != server date
        // conflict...... highlight and manually sort. program does nothing...
        // CASE 3 OF 5: db date == local date
        // server version is newer, get server version
        // CASE 4 OF 5: db date == server date
        // local version is newer, upload local version to server
        // CASE 5 OF 5: local date == server date
        // DB update error somewhere? try update DB to local or server date..


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
                Console.WriteLine("6... Blank");
                Console.WriteLine("7... Blank");
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
                        Data.ExecuteSQL(Data.CreateFilesTableCommandString());
                        break;
                    case "4":
                        Data.ExecuteSQL(Data.GetFilesCommandString());
                        break;
                    case "5":
                        Data.ExecuteSQL(Data.DeleteFilesTableCommand());
                        break;
                    case "6":
                        
                        break;
                    case "7":
                        
                        break;
                    case "8":
                        Console.Write("File:");
                        string file8 = Console.ReadLine();
                        Console.Write("New DateModified:");
                        string date8 = Console.ReadLine();
                        Data.ExecuteSQL(Data.UpdateFileDateModified(file8, date8));
                        break;
                    case "9":
                        Console.Write("File:");
                        string file9 = Console.ReadLine();
                        Data.ExecuteSQL(Data.DeleteFile(file9));
                        break;

                    case "10":
                        // do something with date time
                        // put file name, check DB if file exists, return date modified if so...

                        Console.WriteLine("Enter Offline Full File Path");
                        string path10 = Console.ReadLine();
                        Console.WriteLine(Data.GetFileDateModified(path10));

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
                            syncFile(fi1, offlinePath);
                        } 
                        else
                        {
                            Directory.CreateDirectory(directory);
                            syncFile(fi1, offlinePath);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The process failed: {e.ToString()}");
                }
            }
        }
        
        private static void syncFile(FileInfo fi, string offlineFilePath)
        {
            string dbDateModified = Data.GetFileDateModified(offlineFilePath);
            
            if (dbDateModified != null)
            {
                // TODO what if file exists locally but not on server or DB ie new file created offline... need to sync to server
                
                // file exists in database

                // check that file exists in offline dir
                if (File.Exists(offlineFilePath))
                {
                    if (fi.Exists)
                    {
                        // compare file dates and sync accordingly

                        // get file info of existing offline copy
                        FileInfo fiOfflineCopy = new FileInfo(offlineFilePath);

                        DateTime dbDate = DateTime.Parse(dbDateModified);
                        DateTime localDate = fiOfflineCopy.LastWriteTime;
                        DateTime serverDate = fi.LastWriteTime;

                        // sync cases when a file is stored offline

                        // CASE 1 OF 5: db date == local date == server date
                        // no sync required
                        if (dbDate == localDate && dbDate == serverDate && localDate == serverDate)
                        {
                            // do nothing
                        }
                        // CASE 2 OF 5: db date != local date != server date
                        // conflict...... highlight and manually sort. program does nothing...
                        else if (dbDate != localDate && dbDate != serverDate && localDate != serverDate)
                        {
                            Console.WriteLine($"Conflict with file: {fiOfflineCopy.FullName}");
                        }
                        // CASE 3 OF 5: db date == local date
                        // server version is newer, get server version
                        else if (dbDate == localDate && serverDate > dbDate)
                        {
                            try
                            {
                                fi.CopyTo(offlineFilePath, true);
                                Data.UpdateFileDateModified(fi.FullName, fi.LastWriteTime.ToString());
                            } catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            
                        }
                        // CASE 4 OF 5: db date == server date
                        // local version is newer, upload local version to server
                        else if (dbDate == serverDate && localDate > dbDate)
                        {
                            try
                            {
                                fiOfflineCopy.CopyTo(fi.FullName, true);
                                Data.UpdateFileDateModified(fi.FullName, fiOfflineCopy.LastWriteTime.ToString());
                            } 
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        // CASE 5 OF 5: local date == server date
                        // DB update error somewhere? try update DB to local or server date..
                        else if (dbDate != localDate && localDate == serverDate)
                        {
                            Console.WriteLine("Potential error...");
                            Data.UpdateFileDateModified(fi.FullName, fiOfflineCopy.LastWriteTime.ToString());
                        }
                    } 
                    else
                    {
                        //file doesn't exist on server
                    }







                }
                else
                {
                    // TODO: CHECK THIS IS OPERATION WE WANT --> file has been deleted from local... delete from DB & re-download
                    Data.DeleteFile(offlineFilePath);                  
                    fi.CopyTo(offlineFilePath);
                    // add file to database
                    Data.AddFile(offlineFilePath, fi.LastWriteTime.ToString());
                }

                
                
                
            } else
            {
                Console.WriteLine("Test2");
                // file doesn't exist in database
                fi.CopyTo(offlineFilePath);
                // add file to database
                Data.AddFile(offlineFilePath, fi.LastWriteTime.ToString());
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

