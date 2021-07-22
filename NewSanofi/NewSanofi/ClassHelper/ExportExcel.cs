using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using NewSanofi.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace NewSanofi.ClassHelper
{
    public static class ExportExcel
    {
        public static DataTable import_start(string file)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                string selectedFileName = file;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;

                var missing = System.Reflection.Missing.Value;

                Excel.Application excel = new Excel.Application();
                xlWorkBook = excel.Workbooks.Open(selectedFileName, false, true, missing, missing, missing, true, Excel.XlPlatform.xlWindows, '\t', false, false, 0, false, true, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                Excel.Range xlRange = xlWorkSheet.UsedRange;
                Array myValues = (Array)xlRange.Cells.Value2;

                int vertical = myValues.GetLength(0);
                int horizontal = myValues.GetLength(1);



                // must start with index = 1
                // get header information
                for (int i = 1; i <= horizontal; i++)
                {
                    if (myValues.GetValue(1, i) == null)
                    {
                        horizontal = i - 1;
                        break;
                    }
                    dt.Columns.Add(new DataColumn(myValues.GetValue(1, i).ToString()));
                }

                // Get the row information
                for (int a = 2; a <= vertical; a++)
                {
                    object[] poop = new object[horizontal];
                    for (int b = 1; b <= horizontal; b++)
                    {
                        poop[b - 1] = myValues.GetValue(a, b);
                    }
                    DataRow row = dt.NewRow();
                    row.ItemArray = poop;
                    dt.Rows.Add(row);
                }
                xlWorkBook.Close(false, missing, missing);
                excel.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(excel);
                // assign table to default data grid view
                return dt;


            }
            catch
            {
                return null;
            }
            //report_dg.ItemsSource = data.DefaultView;

        }
       

        public static void ExportToExcel(DataGrid report_dg, string path)
        {

            report_dg.UpdateLayout();


            Excel.Application excel = new Excel.Application();
            excel.Visible = false;
            excel.DisplayAlerts = false;
            Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];

            for (int j = 0; j < report_dg.Columns.Count; j++)
            {
                Excel.Range myRange = (Excel.Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = report_dg.Columns[j].Header;


            }
            for (int i = 0; i < report_dg.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < report_dg.Items.Count; j++)
                {
                    TextBlock b = null;
                    b = report_dg.Columns[i].GetCellContent(report_dg.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    if (b == null)
                    {
                        report_dg.ScrollIntoView(report_dg.Items[j]);
                        report_dg.UpdateLayout();
                        b = report_dg.Columns[i].GetCellContent(report_dg.Items[j]) as TextBlock;

                    }
                    if (b != null)
                    {
                        myRange.Value2 = b.Text;
                    }
                }
            }
            DateTime t = DateTime.Now;
            workbook.SaveAs(path + @"\" + t.Second + '_' + t.Minute + '_' + t.Hour + '_' + t.Day.ToString() + '_' + t.Month.ToString() + "_" + t.Year.ToString() + "_" + "Report", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, System.Type.Missing, System.Type.Missing,
            true, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
            System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing);
            excel.Quit();





        }

        public static void RunInsertTable(DataTable report_dg, string path, string name)
        {
            

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(report_dg,"Sheet 1");
           
            //CreateMergedCell(worksheet, report_dg.Items.Count);

            worksheet.Columns().AdjustToContents();
            
            EmulateSave(workbook, path, name);
        }

        public static void RunInsertMultiTable(List<ItemReport> report_dgList, string path, string name)
        {
            var workbook = new XLWorkbook();
            foreach (var item in report_dgList)
            {

                var worksheet = workbook.Worksheets.Add(item.Report, item.Name);
                worksheet.Columns().AdjustToContents();
            }
            

            //CreateMergedCell(worksheet, report_dg.Items.Count);

          

            EmulateSave(workbook, path, name);
        }

        public static void RunInsertTable(DataTable report_dg, string path, string name,string district)
        {
            System.IO.Directory.CreateDirectory(path);

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(report_dg, district);


            worksheet.Columns().AdjustToContents();

            EmulateSave(workbook, path, name);
        }

        public static void RunInsertTableError(DataGrid report_dg, string path, string name)
        {
            var rows = new List<ErrorRow>();

            for (int i = 0; i < report_dg.Items.Count; i++)
            {
                var row = GenerateRow<ErrorRow>(report_dg, i);
                rows.Add(row);
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet 1");
            worksheet.Cell(1, 1).InsertTable(rows);

            //CreateMergedCell(worksheet, report_dg.Items.Count);

            worksheet.Columns().AdjustToContents();

            EmulateSave(workbook, path, name);
        }

        private static T GenerateRow<T>(DataGrid report_dg, int rowIndex) where T : new()
        {
            report_dg.UpdateLayout();
            var row = new T();
            var rowProps = row.GetType().GetProperties();
            for (int i = 0; i < report_dg.Columns.Count; i++)
            {
                var text = report_dg.Columns[i].GetCellContent(report_dg.Items[rowIndex]) as TextBlock;
                if (text != null)
                {
                    rowProps[i].SetValue(row, text.Text);
                }
                else
                {
                    report_dg.ScrollIntoView(report_dg.Items[rowIndex]);
                    report_dg.UpdateLayout();
                    text = report_dg.Columns[i].GetCellContent(report_dg.Items[rowIndex]) as TextBlock;
                    rowProps[i].SetValue(row, text.Text);
                }
            }
            return row;
        }

        private static void CreateMergedCell(IXLWorksheet worksheet, int rowCount)
        {
            worksheet.Cell(rowCount + 2, 1).Value = "Merged cell";
            var range = worksheet.Range(rowCount + 2, 1, rowCount + 2, 2);
            range.Row(1).Merge();
        }

        private static void EmulateSave(XLWorkbook workbook, string path, string name)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                var pa = System.IO.Path.Combine(path, name);
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                FileStream file = new FileStream(pa, FileMode.Create, FileAccess.Write);
                memoryStream.WriteTo(file);

            }
        }




        public static void ExportToExcelReport(DataGrid report_dg, string path, string name)
        {

            report_dg.UpdateLayout();


            Excel.Application excel = new Excel.Application();
            excel.Visible = false;
            excel.DisplayAlerts = false;
            Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];

            for (int j = 0; j < report_dg.Columns.Count; j++)
            {
                Excel.Range myRange = (Excel.Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = report_dg.Columns[j].Header;


            }
            for (int i = 0; i < report_dg.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < report_dg.Items.Count; j++)
                {
                    TextBlock b = null;
                    b = report_dg.Columns[i].GetCellContent(report_dg.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    if (b == null)
                    {
                        report_dg.ScrollIntoView(report_dg.Items[j]);
                        report_dg.UpdateLayout();
                        b = report_dg.Columns[i].GetCellContent(report_dg.Items[j]) as TextBlock;

                    }
                    if (b != null)
                    {
                        if (i == 4 || i == 6 || i == 7)
                            myRange.Value2 = b.Text;
                        else
                            myRange.Value2 = "'" + b.Text;
                    }
                }
            }
            DateTime t = DateTime.Now;
            workbook.SaveAs(path + @"\" + name, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, System.Type.Missing, System.Type.Missing,
            true, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
            System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing);
            excel.Quit();





        }

        public static void ExportToExcel(DataGrid report_dg, string path, string name)
        {

            report_dg.UpdateLayout();


            Excel.Application excel = new Excel.Application();
            excel.Visible = false;
            excel.DisplayAlerts = false;
            Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];

            for (int j = 0; j < report_dg.Columns.Count; j++)
            {
                Excel.Range myRange = (Excel.Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = report_dg.Columns[j].Header;


            }
            for (int i = 0; i < report_dg.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < report_dg.Items.Count; j++)
                {
                    TextBlock b = null;
                    b = report_dg.Columns[i].GetCellContent(report_dg.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    if (b == null)
                    {
                        report_dg.ScrollIntoView(report_dg.Items[j]);
                        report_dg.UpdateLayout();
                        b = report_dg.Columns[i].GetCellContent(report_dg.Items[j]) as TextBlock;

                    }
                    if (b != null)
                        myRange.Value2 = "'" + b.Text;
                }
            }
            DateTime t = DateTime.Now;
            workbook.SaveAs(path + @"\" + name, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, System.Type.Missing, System.Type.Missing,
            true, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
            System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing, System.Type.Missing);
            excel.Quit();





        }

        static private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
