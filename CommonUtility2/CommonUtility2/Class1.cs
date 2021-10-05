using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility
{
    static class Program
    {
        static void Main()
        {
            try
            {
                ErrorManager.ErrorManager _err = new ErrorManager.ErrorManager(1);

                ComWindowsScriptHostObjectModel com = new ComWindowsScriptHostObjectModel(_err);
                com.CreateInstance();
                string path = @"C:\Users\OK\source\repos\CommonUtility2\CommonUtility2\Files\CommonUtility2 - ショートカット.lnk";
                
                dynamic shortcut = com.ComInstance.CreateShortcut(path);
                // ショートカットのリンク先の取得
                string targetPath = shortcut.TargetPath.ToString();
                Console.WriteLine("shortcut.TargetPath.ToString()="+targetPath);

            }
            catch (Exception ex)
            {
                Console.WriteLine("* Exception:");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            } finally
            {
                Console.WriteLine("Done.");
                Console.ReadKey();
            }
        }
    }
}
