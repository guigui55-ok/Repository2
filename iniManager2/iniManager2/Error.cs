using System;
using System.Diagnostics;

namespace IniManager
{
    class Error
    {
        public  int DebugMode = 1;
        public string ErrorMessageToUser = "";
        public Exception LastException = null;
        public string LastExceptionMessage = "";
        public string LastErrorMessage = "";

        /// <summary>
        ///  コンストラクタ。引数は 1 ならエラー表示時に Debug.Writeline で出力する。
        /// </summary>
        public Error(int debugMode)
        {
            DebugMode = debugMode;
        }

        /// <summary>
        ///  最後に発生した例外の Message を取得する
        /// </summary>
        public string GetExceptionMessage() { 
            if (LastException == null) { return ""; }
            return LastException.Message;  
        }

        /// <summary>
        ///  最後に例外が発生したときの LastErrorMessage を取得する
        /// </summary>
        public string GetErrorMessage() { return LastErrorMessage; }

        /// <summary>
        ///  最後に例外が発生したときの LastErrorMessage と 例外の Message を取得する
        /// </summary>
        public string GetMessages() { return GetErrorMessage() + "\n" + GetExceptionMessage(); }


        /// <summary>
        ///  例外情報を追記する
        /// </summary>
        public void AddException(Exception ex,string msg)
        {
            if (!(ex == null))
            {
                LastExceptionMessage += "\n" +ex.Message;
            }
            LastErrorMessage += "\n" + msg;
        }

        /// <summary>
        ///  例外情報を保存する
        /// </summary>
        public void SaveException(Exception ex,string msg)
        {
            LastException = ex;
            LastExceptionMessage = ex.Message;
            LastErrorMessage = msg;
            if (DebugMode == 1)
            {
                Debug.WriteLine(msg);
                Debug.WriteLine(ex.Message);
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
            LastErrorMessage = "";
            LastException = null;
        }
    }
}
