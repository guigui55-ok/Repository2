using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ErrorUtility.LogUtility
{
    public class LogData
    {
        public int Priority;
        public int LogType;
        public string Value = "";
        public string Notes = "";
        public string MessageToUser = "";
        public Exception ex = null;
        public DateTime DateTime = DateTime.Now;
        public bool IsAccessed = false;
        public int ThreadId;
    }
    public class LogManager
    {
        public int LogMode = 1;
        public string LogFilePath = "";
        public List<LogData> LogList;
        public LogUtility.Constants Constants = new LogUtility.Constants();
        public int LimitIndex = 2000;
        protected ErrorManager _err;
        public bool IsWritingWhenIndexLimit = false;

        public LogManager(ErrorManager err, int mode,string filePath)
        {
            LogList = new List<LogData>();
            LogMode = mode;
            LogFilePath = filePath;
            _err = err;
        }

        public bool HasAlert()
        {
            return HasLogPriority(Constants.PRIORITY_ALERT);
        }

        public bool HasWarning()
        {
            return HasLogPriority(Constants.PRIORITY_WARNING);
        }

        public bool HasAlertOrWarning()
        {
            bool hasAlert = HasLogPriority(Constants.PRIORITY_ALERT);
            bool hasWarning = HasLogPriority(Constants.PRIORITY_WARNING);
            return hasAlert || hasWarning;
        }

        public bool HasLogPriority(int priority)
        {
            try
            {
                if (LogList == null) { return false; }
                if (LogList.Count < 1) { return false; }
                List<string> retList = new List<string>();
                foreach (LogData data in LogList)
                {
                    if (data.Priority == priority)
                    {
                        return true;
                    }
                }
                return false;
            } catch (Exception ex)
            {
                Debug.WriteLine("** Log.HasLogPriority Exception **");
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }


        public string[] GetAlertLogs(bool IsLastOnly = false)
        {
            try
            {
                if(LogList == null) { return null; }
                if(LogList.Count < 1) { return null; }
                List<string> retList = new List<string>();
                foreach(LogData data in LogList)
                {
                    if (IsLastOnly)
                    {
                        // last Only
                        if (!data.IsAccessed)
                        {
                            if (data.Priority == Constants.PRIORITY_ALERT)
                            {
                                retList.Add(GetMessageFromLogData(data));
                                data.IsAccessed = true;
                            }
                        }
                    } else
                    {
                        // all
                        if (data.Priority == Constants.PRIORITY_ALERT)
                        {
                            retList.Add(GetMessageFromLogData(data));
                        }
                    }
                }
                return retList.ToArray();
            } catch (Exception ex)
            {
                Debug.WriteLine("** Log.GetAlerts Exception **");
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return null;
            }
        }

        private string GetMessageFromLogData(LogData data)
        {
            try
            {
                if(data == null) { return ""; }
                if((data.MessageToUser == "")||(data.MessageToUser == null))
                {
                    if ((data.Value == "") || (data.Value == null))
                    {
                        if(data.ex == null)
                        {
                            return "Log Nothing";
                        } else
                        {
                            return data.ex.Message;
                        }
                    }
                    else
                    {
                        return data.Value;
                    }
                } else
                {
                    return data.MessageToUser;
                }
            } catch (Exception ex)
            {
                Debug.WriteLine("** Log.GetMessageFromLogData Exception **");
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return "";
            }
        }

        public void AddWithMessageToUser(string logValue,string message,string notes = "")
        {
            this.Add(Constants.PRIORITY_NORMAL, Constants.TYPE_LOG, logValue, message, notes);
        }

        public void Add(string logValue)
        {
            this.Add(Constants.PRIORITY_NORMAL, Constants.TYPE_LOG, logValue, "", "");
        }

        public void Add(object argObject,string logValue)
        {
            if(argObject != null)
            {
                string buf = argObject.GetType().ToString() + "." + logValue;
                this.Add(Constants.PRIORITY_NORMAL, Constants.TYPE_LOG, buf, "", "");
            }
        }
        //public void Add<Type>(Type classType,string value, string notes)
        //{
        //    if (classType != null)
        //    {
        //        value = classType.GetType() + "." + value;
        //    }
        //    this.Add(Constants.PRIORITY_NORMAL, Constants.TYPE_NONE, value,"", notes);
        //}

        public void Add(Type classType,int priority, int logType, string value, string notes, Exception exception = null)
        {
            if (classType != null)
            {
                value = classType.GetType() + "." + value;
            }            
            Add(priority, logType, value, "" ,notes, exception);
        }

        public void AddCaution(string logValue, string messageToUser, string notes, Exception exception = null)
        {
            int logtype;
            if (exception == null) { logtype = Constants.TYPE_LOG; }else { logtype = Constants.TYPE_EXCEPTION; }
            Add(Constants.PRIORITY_CAUTION, logtype, logValue, messageToUser, notes, exception);
        }
        public void AddWarning(string logValue, string messageToUser, string notes, Exception exception = null)
        {
            int logtype;
            if (exception == null) { logtype = Constants.TYPE_LOG; }
            else { logtype = Constants.TYPE_EXCEPTION; }
            Add(Constants.PRIORITY_WARNING, logtype, logValue, messageToUser, notes, exception);
        }

        public void AddAlert(string logValue, string messageToUser, string notes, Exception exception = null)
        {
            int logtype;
            if (exception == null) { logtype = Constants.TYPE_LOG; }
            else { logtype = Constants.TYPE_EXCEPTION; }
            Add(Constants.PRIORITY_ALERT, logtype, logValue, messageToUser, notes, exception);
        }
        public void AddAlert(string logValue, string messageToUser,  Exception exception = null)
        {
            int logtype;
            if (exception == null) { logtype = Constants.TYPE_LOG; }
            else { logtype = Constants.TYPE_EXCEPTION; }
            Add(Constants.PRIORITY_ALERT, logtype, logValue, messageToUser, "", exception);
        }

        /// <summary>
        ///  Log.Add Main Function
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="logType"></param>
        /// <param name="value"></param>
        /// <param name="messateToUser"></param>
        /// <param name="notes"></param>
        /// <param name="exception"></param>
        public void Add(int priority,int logType,string value,string messateToUser,string notes,Exception exception = null)
        {
            try
            {
                if (IsWritingWhenIndexLimit)
                {
                    Console.WriteLine(this.ToString()+ ".Add IsWritingWhenIndexLimit=true");
                    return;
                }
                
                if (LogMode == 1)
                {
                    Console.Write("AddLog:");
                    Console.Write(GetPriorityToString(priority));
                    Console.Write(" " + GetTypeToString(logType) + " ");
                    Console.Write("[" + Thread.CurrentThread.ManagedThreadId + "]");
                    
                    Console.Write("  " + value);
                    if (exception != null)
                    {
                        if (exception.StackTrace != null)
                        {
                            if (notes == null) { notes = ""; }
                            notes += "\n" + exception.Message + "\n" + exception.StackTrace;
                        }
                    }
                    if ((notes != null)&&(notes != ""))
                    {
                        Console.WriteLine(" , "+notes);
                    } else { Console.WriteLine(""); }
                    if ((messateToUser != null) && (messateToUser != ""))
                    {
                        Console.WriteLine("messageToUser = " + messateToUser);
                    }
                    if (exception != null)
                    {
                        Console.WriteLine(exception);
                        if (exception.StackTrace != null)
                        {
                            Console.WriteLine(exception.StackTrace);
                        }
                    }
                }
                if ((value == "")) { return; }
                LogData nowLog = new LogData
                {
                    Priority = priority,
                    LogType = logType,
                    Value = value,
                    Notes = notes,
                    MessageToUser = messateToUser,
                    ThreadId = Thread.CurrentThread.ManagedThreadId
                };
                LogList.Add(nowLog);

                // LogList の Index が上限に達したときはログに書き込み、ログをすべて消去する
                WriteLogWhenIndexIsLimit();
            } catch (Exception ex)
            {
                Console.WriteLine("** Log.Add Exception **");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void WriteLogWhenIndexIsLimit()
        {
            try
            {
                IsWritingWhenIndexLimit = true;
                if(LogFilePath.Length < 4) { return; }
                // path がないとき永遠とログがで続けるのでここでチェックする
                if (!Directory.Exists(Directory.GetDirectoryRoot(LogFilePath)))
                {
                    //Debug.WriteLine("WriteLogWhenIndexIsLimit LogFilePath is Not Exists");
                    return;
                }
                if (LogList == null) { throw new Exception("LogList is Null"); }
                if (LogList.Count > this.LimitIndex)
                {
                    Debug.WriteLine(this.ToString() + "WriteLogWhenIndexIsLimit");
                    Console.WriteLine(this.ToString() + "WriteLogWhenIndexIsLimit");
                    WriteLog();
                    RemoveAll();
                }
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString()+ ".WriteLogWhenIndexIsLimit");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Source);
            }finally
            {
                IsWritingWhenIndexLimit = false;
            }
        }

        public void RemoveAll()
        {
            try
            {
                Console.WriteLine(this.ToString() + ".RemoveAll");
                Debug.WriteLine(this.ToString()+ ".RemoveAll");
                if (LogList == null) { throw new Exception("LogList is Null"); }
                if (LogList.Count < 0) { throw new Exception("LogList.Count < 0"); }
                
                while(LogList.Count > 1)
                {
                    LogList.RemoveAt(0);
                }                
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".RemoveAll");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Source);
            }
        }

        public bool WriteLog()
        {
            return  this.WriteLog(ref this.LogFilePath, this.GetLogDataListAtString());
        }
        public  bool WriteLog(ref string path, string log)
        {
            try
            {
                if(path.Length < 4) { return false; }
                if (Directory.Exists(Directory.GetDirectoryRoot(path)))
                {
                    string writePath = new ErrorUtility.FileUtility(_err).MakeNewFileNameNotLock(
                        System.IO.Path.GetFileName(path), System.IO.Path.GetDirectoryName(path));
                    if (!writePath.Equals(path))
                    {
                        // 一度パスを書き換えたらそのままにする
                        Console.WriteLine("** ref FilePath Change pat=" + path);
                        path = System.IO.Path.GetDirectoryName(path) + "\\" +  writePath;
                    }

                    using (var writer = new System.IO.StreamWriter(path, true))
                    {                        
                        writer.WriteLine(log);
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("Directory Not Found [" + path + "]");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".WriteLog");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Source);
                return false;
            }
        }

        public bool IsLock(string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return false;
        }

        public string GetLogDataListAtString()
        {
            try
            {
                string ret = "";
                if (LogList == null) { throw new Exception("LogDataList Is Null"); }
                if(LogList.Count > 0)
                {
                    foreach(LogData value in this.LogList)
                    {
                        ret += ConvertLogDataToString(value) + "\n";
                    }
                }
                return ret;
            } catch (Exception ex)
            {
                Console.WriteLine("** Log.GetLogDataListAtString Exception **");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return "** Log.GetLogDataListAtString Exception **";
            }
        }
        private string ConvertLogDataToString(LogData log)
        {
            try
            {
                string buf;
                buf = log.DateTime.ToString();
                buf += " " + GetPriorityToString(log.Priority);
                buf += GetTypeToString(log.LogType);
                buf += "[" + log.ThreadId + "]";
                buf += " " + log.Value;
                if (log.MessageToUser != "")
                {
                    buf += "[" + log.MessageToUser + "]";
                }
                if (log.Notes != "")
                {
                    buf += "(" + log.Notes + ")";
                }
                return buf;
            } catch (Exception ex)
            {
                Console.WriteLine("** Log.ConvertLogDataToString Exception **");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return "** Log.ConvertLogDataToString Exception **";
            }
        }
        private string GetPriorityToString(int priority)
        {
            try
            {
                switch (priority)
                {
                    case 1: return "[ALERT__]";
                    case 2: return "[WARNING]";
                    case 3: return "[CAUTION]";
                    case 4: return "[NORMAL_]";
                    case 5: return "[NONE___]";
                    default: return "[ETC____]";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("** Log.GetPriorityToString Exception **");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return "";
            }
        }
        /// <summary>
        /// Log,Exception,DebugLog,ReleaseLog,Etc
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetTypeToString(int type)
        {
            try
            {
                switch (type)
                {
                    case 1: return "[LOG]";
                    case 2: return "[EXP]";
                    default:return "[ETC__]";
                }
            } catch (Exception ex)
            {
                Console.WriteLine("** Log.GetTypeToString Exception **");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return "";
            }
        }
    }
    public class Constants
    {
        public readonly int TYPE_LOG = 1;
        public readonly int TYPE_EXCEPTION = 2;
        public readonly int PRIORITY_ALERT = 1;
        public readonly int PRIORITY_WARNING = 2;
        public readonly int PRIORITY_CAUTION = 3;
        public readonly int PRIORITY_NORMAL = 4;
        public readonly int PRIORITY_NONE = 5;
    }
}
