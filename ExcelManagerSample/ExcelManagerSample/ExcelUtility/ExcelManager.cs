using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Threading;
using CommonUtility;
using CommonUtility.Pinvoke;
using ExcelUtility.ExcelObject;
using CommonUtility.Shortcut;

namespace ExcelUtility
{
    public class ExcelManager
    {
        protected ErrorManager.ErrorManager _Error;
        protected ProcessUtility _ProcUtil;
        protected WindowControlUtility _WindowUtil;
        protected FileUtility _FileUtil;
        protected List<ExcelApps> _ExcelAppsList = new List<ExcelApps>();
        // ExcelAppsList.Count が 0 になったとき
        public EventHandler ExcelAppsCountIsZeroEvent;
        // Excel のプロセス名
        protected string _ExcelProcName = "EXCEL";
        protected string _ExcelExeFileName = "EXCEL.EXE";
        // 外部へ紐づけ用 Event
        public IExcelAppsEventBridgeInterface ExcelEventBridge;
        protected ExcelWorkbookSyncer _excelWorkbookSyncer;
        // Workbook を開いているか(状態管理)
        public bool IsWorkbookOpening;
        // 初回起動時用のフラグ
        public bool IsFirstRunApplication = false;
        // ExcelAppsListを更新中(状態管理)
        public bool IsUpdating = false;
        // Close 後に ExcelAppsList を更新する
        public bool IsDoUpdateWhenCloseWorkbook = true;
        // 外部へ紐づけ用 Event
        public EventHandler UpdateExcelAppsListAfterEvent;
        // 常に Excel.Application を開いておく
        public bool IsAlwaysOpenExcelApplication = true;
        // エクセルをほかのプロセス(エクスプローラなど)から開いたときに同期する
        // ExcelAppsList 内の Application のイベント(Open,Close,WindowActivate)がとらえられている場合同期する
        public bool IsSyncExcelWorkbook
        {
            get
            {
                if (_excelWorkbookSyncer != null) { return _excelWorkbookSyncer.IsSyncExcelWorkbook; }
                else { return false; }
            }
            set
            {
                if (_excelWorkbookSyncer != null) { _excelWorkbookSyncer.IsSyncExcelWorkbook = value; }
                else { Console.WriteLine("IsSyncExcelWorkbook Property excelWorkbookSyncer == null"); }
            }
        }
        public bool IsWorkbookOpened
        {
            get
            {
                if (_excelWorkbookSyncer != null) { return _excelWorkbookSyncer.IsWorkbookOpened; }
                else { return false; }
            }
        }
        public bool IsWorkbookClosed
        {
            get
            {
                if (_excelWorkbookSyncer != null) { return _excelWorkbookSyncer.IsWorkbookClosed; }
                else { return false; }
            }
        }

        public class ExcelFileType
        {
            public readonly string[] Types =
                { ".xlsx",".xlsm",".xlsb",".xltx",".xltm",".xls",".xlt","xml",".xlam",".xla",".xlw",".xlr" };

            public List<string> GetTypesList()
            {
                return Types.ToList<string>();
            }
        }
        public ExcelManager(ErrorManager.ErrorManager error)
        {
            _Error=error;
            _ProcUtil = new ProcessUtility(error);
            _WindowUtil = new CommonUtility.Pinvoke.WindowControlUtility(error);
            _excelWorkbookSyncer = new ExcelWorkbookSyncer(_Error, this);
        }

        public void ChangeActiveCell(in Application application ,string workbookName ,string worksheetName,string address)
        {
            try
            {
                try
                {
                    _Error.AddLog(this, "ChangeActiveCell");
                    if (application == null) { _Error.AddLogAlert("application is null"); return; }
                    if ((workbookName == "")||(workbookName == null)) { _Error.AddLogAlert("workbookName is null"); return; }
                    if ((worksheetName == "") || (worksheetName == null)) { _Error.AddLogAlert("worksheetName is null"); return; }
                    if ((address == "") || (address == null)) { _Error.AddLogAlert("address is null"); return; }
                    application.Workbooks[workbookName].Activate();
                    application.Workbooks[workbookName].Worksheets[worksheetName].activate();
                    ((Worksheet)application.Workbooks[workbookName].Worksheets[worksheetName]).Range[address].Activate();
                } catch
                {
                    throw new Exception( this.ToString() + ".ChangeActiveCell");
                }
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ChangeActiveCell");
            }
        }

        private void ActivateEvent(Workbook Wb,object Worksheet,Range Target)
        {
            try
            {
                // AppsList内のいずれかの workbook が Active になったとき発生させる
                // ここでもイベントを発生させて、外部で処理する
                _Error.AddLog(this.ToString()+".ActivateEvent : ExcelManger") ;
                string workbookName = Wb.Name;
                string sheetName = ((Worksheet)Worksheet).Name;
                string Address = Target.Address;
                
            } catch ( Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ActivateEvent");
            }
        }


        /// <summary>
        ///  ExcelApps のリストを取得する
        /// </summary>
        /// <returns></returns>
        public List<ExcelApps> GetExcelAppsList()
        {
            return this._ExcelAppsList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public void ActivateWorkbook(AppsInfo info)
        {
            try
            {
                ExcelApps apps = GetExcelAppsFromAppsInfo(info);
                if (_Error.hasError) { return; }
                if (apps == null) {
                    throw new Exception("ExcelApps is null");
                }
                // ウィンドウを最前面へ、最小化時は元に戻す
                _WindowUtil.WakeupWindow(apps.ProcessId);
                if (_Error.hasError) { return; }
                
                if (!IsActive(info))
                {
                    apps.Application.Workbooks[info.FileName].Activate();
                }

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ActivateWorkbook");
                return;
            }
        }

        /// <summary>
        /// AppsInfo で指定した Workbook(Filename) がアクティブか判定する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsActive(AppsInfo info)
        {
            try
            {
                if (_ExcelAppsList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if(_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                foreach(ExcelApps apps in _ExcelAppsList)
                {
                    if (apps.IsActive(info.FileName))
                    {
                        return true;
                    }
                }
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsActive");
                return false;
            }
        }

        /// <summary>
        ///  ExcelApps List の要素 0 を取得する
        /// </summary>
        /// <returns></returns>
        public ExcelApps GetFirstObject()
        {
            try
            {
                if (_ExcelAppsList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if (_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                return _ExcelAppsList[0];
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetFirstObject");
                return null;
            }
        }

        /// <summary>
        /// ExcelApps のオブジェクトがあるとき、Window を最前面に表示する
        /// </summary>
        public int WindowActivate()
        {
            int ret = 0;
            try
            {
                _Error.AddLog(this.ToString()+ "WindowActivate");
                ExcelApps apps = this.GetActiveExcelApps();
                if (_Error.hasAlert) { 
                    throw new Exception("GetActiveExcelApps Failed"); 
                }

                if (apps == null) {
                    _Error.AddLogWarning("ExcelApps.ExcelApps Is Null");
                    apps = GetFirstObject();
                }
                if (apps == null) { throw new Exception("ExcelApps.ExcelApps Is Null (After GetFirstObject)"); }

                // ウィンドウを最前面へ、最小化時は元に戻す
                ret = _WindowUtil.WakeupWindowR(apps.ProcessId);
                if (ret < 1) { _Error.AddLogAlert("_WindowUtil.WakeupWindowR Failed ret="+ret); return ret; }
                if (_Error.hasAlert) {  throw new Exception("_WindowUtil.WakeupWindow Failed. ret="+ret);  }
                //
                string FileName = apps.GetActiveWorkbookName();
                if (_Error.hasAlert) { throw new Exception("GetActiveWorkbookName Failed"); }

                ret = apps.ActivateWorkbook(FileName);
                if (ret < 0)
                { _Error.AddLogAlert("apps.ActivateWorkbook Failed"); return ret; }
                if (_Error.hasAlert) { throw new Exception("apps.ActivateWorkbook Failed"); }

                return (int)ExcelManagerConstants.RESULT_SUCCESS;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this,"WindowActivate",
                    ExcelManagerConst.GetErrorMessage(ExcelManagerErrorCodes.UNEXPECTED_ERROR_M));
                return -(int)ExcelManagerErrorCodes.UNEXPECTED_ERROR_M;
            }
        }
        /// <summary>
        /// Application.ActiveCell からアドレス値(string)を取得する
        /// </summary>
        /// <returns></returns>
        public string GetActivateCell()
        {
            int ret = 0;
            try
            {
                if (_ExcelAppsList.Count < 1)
                {
                    throw new Exception("ExcelAppsList is Nothing");
                }
                ExcelApps apps = _ExcelAppsList[0];
                string workbookName = apps.GetActiveWorkbookName();
                string sheetName = apps.GetActiveSheetName();
                string activeCell = apps.GetActiveCellsAddress();
                string buf = "* GetActivateCell : ";
                buf += workbookName + " > " + sheetName + " > " + activeCell;
                _Error.AddLog(buf);

                ret = WindowActivate();
                if(ret < 1) { _Error.AddLogAlert(" WindowActivate Failed"); return ""; }
                if (_Error.hasAlert) { throw new Exception("WindowActivate Failed"); }
                if (apps.IsGhost) { return""; }
                return apps.Application.ActiveCell.Address;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.Message.IndexOf(ExcelManagerErrorCodes.RPC_E_CALL_REJECTED.ToString()) > 0)
                {
                    string msg = ExcelManagerConst.GetErrorMessage(ExcelManagerErrorCodes.RPC_E_CALL_REJECTED);
                    _Error.AddException(ex, this, "GetActivateCell Failed(System.Runtime.InteropServices.COMException)", msg);
                }
                else
                {
                    _Error.AddException(ex, this.ToString() + ".GetActivateCell COMException");
                }
                return "";
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetActivateCell");
                return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetLastActiveWorkbook()
        {
            try
            {
                // 未設定なら Application List の最後の Application で、Workooks index が最初のものにする

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetLastActiveWorkbook");
                return;
            }
        }
        /// <summary>
        /// ExcelAppsList が有効な状態(!=null , List.Count > 0) であるか判定する
        /// </summary>
        /// <returns></returns>
        public bool AppsListIsValid()
        {
            try
            {
                if (_ExcelAppsList == null) { 
                    return false; 
                }
                if (_ExcelAppsList.Count < 1) { 
                    return false; 
                }
                return true;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".AppsListIsValid");
                return false;
            }
        }
        /// <summary>
        /// 現在の ActiveWorkbook を保持している ExcelApps を取得する
        /// </summary>
        /// <returns></returns>
        public ExcelApps GetActiveExcelApps()
        {
            try
            {
                _Error.AddLog(this.ToString()+ ".GetActiveExcelApps");
                if(_ExcelAppsList == null){ throw new Exception("ExcelApps List Is Null"); }
                if(_ExcelAppsList.Count < 1){ throw new Exception("ExcelApps List>Count Is Zero"); }
                foreach (var apps in _ExcelAppsList)
                {
                    if (apps.IsActivate > 0){
                        return apps;
                    }
                }
                _Error.AddLogWarning("  ActiveExcelApps is Nothing");
                return null;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetActiveExcelApps");
                return null;
            }
        }


        /// <summary>
        /// 現在開いている Excel の Workbook をすべて取得し、ExcelAppsList を更新する
        /// </summary>
        public void UpdateOpendExcelApplication()
        {
            int ret = 0;
            bool isAddList = false;
            try
            {
                _Error.AddLog(this,"UpdateOpendExcelApplication");
                if (IsUpdating) { _Error.AddLogWarning("  IsUpdatting = true , return"); return; }
                else { IsUpdating = true; }
                _Error.ReleaseErrorState();
                _Error.ClearError();


                List<ExcelApps> appsList = _ExcelAppsList;

                // GetActiveObject から Excel.Application を取得する
                // System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application")
                ExcelApps newApps = new ExcelApps(_Error, ExcelEventBridge)
                {
                    // すでに Excel.Application があれば取得する
                    // Application が無く Null でも例外扱いとしない、Log に格納する
                    Application = this.GetExcelApplicationFromGetActiveObject()
                };
                if (_Error.hasError) 
                { 
                    _Error.AddLogWarning(this,"GetExcelApplicationFromGetActiveObject Failed");
                    return; 
                }
                if (newApps.Application == null)
                { 
                    _Error.AddLogWarning(this, "GetExcelApplicationFromGetActiveObject Applicatoin is Null");

                    // Application が Null の場合は追加する
                    isAddList = true;

                    // 常に Exce.Application を起動しておくフラグが True
                    // ＆ Application がない場合は Create する
                    if (IsAlwaysOpenExcelApplication)
                    {
                        // ExcelApplication がない場合は作る
                        _Error.AddLog(this,"ExcelApplicationRunWhenNothing=true");
                        _Error.AddLog(this, "**NewApplication=>ExcelAppsList.Add");
                        ExcelApps apps = new ExcelApps(_Error);
                        apps.CreateNewApplication(true);
                        apps.IsGhost = true;
                        _Error.AddLog(" set apps.IsGhost=true");
                        // ProcessId をメンバ変数へセットする
                        apps.SetProcessIdFromExcelApplication();
                        if (_Error.hasError) { _Error.AddLogWarning("SetProcessIdFromExcelApplication failed"); _Error.ClearError(); }
                        // WorkbookActivate イベントハンドラをセットする
                        apps.SetWorkbookEvent();
                        _excelWorkbookSyncer.SetEvent(ref apps.Application);
                        newApps = apps;

                        // 常に起動する場合は追加する
                        isAddList = true;
                    } else
                    {
                        _Error.AddLog(this, "ExcelApplicationRunWhenNothing=false");
                    }

                    //return; 
                } else {
                    if(_ExcelAppsList.Count < 1)
                    {
                        isAddList = true;
                    }
                }

                
                // Excel.Application 起動時は newApps.Application のみある状態
                // ProcessId をメンバ変数へセットする
                newApps.SetProcessIdFromExcelApplication();
                if (_Error.hasError)
                { _Error.AddLogAlert(this, "SetProcessIdFromExcelApplication Failed"); return; }

                // プロセス ID から PathList を取得する
                // PathList から Workbook を取得する
                // Workbook から Application を取得する
                GetExcelApplicationFromProcess(newApps, newApps.ProcessId);
                if (_Error.hasError)
                { _Error.AddLogAlert(this, "GetExcelApplicationFromProcess Failed"); return; }

                // 既存のものと ProcessId が合致しない場合は追加する
                if (!IsExistsProcessId(newApps.ProcessId))
                {
                    _Error.AddLog(" IsExistsProcessId=false ,pid="+newApps.ProcessId);
                    // FileList をセットする
                    // WorkbookActivate イベントハンドラをセットする
                    // Activate フラグをセットする
                    ret = UpdateExcelApps(newApps);
                    if (ret < 1) { _Error.AddLogAlert(this, "UpdateExcelApps Failed"); }

                    if (isAddList)
                    {
                        // リストに追加する
                        appsList.Add(newApps);
                        _Error.AddLog("  appsList.Add(newApps) 2");
                    }
                } else
                {
                    _Error.AddLog("  IsExistsProcessId=true , pid="+newApps.ProcessId);
                    // WorkbookList が更新されている場合があるので更新する
                    UpdateExcelApps(newApps);
                    OverWriteExcelApps(newApps);
                }

                // 複数のプロセス EXCEL.EXE があれば取得する
                Process[] ArrayProcess = Process.GetProcesses();

                // 現在のプロセスリストから Excel のみを取り出す
                List<Process> procList = _ProcUtil.GetProcessListMatchToProcessName(ArrayProcess, _ExcelProcName);
                if (_Error.hasError)
                { _Error.AddLogAlert("ProcessUtility.GetProcessListMatchToProcessName Failed"); return; }

                if (procList.Count > 0)
                {
                    // 先に取得した ProcessId を保存しておく
                    int alreadyExcelId = newApps.ProcessId;
                    foreach (Process bufProc in procList)
                    {
                        // 先に取得した ProcessId は保存済みなのでスキップする
                        //if (bufProc.Id != alreadyExcelId)
                        if(!IsExistsProcessId(bufProc.Id))
                        {
                            _Error.AddLog("  IsExistsProcessId=false , pid=" + bufProc.Id);
                            newApps = new ExcelApps(_Error,ExcelEventBridge);
                            // プロセス ID から PathList を取得する
                            // PathList から Workbook を取得する
                            // Workbook から Application を取得する
                            GetExcelApplicationFromProcess(newApps, bufProc.Id);
                            if (_Error.hasError)
                            { _Error.AddLogAlert(this, "GetExcelApplicationFromProcess Failed"); continue; }

                            // ProcessId をメンバ変数へセットする
                            // Application.Hwnd から 開いているファイルリストを作成する
                            // ファイルリストがなければ Ghost とする
                            newApps.SetProcessIdFromExcelApplication();
                            if (_Error.hasError)
                            { _Error.AddLogAlert(this, "SetProcessIdFromExcelApplication Failed"); continue; }

                            // Ghost の場合は ProcessId を bufProc から取得する
                            if (newApps.IsGhost) { newApps.ProcessId = bufProc.Id; }
                            // 既存のものと ProcessId が合致しない場合は追加する
                            if (!IsExistsProcessId(newApps.ProcessId))
                            {
                                // FileList をセットする
                                // WorkbookActivate イベントハンドラをセットする
                                // Activate フラグをセットする
                                ret = UpdateExcelApps(newApps);
                                if (ret < 1) { _Error.AddLogAlert(this, "UpdateExcelApps Failed"); }
                                
                                // リストに追加する
                                appsList.Add(newApps);
                                _Error.AddLog("  appsList.Add(newApps) 3");
                            }
                        } else
                        {
                            _Error.AddLog("  IsExistsProcessId=true , pid="+bufProc.Id);
                            if(!(newApps.ProcessId == bufProc.Id))
                            {
                                // pid が異なる場合は更新する
                                newApps = GetExcelAppsFromProcessId(bufProc.Id);
                                // WorkbookList が更新されている場合があるので更新する
                                UpdateExcelApps(newApps);
                                OverWriteExcelApps(newApps);
                            }
                            else
                            {

                            }
                        }
                    }
                }
                else
                {
                    _Error.AddLog("  [EXCEL] procList.Count > 0");
                }
                _Error.AddLog("  ExcelAppsList.Count="+_ExcelAppsList.Count);
                if (IsUpdating) { _Error.AddLog(" IsUpdating,true=>false"); IsUpdating = false; }
                return;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".UpdateOpendExcelApplication");
                if (IsUpdating) { _Error.AddLog(" IsUpdating,true=>false"); IsUpdating = false; }
                return;
            }
            finally            {
                if (IsWorkbookOpened) {
                    _Error.AddLog("   IsWorkbookOpened,true=>false"); 
                    _excelWorkbookSyncer.IsWorkbookOpened = false; 
                }
                if (IsWorkbookClosed) { 
                    _Error.AddLog("   IsWorkbookClosed,true=>false");
                    _excelWorkbookSyncer.IsWorkbookClosed = false;
                }
                if (IsUpdating) { _Error.AddLog(" IsUpdating,true=>false"); IsUpdating = false; }
                if (UpdateExcelAppsListAfterEvent != null) { UpdateExcelAppsListAfterEvent.Invoke(null, EventArgs.Empty); }
            }
        }

        public int OverWriteExcelApps(ExcelApps apps)
        {
            try
            {
                _Error.AddLog(this,"OverWriteExcelApps");
                if(_ExcelAppsList == null) { _Error.AddLog("  _ExcelAppsList == null"); return -1; }
                if(_ExcelAppsList.Count < 1) { _Error.AddLog("  _ExcelAppsList.Count < 1"); return -2; }

                for(int i=0; i<_ExcelAppsList.Count; i++)
                {
                    if(_ExcelAppsList[i].ProcessId == apps.ProcessId)
                    {
                        _ExcelAppsList[i] = apps;
                        _Error.AddLog("  OverWriteList index="+i);
                        return 1;
                    }
                }
                _Error.AddLog("  OverWriteList failed");
                return -3;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this,"OverWriteExcelApps");
                return 0;
            }
        }

        public int UpdateExcelApps(ExcelApps apps)
        {
            int ret;
            try
            {
                _Error.AddLog(this, "UpdateExcelApps");
                // プロセス ID から PathList を取得する
                // PathList から Workbook を取得する
                // Workbook から Application を取得する
                GetExcelApplicationFromProcess(apps, apps.ProcessId);
                if (_Error.hasError)
                { _Error.AddLogAlert(this, "GetExcelApplicationFromProcess Failed"); }

                // FileList をセットする
                apps.ReSetFileListFromApplication();
                if (_Error.hasError)
                { _Error.AddLogAlert(this, "ReSetFileListFromApplication Failed");  }

                // WorkbookActivate イベントハンドラをセットする
                apps.SetWorkbookEvent();
                if (_Error.hasError)
                { _Error.AddLogAlert(this, "SetWorkbookEvent Failed"); }
                ret = _excelWorkbookSyncer.SetEvent(ref apps.Application);
                if (ret < 1) { _Error.AddLogAlert(this, "_excelWorkbookSyncer.SetEvent Failed"); }

                // Activate フラグをセットする
                apps.SetActivateFlag();
                if (_Error.hasError)
                { _Error.AddLogAlert(this, "SetActivateFlag Failed");  }

                return 1;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".UpdateExcelApps");
                return 0;
            }
        }

        //public void OpenWorkbook(string path)
        /// <summary>
        /// ProcessID が存在しているか判定する
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public bool IsExistsProcessId(int pid)
        {
            try
            {
                if (_ExcelAppsList.Count < 1) { _Error.AddLog(this,"IsExistsProcessId list.Count<1"); return false; }
                
                foreach (ExcelApps apps in _ExcelAppsList)
                {
                    if ( apps.ProcessId == pid) { 
                        _Error.AddLog(this,"IsExistsProcessId , true pid="+ pid); 
                        return true;
                    }
                    else
                    {
                        _Error.AddLog(this, "IsExistsProcessId Loop pid =" + apps.ProcessId);
                    }
                }
                _Error.AddLog(this, "IsExistsProcessId , false pid=" + pid);
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExistsProcessId");
                return false;
            }
        }

        /// <summary>
        /// GetActiveObject メソッドから Excel.Application を取得する
        /// </summary>
        /// <returns></returns>
        public Application GetExcelApplicationFromGetActiveObject()
        {
            try
            {
                return (Microsoft.Office.Interop.Excel.Application)
                    System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch (Exception ex)
            {
                // Application が無く Null でも例外扱いとしない、Log に格納する
                //_Error.AddException(ex, this.ToString() + ".GetExcelApplicationFromGetActiveObject");
                _Error.AddLogAlert(this.ToString()+ ".GetExcelApplicationFromGetActiveObject","",ex);
                return null;
            }
        }
        /// <summary>
        /// 現在開いている Excel ファイル名をすべて取得する
        /// UpdateList 処理を行った後に取得する
        /// </summary>
        /// <returns></returns>
        public List<string> GetOpendFileNameList()
        {
            List<string> retList = new List<string>();
            try
            {
                UpdateOpendExcelApplication();
                if (_Error.HasException()) { return retList; }
                // Application のセットが終わったので
                // あとは、Workbook.FileName を列挙して戻り値にする
                foreach (var bufApp in _ExcelAppsList)
                {
                    retList.AddRange(bufApp.GetWorkbookNameList());
                    if (_Error.HasException()) { return retList; }
                }
                return retList;
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetOpendFileNameList");
                return retList;
            }
        }

        /// <summary>
        /// ProcessId から Excel.Application を取得しセットする
        /// </summary>
        /// <param name="newApps"></param>
        /// <param name="pid"></param>
        private void GetExcelApplicationFromProcess(ExcelApps newApps,int pid)
        {
            try
            {
                _Error.AddLog(this, "GetExcelApplicationFromProcess");
                ExcelFilePathGetterFromPid util = 
                    new ExcelFilePathGetterFromPid(_Error,new ExcelFileType().GetTypesList());
                // ProcessId から開いているパスを取得する
                newApps.FilePathList = util.GetListExcelFilePathFromPid(pid);
                if (_Error.hasError) { throw new Exception("ExcelFilePathGetterFromPid.GetListExcelFilePathFromPid Failed."); }

                if (newApps.FilePathList.Count < 1)
                {
                    // ProcessId があるのに、ファイルパスが取得できていないのはゴーストプロセスとする
                    _Error.AddLog(_Error.LogConstants.PRIORITY_CAUTION,_Error.LogConstants.TYPE_LOG,
                        "  Set IsGhost=true , pid=" + pid.ToString());
                    newApps.IsGhost=true;
                    return;
                } else
                {
                    newApps.IsGhost = false;
                    _Error.AddLog("  set newApps.IsGhost=false");
                }
                // Path から Workbook を取得する
                // Workbook から Application を取得する
                newApps.SetApplicationFromFilePath();
                if (_Error.HasException()) { throw new Exception(this.ToString()+".SetApplicationFromFilePath Failed."); }

            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetExcelApplicationFromProcess");
                return;
            }
        }

        /// <summary>
        /// テストメソッド：シート名のリストを取得し、Consoleへ出力する
        /// </summary>
        /// <param name="filePath"></param>
        public void GetSheetTest(string filePath)
        {
            try
            {
                _Error.AddLog(this.ToString()+ ".GetSheetTest");
                UpdateOpendExcelApplication();
                if (_Error.hasError) { return; }

                ExcelApps apps = GetExcelAppsWhenOpendFile(filePath);
                if (_Error.hasError) { return; }
                if (apps == null) { throw new Exception("GetSheetTest: ExcelApps Is Null"); }
                if (!apps.IsExsitsWorkbookOneOrMoreInApplication())
                {
                    throw new Exception("GetSheetTest: ExcelApps Is Null");
                }
                if (apps.FilePathList.Count < 1) {
                    throw new Exception("GetSheetTest: ExcelApps.FileList.Count Is Zero"); 
                } else
                {
                    apps.SetApplicationFromFilePath();
                }
                //if (apps.IsGhost) { }
                //if (WorkbookIndex < 0) { throw new Exception("GetSheetTest: WorkbookIndex Under Zero"); }

                if (apps.OperationWorkbook == null)
                {
                    apps.OperationWorkbook = new ExcelWorkbook(_Error);
                }
                // Workbook を ExcelApps.OperationWorkbook にセットする
                apps.SetWorkbookForOperation(filePath);
                if (_Error.HasException()) { return; }

                List<string> list = apps.OperationWorkbook.GetSheetList();
                if (_Error.HasException()) { return; }

                if (list.Count < 1) { throw new Exception("GetSheetTest: WorkSheets.Count Is Zero"); }
                _Error.AddLog("---- sheet list -----");
                foreach(string buf in list)
                {
                    _Error.AddLog(buf);
                }
                _Error.AddLog("---------------------");
                return;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetSheetTest");
                return;
            }
        }

        /// <summary>
        /// 拡張子 lnk のファイルパス文字列からリンク元のパスを取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetTargetPathFromLnkFile(string path)
        {
            try
            {

                if (System.IO.Path.GetExtension(path) == ".lnk")
                {
                    ShortcutUtility shortcut = new ShortcutUtility(_Error);
                    return shortcut.GetTargetPath(path);
                }
                else
                {
                    return path;
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetTargetPathFromLnkFile");
                return "";
            }
        }

        /// <summary>
        /// path がエクセル関係の拡張子か判定する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsExcelType(string path)
        {
            try
            {
                List<string> typeList = new ExcelFileType().GetTypesList();
                foreach(string typeValue in typeList)
                {
                    if( System.IO.Path.GetExtension(path) == typeValue)
                    {
                        return true;
                    }
                }
                return false;

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExcelType");
                return false;
            }
        }

        public void ExcelApplicationRunWhenNothing(bool IsWindowVisible = true)
        {
            try
            {
                _Error.AddLog(this, "ExcelApplicationRunWhenNothing");
                // Null、List.Count<1 かを判定する
                if (AppsListIsValid()) { _Error.AddLog(" AppsListIsValid=true"); return; }
                else { _Error.AddLog("  AppsListIsValid=false"); }
                // ExcelAppsList がない状態なので Application を取得したうえ new して、AppsList に追加する
                _ExcelAppsList = new List<ExcelApps>();
                ExcelApps apps = new ExcelApps(_Error);
                // 開いている Excel.Application があれば取得する
                apps.Application = this.GetExcelApplicationFromGetActiveObject();
                // Application がなければ new する
                if(apps.Application == null) {
                    // NewApplication
                    _Error.AddLog(this, "**NewApplication=>ExcelAppsList.Add");
                    apps.CreateNewApplication(true);
                    apps.IsGhost = true;
                    _Error.AddLog("  apps.IsGhost=true");
                    // ProcessId をメンバ変数へセットする
                    apps.SetProcessIdFromExcelApplication();
                    _Error.AddLog("  apps.ProcessId = " + apps.ProcessId);
                    if (_Error.hasError) { return; }
                    // WorkbookActivate イベントハンドラ
                    UpdateExcelApps(apps);
                    _excelWorkbookSyncer.SetEvent(ref apps.Application);
                    // リストに追加する
                    _ExcelAppsList.Add(apps);
                    _Error.AddLog("  _ExcelAppsList.Add(new ExcelApps(_Error)) 4");
                }
                else { 
                    _Error.AddLogWarning(" Already Exists Excel Application => return");
                    //apps.Dispose();
                    //apps.Close();
                    apps = null;
                    return; 
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this, "ExcelApplicationRunWhenNothing");
            }
        }

        /// <summary>
        /// エクセルファイルを開く
        /// 開いた後 ExcelAppsList へ情報を保存する
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenFile(string filePath)
        {
            try
            {
                _Error.AddLog(this, "OpenFile : path="+filePath);
                IsWorkbookOpening = true;
                _Error.ClearError();
                if (_ExcelAppsList.Count > 0)
                {
                    // 現在開いている Excel をすべて取得する、メンバ変数 _ExcelAppsList に保存する
                    UpdateOpendExcelApplication();
                    if (_Error.hasError) { return; }
                }

                // ファイルが存在するか判定する
                if (!File.Exists(filePath)) { 
                    throw new Exception("FilePath Not Exists.[" + filePath + "]");
                }

                // path が .lnk の場合リンク元を取得する
                filePath = GetTargetPathFromLnkFile(filePath);
                if (_Error.hasError) { return; }

                // lnk内から取得したパスの存在チェックする
                if (!File.Exists(filePath))
                {
                    throw new Exception("FilePath Not Exists.[" + filePath + "]");
                }

                // path が Excel 関連のものか判定する
                if (!IsExcelType(filePath))
                {
                    throw new Exception("FilePath Not Excel Type.[" + filePath + "]");
                }
                // _ExcelAppsList がなければ new する
                ExcelApps openApps = null;

                // ファイルを開いている、開いていないにかかわらず
                // ExcelAppsList.Count>0 で Application が存在していればそれを取得する

                if (this._ExcelAppsList.Count > 0)
                {
                    // Excel.Appliction がある場合 ExcelApps を取得する
                    //openApps = GetExcelAppsWhenOpendFile(filePath);
                    //if (_Error.HasException()) { return; }
                    openApps = _ExcelAppsList[0];
                }
                else
                {
                    // Application を始めて起動するとき、WakeUpWindow 実行が早すぎるとエラーが出る？用のフラグ
                    IsFirstRunApplication = true;
                    // もし、Application List がない場合は作る
                    openApps = new ExcelApps(_Error, ExcelEventBridge);
                    _ExcelAppsList.Add(openApps);
                    _Error.AddLog("  _ExcelAppsList.Add(openApps) 5");
                }
                // もし、Nullとなった場合
                if (openApps == null)
                {
                    throw new Exception("GetExcelAppsWhenOpendFile Value Is Null");
                }
                // ファイルを開く用の ExcelApps を取得した状態
                // ファイルを開いているか判定する
                if (openApps.IsOpendFile(filePath))
                {
                    _Error.AddLog("  openApps.IsOpendFile = true: filePath=" + filePath);
                    // ExcelApps.FileList ファイルリストを追加(更新)する
                    openApps.ReSetFileListFromApplication();
                    if (_Error.hasError) { return; }
                    // pathList に追加してない場合があるので追加する、重複時は追加しない
                    openApps.AddFilePathListIfValueNotExists(filePath);
                    if (_Error.hasError) { return; }
                    // Workbook をアクティブにする
                    openApps.ActivateWrokbook(filePath);
                    if (_Error.hasError) { return; }
                } else
                {
                    _Error.AddLog("  openApps.IsOpendFile = false: filePath="+filePath);
                    // pathList に追加してない場合があるので追加する、重複時は追加しない
                    openApps.AddFilePathListIfValueNotExists(filePath);
                    if (_Error.hasError) { return; }

                    if (openApps.Application == null)
                    {
                        // 開いていない場合、かつ、既存の Application がない場合(_ExcelApplList.Count<1だった)、
                        // 新たに Application を作成、 openApps にコピーしてそこから Open する
                        // 開いていない場合は、新たに Application を new する
                        openApps.CreateApplication();
                        if (_Error.hasError) { return; }
                        _Error.AddLog("openApps.Application is Null => openApps.CreateApplication");
                    } else
                    {
                        // 開いていない場合、かつ、既存の Application がある場合、
                        // 既存の Application を openApps にコピーしてそこから Open する
                        //openApps.SetAppliction(_ExcelAppsList[0].Application);
                    }

                    // ファイルリストに追加してオープンする
                    openApps.OpenFileAddToList(filePath);
                    if (_Error.hasError) { return; }
                    // Application をセットする
                    openApps.SetApplicationFromFilePath();
                    if (_Error.hasError) { return; }
                    // ProcessId をメンバ変数へセットする
                    openApps.SetProcessIdFromExcelApplication();
                    if (_Error.hasError) { return; }
                    // ExcelApps.FileList ファイルリストを追加(更新)する
                    openApps.ReSetFileListFromApplication();
                    if (_Error.hasError) { return; }
                    // WorkbookActivate イベントハンドラをセットする
                    openApps.SetWorkbookEvent();
                    _excelWorkbookSyncer.SetEvent(ref openApps.Application);
                    if (_Error.hasError) { return; }
                    // Open した Workbook のイベントハンドラをセットする
                    openApps.SetWorkbookEvent(filePath, System.IO.Path.GetFileName(filePath));
                    if (_Error.hasError) { return; }
                    // Workbook をアクティブにする
                    openApps.ActivateWrokbook(filePath);
                    if (_Error.hasError) { return; }
                    OverWriteExcelApps(openApps);
                }

                Task task = Task.Run(() =>
                {
                    openApps.ApplicationVisible(true);
                    if (_Error.hasError) { return; }
                });
                task.Wait();

                if (IsFirstRunApplication)
                {
                    IsFirstRunApplication = false;
                }
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".OpenFile");
                return;
            } finally
            {
                IsWorkbookOpening = false;
            }
        }
        /// <summary>
        /// filePath を保持している ExcelApps を取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ExcelApps GetExcelAppsWhenOpendFile(string filePath)
        {
            try
            {
                if (this._ExcelAppsList.Count < 1) { return null; }
                bool flag = false;
                foreach(ExcelApps apps in _ExcelAppsList)
                {
                    flag = apps.IsOpendFile(filePath);
                    if (_Error.hasError) { return null; }
                    if (flag) {
                        return apps;
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetExcelAppsWhenOpendFile");
                return null;
            }
        }

        /// <summary>
        /// ファイルがロックされているか判定する
        /// Stream クラスでアクセスすることで判定している
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsFileOpend(string filePath)
        {
            try
            {
                if (this._ExcelAppsList.Count < 1) {
                    // Process EXCEL が一つもないとき
                    return false;
                }
                bool flag = false;
                foreach(ExcelApps apps in _ExcelAppsList)
                {
                    flag = apps.IsOpendFile(filePath);
                    if (flag) { break; }
                }
                return flag;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsFileOpend");
                return false;
            }
        }
        /// <summary>
        /// AppsInfo List を取得する(Workbook がない Application の場合は ProcessName を取得)
        /// </summary>
        /// <returns></returns>
        public List<AppsInfo> GetAppsInfoListAndGhostProcessNameList()
        {
            List<AppsInfo> retList = new List<AppsInfo>();
            try
            {
                _Error.AddLog(this, "GetAppsInfoListAndGhostProcessNameList");
                if (_ExcelAppsList.Count < 1)
                {
                    _Error.AddLogAlert("  _ExcelAppsList.Count < 1 return"); return retList;
                }

                AppsInfo bufInfo;
                int count = 0;
                foreach (ExcelApps apps in _ExcelAppsList)
                {
                    bufInfo = new AppsInfo();
                    if (apps == null) { continue; }
                    if (apps.IsGhost)
                    {
                        // EXCEL.EXE
                        bufInfo.FileName = "EXCEL.EXE";
                        bufInfo.ProcessId = apps.ProcessId;
                        bufInfo.Index = count;
                        retList.Add(bufInfo);
                    }
                    else
                    {
                        if (apps.Application.Workbooks.Count > 0)
                        {
                            for (int i = 1; i <= apps.Application.Workbooks.Count; i++)
                            {
                                bufInfo.FileName = apps.Application.Workbooks[i].Name;
                                bufInfo.ProcessId = apps.ProcessId;
                                bufInfo.Index = i;
                                retList.Add(bufInfo);
                            }
                        }
                    }
                }
                return retList;

            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetAppsInfoListAndGhostProcessNameList");
                return retList;
            }
        }
        /// <summary>
        /// WorkbookNameList を取得する(Workbook がない Application の場合は ProcessName を取得)
        /// </summary>
        /// <returns></returns>
        public List<string> GetWorkbookNameListAndGhostProcessNameList()
        {
            List<string> retList = new List<string>();
            try
            {
                _Error.AddLog(this,"GetWorkbookNameListAndGhostProcessNameList");
                UpdateOpendExcelApplication();
                if (_Error.hasError) { throw new Exception("UpdateOpendExcelApplication Failed"); }

                if (_ExcelAppsList.Count < 1) {
                    _Error.AddLogAlert("  _ExcelAppsList.Count < 1 return"); return retList;
                }

                string buf = "";
                List<string> FileNameList;
                foreach(ExcelApps apps in _ExcelAppsList)
                {
                    if (apps == null) { continue; }
                    FileNameList = new List<string>();
                    if (apps.IsGhost)
                    {
                        // EXCEL.EXE
                        buf = "[" + apps.ProcessId + "] " + _ExcelExeFileName;
                        FileNameList.Add(buf);
                    } else
                    {
                        FileNameList = apps.GetWorkbookNameList();
                        if(FileNameList.Count > 0)
                        {
                            for (int i = 0; i < FileNameList.Count; i++)
                            {
                                FileNameList[i] = "[" + apps.ProcessId + "] " + FileNameList[i];
                            }
                        }
                    }
                    retList.AddRange(FileNameList);
                }
                return retList;

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetWorkbookNameListAndGhostProcessNameList");
                return retList;
            }
        }
        /// <summary>
        /// Workbook を閉じる
        /// Pid,Book名,index (Book名が同じ場合があるので)から Workbook を判別する
        /// GhostProcess にも使用できる
        /// Close の実行関数
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="bookName"></param>
        /// <param name="index"></param>
        /// <param name="isSave"></param>
        /// <param name="isConform"></param>
        /// <param name="SaveAsName"></param>
        /// <param name="saveFilePath"></param>
        public void CloseWorkbookByPidAndBookName(int pid,string bookName,int index,bool isSave,bool isConform,
            string SaveAsName = "",string saveFilePath = "")
        {
            try
            {
                _Error.AddLog(this.ToString()+ ".CloseWorkbookByPidAndBookName");
                _Error.AddLog(pid + "/"+bookName+ "/ExcelAppsList.index=" + index+"/isSave="+isSave+"/"+"isConform="+isConform+
                    "/saveAsName="+SaveAsName+ "/"+"saveFilePath="+saveFilePath);
                if (_ExcelAppsList.Count < 1){ throw new Exception("ExcelAppsList.Count Is Null"); }
                int count = 0;
                // index を比較するために、最初から count++ しなければならない
                bool isRemove = false;
                for (int j = 0; j < _ExcelAppsList.Count; j++)
                {
                    ExcelApps apps = _ExcelAppsList[j];
                    count = 0;
                    if (apps.IsGhost)
                    {
                        // EXCEL.EXE
                        //if (bookName.Contains(_ExcelExeFileName))
                        //{
                        //    if (apps.ProcessId == pid)
                        //    {
                        //        CloseApplicationWithKillProcess(apps);
                        //        if (_Error.hasError) { return; }
                        //        isRemove = true;
                        //    }
                        //}
                        if (apps.ProcessId == 0){
                            isRemove = true;
                            _Error.AddLog(this , " ProcessID=0");
                        }
                        if (apps.ProcessId == pid)
                        {
                            CloseApplicationWithKillProcess(apps);
                            if (_Error.hasError) { return; }
                            isRemove = true;
                        }
                        count++;
                    }
                    else
                    {
                        for(int i=1; i<=apps.Application.Workbooks.Count; i++)
                        {
                            if ((apps.ProcessId == pid) &&
                                (apps.Application.Workbooks[i].Name.Contains(bookName))&&
                                (count == index))
                            {
                                // info が合致した場合閉じる
                                string path = apps.Application.Workbooks[i].Path + "\\" + apps.Application.Workbooks[i].Name;
                                // Workbook を Close して ExcelAppsList からも削除 Remove する
                                CloseWorkbook(path, isSave, isConform, SaveAsName, saveFilePath);
                                if (_Error.hasAlert) { throw new Exception("CloseWorkbook Failed"); }
                                isRemove = true;
                                break;
                            }
                            count++;
                        }
                    }
                }
                if (isRemove)
                {
                    _Error.AddLog("RemoveAppsAfterCloseProcess Excute Again");
                    RemoveAppsAfterCloseProcess(pid);
                    if (_Error.hasAlert) { throw new Exception("RemoveAppsAfterCloseProcess Again Failed"); }
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".CloseWorkbook");
                return;
            }
        }
        /// <summary>
        /// Workbook を終了する
        /// 同じファイルパスのものはは開いていない想定
        /// 読み取り専用の場合は保存しない
        /// </summary>
        /// <param name="readFilePath"></param>
        /// <param name="isSave"></param>
        /// <param name="isConform"></param>
        /// <param name="SaveAsName"></param>
        /// <param name="saveFilePath"></param>
        public void CloseWorkbook(string readFilePath,bool isSave, bool isConform,
            string SaveAsName = "",string saveFilePath = "")
        {
            Workbook book = null;
            try
            {
                _Error.AddLog(this, "CloseWorkbook");
                if (_ExcelAppsList.Count < 1) {
                    throw new Exception("ExcelAppsList.Count Is Zero");
                    //return; 
                }
                string fileName = System.IO.Path.GetFileName(readFilePath);
                ExcelApps apps = GetExcelAppsWhenOpendFile(readFilePath);
                if (apps != null)
                {
                    // 保存・終了対象の Workbook を取得する
                    for (int i = 1; i <= apps.Application.Workbooks.Count; i++)
                    {
                        book = apps.Application.Workbooks[i];
                        string buf = apps.Application.Workbooks[i].Path + "\\" + apps.Application.Workbooks[i].Name;
                        if (buf.Contains(readFilePath))
                        {
                            book = apps.Application.Workbooks[i];
                            break;
                        }
                    }
                    //Console.WriteLine(((string)book.Windows[book.Name].Caption));
                    int pos = ((string)book.Windows[book.Name].Caption).IndexOf("[読み取り専用]");
                    //if (!book.ReadOnly)
                    if (!book.ReadOnly)
                    {
                        // 確認メッセージを非表示に設定する
                        //apps.Application.DisplayAlerts = isConform;
                        // 対象の Workbook が読み取り専用でなければ以下を実行
                        // 名前を付けて保存するか
                        if (SaveAsName != "")
                        {
                            if (saveFilePath != "")
                            {
                                // 保存場所を指定する
                                _Error.AddLogAlert("********** 保存場所を指定する 未実装");
                            }
                            // ファイル名を指定して、別名保存する
                            book.SaveCopyAs(SaveAsName);

                        }
                        else
                        {
                            // 保存する
                            //book.Close(isSave);
                            CloseWorkbookMain(apps, book.Name, isSave, 1500);
                        }
                        if (!isConform)
                        {
                            // 確認メッセージを表示するに設定する
                            apps.Application.DisplayAlerts = true;
                        }
                    }
                    else
                    {
                        // 保存しない
                        //book.Close(false); // freeze
                        //book.Close(false,"abc.xls",false); // freeze
                        //book.Windows[book.Name].Close(false); // feeze
                        _Error.AddLog("  [ReadOnly=True] change flag : isSave = false");
                        CloseWorkbookMain(apps, book.Name, false, 500);
                        if (_Error.hasAlert) { _Error.AddLogAlert(this, "CloseWorkbookMain Failed."); }
                    }
                } else
                {
                    throw new Exception("Application is Null");
                }

                // Close 後 ExcelApps.Application.Workbooks.Count<1 のとき、削除する
                // 削除する
                RemoveAppsAfterCloseProcess(apps.ProcessId);
                if (_Error.hasAlert) { _Error.AddLogWarning("RemoveAppsAfterCloseProcess Failed"); return; }

            } catch ( Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".CloseWorkbook");
                return;
            }
            finally
            {            
            }
        }
        /// <summary>
        /// Workbook を Close するときのメイン関数
        /// </summary>
        /// <param name="apps"></param>
        /// <param name="workbookName"></param>
        /// <param name="isSave"></param>
        /// <param name="Timeout"></param>
        private void CloseWorkbookMain(ExcelApps apps,string workbookName,bool isSave,int Timeout)
        {
            try
            {
                
                    // 非同期処理をCancelするためのTokenを取得する
                    var tokenSource = new CancellationTokenSource();
                    var cancelToken = tokenSource.Token;

                    Task ret = Task.Run(() =>
                    {
                        try
                        {
                            apps.Application.Workbooks[workbookName].Close(isSave);
                            //apps.Application.Workbooks[workbookName].Close(isSave,"name.xls",false);
                        } catch (OperationCanceledException ex)
                        {
                            Console.WriteLine("OperationCanceledException");
                            Console.WriteLine(ex.Message);
                            _Error.AddLogWarning("OperationCanceledException");
                        }
                        catch (Exception ex)
                        {
                            _Error.AddException(ex,this.ToString()+ ".CloseWorkbookMain Async Task.Run");
                        }
                        finally
                        {
                        }
                    }, tokenSource.Token);

                    // タスクが終了するまで待機する 5秒
                    ret.Wait(Timeout);
                    if (ret.IsCompleted)
                    {
                    // Task が終了していたら OK
                        _Error.AddLog("  CloseWorkbookMain Task End.");
                        return;
                    } else
                    {
                        // タスクをキャンセルする
                        _Error.AddLog("  CloseWorkbookMain Cancel. filepath = " + apps.Application.Workbooks[workbookName].Path);
                        tokenSource.Cancel();
                        return;
                    }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".CloseWorkbookMain");
                return;
            }
        }
        /// <summary>
        /// Close 後 ExcelApps.Application.Workbooks.Count<1 のとき、削除する
        /// </summary>
        /// <param name="pid"></param>
        private void RemoveAppsAfterCloseProcess(int pid)
        {
            try
            {
                _Error.AddLog(this,"RemoveAppsAfterCloseProcess : pid = "+ pid);
                if (_ExcelAppsList.Count < 1) { _Error.AddLogWarning("ExcelAppsList.Count Is Zero"); return; }

                int count = -1;
                int listCount = _ExcelAppsList.Count;
                // AppsList For
                for (int i=0; i < listCount; i++)
                {
                    ExcelApps apps = _ExcelAppsList[i];
                    if (apps.ProcessId == pid)
                    {
                        if (apps.GetWorkBooksCount() <= 0)
                        {
                            count = i;
                        }
                    }
                }
                if (count >= 0)
                {
                    CloseApplicationWithKillProcess(_ExcelAppsList[count]);
                    RemoveExcelAppsList(count);
                    // 削除したときは処理の順番が変わる
                    _Error.AddLog("_ExcelAppsList.RemoveAt Success count="+count);
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".RemoveAppsAfterCloseProcess");
                return;
            }
        }
        private int RemoveExcelAppsList(int index)
        {
            try
            {
                _ExcelAppsList.RemoveAt(index);

                if(_ExcelAppsList.Count < 1)
                {
                    _Error.AddLog(this, "RemoveExcelAppsList : ExcelAppsList.Count<1");

                    if (IsAlwaysOpenExcelApplication)
                    {
                        _Error.AddLog("  IsAlwaysOpenExcelApplication=true");
                        ExcelApplicationRunWhenNothing(false);
                    }
                    if (ExcelAppsCountIsZeroEvent != null) { ExcelAppsCountIsZeroEvent.Invoke(null, EventArgs.Empty); }
                }
                return 1;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this,"RemoveExcelAppsList");
                return 0;
            }
        }
        /// <summary>
        /// 引数に ExcelApps を渡して、そのプロセスを終了する
        /// 主に GhostProcess に使用する
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        public bool CloseApplicationWithKillProcess(ExcelApps apps)
        {
            try
            {
                _Error.AddLog(this, "CloseApplicationWithKillProcess");
                apps.Close();
                if (_Error.hasAlert) { _Error.AddLogWarning("  Close Failed"); _Error.ReleaseErrorState(); }
                if (apps.GetWorkBooksCount() <= 0)
                {
                    bool ret = _ProcUtil.KillProcess(apps.ProcessId);
                    if (_Error.hasError) { _Error.AddLogAlert("_ProcUtil.KillProcess Failed"); return true; }
                    return ret;
                }
                return true;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".CloseApplicationWithKillProcess");
                return false;
            }
        }

        /// <summary>
        /// 対象の ProcessId のプロセスを終了する
        /// 主に GhostProcess に使用する
        /// あと、Workbook を閉じた後、Workbook がなくなった Excel.Application にも使用する
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int CloseProcess(int pid)
        {
            try
            {
                if (_ExcelAppsList.Count < 0) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                int count = 0;
                bool flag = false;
                foreach(ExcelApps apps in _ExcelAppsList)
                {
                    if (apps.ProcessId == pid)
                    {
                        flag = CloseApplicationWithKillProcess(apps);
                        if (_Error.HasException()) { return -1; }
                        if (flag) { return count; }
                    }
                    count++;
                }
                _ExcelAppsList.RemoveAt(count);
                return -2;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".CloseProcess");
                return -1;
            }
        }
        /// <summary>
        /// ProcessID から ExcelApps を取得する
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ExcelApps GetExcelAppsFromProcessId(int pid)
        {
            try
            {
                if(_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }

                foreach(ExcelApps apps in _ExcelAppsList)
                {
                    if(apps.ProcessId == pid)
                    {
                        return apps;
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetExcelAppsFromProcessId");
                return null;
            }
        }
        /// <summary>
        /// AppsInfo から ファイルパスを取得する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetPathFromAppsInfo(AppsInfo info)
        {
            object ret = GetDataFromAppsInfo(info.ProcessId, info.FileName, info.Index, 1);
            return ret.ToString();
        }
        /// <summary>
        /// AppsInfo から ExcelApps を取得する
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ExcelApps GetExcelAppsFromAppsInfo(AppsInfo info)
        {
            return (ExcelApps)GetDataFromAppsInfo(info.ProcessId, info.FileName, info.Index, 2);
        }
        /// <summary>
        /// WorkbookName から ExcelApps を取得する
        /// </summary>
        /// <param name="WorkbookName"></param>
        /// <returns></returns>
        public ExcelApps GetExcelAppsFromWorkbookName(string WorkbookName)
        {
            try
            {
                AppsInfo info = new AppsInfo
                {
                    FileName = WorkbookName
                };
                return (ExcelApps)GetDataFromAppsInfo(-1, info.FileName, -1, 2);
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetExcelAppsFromWorkbookName");
                return null;
            }
        }

        /// <summary>
        /// Pid、WorkbookName に合致したとき、パスまたは ExcelApps オブジェクトを取得する
        /// 戻り値が object 型なので値取得後にキャストなどの処理が必要
        /// [Mode によって戻り値の型が変わるため注意 ,1:ExcelApps,2:string]
        /// </summary>
        /// <param name="pid">ProcessID</param>
        /// <param name="bookName">Workbook Name</param>
        /// <param name="index">this.ExcelAppsList Index</param>
        /// <param name="getMode">Mode = 1:path,2:ExcelApps</param>
        /// <returns></returns>
        public object GetDataFromAppsInfo(int pid,string bookName,int index,int getMode)
        {
            try
            {
                _Error.AddLog(this, "GetDataFromAppsInfo");
                if (_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Null"); }
                int count = 0;
                // index を比較するために、最初から count++ しなければならない
                foreach (ExcelApps apps in _ExcelAppsList)
                {
                    if (apps.IsGhost)
                    {
                        if (bookName.Equals(_ExcelExeFileName))
                        {
                            if (apps.ProcessId == pid)
                            {
                                if (getMode == 1)
                                {
                                    return "";
                                }
                                else if (getMode == 2)
                                {
                                    return apps;
                                }
                                else
                                {
                                    throw new Exception("Mode is Invalid.");
                                }
                            }
                        }
                        count++;
                    }
                    else
                    {
                        for (int i = 1; i <= apps.Application.Workbooks.Count; i++)
                        {
                            bool flag = false;
                            if ((apps.ProcessId == pid) &&
                                (apps.Application.Workbooks[i].Name.Contains(bookName)) &&
                                (count == index))
                            {
                                flag = true;
                            }
                            if (index == -1)
                            {
                                if ((apps.ProcessId == pid) &&
                                    (apps.Application.Workbooks[i].Name.Contains(bookName)))
                                {
                                    flag = true;
                                }
                            }
                            if ((pid == -1) && (index == -1))
                            {
                                if (apps.Application.Workbooks[i].Name.Contains(bookName))
                                {
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                if (getMode == 1)
                                {
                                    string path = apps.Application.Workbooks[i].Path + "\\" + apps.Application.Workbooks[i].Name;
                                    return path;
                                }
                                else if (getMode == 2)
                                {
                                    return apps;
                                }
                                else
                                {
                                    throw new Exception("Mode is Invalid.");
                                }
                            }
                            count++;
                        }
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetDataFromAppsInfo");
                return null;
            }
        }
        /// <summary>
        /// Appliction 内にある WorkbookNme の Index を取得する
        /// </summary>
        /// <param name="WorkbookName"></param>
        /// <returns></returns>
        public int GetWorkBookIndex(string WorkbookName)
        {
            try
            {
                if (_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Null"); }
                // index を比較するために、最初から count++ しなければならない
                foreach (ExcelApps apps in _ExcelAppsList)
                {
                    if (apps.IsGhost)
                    {
                        return 0;
                    } else
                    {
                        for (int i = 1; i <= apps.Application.Workbooks.Count; i++)
                        {
                            if ( apps.Application.Workbooks[i].Name == WorkbookName)
                            {
                                return i;
                            }
                        }
                    }
                }
                return 0;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetWorkBookIndex");
                return 0;
            }
        }

        /// <summary>
        ///  ExcelManager を Close する
        /// </summary>
        public void Close()
        {
            try
            {
                _Error.AddLog(this,"Close");
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Close");
                return;
            }
        }

    }
}
