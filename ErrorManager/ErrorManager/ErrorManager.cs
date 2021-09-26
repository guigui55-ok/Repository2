using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ErrorManager
{
    public class ErrorManager
    {
        public int DebugMode = 1;
        public string ErrorMessageToUser = "";
        public Exception LastException = null;
        public string LastExceptionMessage = "";
        public string LastErrorMessageForDebug = "";
        public string LastErrorMessageToUser = "";

        protected string ErrorLogFilePath = "";
        public Constants Constants = new Constants();
        public Log.LogConstants LogConstants;
        public IErrorMessenger Messenger;
        public bool IsSuppressErrorShow = false;

        public List<Type> TypeList = new List<Type>();

        protected List<Exception> _ExList = new List<Exception>();
        protected List<DateTime> _DateTimeList = new List<DateTime>();
        protected List<String> _MsgList = new List<string>();

        protected DebugData _LastDebugData;
        protected List<DebugData> _DebugDataList = new List<DebugData>();
        protected Log.LogManager Log;
        public int SetLogIndexLimit { get { return Log.LimitIndex; } set { Log.LimitIndex = value; } }
        public bool hasAlert
        {
            get { return this.HasAlert(); }
        }
        public bool hasAboveWarning
        {
            get { return this.HasAboveWorning(); }
        }
        public bool hasError
        {
            get { return this.HasError(); }
        }


        /// <summary>
        ///  コンストラクタ。引数は 1 ならエラー表示時に Debug.Writeline で出力する。
        /// </summary>
        public ErrorManager(int debugMode)
        {
            DebugMode = debugMode;
            Log = new Log.LogManager(this, debugMode, "");
            LogConstants = Log.Constants;
        }

        public ErrorManager(int debugMode, string logDirectory, string errorLogFileName, string logFileName)
        {
            DebugMode = debugMode;
            if (!System.IO.Directory.Exists(logDirectory))
            {
                AddException(new Exception("Directory Not Exists [" + logDirectory + "]"), "ErrorManager Constracta Failed");
            }
            Log = new Log.LogManager(this, debugMode, logDirectory + "\\" + logFileName);
            LogConstants = Log.Constants;

            ErrorLogFilePath = logDirectory + "\\" + errorLogFileName;
        }

        public void AddType(Type type)
        {
            if (type != null)
            {
                TypeList.Add(type);
            }
        }
        public void AddType(object argObject)
        {
            if (argObject != null)
            {
                TypeList.Add(argObject.GetType());
            }
        }
        public int GetTypeNumber(object argObject)
        {
            if (argObject != null)
            {
                return GetTypeNumber(argObject.GetType());
            }
            return 0;
        }
        public int GetTypeNumber(Type type)
        {
            if (TypeList.Count < 1) { return 0; }
            int count = 0;
            foreach(Type value in TypeList)
            {
                if (type.Equals(value))
                {
                    return count;
                }
                count++;
            }
            return count;
        }

        public string GetErrorLogPath{ get{ return this.ErrorLogFilePath; } }

        /// <summary>
        /// Log.AddWithMessageToUser
        /// </summary>
        /// <param name="value"></param>
        /// <param name="messageToUser"></param>
        /// <param name="notes"></param>
        public void AddLog(string value,string messageToUser ,string notes = "")
        {
            Log.AddWithMessageToUser(value, messageToUser, notes);
        }

        public void AddLog(object argObject, string value)
        {
            Log.Add(argObject, value);
        }

        /// <summary>
        /// Log.Add not MessageToUser
        /// </summary>
        /// <param name="value"></param>
        /// <param name="notes"></param>
        public void AddLog(string value,string notes = "")
        {
            Log.Add(value);
            //
            //this.Add(Constants.PRIORITY_NORMAL, Constants.TYPE_LOG, logValue, "", "");
        }
        //public void AddLog<Type>(T classType, string value, string notes = "")
        //{
        //    Log.Add(classType, value, notes);
        //}


        /// <summary>
        /// LogList に AlertType の Log を記録する (hasError=true にはならない)
        /// </summary>
        /// <param name="logValue"></param>
        /// <param name="messageToUser"></param>
        /// <param name="exception"></param>
        public void AddLogAlert(Exception exception)
        {
            string buf = "";
            if (exception != null)
            {
                buf = exception.Message;
            }
            Log.AddAlert(buf, "","",exception);
        }

        /// <summary>
        /// LogList に AlertType の Log を記録する (hasError=true にはならない)
        /// </summary>
        /// <param name="logValue"></param>
        /// <param name="messageToUser"></param>
        /// <param name="exception"></param>
        public void AddLogAlert(string logValue,string messageToUser = "",Exception exception = null)
        {
            Log.AddAlert(logValue, messageToUser, "",exception);
        }


        public void AddLogAlert(object argObject,string logValue, string messageToUser = "", Exception exception = null)
        {
            string buf = "";
            if (argObject != null) { buf = argObject.GetType().ToString() + "." + logValue; }
            Log.AddAlert(buf, messageToUser, "", exception);
        }


        public void AddLogCaution(string logValue, string messageToUser = "", Exception exception = null)
        {
            Log.AddCaution(logValue, messageToUser, "", exception);
        }

        /// <summary>
        /// LogList に WarningType の Log を記録する (hasError=true にはならない)
        /// </summary>
        /// <param name="logValue"></param>
        /// <param name="messageToUser"></param>
        /// <param name="exception"></param>
        public void AddLogWarning(string logValue, string messageToUser="", Exception exception = null)
        {
            Log.AddWarning(logValue, messageToUser, "", exception);
        }

        public void AddLogWarning(object argObject, string logValue, string messageToUser = "", Exception exception = null)
        {
            string buf = "";
            if (argObject != null) { buf = argObject.GetType().ToString() + "." + logValue; }
            Log.AddWarning(buf, messageToUser, "", exception);
        }


        public void  AddLog(int priority,int logType,string value,string messageToUser,Exception exception = null)
        {
            Log.Add(priority, logType, value, messageToUser, "", exception); 
        }

        public void AddLog(int priority,int logType,string value,Exception exception = null)
        {
            Log.Add(priority, logType, value,"", "", exception);
        }

        //public void AddLog(object classType, int priority, int logType, string value, string notes, Exception exception = null)
        //{
        //    Log.Add(classType, priority, logType, value, notes, exception);
        //}


        public string GetLogDataListAtString()
        {
            return Log.GetLogDataListAtString();
        }

        public string GetLogPath {get{ return Log.LogFilePath; } }

        /// <summary>
        ///  最後に発生した例外の Message を取得する
        /// </summary>
        public string GetExceptionMessage() { 
            if (LastException == null) { return ""; }
            string ret = LastException.Message;
            LastException = null;
            return ret;
        }

        public string GetExceptionMessage(int typeUpper,bool exMsg,bool exStack,bool uMsg,bool time)
        {
            try
            {
                if (_DebugDataList == null) { return ""; }
                if (_DebugDataList.Count < 1) { return ""; }

                string buf = "";
                bool flag = false;
                foreach (DebugData data in _DebugDataList)
                {
                    flag = false;
                    if (!data.IsAccecced)
                    {
                        if (typeUpper == 0)
                        {
                            // すべての Tyep を取得する
                            flag = true;
                        } else if (typeUpper >= data.DataType)
                        {
                            // typeupper 以上のものを取得する
                            flag = true;
                        } else
                        {

                        }
                    }
                    if (flag)
                    {
                        if (time) { buf += data.DateTime.ToString(); }
                        buf += " " + data.GetErrorTypeName(data.DataType) + "\n";
                        if (uMsg) { buf += data.MessageForUser + "\n"; }
                        if (exMsg) { buf += data.MessageForDebug + "\n"; }
                        if (exStack) { buf += data.Exception.StackTrace + "\n"; }
                        data.IsAccecced = true;
                    }
                }
                return buf;
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".AddDebugData");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return "";
            }
        }

        /// <summary>
        ///  最後に発生した例外の Message と StacTrace を取得する
        /// </summary>
        public string GetExceptionMessageAndStackTrace()
        {
            if (LastException == null) { return ""; }
            string buf = LastException.Message + "\n--- StacTrace ---\n" + LastException.StackTrace;
            LastException = null;
            return buf;
        }

        public void ReleaseErrorState()
        {
            try
            {
                if(_DebugDataList == null) { return; }
                if(_DebugDataList.Count < 1) { return; }
                int count = 0;
                foreach(DebugData data in _DebugDataList)
                {
                    if (!data.IsAccecced) { data.IsAccecced = true;  count++; }                    
                }
                Log.Add(Log.Constants.PRIORITY_CAUTION, Log.Constants.TYPE_LOG, "ReleaseErrorState : count="+count, "", "", null);
            } catch (Exception ex)
            {
                Log.AddAlert(this.ToString() + ".ReleaseErrorState", "", ex);
            }
        }

        /// <summary>
        ///  最後に例外が発生したときの LastErrorMessage を取得する
        ///  User 向けが空なら Debug用 を取得、それが空なら Exception｡Message を取得する、それが空なら文字列 "UNEXPECTED ERROR!" を返す
        /// </summary>
        public string GetErrorMessage() { 
            return GetErrorMessgefromDebugDataList(_LastDebugData);
        }

        public string GetErrorMessageToUser() {
            try
            {
                ClearError();
                return LastErrorMessageToUser;
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".GetErrorMessageToUser");
                Log.AddAlert(this.ToString() + ".GetErrorMessageToUser", "", "", ex);
                return "";
            }
        }

        public string GetLastAlertMessages(bool orderIsRev = false, bool isAddExceptionMessage = false)
        {
            try
            {
                string ret="";
                string[] msgs = GetLastErrorMessagesAsArray(4, orderIsRev, isAddExceptionMessage);
                if((msgs != null)&&(msgs.Length > 0))
                {
                    foreach(string val in msgs)
                    {
                        ret += val + "\n";
                    }
                    ret = ret.Substring(0, ret.Length - 1);
                    return ret;
                }
                return ret;
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".GetLastAlertMessages");
                Log.AddAlert(this.ToString() + ".GetLastAlertMessages", "", "", ex);
                return "";
            }
        }

        public bool HasError()
        {
            if(_DebugDataList == null){ return false; }
            if (_DebugDataList.Count > 0) { return true; }
            else { return false; }
        }
        public bool HasAlert(){
            bool errorHasAlert = HasErrorType(Constants.TYPE_ALERT);
            return errorHasAlert;
        }

        public bool HasAboveWorning()
        {
            bool hasAlert = HasAlert();
            bool errorHasWarning = HasErrorType(Constants.TYPE_WARNING);
            return hasAlert || errorHasWarning;
        }
        //public bool HasWarningAndAlert() { 
        //    bool hasAlert = HasErrorType(Constants.TYPE_ALERT); 
        //}

        private bool HasErrorType(int type)
        {
            try
            {
                if (_DebugDataList == null) {  return false; }
                if (_DebugDataList.Count < 1) { return false; }
                foreach (DebugData data in _DebugDataList)
                {
                    if (!data.IsAccecced)
                    {
                        if (data.DataType == type)
                        {
                            return true;
                        }
                    }
                }
                return false;
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".HasErrorType");
                Log.AddAlert(this.ToString() + ".HasErrorType", "","",ex);
                return false;
            }
        }

        /// <summary>
        ///  最後に例外が発生したときの LastErrorMessage と 例外の Message を取得する
        /// </summary>
        //public string GetMessages() { 
        //    return GetErrorMessage() + "," + GetExceptionMessage(); 
        //}

        public string GetMesseges()
        {
            try
            {
                string[] ary1 = this.LastErrorMessageForDebug.Split('\n');
                string[] ary2 = this.LastExceptionMessage.Split('\n');
                string ret = "";
                for(int i = 0; i<ary1.Length; i++)
                {
                    ret += ary2[i] + "." + ary1[i] + "\n"; 
                }
                return ret;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        //public DebugData GetLastException()
        //{
        //    try
        //    {
        //        return null
        //    } catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        return null;
        //    }
        //}

        private string GetErrorMessgefromDebugDataList(DebugData data)
        {
            string ret = "";
            try
            {
                // ユーザに表示するデータを取得
                if ((data.MessageForUser == "") || (data.MessageForUser == null))
                {
                    // ユーザー向けメッセージがなければ、Debug用メッセージを取得
                    if ((data.MessageForDebug == "") || (data.MessageForDebug == null))
                    {
                        // DebugMessageもなければ、例外メッセージを取得する
                        if (data.Exception == null)
                        {
                            // これもなければ不明なエラーとする
                            ret = "UNEXPECTED ERROR!";
                        }
                        else
                        {
                            ret = data.Exception.Message;
                        }
                    }
                    else
                    {
                        ret = data.MessageForDebug;
                    }
                }
                else
                {
                    ret = data.MessageForUser;
                }
                return ret;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moreThanThisType"></param>
        /// <param name="orderIsRev"></param>
        /// <param name="isAddExceptionMessage"></param>
        /// <returns></returns>
        public string GetLastErrorMessagesAsString(
            int moreThanThisType = 3,bool orderIsRev = false ,bool isAddExceptionMessage = false)
        {
            try
            {
                string[] msgs = GetLastErrorMessagesAsArray(moreThanThisType, orderIsRev, isAddExceptionMessage);
                string ret = "";
                if((msgs != null)||(msgs.Length < 1))
                {
                    foreach(string val in msgs)
                    {
                        ret += val + "\n";
                    }
                    ret = ret.Substring(0, ret.Length - 1);
                }
                return ret;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// moreThanThisType 以上の ErrorLogType の値の Message を取得する、Type 規定値は 4 Alert のみ。
        /// </summary>
        /// <param name="moreThanThisType"></param>
        /// <returns></returns>
        public string[] GetLastErrorMessagesAsArray(int moreThanThisType = 4, bool orderIsRev = false, bool isAddExceptionMessage = false)
        {
            try
            {
                
                List<string> list = GetLastErrorMessagesList(new int[] { moreThanThisType },orderIsRev, isAddExceptionMessage);
                ClearError();
                return list.ToArray();
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                this.AddLog(Log.Constants.PRIORITY_ALERT,Log.Constants.TYPE_EXCEPTION,
                    this.ToString()+ ".GetLastErrorMessages","ErrorLog Process Error",ex);
                return new string[] { };
            }
        }

        /// <summary>
        /// ErrorLogType が Warning 以上 (Warning と Alert) の Message を string[] で取得する
        /// </summary>
        /// <returns></returns>
        public string[] GetLastErrorMessageAboveWarning()
        {
            List<string> list = GetLastErrorMessagesList(new int[] { Constants.TYPE_WARNING, Constants.TYPE_ALERT });
            ClearError();
            return list.ToArray();
        }


        public List<string> GetLastErrorMessagesList(int[] errorTypes, bool orderRev = true, bool isAddExceptionMessage=false)
        {
            List<string> retList = new List<string>();
            try
            {
                if (orderRev) { }
                if (_DebugDataList == null) { Debug.WriteLine("GetLastErrorMessages DebugDtaList Is Null"); return retList; }
                if (_DebugDataList.Count < 1) { Debug.WriteLine("GetLastErrorMessages DebugDtaList.Count < 1"); return retList; }
                if (errorTypes == null) { Debug.WriteLine("GetLastErrorMessages errorTypes Is Null"); return retList; }
                if (errorTypes.Length < 1) { Debug.WriteLine("GetLastErrorMessages errorTypes.length < 1"); return retList; }

                foreach (DebugData data in _DebugDataList)
                {

                    GetDataForDebugList(data, ref retList, errorTypes, isAddExceptionMessage);
                }
                //ClearError();
                return retList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                this.AddLog(Log.Constants.PRIORITY_ALERT, Log.Constants.TYPE_EXCEPTION,
                    this.ToString() + ".GetLastErrorMessages", "ErrorLog Process Error", ex);
                return retList;
            }
        }

        private List<string> GetLastErrorMessagesListOrderRev(int[] targetErrorTypes, bool orderRev = true, bool isAddExceptionMessage = false)
        {
            List<string> retList = new List<string>();
            try
            {
                if (_DebugDataList == null) { Debug.WriteLine("GetLastErrorMessages DebugDtaList Is Null"); return retList; }
                if (_DebugDataList.Count < 1) { Debug.WriteLine("GetLastErrorMessages DebugDtaList.Count < 1"); return retList; }
                if (targetErrorTypes == null) { Debug.WriteLine("GetLastErrorMessages errorTypes Is Null"); return retList; }
                if (targetErrorTypes.Length < 1) { Debug.WriteLine("GetLastErrorMessages errorTypes.length < 1"); return retList; }

                for(int i= _DebugDataList.Count-1; i>=0; i--)
                {
                    GetDataForDebugList(_DebugDataList[i],ref retList, targetErrorTypes, isAddExceptionMessage);
                }
                return retList;
            } catch (Exception ex)
            {
                this.AddLog(Log.Constants.PRIORITY_ALERT, Log.Constants.TYPE_EXCEPTION,
                    this.ToString() + ".GetLastErrorMessagesListOrderRev", "ErrorLog Process Error", ex);
                return retList;
            }
        }

        private void GetDataForDebugList(
            DebugData data,ref List<string> list, int[] targetErrorTypes,bool isAddExceptionMessage)
        {
            try
            {
                if (!data.IsAccecced)
                {
                    foreach (int type in targetErrorTypes)
                    {
                        //Console.WriteLine("type ="+type + " ,data.type="+data.DataType);
                        if (type <= data.DataType)
                        {
                            string buf = GetErrorMessgefromDebugDataList(data);
                            if (isAddExceptionMessage)
                            {
                                if (data.Exception != null)
                                {
                                    buf += ":" + data.Exception.Message;
                                }
                            }
                            list.Add(buf);
                        }
                    }
                    data.IsAccecced = true;
                }
                // IsAccessed=true の時は何もしない
            }
            catch (Exception ex)
            {
                this.AddLog(Log.Constants.PRIORITY_ALERT, Log.Constants.TYPE_EXCEPTION,
                    this.ToString() + ".GetDataForDebugList", "ErrorLog Process Error", ex);
            }
        }

        //private void AddDebugData(DebugData data)
        //{
        //    try
        //    {
        //        this._DebugDataList.Add(data);
        //    } catch ( Exception ex)
        //    {
        //        Debug.WriteLine(this.ToString() +".AddDebugData");
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine(ex.StackTrace);
        //        return;
        //    }
        //}

        public string GetUserMessageOnlyAsString(bool orderRev = true,bool isAddExceptionMessage = false)
        {
            try
            {
                List<string> list = GetUserMessageOnly(orderRev,isAddExceptionMessage);
                if (list.Count < 1) { return ""; }
                string ret="";
                foreach(string val in list)
                {
                    ret += val + "\n";
                }
                if (ret.Length > 0) { ret = ret.Substring(0, ret.Length - 1); }
                return ret;
            } catch(Exception ex)
            {
                Log.AddAlert(this.ToString() + ".GetUserMessageOnlyAsString", "", ex);
                return "";
            }
        }

        public List<string> GetUserMessageOnly(bool orderRev = true,bool isAddExceptionMessage = false)
        {
            List<string> retList = new List<string>();
            try
            {
                if (_DebugDataList == null) { return retList; }
                if (_DebugDataList.Count < 1) { return retList; }
                DebugData data=null;
                if (orderRev)
                {
                    for (int i = _DebugDataList.Count-1; i >= 0; i--)
                    {
                        data = _DebugDataList[i];
                        if (!data.IsAccecced)
                        {
                            if (data.MessageForUser != "")
                            {
                                string buf = data.MessageForUser;
                                if (data.Exception != null)
                                {
                                    buf += data.Exception.Message;
                                }
                                retList.Add(buf);
                            }
                            data.IsAccecced = true;
                        }
                    }
                } else
                {
                    for (int i = 0; i < _DebugDataList.Count; i++)
                    {
                        data = _DebugDataList[i];
                        if (data.MessageForUser != "")
                        {
                            retList.Add(data.MessageForUser);
                        }
                    }
                }
                return retList;
            } catch (Exception ex)
            {
                Log.AddAlert(this.ToString()+ ".GetUserMessageOnly","",ex);
                return retList;
            }
        }

        public void AddUserMessage(string msgToUser)
        {
            AddException(null, "", msgToUser, Constants.TYPE_ALERT, 0);
        }
        public long GetLastErrorCode()
        {
            try
            {
                if (_DebugDataList == null) { return 0; }
                if (_DebugDataList.Count < 1) { return 0; }
                return _DebugDataList[_DebugDataList.Count - 1].ErrorCode;
            } catch (Exception ex)
            {
                Log.AddAlert("GetLastErrorCode Failed", "",ex);
                return 0;
            }
        }
        public void AddAlert(Exception exception, string msgForDebug, int errorCode=0)
        {
            AddException(exception, msgForDebug, "", Constants.TYPE_ALERT, errorCode);
        }
        public void AddException(Exception exception, string msg, int type)
        {
            AddException(exception, msg,"",type,0);
        }

        public void AddAlert(string msgForDebug,string msgToUser)
        {
            AddException(null, msgForDebug, msgToUser,Constants.TYPE_ALERT,0);
        }

        public void AddWarning(Exception exception, string msgForDebug, string msgToUser)
        {
            AddException(exception, msgForDebug, msgToUser, Constants.TYPE_WARNING,0);
        }

        // MainMethod
        public void AddException(Exception exceptoin,string msgForDebug, string msgToUser,int type,int errorCode)
        {
            try
            {
                //if (type >= Constants.TYPE_ERROR)
                //{
                //    // Alert,warning
                //    NowDebugData = new DebugData
                //    {
                //        MessageForDebug = msgForDebug,
                //        Exception = exceptoin,
                //        DateTime = DateTime.Now,
                //        DataType = type,
                //        MessageForUser = msgToUser
                //    };
                //    _DebugDataList.Add(NowDebugData);
                //    return;
                //}
                //if (!(exceptoin == null))
                // ex==null かどうか関係なく管理する
                {
                    _LastDebugData = new DebugData
                    {
                        MessageForDebug = msgForDebug,
                        Exception = exceptoin,
                        DateTime = DateTime.Now,
                        DataType = type,
                        MessageForUser = msgToUser,
                        ErrorCode = errorCode
                    };
                    _DebugDataList.Add(_LastDebugData);

                    // Log にも追加する
                    // ex == null でも AddException からの Log 追加は TYPE_EXCEPTION とする
                    switch (type)
                    {
                        case 1: // unexpected
                            AddLog(Log.Constants.PRIORITY_NORMAL, Log.Constants.TYPE_EXCEPTION, msgForDebug, msgToUser, exceptoin);
                            break;
                        case 2: // error
                            AddLog(Log.Constants.PRIORITY_CAUTION,Log.Constants.TYPE_EXCEPTION,msgForDebug, msgToUser, exceptoin);
                            break;
                        case 3: // warning
                            AddLogWarning(msgForDebug, msgToUser, exceptoin); break;
                        case 4: // alert
                            AddLogAlert(msgForDebug, msgToUser, exceptoin);break;
                        default:
                            break;
                    }

                    LastExceptionMessage += "\n" + exceptoin.Message;
                    LastException = exceptoin;
                    LastErrorMessageToUser = msgToUser;

                    if (DebugMode == 1)
                    {
                        ErrorLogWriteToConsole(_LastDebugData);
                    }
                }
                //else
                //{
                //    // exception == null
                //    NowDebugData = new DebugData
                //    {
                //        MessageForDebug = "-"
                //    };
                //    LastExceptionMessage += "\n" + "_";
                //    LastErrorMessageToUser = msgToUser;
                    
                //}

                if ((msgForDebug == "")||(msgForDebug == null)) {
                    msgForDebug = "_";
                }
                LastErrorMessageForDebug += "\n" + msgForDebug;
                //if (!hasError)
                //{
                //    LastErrorMessageForDebug = LastErrorMessageForDebug.Substring(1, LastErrorMessageForDebug.Length - 1);
                //    LastExceptionMessage = LastExceptionMessage.Substring(1, LastExceptionMessage.Length - 1);
                //}
                //hasError = true;
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".AddDebugData");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return;
            }
        }

        private void ErrorLogWriteToConsole(DebugData data)
        {
            Console.WriteLine("------");
            Console.WriteLine("**** AddException : " + data.MessageForDebug);
            Console.WriteLine(data.ErrorCode+ " / "+ data.MessageForUser);
            Console.WriteLine(data.Exception.Message);
            Console.WriteLine(data.Exception.StackTrace);
            Console.WriteLine("------");
        }

        /// <summary>
        /// ErrorType [Alert,Warning] 以下の Exception を追加する (hasAboveWarning にカウントされない)
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msgForDebug"></param>
        /// <param name="msgToUser"></param>
        public void AddExceptionNormal(Exception ex, string msgForDebug, string msgToUser = "")
        {
            AddException(ex, msgForDebug, msgToUser, Constants.TYPE_ERROR,0);
        }

        public void AddException(Exception ex)
        {
            //AddException(Exception exceptoin,string msgForDebug, string msgToUser,int type)
            AddException(ex,ex.Message,"",Constants.TYPE_ALERT,0);
        }

        public void AddException(Exception ex, string msg)
        {
            AddException(ex, msg, "");
        }

        public void AddExceptionWarning(Exception ex,string msgForDebug, string msgToUser)
        {
            AddException(ex, msgForDebug, msgToUser, Constants.TYPE_WARNING,0);
        }

        public void AddException(Exception ex, object argObject,string msgForDebug,string msgToUser="")
        {
            if (argObject != null)
            {
                AddException(ex,argObject.GetType().ToString()+"."+msgForDebug,msgToUser,Constants.TYPE_ALERT,0);
            }            
        }
        /// <summary>
        ///  例外情報を追記する
        /// </summary>
        public void AddException(Exception ex,string msg,string msgToUser)
        {
            this.AddException(ex, msg, msgToUser, Constants.TYPE_ALERT, 0);
            //if (!(ex == null))
            //{
            //    _LastDebugData = new DebugData
            //    {
            //        MessageForDebug = msg,
            //        Exception = ex,
            //        DateTime = DateTime.Now,
            //        DataType = this.Constants.TYPE_ALERT,
            //        MessageForUser = msgToUser
            //    };
            //    _DebugDataList.Add(_LastDebugData);

            //    Log.AddAlert(msg, msgToUser, ex);

            //    LastExceptionMessage += "\n" +ex.Message;
            //    LastException = ex;
            //    LastErrorMessageToUser = msgToUser;

            //    if (DebugMode == 1)
            //    {
            //        ErrorLogWriteToConsole(_LastDebugData);
            //    }
            //} else
            //{
            //    // 例外が Null
            //    LastExceptionMessage += "\n" + "_";
            //}
            //if ((msg == "")||(msg == null)) { msg = "_"; }
            //LastErrorMessageForDebug += "\n" + msg;
            //if (!hasError)
            //{
            //    LastErrorMessageForDebug = LastErrorMessageForDebug.Substring(1, LastErrorMessageForDebug.Length - 1);
            //    LastExceptionMessage = LastExceptionMessage.Substring(1, LastExceptionMessage.Length - 1);
            //}
        }

        /// <summary>
        ///  例外情報を保存する
        /// </summary>
        public void SaveException(Exception ex,string msg)
        {
            LastException = ex;
            AddException(ex, msg);
            if (DebugMode == 1)
            {
                Debug.WriteLine(msg);
                if (!(ex == null)) { Debug.WriteLine(ex.Message); }
            }
        }

        /// <summary>
        ///  例外が発生しているか
        /// </summary>
        public bool HasException()
        {
            if (!(LastException == null)) { return true; }
            return false;
        }

        /// <summary>
        ///  保持している例外情報をクリアする
        /// </summary>
        public void ClearError()
        {
            LastErrorMessageToUser = "";
            LastErrorMessageForDebug = "";
            LastException = null;
            ClearAllDebugDataList();
        }

        private void ClearAllDebugDataList()
        {
            try
            {
                if(_DebugDataList == null) { return; }
                if(_DebugDataList.Count < 1) { return; }
                int count = 0;
                while (_DebugDataList.Count > 0)
                {
                    _DebugDataList.RemoveAt(0);
                    count++;
                }
                Log.Add(this.ToString()+ ".ClearAllDebugDataList : ClearDataCount=" + count);
            } catch (Exception ex)
            {
                Log.AddAlert(this.ToString()+ ".ClearAllDebugDataList", "",ex);
            }
        }

        public bool SetLogFilePath(string FilePath)
        {
            try
            {
                ErrorLogFilePath = FilePath;
                if (System.IO.File.Exists(FilePath))
                {
                    return true;
                } else
                {
                    return false;
                }
            } catch (Exception ex)
            {
                this.AddException(ex, "SetLogFilePath Failed");
                return false;
            }
        }

        private string MakeLog()
        {
            string ret;
            try
            {
                if(_DebugDataList.Count < 1) { return ""; }

                ret = "---------------------\n";
                for(int i = 0; i< _DebugDataList.Count; i++)
                {
                    ret += _DebugDataList[i].DateTime.ToString() + " ";
                    ret += _DebugDataList[i].GetErrorTypeName(_DebugDataList[i].DataType) + " ";
                    ret += _DebugDataList[i].MessageForDebug + " ";
                    if (_DebugDataList[i].MessageForUser != "")
                    {
                        ret += " (" +_DebugDataList[i].MessageForUser + ")\n";
                    }
                    ret += _DebugDataList[i].Exception.StackTrace + "\n";
                }
                ret += "---------------------\n";
                return ret;
            } catch (Exception ex)
            {
                Debug.WriteLine("MakeLog Failed");
                this.AddException(ex, "MakeLog Failed");
                return null;
            }
        }

        public bool WriteLog(string path,string log)
        {
            try
            {
                if (Directory.Exists(Directory.GetDirectoryRoot(path)))
                {
                    using (var writer = new System.IO.StreamWriter(path, true))
                    {
                        writer.WriteLine(log);
                    }
                    return true;
                } else
                {
                    Console.WriteLine("Directory Not Found [" + path + "]");
                    return false;
                }
            } catch (Exception ex)
            {
                this.AddException(ex, "WriteLog Failed");
                return false;
            }
        }
        public bool WriteErrorLog()
        {
            //return WriteLog(this.ErrorLogFilePath,this.MakeLog());
            return Log.WriteLog(ref this.ErrorLogFilePath, this.MakeLog());
        }

        public bool WriteLog()
        {
            //return WriteLog(Log.LogFilePath,Log.GetLogDataListAtString());
            return Log.WriteLog();

        }
    }
}
