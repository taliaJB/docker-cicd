using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Eldan.Protocols;
using Microsoft.Web.Administration;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface INFSService
{

    [OperationContract]
    void CreateDirectory(string Path, bool Overwrite);

    [OperationContract]
    void Copy(string sourceFileName, string destFileName, bool Overwrite);

    [OperationContract]
    void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs);

    [OperationContract]
    void Delete(string sourceFileName);

    [OperationContract]
    void CopyAndConvert(string sourceFileName, enmFileType sourceFileType,
                        string destFileName, enmFileType destFileType,
                        int? maxConvertPages);

    [OperationContract]
    void WriteToFileHebrew(string fileFullName, string text);

    [OperationContract]
    void WriteToFile(string fileFullName, string text, System.Text.Encoding encoding);

    [OperationContract]
    void Move(string oldFileFullName, string newFileFullName);

    [OperationContract]
    string[] GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption);

    [OperationContract]
    NFS.FileInfoSr[] GetFilesInfo(string path, string searchPattern, System.IO.SearchOption searchOption);

    [OperationContract]
    ObjectState ManageAppPool(string appPoolName, NFS.enmAppPoolAction appPoolAction);

    [OperationContract]
    void MergePdf(string[] pSource_files, String pResult);

    [OperationContract]
    List<string> GetDirectoriesFullNames(string path);

    [OperationContract]
    void DeleteDir(string dirName, bool recursive);

}

