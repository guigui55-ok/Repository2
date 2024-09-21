using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.IO;
using System.Text.RegularExpressions;

namespace CommonModules
{

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
