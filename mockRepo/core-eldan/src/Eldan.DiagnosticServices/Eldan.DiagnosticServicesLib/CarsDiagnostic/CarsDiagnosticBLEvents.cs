using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eldan.LoggerBase;
using Eldan.ServiceMonitor;
using System.Configuration;

namespace Eldan.DiagnosticServicesLib.CarsDiagnostic
{
    public class CarsDiagnosticBLEvents : IServiceLoggerEvents
    {
        public void logCreateBL(string ServiceName, string LoggerSessionID)
        {
            //throw new NotImplementedException();
        }

        public void LogMethodEnd(string ServiceName, MethodInfo Method, object[] Params, object ReturnValue, string LoggerSessionID)
        {
            //throw new NotImplementedException();
        }

        public void LogMethodException(string ServiceName, MethodInfo Method, object[] Params, Exception MethodException, string LoggerSessionID)
        {
            string key = CarsDiagnosticBL.LOGGER_FILE_PATH_ONLINE_KEY;
            if (Method.Name == nameof(CarsDiagnosticBL.UpdateSuppliersData))
                key = CarsDiagnosticBL.LOGGER_FILE_PATH_SCHEDUALE_KEY;

            new MonitorDispatcher
            {
                MailAddress = ConfigurationManager.AppSettings["CarsDiagnosticFaultsRecipients"],
                LogFilePathKey = key,
                ServiceName = CarsDiagnosticBL.SERVICE_NAME,
                LoggerSessionID = LoggerSessionID
            }.SendFault(MethodException.InnerException);
        }

        public void LogMethodStart(string ServiceName, MethodInfo Method, object[] Params, string LoggerSessionID)
        {
            //throw new NotImplementedException();
        }
    }
}
