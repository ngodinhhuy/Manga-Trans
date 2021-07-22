using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace NewSanofi.ClassHelper
{
    public static class ImportExcel
    {
        public static List<string> import_start(string file)
        {
            List<string> result = new List<string>();
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
                

                // Get the row information
                for (int a = 1; a <= vertical; a++)
                {
                    try
                    {
                        result.Add(myValues.GetValue(a, 1).ToString());
                    }
                    catch
                    {
                        break;
                    }
                }
                xlWorkBook.Close(false, missing, missing);
                excel.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(excel);
                // assign table to default data grid view
                return result;


            }
            catch (Exception ex)
            {
                return null;
            }
            //report_dg.ItemsSource = data.DefaultView;

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
