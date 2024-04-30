using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Eldan.Logger
{
    public interface IServiceLoggerEvents
    {
        void logCreateBL(string ServiceName);
        void LogMethodStart(string ServiceName, MethodInfo Method, object[] Params);
        void LogMethodEnd(string ServiceName, MethodInfo Method, object[] Params, object ReturnValue);
        void LogMethodException(string ServiceName, MethodInfo Method, object[] Params, Exception MethodException);
    }
}
