namespace AppLoggerModuleB
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public enum LogLevel
    {
        DEF,
        CRITICAL,
        ERR,
        WARN,
        NORMAL,
        INFO,
        DEBUG,
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

    public class AppLoggerB
    {
        public LogLevel LoggerLogLevel { get; set; } = LogLevel.INFO;
        public string FilePath { get; set; } = "";
        public string LogFileTimeFormat { get; set; } = "_yyyyMMdd_HHmmss";
        public OutputMode LogOutPutMode { get; set; } = OutputMode.DEBUG_WINDOW;
        public bool AddTime { get; set; } = true;

        public AppLoggerB() { }

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
    }
}