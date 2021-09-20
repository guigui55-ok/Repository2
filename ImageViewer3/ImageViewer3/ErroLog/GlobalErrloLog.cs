using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorLog;

 namespace ErrorLog
{
    // エラーログ実行クラス
    static public class GlobalErrloLog
    {
        static public IErrorLog ErrorLog;

        static public void NewErrorLog()
        {
            ErrorLog = new ErrorLog();
        }

        static public int SetErrorLog(object errorLog)
        {
            try
            {
                if (errorLog.GetType().Equals(typeof(IErrorLog )))
                {
                    ErrorLog = (IErrorLog)errorLog;
                }
                else
                {
                    return -1;
                }
                return 1;
            }
            catch (Exception ex)
            {
                ErrorLog.AddException(ex, "setErrorLog");
                return 0;
            }
        }
    }
}
