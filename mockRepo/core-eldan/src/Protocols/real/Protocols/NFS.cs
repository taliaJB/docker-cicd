using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Eldan.ImageProcessing;
using Eldan.TypeExtensions;
using Microsoft.Web.Administration;
using System.Threading;
using System.Runtime.Serialization;

namespace Eldan.Protocols
{
    public enum enmFileType
    {
        TIFF,
        PDF
    }

    public class NFS
    {
        // Summary:
        //     Creates all directories and subdirectories as specified by path.
        //
        // Parameters:
        //   path:
        //     The directory path to create.
        //
        // Returns:
        //     A System.IO.DirectoryInfo as specified by path.
        //
        // Exceptions:
        //   Directory already exsited
        //
        //   System.IO.IOException:
        //     The directory specified by path is read-only.
        //
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission.
        //
        //   System.ArgumentException:
        //     path is a zero-length string, contains only white space, or contains one
        //     or more invalid characters as defined by System.IO.Path.InvalidPathChars.-or-path
        //     is prefixed with, or contains only a colon character (:).
        //
        //   System.ArgumentNullException:
        //     path is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters and file names must be less than 260 characters.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The specified path is invalid (for example, it is on an unmapped drive).
        //
        //   System.NotSupportedException:
        //     path contains a colon character (:) that is not part of a drive label ("C:\").
        public void CreateDirectory(string Path, bool Overwrite)
        {
            if (Directory.Exists(Path))
            {
                if (!Overwrite)
                {
                    throw new Exception("Directory already exsited");
                }
            }
            else
            {
                Directory.CreateDirectory(Path);
            }
        }

        public enum enmClusteredPeriod
        {
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Yearly = 4
        }

        public string Copy(string sourceFileName, string destFileName, bool Overwrite, enmClusteredPeriod ClusteredPeriod)
        {
            string NewDest = GetFullNameByPeriod(destFileName, ClusteredPeriod);
            Copy(sourceFileName, NewDest, Overwrite);
            return NewDest;
        }

        public string GetFullNameByPeriod(string fullName, enmClusteredPeriod ClusteredPeriod)
        {
            return GetFullNameByPeriod(fullName, ClusteredPeriod, DateTime.Now);
        }

        public string GetFullNameByPeriod(string fullName, enmClusteredPeriod ClusteredPeriod, DateTime SaveDate)
        {
            string PathOnly = Path.GetDirectoryName(fullName);
            string NameOnly = Path.GetFileName(fullName);
            string NewPath = Path.Combine(PathOnly, GetCurrentPeriod(ClusteredPeriod, SaveDate));
            if (!Directory.Exists(NewPath))
                Directory.CreateDirectory(NewPath);

            return Path.Combine(NewPath, NameOnly);
        }

        private string GetCurrentPeriod(enmClusteredPeriod ClusteredPeriod, DateTime SaveDate)
        {
            if (SaveDate.Year == 2018)
            {
                return "2018";
            }

            switch (ClusteredPeriod)
            {
                case enmClusteredPeriod.Daily:
                    return SaveDate.ToString("yyyy_MM_dd");
                case enmClusteredPeriod.Weekly:
                    return SaveDate.Year + "_" + SaveDate.Month + "_" + ((SaveDate.Day - 1) / 7 + 1).ToString();
                case enmClusteredPeriod.Monthly:
                    return SaveDate.ToString("yyyy_MM");
                case enmClusteredPeriod.Yearly:
                    return SaveDate.ToString("yyyy");
            }

            return null;
        }

        // Summary:
        //     Copies an existing file to a new file. Overwriting a file of the same name
        //     is allowed.
        //
        // Parameters:
        //   sourceFileName:
        //     The file to copy.
        //
        //   destFileName:
        //     The name of the destination file. This cannot be a directory.
        //
        //   overwrite:
        //     true if the destination file can be overwritten; otherwise, false.
        //
        // Exceptions:
        //   System.UnauthorizedAccessException:
        //     The caller does not have the required permission. -or-destFileName is read-only.
        //
        //   System.ArgumentException:
        //     sourceFileName or destFileName is a zero-length string, contains only white
        //     space, or contains one or more invalid characters as defined by System.IO.Path.InvalidPathChars.-or-
        //     sourceFileName or destFileName specifies a directory.
        //
        //   System.ArgumentNullException:
        //     sourceFileName or destFileName is null.
        //
        //   System.IO.PathTooLongException:
        //     The specified path, file name, or both exceed the system-defined maximum
        //     length. For example, on Windows-based platforms, paths must be less than
        //     248 characters, and file names must be less than 260 characters.
        //
        //   System.IO.DirectoryNotFoundException:
        //     The path specified in sourceFileName or destFileName is invalid (for example,
        //     it is on an unmapped drive).
        //
        //   System.IO.FileNotFoundException:
        //     sourceFileName was not found.
        //
        //   System.IO.IOException:
        //     destFileName exists and overwrite is false.-or- An I/O error has occurred.
        //
        //   System.NotSupportedException:
        //     sourceFileName or destFileName is in an invalid format.
        public void Copy(string sourceFileName,string destFileName, bool Overwrite)
        {
            File.Copy(sourceFileName, destFileName, Overwrite);
        }

        public void CopyAndConvert(string sourceFileName, enmFileType sourceFileType,
                                   string destFileName, enmFileType destFileType,
                                   int? maxConvertPages)
        {
            if (sourceFileType == enmFileType.PDF && destFileType == enmFileType.TIFF)
                throw new Exception("NFS.CopyAndConvert - converting from PDF to TIFF is not supported");

            if (sourceFileType == destFileType)
            {
                Copy(sourceFileName, destFileName, true);
                return;
            }

            string emptyPDFFullName = ConfigurationManager.AppSettings["emptyPDFFullName"];

            clsImagesConverter.GeneratePDFFromTiff(sourceFileName, destFileName, emptyPDFFullName, maxConvertPages);

        }

        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public void Delete(string sourceFileName)
        {
            File.Delete(sourceFileName);
        }

        public void DeleteDir(string dirName, bool recursive)
        {
            Directory.Delete(dirName, recursive);
        }

        public void WriteToFileHebrew(string fileFullName, string text)
        {
            WriteToFile(fileFullName, text, System.Text.Encoding.GetEncoding(1255));
        }

        public void WriteToFile(string fileFullName, string text, System.Text.Encoding encoding)
        {
            using (var file2 = new StreamWriter(fileFullName, true, encoding))
            {
                file2.Write(text);
            }
        }

        public string Move(string oldFileFullName, string newFileFullName, enmClusteredPeriod ClusteredPeriod)
        {
            string NewDest = GetFullNameByPeriod(newFileFullName, ClusteredPeriod);
            Move(oldFileFullName, NewDest);
            return NewDest;
        }

        public void Move(string oldFileFullName, string newFileFullName)
        {
            File.Move(oldFileFullName, newFileFullName);
        }

        public string[] GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        [DataContract]
        public struct FileInfoSr
        {
            [DataMember]
            public string Name;
            [DataMember]
            public string FullName;
            [DataMember]
            public long Length;
            [DataMember]
            public DateTime CreationTime;
            [DataMember]
            public DateTime LastWriteTime;
        }

        public FileInfoSr[] GetFilesInfo(string path, string searchPattern, System.IO.SearchOption searchOption)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            var Res = di.GetFiles(searchPattern, searchOption);
            
            return GetFileInfoSrs(Res);
        }

        public List<string> GetDirectoriesFullNames(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            var res = di.GetDirectories();

            return res.Select(d => d.FullName).ToList();   
        }

        private FileInfoSr[] GetFileInfoSrs(FileInfo[] fileInfos)
        {
            var FileInfoSrs = from fileInfo in fileInfos
                              select new FileInfoSr { Name = fileInfo.Name, 
                                                      FullName = fileInfo.FullName, 
                                                      Length = fileInfo.Length, 
                                                      CreationTime = fileInfo.CreationTime,
                                                      LastWriteTime = fileInfo.LastWriteTime,
                                                    };

            return FileInfoSrs.ToArray();
        }

        public enum enmAppPoolAction
        {
            GetState = 0,
            Start = 1,
            Stop = 2,
            Recycle = 3
        }

        public ObjectState ManageAppPool(string appPoolName, enmAppPoolAction appPoolAction)
        {
            return ManageAppPool(appPoolName, appPoolAction, "localhost");
        }

        public static ObjectState ManageAppPool(string appPoolName, enmAppPoolAction appPoolAction, string fullServerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fullServerName))
                    throw new Exception("parameter: fullServerName is null or empty");

                if (string.IsNullOrWhiteSpace(appPoolName))
                    throw new Exception("parameter: appPoolName is null or empty");

                var appPoolList = new List<ApplicationPool>();

                using (var server = ServerManager.OpenRemote(fullServerName))
                {
                    var appPools = server.ApplicationPools;
                    ApplicationPool pool = GetAppPool(appPools, appPoolName);

                    switch (appPoolAction)
                    {
                        case enmAppPoolAction.Start:
                            pool.Start();
                            while (pool.State == ObjectState.Starting)
                            {
                                Thread.Sleep(200);
                            }
                            break;
                        case enmAppPoolAction.Stop:
                            pool.Stop();
                            while (pool.State == ObjectState.Stopping)
                            {
                                Thread.Sleep(200);
                            }
                            break;
                        case enmAppPoolAction.Recycle:
                            pool.Recycle();
                            break;
                        default:
                            break;
                    }

                    return pool.State;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Can't {0} application pool: '{1}' on server: '{2}' because: {3}", appPoolAction.ToString(),
                                                                                                            appPoolName.MaskNull(),
                                                                                                            fullServerName.MaskNull(),
                                                                                                            ex.Message));
            }
        }

        private static ApplicationPool GetAppPool(ApplicationPoolCollection pools, string poolName)
        {
            var res = (from pool in pools
                       where pool.Name == poolName
                       select pool).First();

            return res;
        }

        public void MergePdf(string[] pSource_files, String pResult)
        {
            clsImagesConverter.MergePdf(pSource_files, pResult);
        }
    }
}
