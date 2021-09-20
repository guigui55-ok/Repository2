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
                Test_Excute();
            } finally
            {
                Console.WriteLine("Program Done. Any Press Key.");
                Console.ReadKey();
            }
        }

        static void Test_Excute()
        {
            ErrorManager.ErrorManager err = null;
            ExcelManager excelManager = null;
            string method = "Program.Test_Update";
            try
            {
                err = new ErrorManager.ErrorManager(1);
                excelManager = new ExcelManager(err);
                err.AddLog(method + " Start");
                //-----------------
                //Test_Open(err, excelManager);
                Test_Update(err, excelManager);
                //Test_WindowOnly(err, excelManager);
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

        static void Test_WindowOnly(ErrorManager.ErrorManager _err, ExcelManager excelManager)
        {
            _err.AddLog("Test_WindowOnly");
            excelManager.ExcelApplicationRunWhenNothing();
        }

        static void Test_Update(ErrorManager.ErrorManager _err,ExcelManager excelManager)
        {
            _err.AddLog("Test_Update");
            excelManager.UpdateOpendExcelApplication();
            int count = 0;
            foreach(ExcelApps apps in excelManager.GetExcelAppsList())
            {
                Console.Write(count + " : pid=" + apps.ProcessId);
                bool flag;
                if(apps.ApplicationIsNull()) { flag = false; }
                else { }
                Console.WriteLine("  ,apps.ApplicationIsNull()="+ apps.ApplicationIsNull());
                count++;
            }
        }

        static void Test_Open(ErrorManager.ErrorManager _err,ExcelManager excelManager)
        {
            _err.AddLog("Test_Open");
            string directory = System.IO.Directory.GetCurrentDirectory();
            string filename = "SampleFile1.xlsx";
            filename = "";
            string path = directory + "\\" + filename;
            excelManager.OpenFile(path);
        }
    }
}
