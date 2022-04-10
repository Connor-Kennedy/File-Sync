using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileSync
{
    static class Sync
    {

        // sync cases:

        // file exists only in DB
        // -- delete from DB?
        // file exists only locally
        // -- copy to server and create DB entry
        // file exists only on server
        // -- copy to local and create DB entry
        // file exists in db and locally - but not on server
        // -- file has been deleted from server...... delete locally and from db.
        // file exists in db and server - but not locally
        // -- file has been deleted locally.......... delete from db (and server?????????)
        // file exists locally and on server - but not on DB
        // -- add file to DB
        // -- SYNC.. Latest modified file as correct????????
        // file exists in all 3
        // -- SYNC.. 5 cases based on lastModified date for each copy:
        // -- -- CASE 1 OF 5: db date == local date == server date
        // -- -- -- no sync required
        // -- -- CASE 2 OF 5: db date != local date != server date
        // -- -- -- conflict...... highlight and manually sort. program does nothing...
        // -- -- CASE 3 OF 5: db date == local date
        // -- -- -- server version is newer, get server version
        // -- -- CASE 4 OF 5: db date == server date
        // -- -- -- local version is newer, upload local version to server
        // -- -- CASE 5 OF 5: local date == server date
        // -- -- -- DB update error somewhere? try update DB to local or server date..

        //TODO Implement functionallity to ignore lockfiles?

        public static void syncFiles(List<string> filePathList, string offlineRootDirectory, bool filesServerFormat)
        {
            foreach (string filePath in filePathList)
            {
                // ignore any kind of lock file
                if (!filePath.Contains(".lck", StringComparison.OrdinalIgnoreCase))
                {
                    if (filesServerFormat)
                    {
                        string offlineFilePath = convertServerFilePathToOfflineFilePath(filePath, offlineRootDirectory);
                        if (offlineFilePath != null) 
                        {
                            syncFile(offlineFilePath, filePath);
                        }
                    } 
                    else
                    {
                        string serverFilePath = convertOfflineFilePathToServerFilePath(filePath, offlineRootDirectory);
                        if (serverFilePath != null)
                        {
                            syncFile(serverFilePath, filePath);
                        }
                    }
                }
            }
        }

        private static void syncFile(string offlineFilePath, string serverFilePath)
        {
            FileInfo serverFile = new FileInfo(serverFilePath);
            FileInfo offlineFile = new FileInfo(offlineFilePath);

            DateTime serverFileDate = serverFile.LastWriteTime;
            DateTime offlineFileDate = offlineFile.LastWriteTime;
            DateTime dbFileDate;


            string dbDateModified = Data.GetFileDateModified(offlineFilePath);
            bool dbFileExists = false;

            if (dbDateModified != null)
            {
                dbFileExists = true;
                dbFileDate = DateTime.Parse(dbDateModified);
            }


            // file exists only in DB
            if (!serverFile.Exists && !offlineFile.Exists && dbFileExists)
            {
                // delete from DB?
                Data.DeleteFile(offlineFilePath);
            }
            // file exists only locally
            else if (!serverFile.Exists && offlineFile.Exists && !dbFileExists)
            {
                // copy to server and create DB entry
                offlineFile.CopyTo(serverFilePath);
                Data.AddFile(offlineFilePath, offlineFileDate.ToString());
            }
            // file exists only on server
            else if (serverFile.Exists && !offlineFile.Exists && !dbFileExists)
            {
                // copy to local and create DB entry
                createDirectory(offlineFilePath);
                serverFile.CopyTo(offlineFilePath);
                Data.AddFile(offlineFilePath, serverFileDate.ToString());
            }
            // file exists in db and locally - but not on server
            else if (!serverFile.Exists && offlineFile.Exists && dbFileExists)
            {
                // file has been deleted from server...... delete locally and from db.
                offlineFile.Delete();
                Data.DeleteFile(offlineFilePath);
            }
            // file exists in db and server - but not locally
            else if (serverFile.Exists && !offlineFile.Exists && dbFileExists)
            {
                // TODO: WHAT FUNCTIONALITY DO WE WANT? CHECK...
                // OPTION 1: file has been deleted locally.......... delete from server and db.
                // OPTION 2: OR file has been deleted locally...... delete from DB and no longer store offline....
                
                // serverFile.Delete(); UNCOMMENT THIS LINE FOR OPTION 1...
                Data.DeleteFile(offlineFilePath);
            }
            // file exists locally and on server - but not on DB
            else if (serverFile.Exists && offlineFile.Exists && !dbFileExists)
            {
                // add file to DB.. which one?
                // TODO: CHECK: SYNC.. Latest modified file as correct????????
                if (offlineFileDate > serverFileDate)
                {
                    // offline file is newer than server file, update server file
                    offlineFile.CopyTo(serverFilePath, true);
                    Data.AddFile(offlineFilePath, offlineFileDate.ToString());
                } else if (offlineFileDate < serverFileDate)
                {
                    // server file is newer than offline file, update offline file
                    createDirectory(offlineFilePath);
                    serverFile.CopyTo(offlineFilePath, true);
                    Data.AddFile(offlineFilePath, serverFileDate.ToString());
                } 
                else
                {
                    // files are already the same, add to DB only
                    Data.AddFile(offlineFilePath, offlineFileDate.ToString());
                }
            }
            // file exists in all 3
            else if (serverFile.Exists && offlineFile.Exists && dbFileExists)
            {
                // SYNC.. 5 cases based on lastModified date for each copy:

                // in case dbFileDate wasn't already instantiated... is there a better way to handle this?
                dbFileDate = DateTime.Parse(dbDateModified);

                // CASE 1 OF 5: db date == local date == server date
                if (serverFileDate == offlineFileDate && serverFileDate == dbFileDate && offlineFileDate == dbFileDate)
                {
                    // no sync required - do nothing
                }

                // CASE 2 OF 5: db date != local date != server date
                else if (serverFileDate != offlineFileDate && serverFileDate != dbFileDate && offlineFileDate != dbFileDate)
                {
                    // conflict...... highlight and manually sort. program informs about conflict... TODO
                    Console.WriteLine("Sync case 2 not implemented exception");
                    throw new NotImplementedException();
                }
                
                // CASE 3 OF 5: db date == local date
                else if (serverFileDate != offlineFileDate && serverFileDate != dbFileDate && offlineFileDate == dbFileDate)
                {
                    if (serverFileDate > offlineFileDate)
                    {
                        // server version is newer, get server version
                        serverFile.CopyTo(offlineFilePath, true);
                        Data.UpdateFileDateModified(offlineFilePath, serverFileDate.ToString());
                    } 
                    else
                    {
                        // server version is older, maybe it has been recovered / rolled back.. highlight conflict to manually be sorted.
                        Console.WriteLine("Sync case 3 not implemented exception");
                        throw new NotImplementedException();
                    }
                }
                
                // CASE 4 OF 5: db date == server date
                else if (serverFileDate != offlineFileDate && serverFileDate == dbFileDate && offlineFileDate != dbFileDate)
                {
                    if (offlineFileDate > serverFileDate)
                    {
                        // local version is newer, upload local version to server
                        offlineFile.CopyTo(serverFilePath, true);
                        Data.UpdateFileDateModified(offlineFilePath, offlineFileDate.ToString());
                    }
                    else
                    {
                        // local version is older, maybe it has been recovered / rolled back.. highlight conflict to manually be sorted.
                        Console.WriteLine("Sync case 4 not implemented exception");
                        throw new NotImplementedException();
                    }
                }
                
                // CASE 5 OF 5: local date == server date
                else if (serverFileDate == offlineFileDate && serverFileDate != dbFileDate && offlineFileDate != dbFileDate)
                {
                    // DB update error somewhere? try update DB to local or server date..
                    Data.UpdateFileDateModified(offlineFilePath, offlineFileDate.ToString());
                }
            }
        }

        private static string convertServerFilePathToOfflineFilePath(string serverPath, string offlineRootDirectory)
        {
            string mappedDrive = serverPath.Substring(0, 3);
            string offlinePath;

            switch (mappedDrive)
            {
                case @"W:\":
                    offlinePath = serverPath.Replace(mappedDrive, offlineRootDirectory + @"\7plus\");
                    break;

                case @"X:\":
                    offlinePath = serverPath.Replace(mappedDrive, offlineRootDirectory + @"\8plus\");
                    break;

                case @"Y:\":
                    offlinePath = serverPath.Replace(mappedDrive, offlineRootDirectory + @"\9plus\");
                    break;

                case @"Z:\":
                    offlinePath = serverPath.Replace(mappedDrive, offlineRootDirectory + @"\10plus\");
                    break;

                case @"L:\":
                    offlinePath = serverPath.Replace(mappedDrive, offlineRootDirectory + @"\11plus\");
                    break;

                case @"M:\":
                    offlinePath = serverPath.Replace(mappedDrive, offlineRootDirectory + @"\12plus\");
                    break;

                default:
                    // ignore files that aren't mapped to one of the above drives
                    return null;
            }

            return offlinePath;
        }

        private static string convertOfflineFilePathToServerFilePath(string offlinePath, string offlineRootDirectory)
        {
            string relativeFilePath = offlinePath.Split(offlineRootDirectory)[1];

            if (relativeFilePath.Contains(@"\7plus\"))
            {
                return relativeFilePath.Replace(@"\7plus\", @"W:\");
            }
            else if (relativeFilePath.Contains(@"\8plus\"))
            {
                return relativeFilePath.Replace(@"\8plus\", @"X:\");
            }
            else if (relativeFilePath.Contains(@"\9plus\"))
            {
                return relativeFilePath.Replace(@"\9plus\", @"Y:\");
            }
            else if (relativeFilePath.Contains(@"\10plus\"))
            {
                return relativeFilePath.Replace(@"\10plus\", @"Z:\");
            }
            else if (relativeFilePath.Contains(@"\11plus\"))
            {
                return relativeFilePath.Replace(@"\11plus\", @"L:\");
            }
            else if (relativeFilePath.Contains(@"\12plus\"))
            {
                return relativeFilePath.Replace(@"\12plus\", @"M:\");
            }
            else
            {
                // ignore files that don't belong to a project drive
                return null;
            }
        }

        private static void createDirectory(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
