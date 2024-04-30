using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Eldan.LoggerBase
{
    public interface IServiceLoggerEvents
    {
        void logCreateBL(string ServiceName, string LoggerSessionID);
        void LogMethodStart(string ServiceName, MethodInfo Method, object[] Params, string LoggerSessionID);
        void LogMethodEnd(string ServiceName, MethodInfo Method, object[] Params, object ReturnValue, string LoggerSessionID);
        void LogMethodException(string ServiceName, MethodInfo Method, object[] Params, Exception MethodException, string LoggerSessionID);
    }
}
