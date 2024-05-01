using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml.Linq;
using System.Configuration;
using Eldan.ServicesSchedulerLib;
using Eldan.TypeExtensions;

namespace Eldan.ServicesScheduler
{
    public static class ManageService
    {
        public static void RunService(string serviceName, string taskId)
        {
            string uri = GetAddress(serviceName);

            TimeSpan? SendTimeout = null;
            int SendTimeoutMinutes;

            if (int.TryParse(ConfigurationManager.AppSettings["SendTimeoutMinutes"], out SendTimeoutMinutes))
                SendTimeout = new TimeSpan(0, SendTimeoutMinutes, 0);

            IScheduler proxy = ServiceModelExtentions.GetProxy<IScheduler>(uri, null, null, null, SendTimeout);

            proxy.Refresh(taskId);
        }

        private static string GetAddress(string serviceName)
        {
            XDocument doc = XDocument.Load(ConfigurationManager.AppSettings["SchedulerConfigXML"]);
            return doc.Descendants("service").Where(x => x.Attribute("name").Value == serviceName).First().Attribute("uri").Value;
        }
    }
}
