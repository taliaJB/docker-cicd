using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Eldan.LoggerBase;

namespace Eldan.LoggerFrm
{
    public class LoggerFrm : ServiceLoggerBase
    {
        public LoggerFrm() : base(GetAppSettings())
        { }

        public LoggerFrm(string serviceName) : base(GetAppSettings(), serviceName)
        { }

        public static Dictionary<string, string> GetAppSettings()
        {
            var appSettings = new Dictionary<string, string>();

            var appSettingsSection = ConfigurationManager.AppSettings;
            if (appSettingsSection != null)
            {
                foreach (var key in appSettingsSection.AllKeys)
                {
                    string value = appSettingsSection[key];
                    appSettings[key] = value;
                }
            }

            return appSettings;
        }
    }
}
