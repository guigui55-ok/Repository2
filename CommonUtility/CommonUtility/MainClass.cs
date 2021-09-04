using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility.CloseHandleUtil;
using ExcelUtility;

namespace CommonUtility
{
    static class MainClass
    {
        static void Main()
        {
            try
            {
                //GetFileNamesFromPidSample();
                ShortCutCynamicSample();
            }
            finally
            {
                Console.WriteLine("Program End. Press Any Key.");
                Console.ReadKey();
            }
        }

        static public void ShortCutCynamicSample()
        {
            try
            {
                ShortcutUtility shortcutUtility = new ShortcutUtility();
                string path = @"C:\Users\OK\source\repos\Repository2\CommonUtility\CommonUtility\TestFiles\Microsoft Edge ShortCut.lnk";
                // ショートカットのフルパスを取得する
                string buf = shortcutUtility.GetFullName(path);
                Console.WriteLine("GetFullName = " + buf);
                // ShortCut のリンク先を取得する
                buf = shortcutUtility.GetTargetPath(path);
                Console.WriteLine("GetTargetPath = " + buf);

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        static public void GetFileNamesFromPidSample()
        {
            try
            {
                ErrorManager.ErrorManager _err = new ErrorManager.ErrorManager(1);
                List<string> typeList = new List<string>() { ".xlsx", ".xlsb" };

                // プロセス名 EXCEL の ProcessID を取得する
                ProcessUtility procUtil = new ProcessUtility(_err);
                List<int> pidList = procUtil.GetPidListContainsProcessNameInNow("EXCEL");
                if (pidList.Count < 1) { Console.WriteLine("pidList.Count < 1"); return; }
                else { Console.WriteLine(string.Join(", ", pidList)); }

                // ProcessID の Handle が保持している ＆ typeList と合致しているものを取得する
                AcccessFileGetterPathFromPidForExcel getter =
                    new AcccessFileGetterPathFromPidForExcel(_err, typeList);
                List<string> pathList = getter.GetListExcelFilePathFromPid(pidList[0]);
                if (pidList.Count < 1) { Console.WriteLine("pathList.Count < 1"); return; }
                else { Console.WriteLine(string.Join(", ", pathList)); }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

    }
}
