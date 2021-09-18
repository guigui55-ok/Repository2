using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtility
{
    class ExcuteAfterWaitForExit
    {
        protected ErrorManager.ErrorManager _err;
        public string Path = "";
        public string FileName = "";
        public System.Action ExcuteActionAfterWaitForExit;
        public EventHandler WaitForExitAfterEvent;
        public ExcuteAfterWaitForExit(ErrorManager.ErrorManager err)
        {
            _err = err;
        }

        public int AsyncWaitForClose(Application application, string workbookName)
        {
            Task task;
            try
            {
                task = Task.Run(() =>
                {
                    try
                    {
                        _err.AddLog(this, "AsyncWaitForClose Task.Run");
                        WaitForClose(application, workbookName);
                        if (ExcuteActionAfterWaitForExit != null) { ExcuteActionAfterWaitForExit(); }
                        WaitForExitAfterEvent?.Invoke(null, EventArgs.Empty);
                    } catch (Exception ex)
                    {
                        _err.AddException(ex,this, "AsyncWaitForClose Task.Run");
                    } finally
                    {
                        _err.AddLog(this, "AsyncWaitForClose Task.Run Finally");
                    }
                });

                return 1;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "WaitForClose");
                return 0;
            }
            finally
            {

            }
        }

        public int WaitForClose(in Application application, string workbookName)
        {
            try
            {
                _err.AddLog(this, "WaitForClose");
                if(application is null) { throw new Exception("Application is Null"); }
                while (application.Workbooks.Count > 0)
                {
                    if(application.Workbooks.Count < 1) {
                        _err.AddLog(this,"WaitForClose , application.Workbooks.Count < 1");
                        break; 
                    }
                }
                return 1;
            } catch (Exception ex)
            {
                _err.AddException(ex,this, "WaitForClose");
                return 0;
            } 
        }

        public int WaitForExit(string path)
        {
            try
            {
                _err.AddLog(this, "WaitForExit");
                if (!System.IO.File.Exists(path)) { _err.AddLog(" FileNotExists. path="+path); return -1; }
                System.Diagnostics.Process p =
                    System.Diagnostics.Process.Start(path);

                if (ExcuteActionAfterWaitForExit != null) { ExcuteActionAfterWaitForExit(); }
                WaitForExitAfterEvent?.Invoke(null, EventArgs.Empty);
                return 1;
            } catch (Exception ex)
            {
                _err.AddException(ex,this, "WaitForExit");
                return 0;
            } finally
            {
                _err.AddLog(this, "WaitForExit Finally");
            }
        }
    }
}
