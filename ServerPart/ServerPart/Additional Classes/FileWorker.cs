using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServerPart
{
    public static class FileWorker
    {
        public static string driveName = null;
        public static string previousPath = null;
        public static string[] ListOfFiles = null;

        public static string[] GetDrives()
        {
            List<string> drives = new List<string>();
            List<string> drivesName = new List<string>();
            DriveInfo[] driveInfo = DriveInfo.GetDrives();
            int i = 1;
            foreach (DriveInfo drive in driveInfo)
            {
                if (drive.IsReady)
                {
                    drives.Add(String.Format("{0}. {1} ({2})", i, drive.Name, drive.VolumeLabel));
                    drivesName.Add(drive.Name);
                    i++;
                }
            }

            ListOfFiles = drivesName.ToArray<string>();
            return drives.ToArray<string>();            
        }

        public static bool IsFileExists(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }

            return false;
        }
        
        public static string GetParentPath()
        {
            DirectoryInfo directory = null;
            try
            {
                directory = Directory.GetParent(Directory.GetParent(ListOfFiles[0]).FullName);
                ListOfFiles[0] = directory.FullName;
            }
            catch (Exception)
            {
                return null;
            }
            return directory.FullName;
        }

        public static string[] GetListOfContainingFiles(int indexOfPath, string extension, ref bool isFile)
        {
            string path;
            if (ListOfFiles.Length > indexOfPath - 1)
            {
                path = ListOfFiles[indexOfPath - 1];
                previousPath = ListOfFiles[indexOfPath - 1];
            }
            else 
            {
                path = previousPath;
            }

            string[] Result = null;
            try
            {
                if (driveName == null)
                {
                    driveName = path;
                    previousPath = path;
                }

                List<string> directoriesList = new List<string>();
                List<string> filesList = new List<string>();

                try
                {
                    Directory.GetDirectories(path);
                }
                catch (Exception)
                {
                    isFile = true;
                    return new string[] { path };
                };

                int index = 1;
                foreach (string directory in Directory.GetDirectories(path))
                {
                    directoriesList.Add(String.Format("{0}. {1}", index, directory.Substring(path.Length)));
                    index++;
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    FileInfo filee = new FileInfo(file);
                    if (filee.Extension == extension)
                    {
                        filesList.Add(String.Format("{0}. {1}", index, file.Substring(path.Length)));
                        index++;
                    }
                }

                Result = new string[directoriesList.Count + filesList.Count];
                for (int i = 0, j = 0; i < Result.Length; i++)
                {
                    if (i < directoriesList.Count)
                    {
                        Result[i] = directoriesList[i];
                    }
                    else
                    {
                        Result[i] = filesList[j];
                        j++;
                    }
                }

                string[] directories = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                string[] result = new string[directories.Length + files.Length];
                Array.Copy(directories, result, directories.Length);
                Array.Copy(files, 0, result, directories.Length, files.Length);
                ListOfFiles = result;

            }
            catch (UnauthorizedAccessException) { }
            return Result;
        }

        public static string[] GetListOfFiles(string catalogOfFiles, string extension)
        {
            driveName = catalogOfFiles;
            List<string> listOfFiles = new List<string>();
            string[] directories = Directory.GetDirectories(catalogOfFiles);
            foreach (string dir in directories)
            {
                try
                {
                    string[] files = Directory.GetFiles(dir);
                    foreach (string f in files)
                    {
                        FileInfo file = new FileInfo(f);
                        if (file.Extension == extension)
                        {
                            listOfFiles.Add(file.Name);
                        }
                    }
                }
                catch (System.UnauthorizedAccessException) { }
            }

            return listOfFiles.ToArray<string>();
        }

        public static string GetFilePath(string fileName, string catalogOfFiles)
        {
            string[] directories = Directory.GetDirectories(catalogOfFiles);
            foreach (string dir in directories)
            {
                try
                {
                    string[] files = Directory.GetFiles(dir);
                    foreach (string f in files)
                    {
                        FileInfo file = new FileInfo(f);
                        if (file.Name == fileName)
                        {
                            return file.FullName;
                        }
                    }
                }
                catch (System.UnauthorizedAccessException) { }
            }

            return null;
        }
    }
}
