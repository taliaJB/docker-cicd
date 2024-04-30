using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Xml.Linq;
using System.IO;

namespace Eldan.TypeExtensions
{
    public static class Excel
    {
        public static string GetXML(string fullPath)
        {
            return GetXMLDoc(fullPath).ToString();
        }

        public static XDocument GetXMLDoc(string fullPath)
        {
            Application xlApp = new Application();
            Workbook xlWorkBook;

            xlWorkBook = xlApp.Workbooks.Open(fullPath, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            string FullFileName = GetNewFullPath(fullPath);

            xlWorkBook.SaveAs(FullFileName, XlFileFormat.xlXMLSpreadsheet);
            xlWorkBook.Close(false, "", true);

            XDocument Doc = XDocument.Load(FullFileName);

            File.Delete(FullFileName);

            return Doc;
        }

        private static string GetNewFullPath(string fullPath)
        {
            var Parts = fullPath.Split('.');
            Parts[Parts.Count() - 1] = "xml";
            Parts[Parts.Count() - 2] += "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            return string.Join(".", Parts);

        }
    }
}
