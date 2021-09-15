using ExcelUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorkbookList
{
    public class ExcelWorkbookList
    {
        protected ErrorManager.ErrorManager _err;
        protected ExcelManager _excelManager;
        protected IWorkbookListControl _workbookListControl;
        public List<BookInfo> WorkbookList;

        public class BookInfo
        {
            public string FileName;
            public int ProcessId;
            public int Index;
            public string ControlValue;
        }
        public ExcelWorkbookList(ErrorManager.ErrorManager err, ExcelUtility.ExcelManager excelManager, IWorkbookListControl workbookListControl)
        {
            _err = err;
            _excelManager = excelManager;
            _workbookListControl = workbookListControl;
            _excelManager.UpdateExcelAppsListAfterEvent += ExcelManger_UpdateWorkbookListAfterEvent;
            WorkbookList = new List<BookInfo>();
        }

        public void ExcelManger_UpdateWorkbookListAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this, "ExcelManger_UpdateWorkbookListAfterEvent");
                SetWorkbookListFromExcelManager();
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ExcelManger_UpdateWorkbookListAfterEvent");
            }
        }

        public void SetWorkbookListToControlFromExcelManager()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "SetWorkbookListToControlFromExcelManager");
            }
        }

        public void SetWorkbookListFromExcelManager()
        {
            try
            {
                _err.AddLog(this, "SetWorkbookListFromExcelManager");
                if (!_excelManager.AppsListIsValid()) { _err.AddLogWarning("_excelManager.AppsListIsValid = false"); return; }
                List<ExcelApps> appsList = _excelManager.GetExcelAppsList();

                List<string> valueList = new List<string>();

                _err.AddLog("  _excelManager.ExcelAppsList.Count=" +_excelManager.GetExcelAppsList().Count);
                foreach (ExcelApps apps in appsList)
                {
                    List<string> bufList = GetListValueForExcelApps(apps);
                    valueList.AddRange(bufList);
                }

                _workbookListControl.SetWorkbookList(valueList);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "SetWorkbookListFromExcelManager");
            }
        }

        public AppsInfo GetAppsInfoFromWorkbookList(int index)
        {
            try
            {
                string value = _workbookListControl.GetItemValue(index);
                AppsInfo info = _workbookListControl.ConvertToAppsInfoFromItemValue(value);
                info.Index = index;
                return info;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetAppsInfoFromWorkbookList");
                return null;
            }
        }

        public List<string> GetListValueForExcelApps(ExcelApps apps)
        {
            List<string> retList = new List<string>();
            try
            {
                _err.AddLog(this, "GetListValueForExcelApps");
                if (apps.IsGhost) { 
                    _err.AddLog("  apps.IsGhost=true");
                    BookInfo info = new BookInfo();
                    info.ControlValue = "[" + apps.ProcessId + "] " + "EXCEL.EXE";
                    info.ProcessId = apps.ProcessId;
                    info.FileName = "";
                    info.Index = 0;
                    retList.Add(info.ControlValue);
                    this.WorkbookList.Add(info);
                    return retList; 
                } else
                {
                    _err.AddLog("  apps.IsGhost=false");
                    _err.AddLog("  apps.ApplicationIsNull=" + apps.ApplicationIsNull());
                }

                List<string> bookList = apps.GetWorkbookNameList();

                if(bookList.Count < 1) { _err.AddLogWarning(" bookList.Count<1"); return retList; }
                
                List<BookInfo> bookInfoList = new List<BookInfo>();
                for (int i = 0; i < bookList.Count; i++)
                {
                    // ControlValue
                    string buf = "[" + apps.ProcessId + "] " + bookList[i];
                    BookInfo info = new BookInfo();
                    info.ControlValue = buf;
                    info.ProcessId = apps.ProcessId;
                    info.FileName = bookList[i];
                    info.Index = i;
                    retList.Add(buf);
                    bookInfoList.Add(info);
                }
                this.WorkbookList = bookInfoList;
                return retList;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "GetListValueForExcelApps");
                return retList;
            }
        }

        public void CloseWorkbookSelectedItems()
        {
            try
            {
                _err.AddLog(this, "CloseWorkbookSelectedItems");
                List<int> indexList = _workbookListControl.GetIndexListSelectedItem();
                if (_err.hasAlert) { return; }

                if (indexList.Count < 1) { _err.AddLogWarning(" indexList.Count < 1"); return; }
                BookInfo info;
                foreach (int n in indexList)
                {
                    info = this.WorkbookList[n];
                    _excelManager.CloseWorkbookByPidAndBookName(
                        info.ProcessId, info.FileName, n, true, false);
                    if (_err.hasAlert) { _err.AddLogAlert("CloseWorkbookByPidAndBookName False"); }
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "CloseWorkbookSelectedItems");
            }
        }
    }
}
