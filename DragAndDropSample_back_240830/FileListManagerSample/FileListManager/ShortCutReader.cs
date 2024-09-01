using AppLoggerModule;
using System;
using System.IO;


namespace CommonUtility.FileListUtility
{
    public class ShortCutReader
    {
        protected AppLogger _logger;
        protected ErrorManager.ErrorManager _err;
        public ShortCutReader(ErrorManager.ErrorManager err)
        {
            _err = err;
        }
        public string GetSourceFromPath(string path)
        {
            try
            {
                // windows host script object model
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                // ショートカットオブジェクトの取得
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);

                // ショートカットのリンク先の取得
                string targetPath = shortcut.TargetPath.ToString();

                return targetPath;
            } catch (Exception ex)
            {
                _logger.AddException(ex,this,"ReadShotcut.GetSourceFromPath Failed");
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
                _logger.AddException(ex,this,"ReadShotcut.GetSourceFromPathWithCheck Failed");
                return "";
            }
        }

    }
}
