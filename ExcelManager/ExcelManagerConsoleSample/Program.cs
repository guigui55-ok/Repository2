using ErrorManager;
using ExcelUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelManagerConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test_WindowOnly();
            } finally
            {
                Console.WriteLine("Program Done. Any Press Key.");
                Console.ReadKey();
            }
        }
        static void Test_WindowOnly()
        {
            ErrorManager.ErrorManager err = null;
            ExcelUtility.ExcelManager excelManager = null;
            string method = "Program.Test_Update";
            try
            {
                err = new ErrorManager.ErrorManager(1);
                excelManager = new ExcelUtility.ExcelManager(err);
                err.AddLog(method + " Start");
                //-----------------
                excelManager.UpdateOpendExcelApplication();
                //-----------------
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:\n" + ex.Message);
            }
            finally
            {
                if (excelManager != null) { excelManager.Close(); }
                if (err.hasError) { Console.WriteLine("ErrorManager:\n" + err.GetUserMessageOnlyAsString()); }
                else { Console.WriteLine(method + " Success!"); }
            }
        }

        static void Test_Update()
        {
            ErrorManager.ErrorManager err = null;
            ExcelUtility.ExcelManager excelManager = null;
            string method = "Program.Test_Update";
            try
            {
                err = new ErrorManager.ErrorManager(1);
                excelManager = new ExcelUtility.ExcelManager(err);
                err.AddLog(method + " Start");
                //-----------------
                excelManager.UpdateOpendExcelApplication();
                //-----------------
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:\n" + ex.Message);
            }
            finally
            {
                if (excelManager != null) { excelManager.Close(); }
                if (err.hasError) { Console.WriteLine("ErrorManager:\n" + err.GetUserMessageOnlyAsString()); }
                else { Console.WriteLine(method + " Success!"); }
            }
        }

        static void Test_Open()
        {
            ErrorManager.ErrorManager err=null;
            ExcelUtility.ExcelManager excelManager = null;
            string method = "Program.Test_Open";
            try
            {
                err = new ErrorManager.ErrorManager(1);
                excelManager = new ExcelUtility.ExcelManager(err);
                err.AddLog(method + " Start");
                //-----------------
                string directory = System.IO.Directory.GetCurrentDirectory();
                string filename = "SampleFile1.xlsx";
                string path = directory + "\\" + filename;
                excelManager.OpenFile(path);
                //-----------------

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:\n"+ex.Message);
            }
            finally
            {
                if (excelManager != null) { excelManager.Close(); }
                if (err.hasError) { Console.WriteLine("ErrorManager:\n"+err.GetUserMessageOnlyAsString()); }
                else { Console.WriteLine(method+" Success!"); }
            }
        }
    }
}
