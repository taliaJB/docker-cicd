using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Eldan.Logger
{
    internal static class clsEventViewer
    {
        //private const string Source = "Eldan application";
        private const string LOG = "Application";
        //private const int EventID = 212;

        internal static void Write(string Source, string Message, int EventID, EventLogEntryType EventType)
        {
            if (!EventLog.SourceExists(Source))
                EventLog.CreateEventSource(Source, LOG);

            EventLog.WriteEntry(Source, Message, EventType, EventID);
        }

    }

}
