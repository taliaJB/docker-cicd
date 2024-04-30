using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using myExcel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Eldan.TypeExtensions
{
    public class ExcelBuilder : System.IDisposable
    {
        myExcel.Application xlApp;
        myExcel.Workbook xlWorkBook;
        public myExcel.Worksheet xlWorkSheet;
        object misValue = System.Reflection.Missing.Value;
        string _path;

        #region Basics

        public virtual void ShowMsg1()
        {
        }

        public virtual void ShowMsg2()
        {
        }

        public virtual void ShowMsg3()
        {
        }

        public ExcelBuilder()
        {
            xlApp = new myExcel.Application();

            if (xlApp == null)
            {
                ShowMsg1();
                return;
            }

        }

        public void CreateExcelFile(string path)
        {
            if (File.Exists(path))
            {
                ShowMsg2();
                xlWorkBook = xlApp.Workbooks.Open(path);
            }
            else
            {

                xlWorkBook = xlApp.Workbooks.Add(misValue);
            }

            _path = path;

            xlWorkSheet = (myExcel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
        }
        public void OpenExcelFile(string path)
        {
            if (!File.Exists(path))
            {
                ShowMsg3();
                return;
            }

            _path = path;
            xlWorkBook = xlApp.Workbooks.Add(path);
            xlWorkSheet = (myExcel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
        }

        public void Dispose()
        {
            try
            {
                xlWorkBook.SaveAs(_path, myExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue,
                    myExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            }

            finally
            {
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);
            }
        }

        #endregion

        #region Cells Values And Design
        public void SetCellValue(int x, int y, string value)
        {
            xlWorkSheet.Cells[x, y] = value;
        }

        public void SetCellValue(string cell, string value)
        {
            myExcel.Range formatRange;
            formatRange = xlWorkSheet.get_Range(cell);
            formatRange.Value = value;
        }

        public void BoldRange(string rangeStart, string rangeEnd = "")
        {
            myExcel.Range formatRange;

            if (rangeEnd == "")
                rangeEnd = rangeStart;

            formatRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);

            formatRange.EntireRow.Font.Bold = true;
        }


        public void AddCellBorder(string rangeStart, string rangeEnd = "")
        {
            if (rangeEnd == "")
                rangeEnd = rangeStart;

            myExcel.Range formatRange = xlWorkSheet.UsedRange;
            myExcel.Range cell = formatRange.get_Range(rangeStart, rangeEnd);
            myExcel.Borders border = cell.Borders;
            border.LineStyle = myExcel.XlLineStyle.xlContinuous;
            border.Weight = 2d;
        }

        public void AddCellsBorder(string rangeStart, string rangeEnd = "")
        {
            if (rangeEnd == "")
                rangeEnd = rangeStart;
            myExcel.Range formatRange;
            formatRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);

            formatRange.BorderAround(myExcel.XlLineStyle.xlContinuous,
            myExcel.XlBorderWeight.xlMedium, myExcel.XlColorIndex.xlColorIndexAutomatic,
            myExcel.XlColorIndex.xlColorIndexAutomatic);
        }

        public void AddUsedRangeBorder()
        {
            xlWorkSheet.UsedRange.BorderAround(myExcel.XlLineStyle.xlContinuous,
            myExcel.XlBorderWeight.xlMedium, myExcel.XlColorIndex.xlColorIndexAutomatic,
            myExcel.XlColorIndex.xlColorIndexAutomatic);
        }

        public void SetBackColorRange(Color color, string rangeStart, string rangeEnd = "")
        {
            myExcel.Range formatRange;

            if (rangeEnd == "")
                rangeEnd = rangeStart;

            formatRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);
            formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
        }

        public void SetForeColorRange(Color color, string rangeStart, string rangeEnd = "")
        {
            myExcel.Range formatRange;

            if (rangeEnd == "")
                rangeEnd = rangeStart;

            formatRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);
            formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(color);
        }

        public void SetFontSize(int size, string rangeStart, string rangeEnd = "")
        {
            myExcel.Range formatRange;

            if (rangeEnd == "")
                rangeEnd = rangeStart;

            formatRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);
            formatRange.Font.Size = size;
        }

        public void MergeCells(string rangeStart, string rangeEnd = "", int sheet = 0)
        {
            int originalSheet = xlWorkSheet.Index;
            if (sheet != 0)
                SetSheet(sheet);
            if (rangeEnd == "")
                rangeEnd = rangeStart;
            xlWorkSheet.get_Range(rangeStart, rangeEnd).Merge(false);

            SetSheet(originalSheet);
        }

        public void HideGridLines()
        {
            xlApp.Windows.Application.ActiveWindow.DisplayGridlines = false;
        }

        public void ShowGridLines()
        {
            xlApp.Windows.Application.ActiveWindow.DisplayGridlines = true;
        }

        #endregion

        #region Headers
        public void AddHeader(string headerText, string rangeStart, string rangeEnd = "", int horizontalAlignment = 3, int verticalAlignment = 3)
        {
            if (rangeEnd == "")
                rangeEnd = rangeStart;

            myExcel.Range formatRange;
            xlWorkSheet.get_Range(rangeStart, rangeEnd).Merge(false);

            formatRange = xlWorkSheet.get_Range(rangeStart, rangeEnd);
            formatRange.FormulaR1C1 = headerText;
            formatRange.HorizontalAlignment = horizontalAlignment;
            formatRange.VerticalAlignment = verticalAlignment;
        }
        #endregion

        #region Sheets

        public void SetSheet(int sheet, string name = "")
        {
            while (xlWorkBook.Sheets.Count < sheet)
                AddSheet("",false);

            xlWorkSheet = (myExcel.Worksheet)xlWorkBook.Worksheets.get_Item(sheet);
            if (name != "")
                xlWorkSheet.Name = name;
        }

        public void SetSheetName(string name, int sheet = 0)
        {
            int originalSheet = xlWorkSheet.Index;
            if (sheet != 0)
                SetSheet(sheet);
            xlWorkSheet.Name = name;
            SetSheet(originalSheet);
        }
        public void SheetRightToLeft(int sheet = 0)
        {
            int originalSheet = xlWorkSheet.Index;
            if (sheet != 0)
                SetSheet(sheet);
            xlWorkSheet.DisplayRightToLeft = true;
            SetSheet(originalSheet);
        }

        public void AddSheet(string sheetName = "", bool setFocusToNewSheet = false)
        {
            if (sheetName == "")
                sheetName = "Sheet" + (xlWorkBook.Sheets.Count + 1);

            var workSheets = xlWorkBook.Worksheets;
            var xlNewSheet = (myExcel.Worksheet)workSheets.Add(Type.Missing, workSheets[xlWorkBook.Sheets.Count], Type.Missing, Type.Missing);
            xlNewSheet.Name = sheetName;

            if (setFocusToNewSheet)
                SetSheet(workSheets.Count);
        }

        public int CurrSheetIndex
        {
            get { return xlWorkSheet.Index; }
            set { SetSheet(value); }
        }

        public void AddSheet(string sheetName = "")
        {
            AddSheet(sheetName, false);
        }

        public void RemoveSheets(int howManyToSave = 1)
        {
            for (int i = xlWorkBook.Sheets.Count; i > howManyToSave; i--)
            {
                var workSheets = xlWorkBook.Worksheets;
                SetSheet(i);
                workSheets[i].Delete();
            }

            SetSheet(1);
        }

        #endregion

        #region Columns

        public delegate string GetColumnData();
        System.Collections.Generic.List<GetColumnData> _dataGenerators = new System.Collections.Generic.List<GetColumnData>();
        System.Collections.Generic.List<string> _columnNames = new System.Collections.Generic.List<string>();

        public void AddColumn(string name, GetColumnData columnData)
        {
            _dataGenerators.Add(columnData);
            _columnNames.Add(name);
        }

        public void ClearColumnsList()
        {
            _dataGenerators.Clear();
            _columnNames.Clear();
        }

        public void WriteColumnHeaders()
        {
            myExcel.Range xlRange = (myExcel.Range)xlWorkSheet.Cells[xlWorkSheet.Rows.Count, 1];
            long lastRow = (long)xlRange.get_End(myExcel.XlDirection.xlUp).Row;
            long newRow = lastRow;
            for (int i = 1; i <= _columnNames.Count; i++)
            {
                SetCellValue((int)newRow, i, _columnNames[i - 1]);
            }
        }

        public void WriteLine()
        {
            myExcel.Range xlRange = (myExcel.Range)xlWorkSheet.Cells[xlWorkSheet.Rows.Count, 1];
            long lastRow = (long)xlRange.get_End(myExcel.XlDirection.xlUp).Row;
            long newRow = lastRow + 1;
            for (int i = 1; i <= _dataGenerators.Count; i++)
            {
                SetCellValue((int)newRow, i, _dataGenerators[i - 1].Invoke());
            }

        }
        #endregion

        public myExcel.Range GetUsedRange()
        {
            return xlWorkSheet.UsedRange;
        }

        public virtual void AddPicture(string path, int left = 0, int top = 0, int width = 85, int height = 85) { }
    }
}
