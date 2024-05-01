using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Eldan.Protocols;
using Eldan.Logger;
using Microsoft.Web.Administration;
using System.IO;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class NFSService : INFSService
{
    private ServiceLogger m_ServiceLogger;
    private NFS m_BL;

    public NFSService()
    {
        m_ServiceLogger = new ServiceLogger("NFSService");
        m_ServiceLogger.SerilizeParamSetBy = ServiceLogger.enmSerilizerType.JSONtonsoft;

        m_BL = m_ServiceLogger.CreateInstance<NFS>();
    }

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
        m_ServiceLogger.DoAction(m_BL.CreateDirectory, Path, Overwrite);
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
    public void Copy(string sourceFileName, string destFileName, bool Overwrite)
    {
        m_ServiceLogger.DoAction(m_BL.Copy, sourceFileName, destFileName, Overwrite);
    }

    public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        m_ServiceLogger.DoAction(m_BL.DirectoryCopy, sourceDirName, destDirName, copySubDirs);
    }


    public void Delete(string sourceFileName)
    {
        m_ServiceLogger.DoAction(m_BL.Delete, sourceFileName);
    }

    public void CopyAndConvert(string sourceFileName, enmFileType sourceFileType, string destFileName, enmFileType destFileType, int? maxConvertPages)
    {
        m_ServiceLogger.DoAction(m_BL.CopyAndConvert, sourceFileName, sourceFileType, destFileName, destFileType, maxConvertPages);
    }

    public void WriteToFileHebrew(string fileFullName, string text)
    {
        m_ServiceLogger.DoAction(m_BL.WriteToFileHebrew, fileFullName, text);
    }

    public void WriteToFile(string fileFullName, string text, Encoding encoding)
    {
        m_ServiceLogger.DoAction(m_BL.WriteToFile, fileFullName, text, encoding);
    }

    public void Move(string oldFileFullName, string newFileFullName)
    {
        m_ServiceLogger.DoAction(m_BL.Move, oldFileFullName, newFileFullName);
    }

    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return m_ServiceLogger.DoFunc(m_BL.GetFiles, path, searchPattern, searchOption);
    }

    NFS.FileInfoSr[] INFSService.GetFilesInfo(string path, string searchPattern, SearchOption searchOption)
    {
        return m_ServiceLogger.DoFunc(m_BL.GetFilesInfo, path, searchPattern, searchOption);
    }

    public ObjectState ManageAppPool(string appPoolName, NFS.enmAppPoolAction appPoolAction)
    {
        return m_ServiceLogger.DoFunc(m_BL.ManageAppPool, appPoolName, appPoolAction);
    }

    public void MergePdf(string[] pSource_files, string pResult)
    {
        m_ServiceLogger.DoAction(m_BL.MergePdf, pSource_files, pResult);
    }

    public List<string> GetDirectoriesFullNames(string path)
    {
        return m_ServiceLogger.DoFunc(m_BL.GetDirectoriesFullNames, path);
    }

    public void DeleteDir(string dirName, bool recursive)
    {
        m_ServiceLogger.DoAction(m_BL.DeleteDir, dirName, recursive);
    }
}
