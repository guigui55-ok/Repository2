using CommonUtility.ComUtility;
using System;
using System.Dynamic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonUtility.Shortcut
{
    public class ShortcutDynamic
    {
        public string Arguments;
        public string Description;
        public string FullName;
        public object Hotkey;
        public string IconLocation;
        public string TargetPath;
        public object WindowStyle;
        public string WorkingDirectory;

    }
    // COMのWindows Script Host Object Modelを参照設定に追加
    public class ShortcutUtility
    {
        ShortcutDynamic _result=null;
        protected ErrorManager.ErrorManager _err;

        public ShortcutUtility(ErrorManager.ErrorManager err)
        {
            _err = err;
        }
        public string GetFullName(string fullPath)
        {

            _result = Get(fullPath,true);
            if (_result == null) { return ""; }
            return _result.FullName;
        }

        public string GetTargetPath(string fullPath)
        {
            _result = Get(fullPath,true);
            if(_result == null) { return ""; }
            return _result.TargetPath;
        }

        public ShortcutDynamic Get(string path, bool isConvUrl = true)
        {
            ComWindowsScriptHostObjectModel com = null;
            try
            {
                if ((!System.IO.File.Exists(path)) && (!System.IO.Directory.Exists(path)))
                {
                    return null;
                }

                // ファイルの拡張子を取得
                string extension = Path.GetExtension(path);
                // ファイルへのショートカットは拡張子".lnk"
                if (".lnk" == extension)
                {
                    //IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();

                    // ショートカットオブジェクトの取得
                    //IWshRuntimeLibrary.IWshShortcut shortcut
                    //    = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(fullPath);
                    com = new ComWindowsScriptHostObjectModel(_err);
                    com.CreateInstance();

                    // ショートカットのリンク先の取得
                    //string targetPath = shortcut.TargetPath.ToString();
                    dynamic shortcut = com.ComInstance.CreateShortcut(path);
                    // ショートカットのリンク先の取得
                    string targetPath = shortcut.TargetPath.ToString();


                    _result = new ShortcutDynamic();
                    _result.FullName = path;
                    _result.TargetPath = targetPath;
                }
                else if (".url" == extension)
                {
                    if (isConvUrl)
                    {
                        using (var sr = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
                        {

                            int count = 0;
                            while (sr.Peek() != -1)
                            {
                                if (count == 1)
                                {
                                    _result = new ShortcutDynamic();
                                    _result.FullName = path;
                                    string buf = sr.ReadLine();
                                    _result.TargetPath = buf.Substring(4, buf.Length - 4);
                                    return _result;
                                }
                                else
                                {
                                    sr.ReadLine();
                                }
                                count++;
                            }
                        }
                    }
                    else
                    {
                        _result.FullName = System.IO.Path.GetFileName(path);
                        _result.TargetPath = path;
                    }
                }
                return _result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ShortcutDynamic.Get:" + ex.Message);
                return null;
            }
            finally
            {
                if (com != null) { com.Dispose(); com = null; }
            }
        }

        public ShortcutDynamic Get(string fullPath)
        {
            dynamic shell = null;   // IWshRuntimeLibrary.WshShell
            dynamic lnk = null;     // IWshRuntimeLibrary.IWshShortcut
            //ExpandoObject lnkEx = null;
            try
            {
                var type = Type.GetTypeFromProgID("WScript.Shell");
                shell = Activator.CreateInstance(type);
                lnk = shell.CreateShortcut(fullPath);

                if (string.IsNullOrEmpty(lnk.TargetPath))
                {
                    // lnkファイルが存在しない場合
                    _result = null;
                    return null;
                }
                //dynamic result = new ExpandoObject();
                dynamic result = new ExpandoObject();
                result.Arguments = lnk.Arguments;
                result.Description = lnk.Description;
                result.FullName = lnk.FullName;
                result.Hotkey = lnk.Hotkey;
                result.IconLocation = lnk.IconLocation;
                result.TargetPath = lnk.TargetPath;
                result.WorkingDirectory = lnk.WorkingDirectory;
                //{
                //    lnk.Arguments,
                //    lnk.Description,
                //    lnk.FullName,
                //    lnk.Hotkey,
                //    lnk.IconLocation,
                //    lnk.TargetPath,
                //    lnk.WindowStyle,
                //    lnk.WorkingDirectory
                //};

                _result = new ShortcutDynamic()
                {
                    Arguments = lnk.Arguments,
                    Description = lnk.Description,
                    FullName = lnk.FullName,
                    Hotkey = lnk.Hotkey,
                    IconLocation = lnk.IconLocation,
                    TargetPath = lnk.TargetPath,
                    WindowStyle = lnk.WindowStyle,
                    WorkingDirectory = lnk.WorkingDirectory
                };

                return _result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _result = null;
                return null;
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
