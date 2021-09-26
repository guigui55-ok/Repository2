
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtility
{
    public class ExcelWorkbookSyncer
    {
        protected ErrorManager.ErrorManager _err;
        protected ExcelManager _excelManager;
        protected bool IsBeforeClose = false;
        public bool IsWorkbookOpened = false;
        public bool IsWorkbookClosed = false;
        public bool IsSyncExcelWorkbook = false;
        ExcuteAfterWaitForExit _excuteAfterWaitForExit;
        protected Stopwatch sw;
        public ExcelWorkbookSyncer(ErrorManager.ErrorManager err,ExcelManager excelManager)
        {
            _err = err;
            _excelManager = excelManager;
            _excuteAfterWaitForExit = new ExcuteAfterWaitForExit(_err);
        }

        public int SetEvent(ref Application application)
        {
            try
            {
                _err.AddLog(this, "SetEvent");
                if(application == null) { _err.AddLogAlert(" application == null)");  return -1; }
                application.WorkbookOpen -= Application_WorkbookOpen;
                application.WorkbookOpen += Application_WorkbookOpen;
                application.WorkbookBeforeClose -= Application_WorkbookBeforeClose;
                application.WorkbookBeforeClose += Application_WorkbookBeforeClose;
                application.WindowActivate -= Application_WindowActivate;
                application.WindowActivate += Application_WindowActivate;
                application.WindowDeactivate -= Application_WindowDeactivate;
                application.WindowDeactivate += Application_WindowDeactivate;
                return 1;
            } catch (Exception ex)
            {
                _err.AddException(ex,this, "SetEvent");
                return 0;
            }
        }

        private void UpdateAfterClose()
        {
            try
            {
                _err.AddLog(this, "UpdateAfterClose");
                if (_excelManager.IsDoUpdateWhenCloseWorkbook)
                {
                    // ExcelApplication が終了してから、Update しなければ
                    // リストが変わらない、処理が停滞するため、非同期で終了を待ってから更新する
                    _excuteAfterWaitForExit.ExcuteActionAfterWaitForExit = _excelManager.UpdateOpendExcelApplication;
                    _excuteAfterWaitForExit.AsyncWaitForClose(
                        _excelManager.GetExcelAppsList()[0].Application,_excuteAfterWaitForExit.FileName);
                    //_excelManager.UpdateOpendExcelApplication();
                }
                //sw = new Stopwatch();
                //sw.Start();
                //while (true)
                //{
                //    if(sw.ElapsedMilliseconds >= 100)
                //    {
                //        _excelManager.UpdateOpendExcelApplication();
                //        break;
                //    }
                //}
            } catch (Exception ex)
            {
                _err.AddLogAlert(this, "Event UpdateAfterClose", "UpdateAfterClose Failed", ex);
            } finally
            {
                _err.AddLog(this, "UpdateAfterClose Finally");
            }
        }

        private void Application_WindowDeactivate(Workbook Wb, Window Wn)
        {
            try
            {
                _err.AddLog(this, "Application_WindowDeactivate");

                if(IsWorkbookClosed)
                {
                    IsWorkbookClosed = true;
                    // 最後の Workbook を閉じたときは ファイル名とパスを保存する する
                    if ((_excelManager.GetExcelAppsList().Count <= 1)
                        && (_excelManager.GetExcelAppsList()[0].GetWorkBooksCount() <= 1))
                    {
                        _err.AddLog("Application_WindowDeactivate , workbooks.Count<=1");
                        _excuteAfterWaitForExit.Path = Wb.Path + "\\" + Wb.Name;
                        _excuteAfterWaitForExit.FileName = Wb.Name;
                        _err.AddLog("   _excuteAfterWaitForExit.Path=" + _excuteAfterWaitForExit.Path);
                        UpdateAfterClose();

                        // Close 後の Activate で更新しているが、最後の時は Application イベントが補足できないので
                        // 別途ここで Update する
                    }
                    else
                    {
                        if (_excelManager.IsDoUpdateWhenCloseWorkbook)
                        {
                            _excelManager.UpdateOpendExcelApplication();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _err.AddLogAlert(this, "Event Application_WindowDeactivate", "Application_WindowDeactivate Failed", ex);
            }
            finally
            {
                _err.AddLog(this, "Application_WindowDeactivate Finally");
            }
        }

        private void Application_WindowActivate(Workbook Wb, Window Wn)
        {
            try
            {
                if (IsBeforeClose)
                {
                    // このブロックに入ってすぐにフラグを変更しないとループする
                    IsBeforeClose = false;
                    IsWorkbookClosed = true;
                    if (IsSyncExcelWorkbook)
                    {
                        _err.AddLog(this, "IsBeforeClose=true, IsSyncExcelWorkbook=true");
                        // Workbook を閉じた後に WorkbookList を更新する
                        _excelManager.UpdateOpendExcelApplication();
                    }

                }
            }catch (Exception ex)
            {
                _err.AddLogAlert(this, "Event Application_WindowActivate", "Application_WindowActivate Failed",ex);
            }
        }

        private void Application_WorkbookBeforeClose(Workbook Wb, ref bool Cancel)
        {
            IsBeforeClose = true;
        }

        private void Application_WorkbookOpen(Workbook Wb)
        {
            _err.AddLog(this, "Application_WorkbookOpen");
            IsWorkbookOpened = true;
            if (IsSyncExcelWorkbook)
            {
                _err.AddLog(this, "Application_WorkbookOpen , IsSyncExcelWorkbook=true");
                _excelManager.UpdateOpendExcelApplication();
            }
        }
    }
}
