using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Eldan.Logger;
using Eldan.TypeExtensions;

namespace Eldan.LoggerBase
{
    public enum enmLogType
    {
        Error = 1,
        Warning = 2,
        Information = 4
    }

    public abstract class LoggerBase
    {
        private enum enmFileWriteMode
        {
            Vertical = 1,
            Horizontal = 2
        }        
        
        bool m_UseEventViewer;
        bool m_UseFileSystem;
        string m_Source;
        int m_LogID;
        string m_FilePath;
        enmFileWriteMode m_FileWriteMode;
        bool m_AvoidErrorMessages;
        bool m_AvoidWarningMessages;
        bool m_AvoidInformationMessages;
        bool m_ShowInnerException;
        bool m_AddMachineMameToFileNamePrefix;
        bool m_AddMachineMameToMessage;
        readonly object objLock;

        public const string DEFAULT_FILE_PATH_KEY = "loggerFilePath";
        public const string DEFAULT_FILE_PATH = @"C:\EldanLog.txt";

        private const bool DEFAULT_USE_EVENT_VIEWER = true;

        Dictionary<string, string> m_configuration;

        private string m_LoggerSessionID = "<Empty>";
        public string LoggerSessionID
        {
            get { return m_LoggerSessionID; }
            set { m_LoggerSessionID = value; }
        }

        public void GenerateLoggerSessionID()
        {
            m_LoggerSessionID = UniqueIdGenerator.GenerateUniqueId();
        }

        protected LoggerBase(Dictionary<string, string> configuration)
        {
            objLock = new object();
            m_configuration = configuration;
            GenerateLoggerSessionID();

            // UseEventViewer
            if (!bool.TryParse(GetConfigVal("loggerUseEventViewer"), out m_UseEventViewer))
            {
                m_UseEventViewer = DEFAULT_USE_EVENT_VIEWER;
            }

            // UseFileSystem
            if (!bool.TryParse(GetConfigVal("loggerUseFileSystem"), out m_UseFileSystem))
            {
                m_UseFileSystem = false;
            }

            // Source
            m_Source = GetConfigVal("loggerSource");
            if (string.IsNullOrEmpty(m_Source))
            {
                m_Source = "Eldan Application";
            }

            // LogID
            if (!int.TryParse(GetConfigVal("loggerLogID"), out m_LogID))
            {
                m_LogID = 212;
            }

            // FilePath
            m_FilePath = GetFilePath(DEFAULT_FILE_PATH_KEY);

            // FileWriteMode
            int Temp;
            if (int.TryParse(GetConfigVal("loggerFileWriteMode"), out Temp))
            {
                m_FileWriteMode = (enmFileWriteMode)Temp;
            }
            else
            {
                m_FileWriteMode = enmFileWriteMode.Vertical;
            }

            // AvoidErrorMessages - obsolite
            if (!bool.TryParse(GetConfigVal("loggerAvoidErrorMessages"), out m_AvoidErrorMessages))
            {
                m_AvoidErrorMessages = false;
            }
            bool LogErrorMessages;
            if (bool.TryParse(GetConfigVal("loggerLogErrorMessages"), out LogErrorMessages))
            {
                m_AvoidErrorMessages = !LogErrorMessages;
            }
            // AvoidWarningMessages - obsolite
            if (!bool.TryParse(GetConfigVal("loggerAvoidWarningMessages"), out m_AvoidWarningMessages))
            {
                m_AvoidWarningMessages = false;
            }
            bool LogWarningMessages;
            if (bool.TryParse(GetConfigVal("loggerLogWarningMessages"), out LogWarningMessages))
            {
                m_AvoidWarningMessages = !LogWarningMessages;
            }

            // AvoidInformationMessages - obsolite
            if (!bool.TryParse(GetConfigVal("loggerAvoidInformationMessages"), out m_AvoidInformationMessages))
            {
                m_AvoidInformationMessages = false;
            }
            bool LogInformationMessages;
            if (bool.TryParse(GetConfigVal("loggerLogInformationMessages"), out LogInformationMessages))
            {
                m_AvoidInformationMessages = !LogInformationMessages;
            }

            if (!bool.TryParse(GetConfigVal("loggerShowInnerException"), out m_ShowInnerException))
            {
                m_ShowInnerException = false;
            }

            if (!bool.TryParse(GetConfigVal("loggerAddMachineMameToFileNamePrefix"), out m_AddMachineMameToFileNamePrefix))
            {
                m_AddMachineMameToFileNamePrefix = false;
            }

            if (!bool.TryParse(GetConfigVal("loggerAddMachineMameToMessage"), out m_AddMachineMameToMessage))
            {
                m_AddMachineMameToMessage = false;
            }
        }

        protected string GetConfigVal(string key)
        {
            if (m_configuration.ContainsKey(key))
                return m_configuration[key];
            return
                null;
        }

        private string GetFilePathByKey(string FilePathKey)
        {
            if (FilePathKey == DEFAULT_FILE_PATH_KEY || FilePathKey == null)
                return m_FilePath;
            else
                return GetFilePath(FilePathKey, DEFAULT_FILE_PATH_KEY);
        }

        private string GetFilePath(string FilePathKey, string FilePathKeySecondChoice = null)
        {
            string FilePath = GetConfigVal(FilePathKey);
            if (string.IsNullOrEmpty(FilePath))
            {
                if (FilePathKeySecondChoice == null)
                    FilePath = DEFAULT_FILE_PATH;
                else
                    return GetFilePath(FilePathKeySecondChoice, null);            
            }

            return FilePath;
        }

        public bool AvoidErrorMessages
        {
            get { return m_AvoidErrorMessages; }
        }
        public bool LogErrorMessages
        {
            get { return !m_AvoidErrorMessages; }
        }

        public bool AvoidWarningMessages
        {
            get { return m_AvoidWarningMessages; }
        }
        public bool LogWarningMessages
        {
            get { return !m_AvoidWarningMessages; }
        }

        public bool AvoidInformationMessages
        {
            get { return m_AvoidInformationMessages; }
        }
        public bool LogInformationMessages
        {
            get { return !m_AvoidInformationMessages; }
        }

        public bool ShowInnerException
        {
            get { return m_ShowInnerException; }
            set { m_ShowInnerException = value; }
        }

        #region Write
        public virtual void Write(string Message, Exception ex)
        {
            Write(Message, ex, DEFAULT_FILE_PATH_KEY);
        }

        public void Write(string Message, Exception ex, string FilePathKey)
        {
            Write(Message + " - " + GetExToString(ex), enmLogType.Error, FilePathKey);
        }

        public void Write(string Message, Exception ex, string FilePathKey, string EnvPath)
        {
            Write(Message + " - " + GetExToString(ex), enmLogType.Error, FilePathKey, EnvPath);
        }

        public virtual void Write(Exception ex)
        {
            Write(ex, DEFAULT_FILE_PATH_KEY);
        }

        public void Write(Exception ex, string FilePathKey)
        {
            Write(GetExToString(ex), enmLogType.Error, FilePathKey);
        }

        public void Write(Exception ex, string FilePathKey, string EnvPath)
        {
            Write(GetExToString(ex), enmLogType.Error, FilePathKey, EnvPath);
        }

        public void Write(string Message)
        {
            Write(Message, enmLogType.Information);
        }

        public virtual void Write(string Message, enmLogType LogType)
        {
            Write(Message, LogType, DEFAULT_FILE_PATH_KEY);
        }

        public void Write(string Message, enmLogType LogType, string FilePathKey)
        {
            string FilePath;
            FilePath = GetFilePathByKey(FilePathKey);

            WriteByParams(Message, LogType, FilePath, m_UseEventViewer);
        }

        public void Write(string Message, enmLogType LogType, string FilePathKey, string EnvPath)
        {
            string FilePath;
            FilePath = GetFilePathByKey(FilePathKey);

            if (!string.IsNullOrWhiteSpace(EnvPath))
                FilePath = Path.Combine(EnvPath.Trim(), FilePath);

            WriteByParams(Message, LogType, FilePath, m_UseEventViewer);
        }
        #endregion

        #region WriteByParams
        public void WriteByParams(string Message, Exception ex, string FilePath, bool UseEventViewer)
        {
            WriteByParams(Message + " - " + GetExToString(ex), enmLogType.Error, FilePath, UseEventViewer);
        }

        public void WriteByParams(Exception ex, string FilePath, bool UseEventViewer)
        {
            WriteByParams(GetExToString(ex), enmLogType.Error, FilePath, UseEventViewer);
        }

        public void WriteByParams(string Message, enmLogType LogType, string FilePath, bool UseEventViewer)
        {
            const string DATE_TIME_FORMAT = "dd/MM/yyyy HH:mm:ss.ff";
            Message = string.Format("{0} (LoggerSessionID = {1})", Message.ToNullLessString(""), m_LoggerSessionID);

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
                throw new Exception("clsLogger.WriteByParams - event viewr not supported");
                //clsEventViewer.Write(m_Source, Message, m_LogID, (EventLogEntryType)LogType);
            }
            if (m_UseFileSystem && !string.IsNullOrWhiteSpace(FilePath))
            {
                FilePath = GetFileFullName(FilePath);

                if (m_FileWriteMode == enmFileWriteMode.Vertical)
                {
                    lock (objLock)
                    {
                        Writer.WriteToFile(FilePath, "-------------------------------------------------------");
                        Writer.WriteToFile(FilePath, "Date: " + DateTime.Now.ToString(DATE_TIME_FORMAT));
                        //clsFileSystem.WriteToFile(FilePath, "ID: " + m_LogID.ToString());
                        //clsFileSystem.WriteToFile(FilePath, "Sorce: " + m_Source);
                        Writer.WriteToFile(FilePath, "Type: " + LogType.ToString());
                        if (m_AddMachineMameToMessage)
                            Writer.WriteToFile(FilePath, "Machine: " + Environment.MachineName);
                        Writer.WriteToFile(FilePath, "Message: " + Message);
                    }
                }
                else
                {
                    lock (objLock)
                    {
                        Writer.WriteToFile(FilePath, "DATE: " + DateTime.Now.ToString(DATE_TIME_FORMAT) + " " +
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

        public string GetExToString(Exception ex)
        {
            if (ex.InnerException == null || !m_ShowInnerException)
                return ex.ToString();
            else
                return ex.ToString() + Environment.NewLine + GetExToString(ex.InnerException);
        }

        private string GetFileFullName(string FilePath)
        {
            if (!m_AddMachineMameToFileNamePrefix)
                return FilePath;

            string[] Parts = FilePath.Split('\\');
            Parts[Parts.Length - 1] = Environment.MachineName + "_" + Parts[Parts.Length - 1];

            return string.Join(@"\", Parts);
        }
    }
}
