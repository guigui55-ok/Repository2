using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    class ShortcutDynamic
    {
        public string GetFullName(string fullPath)
        {
            return Get(fullPath).FullName;
        }

        public string GetTargetPath(string fullPath)
        {
            return Get(fullPath).TargetPath;
        }

        public dynamic Get(string fullPath)
        {
            dynamic shell = null;   // IWshRuntimeLibrary.WshShell
            dynamic lnk = null;     // IWshRuntimeLibrary.IWshShortcut
            try
            {
                var type = Type.GetTypeFromProgID("WScript.Shell");
                shell = Activator.CreateInstance(type);
                lnk = shell.CreateShortcut(fullPath);

                if (string.IsNullOrEmpty(lnk.TargetPath))
                    // lnkファイルが存在しない場合はここに来る
                    return "";

                var result = new
                {
                    lnk.Arguments,
                    lnk.Description,
                    lnk.FullName,
                    lnk.Hotkey,
                    lnk.IconLocation,
                    lnk.TargetPath,
                    lnk.WindowStyle,
                    lnk.WorkingDirectory
                };

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (lnk != null) Marshal.ReleaseComObject(lnk);
                if (shell != null) Marshal.ReleaseComObject(shell);
            }
        }

        public string GetToString(string fullPath)
        {
            dynamic shell = null;   // IWshRuntimeLibrary.WshShell
            dynamic lnk = null;     // IWshRuntimeLibrary.IWshShortcut
            try
            {
                var type = Type.GetTypeFromProgID("WScript.Shell");
                shell = Activator.CreateInstance(type);
                lnk = shell.CreateShortcut(fullPath);

                if (string.IsNullOrEmpty(lnk.TargetPath))
                    // lnkファイルが存在しない場合はここに来る
                    return "";

                var result = new
                {
                    lnk.Arguments,
                    lnk.Description,
                    lnk.FullName,
                    lnk.Hotkey,
                    lnk.IconLocation,
                    lnk.TargetPath,
                    lnk.WindowStyle,
                    lnk.WorkingDirectory
                };

                return result.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (lnk != null) Marshal.ReleaseComObject(lnk);
                if (shell != null) Marshal.ReleaseComObject(shell);
            }
        }
    }
}
