using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Eldan.Logger
{
    public static class clsLogger
    {
        public enum enmLogType
        {
            Error = 1,
            Warning = 2,
            Information = 4
        }

        private enum enmFileWriteMode
        {
            Vertical = 1,
            Horizontal = 2
        }        
        
        private static bool m_UseEventViewer;
        private static bool m_UseFileSystem;
        private static string m_Source;
        private static int m_LogID;
        private static string m_FilePath;
        private static enmFileWriteMode m_FileWriteMode;
        private static bool m_AvoidErrorMessages;
        private static bool m_AvoidWarningMessages;
        private static bool m_AvoidInformationMessages;
        private static bool m_ShowInnerException;
        private static bool m_AddMachineMameToFileNamePrefix;
        private static bool m_AddMachineMameToMessage;
        private static readonly object objLock;

        public const string DEFAULT_FILE_PATH_KEY = "loggerFilePath";
        public const string DEFAULT_FILE_PATH = @"C:\EldanLog.txt";

        private const bool DEFAULT_USE_EVENT_VIEWER = true;


        static clsLogger()
        {
            objLock = new object();

            // UseEventViewer
            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerUseEventViewer"], out m_UseEventViewer))
            {
                m_UseEventViewer = DEFAULT_USE_EVENT_VIEWER;
            }

            // UseFileSystem
            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerUseFileSystem"], out m_UseFileSystem))
            {
                m_UseFileSystem = false;
            }

            // Source
            m_Source = ConfigurationManager.AppSettings["loggerSource"];
            if (string.IsNullOrEmpty(m_Source))
            {
                m_Source = "Eldan Application";
            }

            // LogID
            if (!int.TryParse(ConfigurationManager.AppSettings["loggerLogID"], out m_LogID))
            {
                m_LogID = 212;
            }

            // FilePath
            m_FilePath = GetFilePath(DEFAULT_FILE_PATH_KEY);

            // FileWriteMode
            int Temp;
            if (int.TryParse(ConfigurationManager.AppSettings["loggerFileWriteMode"], out Temp))
            {
                m_FileWriteMode = (enmFileWriteMode)Temp;
            }
            else
            {
                m_FileWriteMode = enmFileWriteMode.Vertical;
            }

            // AvoidErrorMessages - obsolite
            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerAvoidErrorMessages"], out m_AvoidErrorMessages))
            {
                m_AvoidErrorMessages = false;
            }
            bool LogErrorMessages;
            if (bool.TryParse(ConfigurationManager.AppSettings["loggerLogErrorMessages"], out LogErrorMessages))
            {
                m_AvoidErrorMessages = !LogErrorMessages;
            }
            // AvoidWarningMessages - obsolite
            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerAvoidWarningMessages"], out m_AvoidWarningMessages))
            {
                m_AvoidWarningMessages = false;
            }
            bool LogWarningMessages;
            if (bool.TryParse(ConfigurationManager.AppSettings["loggerLogWarningMessages"], out LogWarningMessages))
            {
                m_AvoidWarningMessages = !LogWarningMessages;
            }

            // AvoidInformationMessages - obsolite
            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerAvoidInformationMessages"], out m_AvoidInformationMessages))
            {
                m_AvoidInformationMessages = false;
            }
            bool LogInformationMessages;
            if (bool.TryParse(ConfigurationManager.AppSettings["loggerLogInformationMessages"], out LogInformationMessages))
            {
                m_AvoidInformationMessages = !LogInformationMessages;
            }

            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerShowInnerException"], out m_ShowInnerException))
            {
                m_ShowInnerException = false;
            }

            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerAddMachineMameToFileNamePrefix"], out m_AddMachineMameToFileNamePrefix))
            {
                m_AddMachineMameToFileNamePrefix = false;
            }

            if (!bool.TryParse(ConfigurationManager.AppSettings["loggerAddMachineMameToMessage"], out m_AddMachineMameToMessage))
            {
                m_AddMachineMameToMessage = false;
            }
        }

        private static string GetFilePathByKey(string FilePathKey)
        {
            if (FilePathKey == DEFAULT_FILE_PATH_KEY || FilePathKey == null)
                return m_FilePath;
            else
                return GetFilePath(FilePathKey, DEFAULT_FILE_PATH_KEY);
        }

        private static string GetFilePath(string FilePathKey, string FilePathKeySecondChoice = null)
        {
            string FilePath = ConfigurationManager.AppSettings[FilePathKey];
            if (string.IsNullOrEmpty(FilePath))
            {
                if (FilePathKeySecondChoice == null)
                    FilePath = DEFAULT_FILE_PATH;
                else
                    return GetFilePath(FilePathKeySecondChoice, null);            
            }

            return FilePath;
        }

        public static bool AvoidErrorMessages
        {
            get { return m_AvoidErrorMessages; }
        }
        public static bool LogErrorMessages
        {
            get { return !m_AvoidErrorMessages; }
        }

        public static bool AvoidWarningMessages
        {
            get { return m_AvoidWarningMessages; }
        }
        public static bool LogWarningMessages
        {
            get { return !m_AvoidWarningMessages; }
        }

        public static bool AvoidInformationMessages
        {
            get { return m_AvoidInformationMessages; }
        }
        public static bool LogInformationMessages
        {
            get { return !m_AvoidInformationMessages; }
        }

        public static bool ShowInnerException
        {
            get { return m_ShowInnerException; }
            set { m_ShowInnerException = value; }
        }


        #region Write
        public static void Write(string Message, Exception ex)
        {
            Write(Message, ex, DEFAULT_FILE_PATH_KEY);
        }

        public static void Write(string Message, Exception ex, string FilePathKey)
        {
            Write(Message + " - " + GetExToString(ex), enmLogType.Error, FilePathKey);
        }

        public static void Write(string Message, Exception ex, string FilePathKey, string EnvPath)
        {
            Write(Message + " - " + GetExToString(ex), enmLogType.Error, FilePathKey, EnvPath);
        }

        public static void Write(Exception ex)
        {
            Write(ex, DEFAULT_FILE_PATH_KEY);
        }

        public static void Write(Exception ex, string FilePathKey)
        {
            Write(GetExToString(ex), enmLogType.Error, FilePathKey);
        }

        public static void Write(Exception ex, string FilePathKey, string EnvPath)
        {
            Write(GetExToString(ex), enmLogType.Error, FilePathKey, EnvPath);
        }

        public static void Write(string Message, enmLogType LogType)
        {
            Write(Message, LogType, DEFAULT_FILE_PATH_KEY);
        }

        public static void Write(string Message, enmLogType LogType, string FilePathKey)
        {
            string FilePath;
            FilePath = GetFilePathByKey(FilePathKey);

            WriteByParams(Message, LogType, FilePath, m_UseEventViewer);
        }

        public static void Write(string Message, enmLogType LogType, string FilePathKey, string EnvPath)
        {
            string FilePath;
            FilePath = GetFilePathByKey(FilePathKey);

            if (!string.IsNullOrWhiteSpace(EnvPath))
                FilePath = Path.Combine(EnvPath.Trim(), FilePath);

            WriteByParams(Message, LogType, FilePath, m_UseEventViewer);
        }
        #endregion

        #region WriteByParams
        public static void WriteByParams(string Message, Exception ex, string FilePath, bool UseEventViewer)
        {
            WriteByParams(Message + " - " + GetExToString(ex), enmLogType.Error, FilePath, UseEventViewer);
        }

        public static void WriteByParams(Exception ex, string FilePath, bool UseEventViewer)
        {
            WriteByParams(GetExToString(ex), enmLogType.Error, FilePath, UseEventViewer);
        }

        public static void WriteByParams(string Message, enmLogType LogType, string FilePath, bool UseEventViewer)
        {
            const string DATE_TIME_FORMAT = "dd/MM/yyyy HH:mm:ss.ff";

            switch (LogType)
            {
                case enmLogType.Error:
                    if (m_AvoidErrorMessages)
                    {
                        return;
                    }
                    break;
                case enmLogType.Warning:
                    if (m_AvoidWarningMessages)
                    {
                        return;
                    }
                    break;
                case enmLogType.Information:
                    if (m_AvoidInformationMessages)
                    {
                        return;
                    }
                    break;
                default:
                    break;
            }

            if (UseEventViewer)
            {
                clsEventViewer.Write(m_Source, Message, m_LogID, (EventLogEntryType)LogType);
            }
            if (m_UseFileSystem && !string.IsNullOrWhiteSpace(FilePath))
            {
                FilePath = GetFileFullName(FilePath);

                if (m_FileWriteMode == enmFileWriteMode.Vertical)
                {
                    lock (objLock)
                    {
                        clsFileSystem.WriteToFile(FilePath, "-------------------------------------------------------");
                        clsFileSystem.WriteToFile(FilePath, "Date: " + DateTime.Now.ToString(DATE_TIME_FORMAT));
                        //clsFileSystem.WriteToFile(FilePath, "ID: " + m_LogID.ToString());
                        //clsFileSystem.WriteToFile(FilePath, "Sorce: " + m_Source);
                        clsFileSystem.WriteToFile(FilePath, "Type: " + LogType.ToString());
                        if (m_AddMachineMameToMessage)
                            clsFileSystem.WriteToFile(FilePath, "Machine: " + Environment.MachineName);
                        clsFileSystem.WriteToFile(FilePath, "Message: " + Message);
                    }
                }
                else
                {
                    lock (objLock)
                    {
                        clsFileSystem.WriteToFile(FilePath, "DATE: " + DateTime.Now.ToString(DATE_TIME_FORMAT) + " " +
                                                             //"ID: " + m_LogID.ToString() + " " +
                                                             //"SORCE: " + m_Source + " " +
                                                             "TYPE: " + LogType.ToString() + " " +
                                                             (m_AddMachineMameToMessage? "MACHINE: " + Environment.MachineName + " " : "") +
                                                             "MESSAGE: " + Message);
                    }
                }
            }
        }
        #endregion //WriteByParams

        public static string GetExToString(Exception ex)
        {
            if (ex.InnerException == null || !m_ShowInnerException)
                return ex.ToString();
            else
                return ex.ToString() + Environment.NewLine + GetExToString(ex.InnerException);
        }

        private static string GetFileFullName(string FilePath)
        {
            if (!m_AddMachineMameToFileNamePrefix)
                return FilePath;

            string[] Parts = FilePath.Split('\\');
            Parts[Parts.Length - 1] = Environment.MachineName + "_" + Parts[Parts.Length - 1];

            return string.Join(@"\", Parts);
        }
    }
}
