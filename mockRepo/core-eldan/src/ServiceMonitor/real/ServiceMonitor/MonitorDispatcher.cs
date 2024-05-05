using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eldan.TypeExtensions;
using System.Configuration;

namespace Eldan.ServiceMonitor
{
    public class MonitorDispatcher
    {
        private const string NL = "<BR/>";
        private bool m_ThrowException = false;

        private LoggerFrm.LoggerFrm _logger;
        
        public bool ThrowException
        {
            get { return m_ThrowException; }
            set { m_ThrowException = value; }
        }


        private string m_ServiceName = "Fredo service";

        public string ServiceName 
        {
            get { return m_ServiceName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    HandleException(new Exception("MonitorDispatcher.ServiceName.set - Parametr: 'ServiceName' can't be null or white space"));
                    return;
                }
                m_ServiceName = value;
            }
        }

        private string m_LoggerSessionID = "unknown";

        public string LoggerSessionID
        {
            get { return m_LoggerSessionID; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    HandleException(new Exception("MonitorDispatcher.LoggerSessionID.set - Parametr: 'LoggerSessionID' can't be null or white space"));
                    return;
                }
                m_LoggerSessionID = value;
            }
        }

        private string m_LogFilePathKey = "loggerFilePath";

        public string LogFilePathKey
        {
            get { return m_LogFilePathKey; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    HandleException(new Exception("MonitorDispatcher.LogFilePathKey.set - Parametr: 'logFilePathKey' can't be null or white space"));
                    return;
                }

                if (!IsLogNameValid(value))
                    return;

                m_LogFilePathKey = value;
            }
        }

        
        [Obsolete("This property is obsolete. Use MailAddress instead.", false)]
        public string FaultMailAddress
        {
            get { return m_MailAddress; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    HandleException(new Exception("MonitorDispatcher.FaultMailAddress.set - Parametr: 'FaultMailAddress' can't be null or white space"));
                    return;
                }
                m_MailAddress = value;
            }
        }

        private string m_MailAddress = ConfigurationManager.AppSettings["DevUnitSupportMail"];
        public string MailAddress
        {
            get { return m_MailAddress; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    HandleException(new Exception("MonitorDispatcher.FaultMailAddress.set - Parametr: 'MailAddress' can't be null or white space"));
                    return;
                }
                m_MailAddress = value;
            }
        }

        public string MailAddressKey
        {
            set 
            {
                if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[value]))
                    HandleException(new Exception($"Invalid key: '{value}'"));

                m_MailAddress = ConfigurationManager.AppSettings[value]; 
            }
        }


        public void SendFault(Exception ex)
        {
            SetLogger();

            if (!IsLogNameValid(m_LogFilePathKey))
                return;

            string message = string.Format("Exception has occurred in Fredo's service: '{0}', (LoggerSessionID: '{1}'), for more details see log: '{2}' in Fredo server - exception details: '{3}'",
                m_ServiceName.ToNullLessString(),
                m_LoggerSessionID.ToNullLessString(),
                ConfigurationManager.AppSettings[m_LogFilePathKey],
                ex.Message);

            SendMail(m_MailAddress, string.Format("Exception in Fredo server for service '{0}'", m_ServiceName), message);
        }

        public enum EnmMassageType
        {
            information = 1,
            warning
        }

        public void SendMessage(string userMessage)
        {
            SendMessage(userMessage, EnmMassageType.information);
        }

        public void SendMessage(string userMessage, EnmMassageType enmMassageType)
        {
            SetLogger();

            if (!IsLogNameValid(m_LogFilePathKey))
                return;

            string message = $"{userMessage}" + NL +
                             $"This {enmMassageType} message has been sent from Fredo's service: '{m_ServiceName.ToNullLessString()}', (LoggerSessionID: '{m_LoggerSessionID.ToNullLessString()}')." + NL +
                             $"For more details see log: '{ConfigurationManager.AppSettings[m_LogFilePathKey]}'.";

            string massageType = (enmMassageType == EnmMassageType.information) ? "Information" : "Warning";

            SendMail(m_MailAddress, $"{massageType} message from Fredo server, service: '{m_ServiceName}'", message);
        }

        private void SetLogger()
        {
            _logger = new LoggerFrm.LoggerFrm(m_ServiceName.ToNullLessString())
            {
                LoggerFilePathKey = m_LogFilePathKey,
                LoggerSessionID = m_LoggerSessionID
            };
        }

        private bool IsLogNameValid(string logFilePathKey)
        {
            if (ConfigurationManager.AppSettings[logFilePathKey] == null)
            {
                HandleException(new Exception(string.Format("MonitorDispatcher.IsLogNameValid - logFilePathKey: '{0}' is not exist with in configuration", logFilePathKey)));
                return false;
            }
            return true;
        }

        private void SendMail(string to, string subject, string content)
        {
            _logger.Write(string.Format("MonitorDispatcher.SendMail - attempt to send mail to: '{0}', subject: '{1}', content: '{2}...'",
                                         to,
                                         subject,
                                         content.Left(100)), LoggerBase.enmLogType.Information);
                               

            try
            {
                using (MailReference.WCFMailServiceClient proxy = new MailReference.WCFMailServiceClient())
                {
                    proxy.SendMail(string.Format("'{0}' in Fredo server", m_ServiceName.ToNullLessString()), to, null, null, subject, content, null);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void HandleException(Exception ex)
        {
            _logger.Write("MonitorDispatcher:LogMonitorException", ex);

            if (m_ThrowException)
                throw ex;
        }
    }
}
