/*
 * 
 * 240829  ErrorManagerとの互換メソッドを追加
 */

namespace AppLoggerModule
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Collections.Generic;

    public enum LogLevel
    {
        DEF,
        CRITICAL,
        ERR,
        ALERT,
        WARN,
        NORMAL,
        INFO,
        DEBUG,
        DETAIL,
        TRACE
    }

    [Flags]
    public enum OutputMode
    {
        NONE = 0,               // 0000
        DEBUG_WINDOW = 1,       // 0001
        CONSOLE = 2,            // 0010
        FILE = 4                // 0100
    }

    public class AppLogger
    {
        public LogLevel LoggerLogLevel { get; set; } = LogLevel.INFO;
        public string FilePath { get; set; } = "";
        public string LogFileTimeFormat { get; set; } = "_yyyyMMdd_HHmmss";
        public OutputMode LogOutPutMode { get; set; } = OutputMode.DEBUG_WINDOW;
        public bool AddTime { get; set; } = true;
        public List<Exception> ErrorList = new List<Exception> { };
        public List<string> ErrorMessagelist = new List<string> { };
        public List<string> AlertMessageList = new List<string> { };
        public AppLogger() { }

        public void SetDefaultValues()
        {
            this.LogFileTimeFormat = "";
            // ファイルパスをCurrentDirectoryに設定
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logFilePath = System.IO.Path.Combine(currentDirectory, "__test_log.log");
            this.LogFileTimeFormat = "";
            this.SetFilePath(logFilePath);
            Debug.Print(string.Format("logFilePath = {0}", logFilePath));
            // ログレベルをINFOに設定
            this.LoggerLogLevel = LogLevel.INFO;
            // ログをコンソールとファイルに出力するように設定
            this.LogOutPutMode = OutputMode.CONSOLE | OutputMode.FILE;
        }

        public void MakeLogDir()
        {
            string dirPath = Path.GetDirectoryName(this.FilePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                Debug.Print("Log CreateDirectory Path= " + dirPath);
            }
        }

        /// <summary>
        /// ログのファイルパスを設定する
        /// </summary>
        /// <remarks>
        /// logFileTimeFormat または this.logFileTimeFormat が設定されているときは
        /// log_[TimeFormat].logというように、時間書式が追加される
        /// </remarks>
        /// <param name="filePath"></param>
        /// <param name="logFileTimeFormat"></param>
        public void SetFilePath(string filePath, string logFileTimeFormat = "")
        {
            if (string.IsNullOrEmpty(logFileTimeFormat))
            {
                logFileTimeFormat = this.LogFileTimeFormat;
            }

            if (string.IsNullOrEmpty(logFileTimeFormat))
            {
                this.FilePath = filePath;
            }
            else
            {
                string dirPath = Path.GetDirectoryName(filePath);
                string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
                string datetimeStr = DateTime.Now.ToString(logFileTimeFormat);
                string ext = Path.GetExtension(filePath);
                this.FilePath = $"{dirPath}\\{fileNameOnly}{datetimeStr}{ext}";
            }
            this.MakeLogDir();
        }

        public void PrintCritical(string value)
        {
            if (LogLevel.CRITICAL <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }

        public void PrintError(string value)
        {
            if (LogLevel.ERR <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }
        public void PrintError(Exception ex, string value)
        {
            if (LogLevel.ERR <= this.LoggerLogLevel)
            {
                this.Print(value);
                this.Print(ex.Message);
                this.Print(ex.StackTrace);
            }
        }

        public void PrintAlert(string value)
        {
            if (LogLevel.ALERT <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }
        public void PrintWarn(string value)
        {
            if (LogLevel.WARN <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }

        public void PrintInfo(string value)
        {
            if (LogLevel.INFO <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }

        public void PrintDebug(string value)
        {
            if (LogLevel.DEBUG <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }
        public void PrintDetail(string value)
        {
            if (LogLevel.DETAIL <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }

        public void PrintTrace(string value)
        {
            if (LogLevel.TRACE <= this.LoggerLogLevel)
            {
                this.Print(value);
            }
        }

        private string AddTimeValue(string value)
        {
            if (this.AddTime)
            {
                return this.GetTimeStr() + "    " + value;
            }
            return value;
        }

        private string GetTimeStr()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyy/MM/dd HH:mm:ss ffffff");
        }

        private void Print(string value)
        {
            value = this.AddTimeValue(value);
            if ((this.LogOutPutMode & OutputMode.DEBUG_WINDOW) == OutputMode.DEBUG_WINDOW)
            {
                Debug.WriteLine(value);
            }
            if ((this.LogOutPutMode & OutputMode.CONSOLE) == OutputMode.CONSOLE)
            {
                Console.WriteLine(value);
            }
            if ((this.LogOutPutMode & OutputMode.FILE) == OutputMode.FILE)
            {
                this.PrintToFile(value);
            }
        }

        private void PrintToFile(string value)
        {
            if (!string.IsNullOrEmpty(this.FilePath))
            {
                this.WriteToFile(value);
            }
        }

        private void WriteToFile(string value)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(this.FilePath, true))
                {
                    writer.WriteLine(value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WriteToFile ERROR: " + ex.Message);
            }
        }
        // ErrorManagerとの互換用メソッド
        public void AddException(string errorMessage)
        {
            this.PrintError(errorMessage);
            this.ErrorMessagelist.Add(errorMessage);
        }
        public void AddException(Exception ex)
        {
            this.PrintError(ex.Message);
            this.PrintError(ex.StackTrace);
            this.ErrorList.Add(ex);
        }
        public void AddException(Exception ex, string errorMessage)
        {
            this.AddException(errorMessage);
            this.AddException(ex);
        }
        public void AddException(Exception ex, string errorMessage, string addMessage)
        {
            this.AddException(errorMessage + ":" + addMessage);
            this.AddException(ex);
        }
        public void AddException(Exception ex, object obj, string addMessage)
        {
            this.AddException(obj.ToString() + "." + addMessage);
            this.AddException(ex);
        }
        public void AddException(object obj, string addMessage, Exception ex)
        {
            this.AddException(obj.ToString() + "." + addMessage);
            this.AddException(ex);
        }
        public void AddLogWarning(string message)
        {
            this.PrintWarn(message);
        }
        public void AddLogWarning(object obj, string message)
        {
            message = obj.ToString() + "." + message;
            this.AddLogWarning(message);
        }
        public void AddLogWarning(object obj, string message, Exception ex)
        {
            message = obj.ToString() + "." + message;
            this.AddLogWarning(message);
            this.AddLogWarning(ex.Message);
            this.AddLogWarning(ex.StackTrace);
        }

        public void AddLog(string message)
        {
            this.PrintInfo(message);
        }
        public void AddLog(object obj, string message)
        {
            message = obj.ToString() + " > " + message;
            this.PrintInfo(message);
        }
        public void AddLogTrace(object obj, string message)
        {
            message = obj.ToString() + " > " + message;
            this.PrintTrace(message);
        }
        public void ClearError()
        {
            this.ErrorList.Clear();
        }

        public void AddLogAlert(string message)
        {
            this.PrintAlert(message);
            this.AlertMessageList.Add(message);
        }

        public void AddLogAlert(object obj , string message)
        {
            message = obj.ToString() + "." + message;
            this.AddLogAlert(message);
        }
        public void AddLogAlert(object obj, string message, Exception ex)
        {
            message = obj.ToString() + "." + message;
            this.AddLogAlert(message);
            this.PrintInfo(ex.Message);
            this.PrintInfo(ex.StackTrace);
        }

        public bool hasAlert()
        {
            if (this.AlertMessageList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool hasError()
        {
            if (this.ErrorList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetLastErrorMessagesAsString(int indexFromLast, bool addExceptionMessage)
        {
            string errmsg = "";
            int count = 0;
            List<string> msgList = this.GetErrorMessages(true, false);
            foreach (string msg in msgList)
            {
                errmsg = errmsg + "\n" + msg;
                count++;
                if (indexFromLast < count)
                {
                    return errmsg;
                }
            }
            return errmsg;
        }
        public List<string> GetErrorMessages(bool OrderRev, bool addExceptionMessage)
        {
            List<string> ret = this.ErrorMessagelist;
            if (OrderRev)
            {
                ret.Reverse();
            }
            return ret;
        }
    }
}