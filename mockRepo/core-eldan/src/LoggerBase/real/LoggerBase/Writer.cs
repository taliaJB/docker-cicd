using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eldan.LoggerBase
{
    internal static class Writer
    {
        private static readonly object objLock = new object();
        
        internal static void WriteToFile(string FilePath, string Text)
        {
            WriteToFile(FilePath, Text, 1);
        }

        private static void WriteToFile(string FilePath, string Text, int Retry)
        {
            try
            {
                lock (objLock)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, true))
                    {
                        file.WriteLine(Text + ((Retry > 1) ? "--- Retry: " + Retry.ToString() + "---" : ""));
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                if (Retry <= 3)
                {
                    System.Threading.Thread.Sleep(Retry * 100);
                    WriteToFile(FilePath, Text, Retry + 1);
                }
                else
                {
                    throw ex;
                }

            }
        }
    }

}
