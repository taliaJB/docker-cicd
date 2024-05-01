using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eldan.Logger;
using System.Configuration;

namespace Eldan.ServicesScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                clsLogger.Write("Program:Main - No argument specified, needs to specify service name", clsLogger.enmLogType.Error);
                return;
            }

            string taskId = null;
            if (args.Length > 1)
                taskId = args[1];

            string[] path = ConfigurationManager.AppSettings["loggerFilePath"].Split('.');
            ConfigurationManager.AppSettings["loggerFilePath"] = path[0] + args[0] + "." + path[1];

            ServiceLogger serviceLogger = new ServiceLogger("Program:Main");
            serviceLogger.ThrowExceptionWhenError = false;
            serviceLogger.DoAction(ManageService.RunService, args[0], taskId);

        }
    }
}
