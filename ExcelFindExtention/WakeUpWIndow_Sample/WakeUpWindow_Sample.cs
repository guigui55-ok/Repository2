using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorManager;
using ExcelFindExtention;

namespace WakeUpWIndow_Sample
{
    class WakeUpWindow_Sample
    {
        static void Main(string[] args)
        {
            if(args == null)
            {
                new WakeUpWindow_Sample().Test1();
            }
            //if(args.Length > 0) { }
        }
        public void Test1()
        {
            Process prs = null;
            try
            {
                ErrorManager.ErrorManager Error = new ErrorManager.ErrorManager(1);

                prs = Process.GetProcessById(8392);
                Console.WriteLine("Process Id = "+prs.Id);

                WindowControl windUtil = new WindowControl(Error);

                windUtil.WakeupWindow((IntPtr)prs.MainWindowHandle);

            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (prs != null) { 
                    prs.Dispose(); 
                    //prs = null; 
                }
                Console.WriteLine("press any key.");
                Console.ReadKey();
            }
        }
    }
}
