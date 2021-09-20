using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ReadShortCut
    {
        public string GetSourceFromPath(string path)
        {

            dynamic shell = null;   // IWshRuntimeLibrary.WshShell
            dynamic lnk = null;     // IWshRuntimeLibrary.IWshShortcut
            try
            {
                var type = Type.GetTypeFromProgID("WScript.Shell");
                shell = Activator.CreateInstance(type);
                lnk = shell.CreateShortcut(path);
                // windows host script object model
                //IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                // ショートカットオブジェクトの取得
                //IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);

                // ショートカットのリンク先の取得
                string targetPath = lnk.TargetPath.ToString();

                return targetPath;
            } catch (Exception ex)
            {
                Debug.WriteLine("ReadShotcut.GetSourceFromPath Failed");
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        public string GetSourceFromPathWithCheck(string path)
        {
            try
            {
                string ret = path;
                // ファイルの拡張子を取得
                string extension = Path.GetExtension(path);
                // ファイルへのショートカットは拡張子".lnk"
                if (".lnk" == extension)
                {
                    ret = GetSourceFromPath(path);
                }
                return ret;
                } catch (Exception ex)
            {
                Debug.WriteLine("ReadShotcut.GetSourceFromPathWithCheck Failed");
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

    }
}
