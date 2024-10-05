using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CommonModules
{

    public class FileMoverWin32
    {
        // MoveFileの宣言
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);

        // MoveFileExの宣言
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, uint dwFlags);

        // MoveFile を使用する関数
        public static void MoveFileUsingWin32(string sourceFilePath, string destinationFilePath)
        {
            if (!MoveFile(sourceFilePath, destinationFilePath))
            {
                // エラー時の処理
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine($"MoveFile failed with error code: {errorCode}");
            }
            else
            {
                Console.WriteLine("ファイルを正常に移動しました。");
            }
        }

        // MoveFileEx を使用する関数
        public static int MoveFileExUsingWin32(string sourceFilePath, string destinationFilePath, uint flags)
        {
            if (!MoveFileEx(sourceFilePath, destinationFilePath, flags))
            {
                // エラー時の処理
                int errorCode = Marshal.GetLastWin32Error();
                Console.WriteLine($"[ERROR] MoveFileEx failed with error code: {errorCode}");
                //3  : 指定したパスが見つかりません。 ERROR_TOO_MANY_OPEN_FILES
                //32 : ファイルが別のプロセスによってロックされている ERROR_SHARING_VIOLATION (32)
                return errorCode;
            }
            else
            {
                Console.WriteLine("ファイルを正常に移動しました。");
                return 0;
            }
        }
    }

    public class FileLockInfo
    {

        public static string GetFileLockingProcess(string handleExePath, string filePath)
        {
            string ret = "";
            try
            {
                // Handle.exe のパス
                //string handleExePath = @"path\to\handle.exe";  // Handle.exe の実行ファイルパス
                string arguments = filePath;

                // Handle.exe のプロセスを実行
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = handleExePath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd(); // Handleの出力結果を取得
                process.WaitForExit();

                // 出力を解析する正規表現 (プロセスIDとプロセス名の取得)
                Regex regex = new Regex(@"(?<ProcessName>\S+)\s+pid:\s+(?<PID>\d+)", RegexOptions.IgnoreCase);

                // 出力結果からプロセス情報を抽出
                MatchCollection matches = regex.Matches(output);
                if (matches.Count > 0)
                {
                    //Console.WriteLine("ファイルをロックしているプロセス:");
                    foreach (Match match in matches)
                    {
                        string processName = match.Groups["ProcessName"].Value;
                        string processId = match.Groups["PID"].Value;
                        ret += $"プロセス名: {processName}, プロセスID: {processId}";
                        Console.WriteLine(ret);
                    }
                }
                else
                {
                    ret = "ファイルをロックしているプロセスは見つかりませんでした。";
                    Console.WriteLine(ret);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
            }
            return ret;
        }
    }


    public static class CommonGeneral
    {
        public static void MoveFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                // まず、ファイルが存在するかを確認
                if (!File.Exists(sourceFilePath))
                {
                    throw new FileNotFoundException("ソースファイルが見つかりません。", sourceFilePath);
                }

                // 既に目的地にファイルが存在する場合、例外を投げる
                if (File.Exists(destinationFilePath))
                {
                    throw new IOException("目的地に同じ名前のファイルが既に存在します。");
                }

                // ファイルをコピーする
                File.Copy(sourceFilePath, destinationFilePath);

                // コピーが成功したら、元ファイルを削除する
                File.Delete(sourceFilePath);

                //Console.WriteLine("ファイルを正常に移動しました。");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"エラーが発生しました: {ex.Message}");
                throw ex;
            }
        }



        public static string GetApplicationPath()
        {
            Assembly myAssembly = Assembly.GetEntryAssembly();
            //_logger.PrintInfo(String.Format("myAssembly.Location = {0}", myAssembly.Location));
            return myAssembly.Location;
        }


        /// <summary>
        /// 指定されたフォルダパスでエクスプローラを開く関数
        /// </summary>
        /// <param name="folderPath">表示したいフォルダのパス</param>
        public static void OpenFolder(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath))
            {
                // エクスプローラでフォルダを開く
                Process.Start("explorer.exe", folderPath);
            }
            else
            {
                // パスが無効の場合はデフォルトの「マイ コンピュータ」を開く
                Process.Start("explorer.exe");
            }
        }

        public static void LogCallerInfo(int stackFrameIndex = 0, int MaxIndex = 0)
        {
            List<string> retList = new List<string>();
            try
            {
                retList = GetCallerInfo(stackFrameIndex, MaxIndex);
                for (int i = 0; i < retList.Count; i++)
                {
                    Console.WriteLine(retList[i]);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        public static List<string> GetCallerInfo(int stackFrameIndex = 0, int MaxIndex = 0)
        {
            List<string> retList = new List<string>();
            try
            {
                // スタックトレースを取得
                StackTrace stackTrace = new StackTrace(true);

                if (MaxIndex < 0) { MaxIndex = stackTrace.FrameCount; }
                for (int i = stackFrameIndex; i < stackTrace.FrameCount; i++)
                {

                    StackFrame frame = stackTrace.GetFrame(i);
                    string fileName = frame.GetFileName();
                    string methodName = frame.GetMethod().Name;
                    int lineNumber = frame.GetFileLineNumber();

                    string buf = $"[{i}]Caller File: {fileName}, Method: {methodName}, Line: {lineNumber}";
                    retList.Add(buf);
                    if (MaxIndex <= i) { break; }
                }
                //for (int i = 0; i < retList.Count; i++)
                //{
                //    Console.WriteLine(retList[i]);
                //}

                //// 指定された階層のスタックフレームを取得
                //if (stackFrameIndex < stackTrace.FrameCount)
                //{
                //    StackFrame frame = stackTrace.GetFrame(stackFrameIndex);
                //    string fileName = frame.GetFileName();
                //    string methodName = frame.GetMethod().Name;
                //    int lineNumber = frame.GetFileLineNumber();

                //    Console.WriteLine($"Caller File: {fileName}, Method: {methodName}, Line: {lineNumber}");
                //}
                //else
                //{
                //    Console.WriteLine($"The specified stack frame index ({stackFrameIndex}) exceeds the stack depth.");
                //}
            }
            catch (Exception ex)
            {
                string err = $"An error occurred: {ex.Message}";
                retList.Add(err);
            }
            return retList;
        }

        /// <summary>
        /// Dictionary(string,object)をすべてConsoleに出力する
        /// （多階層のDictにも対応）
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="num_str"></param>
        /// <param name="withDataType"></param>
        public static void PrintDict(Dictionary<string, object> dict, string num_str = "", bool withDataType = true)
        {
            int count = 0;
            foreach (string key in dict.Keys)
            {
                if (dict[key].GetType().ToString().IndexOf("Dictionary") > 0)
                {
                    Dictionary<string, object> dictB = (Dictionary<string, object>)dict[key];
                    if (num_str != "") { num_str += "-" + count.ToString(); }
                    PrintDict(dictB, num_str, withDataType);
                }
                else
                {
                    string num_strB;
                    if (num_str != "") { num_strB = num_str + "-" + count.ToString(); }
                    else { num_strB = count.ToString(); }
                    string dataTypeStr = "";
                    if (withDataType)
                    {
                        dataTypeStr = dict[key].GetType().ToString();
                        dataTypeStr = string.Format(" {{{0}}} ", dataTypeStr);
                    }
                    Console.WriteLine(String.Format("i={0},{3}{1} : {2}", num_strB, key, dict[key], dataTypeStr));
                }
                count++;
            }
        }

        /// <summary>
        /// 指定されたディレクトリからGIFファイルのパスリストを取得するメソッド
        /// </summary>
        /// <param name="defaultDirPath"></param>
        /// <returns></returns>
        public static List<string> GetPathList(string defaultDirPath, string searchPattern = "*.*")
        {
            List<string> retPaths = new List<string>();

            if (Directory.Exists(defaultDirPath))
            {
                // ディレクトリ内の特定のファイルのみを取得
                retPaths = Directory.GetFiles(defaultDirPath, searchPattern, SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                //_logger.PrintError($"指定されたディレクトリが存在しません: {defaultDirPath}");
            }

            return retPaths;
        }

        /// <summary>
        /// 親ディレクトリを取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetParentDirectory(string filePath, int count = 0)
        {
            // DirectoryInfoを使用する場合
            DirectoryInfo dirInfo = new DirectoryInfo(filePath);

            // 1回目の親ディレクトリ取得
            DirectoryInfo parentDirInfo = dirInfo?.Parent;
            //Console.WriteLine("Parent Directory (DirectoryInfo): " + parentDirInfo.FullName);
            if (count <= 0)
            {
                return parentDirInfo.FullName;
            }
            else
            {
                // 2回目以降の親ディレクトリ取得
                for (int i = 0; i < count; i++)
                {
                    parentDirInfo = parentDirInfo?.Parent;
                    //Console.WriteLine("Grandparent Directory (DirectoryInfo): " + parentDirInfo?.FullName);
                }
                return parentDirInfo.FullName;
            }
        }



        /// <summary>
        /// コントロールリストを任意の型に変換する
        /// </summary>
        /// <param name=""></param>
        /// <param name="convertType"></param>
        /// <returns></returns>
        public static List<object> ConvertControlList(List<Control> controlList, Type convertType)
        {
            List<object> retList = new List<object> { };
            foreach (object con in controlList)
            {
                retList.Add(Convert.ChangeType(con, convertType));
            }
            return retList;
        }

        /// <summary>
        /// タイプとマッチした子コントロールをすべて取得する
        /// （子の子以下のコントロール以下すべてが対象）
        /// </summary>
        /// <param name="control"></param>
        /// <param name="typeObject"></param>
        /// <returns></returns>
        public static List<Control> GetControlListIsMatchType(Control control, Type typeObject)
        {
            List<Control> retList = new List<Control> { };
            foreach (Control con in control.Controls)
            {
                if (con.GetType() == typeObject)
                {
                    retList.Add(con);
                    List<Control> bufList = GetControlListIsMatchType(con, typeObject);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }

        /// <summary>
        /// 名前と合致するコントロールをすべて取得する
        /// （子の子以下のコントロール以下すべてが対象）
        /// </summary>
        /// <param name="control"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static List<Control> GetControlListIsMatchName(Control control, string objectName)
        {
            List<Control> retList = new List<Control> { };
            foreach (Control con in control.Controls)
            {
                Console.WriteLine(string.Format("con.Name = {0}", con.Name));
                Console.WriteLine(string.Format("con.GetType.ToString = {0}", con.GetType().ToString()));
                if (con.Name == objectName)
                {
                    retList.Add(con);
                    List<Control> bufList = GetControlListIsMatchName(con, objectName);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }


        /// <summary>
        /// MenuStrip.Items の名前に合致したものすべてをリストで取得
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static List<ToolStripMenuItem> GetMenuItemListIsMatchNameInMenuStrip(ContextMenuStrip menuStrip, string objectName)
        {
            // MenuStrip, ToolStripMenuItemを両方扱いたかったが、Itemsプロパティがないため別関数で扱う
            //if (menuStrip.GetType() == typeof(MenuStrip))
            //{
            //    menuStrip = (MenuStrip)menuStrip;
            //} else if( menuStrip.GetType() == typeof(ToolStripMenuItem))
            //{
            //    menuStrip = (ToolStripMenuItem)menuStrip;
            //    ToolStripMenuItem item = (ToolStripMenuItem)menuStrip;
            //    item.items
            //}
            //else
            //{
            //    throw new Exception("TypeError " + menuStrip.GetType().ToString());
            //}
            List<ToolStripMenuItem> retList = new List<ToolStripMenuItem> { };
            foreach (ToolStripMenuItem con in menuStrip.Items)
            {
                if (con.Name == objectName)
                {
                    retList.Add(con);
                    List<ToolStripMenuItem> bufList = GetMenuItemListIsMatchNameInMenuItem(con, objectName);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }
        /// <summary>
        /// ToolStripMenuItemのDropDownItems の名前に合致したものすべてをリストで取得
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static List<ToolStripMenuItem> GetMenuItemListIsMatchNameInMenuItem(ToolStripMenuItem menuItem, string objectName)
        {
            List<ToolStripMenuItem> retList = new List<ToolStripMenuItem> { };
            foreach (ToolStripMenuItem con in menuItem.DropDownItems)
            {
                if (con.Name == objectName)
                {
                    retList.Add(con);
                    List<ToolStripMenuItem> bufList = GetMenuItemListIsMatchNameInMenuItem(con, objectName);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }


        public static bool AnyToBool(object value)
        {
            if (value.GetType() == typeof(string))
            {
                string _valueStr = (string)value;
                // trueをメインに判定する
                // 基本的にtrueでなければfalseとする
                if (_valueStr == "1") { return true; }
                if (_valueStr.ToLower() == "true") { return true; }
                // 空文字はfalse
                if (_valueStr == "") { return false; }
                return false;
            }
            if ((value.GetType() == typeof(int)))
            {
                int _valueInt = (int)value;
                if (_valueInt >= 1) { return true; }
                return false;
            }
            if (value.GetType() == typeof(bool))
            {
                return (bool)value;
            }
            else
            {
                return (bool)value;
            }
        }
    }
    public class SimpleFileListManager
    {
        public List<string> GetFilteredFileList(string path, List<string> conditionsRegix)
        {
            List<string> filePaths = new List<string>();

            // フォルダ内の全ファイルを取得
            if (Directory.Exists(path))
            {
                filePaths.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
            }
            else
            {
                throw new DirectoryNotFoundException($"指定されたパスが存在しません: {path}");
            }

            // 条件にマッチしないファイルを除外
            List<string> filteredFilePaths = new List<string>();
            foreach (var filePath in filePaths)
            {
                bool isMatch = false;
                foreach (var pattern in conditionsRegix)
                {
                    if (Regex.IsMatch(Path.GetFileName(filePath), pattern))
                    {
                        isMatch = true;
                        break;
                    }
                }

                if (isMatch)
                {
                    filteredFilePaths.Add(filePath);
                }
            }

            return filteredFilePaths;
        }
    }


    /// <summary>
    /// KeyDownやMouseMove時にログを出力させたいが、ログが出すぎてしまうとき用に使用するタイマー
    /// </summary>
    public class ExecuteDelayTimer
    {
        private Timer _timer;
        private Action<object> _argExecuteTimerMethod;
        private object _timerMethodValue;
        private int _timerCount=0;

        public ExecuteDelayTimer(int interval=1000*10)
        {
            _timer = new Timer(interval);

            // タイマーのTickイベントにメソッドを登録
            _timer.Elapsed += OnTimerElapsed;
        }

        public void Execute(Action<object> ArgExecuteTimerTickMethod, object TimerMethodValue)
        {
            _argExecuteTimerMethod = ArgExecuteTimerTickMethod;
            _timerMethodValue = TimerMethodValue;
            //既に実行されていたら何もしない
            if (!_timer.Enabled)
            {
                // 最初は一回実行
                _argExecuteTimerMethod?.Invoke(_timerMethodValue);
                _timer.Start();
                _timerCount = 0;
            }
            
        }

        // タイマーが経過した時に呼び出されるメソッド
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (0 < _timerCount)
            {
                //1回だけ実行して終了
                _timer.Stop();
            }
            _timerCount += 1;
            // 常に _timerMethodValue を引数に渡してメソッドを実行
            _argExecuteTimerMethod?.Invoke(_timerMethodValue);

        }

        // タイマーの開始
        public void Start()
        {
            _timer.Start();
        }

        // タイマーの停止
        public void Stop()
        {
            _timer.Stop();
        }

        // TimerMethodValue を更新するメソッド
        public void UpdateTimerMethodValue(object newValue)
        {
            _timerMethodValue = newValue;
        }
    }

    public static class CommonUtility
    {
        /// <summary>
        /// 任意の列挙型の値と名前を表示する汎用メソッド
        /// </summary>
        /// <param name="enumType">表示したい列挙型のタイプ</param>
        public static string DisplayEnumValues(Type enumType, int index)
        {
            // 列挙型であるか確認する
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("引数は列挙型でなければなりません。");
            }

            Array enumValues = Enum.GetValues(enumType);

            int count = 0;
            foreach (var value in enumValues)
            {
                string name = Enum.GetName(enumType, value);
                int intValue = (int)value;
                string output = $"{name} [{intValue}]";
                if (index <= count)
                {
                    return output;
                }
            }
            return "";
        }
    }

    public static class Debugger
    {
        public static void DebugPrint(string value)
        {
            Debug.Print(value);
        }
        public static void DebugPrint(params string[] values)
        {
            foreach (var value in values)
            {
                Debug.Print(value);
            }
        }
    }
}
