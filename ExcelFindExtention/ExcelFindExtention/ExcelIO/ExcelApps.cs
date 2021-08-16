using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace ExcelIO
{
    public class ExcelApps
    {
        protected ErrorManager.ErrorManager _Error;
        protected Utility.WindowControlUtility _WindowUtil;
        public Microsoft.Office.Interop.Excel.Application Application = null;
        public int ProcessId = 0;
        public List<string> FilePathList = new List<string>();

        public bool IsGhost = false;
        public int IsActivate = -1;　//IsActive
        public IExcelAppsEventBridgeInterface IExcelAppsEventBridge;

        protected Microsoft.Office.Interop.Excel.Workbook _TempWorkbook = null;
        protected Microsoft.Office.Interop.Excel.Worksheet _TempWorkSheet = null;
        public ExcelWorkbook OperationWorkbook = null;

        public event WorkbookEvents_WindowActivateEventHandler WindowActivate;
        public event AppEvents_WindowActivateEventHandler AppsWindowActivate;
        public event WorkbookEvents_DeactivateEventHandler WorkbookDeactivate;
        public event AppEvents_WindowDeactivateEventHandler AppsDeactivate;
        public event WorkbookEvents_SheetActivateEventHandler SheetActivate;
        public event AppEvents_WindowResizeEventHandler Application_WindowResizeHandler;
        public event WorkbookEvents_OpenEventHandler WokbookOpen;
        public event AppEvents_WorkbookOpenEventHandler AppsWorkbookOpen;
        public event AppEvents_WorkbookBeforeCloseEventHandler AppsWorkbookBeforeClose; 

        protected ActiveCellsInfo _activeCellsInfo;
        protected IExcelAppsEventBridgeInterface _excelEventBridge = null;
        //protected ExcelCellsController CellsControl = null;
        //public bool IsFirstRunApplication = true;

        public ExcelAppsConstants Constants = new ExcelAppsConstants();

        public class ExcelAppsConstants
        {
            public readonly int APPSINFO = 1;
            public readonly int WROKBOOK_NAME = 2;
        }

        public class ActiveCellsInfo
        {
            public string WorkbookName;
            public string WorksheetName;
            public string Address;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public ExcelApps(ErrorManager.ErrorManager error)
        {
            _Error = error;
            _WindowUtil = new Utility.WindowControlUtility(_Error);
            SetEventClassForWorkbookAndApplication();
            _activeCellsInfo = new ActiveCellsInfo();
        }

        public ExcelApps(ErrorManager.ErrorManager error,IExcelAppsEventBridgeInterface excelEvent)
        {
            _Error = error;
            _WindowUtil = new Utility.WindowControlUtility(_Error);
            SetEventClassForWorkbookAndApplication();
            _excelEventBridge = excelEvent;
            _activeCellsInfo = new ActiveCellsInfo();
        }

        

        private void SetActiveCellsInfo(string bookName,string sheetName,string address)
        {
            try
            {
                _activeCellsInfo.WorkbookName = bookName;
                _activeCellsInfo.WorksheetName = sheetName;
                _activeCellsInfo.Address = address;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetActiveCellsInfo");
            }
        }

        private void SetEventClassForWorkbookAndApplication()
        {
            this.WindowActivate +=
                new WorkbookEvents_WindowActivateEventHandler(
                this.Workbook_WindowActivate);
            this.AppsWindowActivate +=
                new AppEvents_WindowActivateEventHandler(this.Appliction_WindowActivate);
            this.WorkbookDeactivate +=
                new WorkbookEvents_DeactivateEventHandler(this.Workbook_WindowDeactivate);
            this.AppsDeactivate +=
                new AppEvents_WindowDeactivateEventHandler(this.AppsWindowDeactivate);
            this.SheetActivate +=
                new WorkbookEvents_SheetActivateEventHandler(this.Worksheet_Activate);
            this.Application_WindowResizeHandler +=
                new AppEvents_WindowResizeEventHandler(Application_WindowResize);
            this.WokbookOpen += new WorkbookEvents_OpenEventHandler(ExcelApps_WokbookOpen);
            this.AppsWorkbookOpen += new AppEvents_WorkbookOpenEventHandler(ExcelApps_ApplicationWokbookOpen);
            this.AppsWorkbookBeforeClose += new AppEvents_WorkbookBeforeCloseEventHandler(ExcelApps_ApplicationWorkbookBeoreClose);
        }

        private void ExcelApps_ApplicationWorkbookBeoreClose(Workbook Wb,ref bool Cancel)
        {
            try
            {
                _Error.AddLog(this.ToString() + ".ExcelApps_ApplicationWorkbookBeoreClose ExcelApps");

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ExcelApps_ApplicationWorkbookBeoreClose");
                _Error.ClearError();
            }
        }
        private void ExcelApps_ApplicationWokbookOpen(Workbook Wb)
        {
            try
            {
                _Error.AddLog(this,"ExcelApps_ApplicationWokbookOpen ExcelApps");

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ExcelApps_ApplicationWokbookOpen");
                _Error.ClearError();
            }
        }

        private void ExcelApps_WokbookOpen()
        {
            try
            {
                _Error.AddLog(this.ToString() + ".ExcelApps_WokbookOpen ExcelApps");

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ExcelApps_WokbookOpen");
                _Error.ClearError();
            }
        }

        public void SetExcelAppsEventBridgeInterface(IExcelAppsEventBridgeInterface value)
        {
            try
            {
                this._excelEventBridge = value;


            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetExcelAppsEventBridgeInterface");
            }
        }

        public bool IsActive(string bookName)
        {
            try
            {
                if (IsGhost) { return false; }
                for(int i=1; i<=Application.Workbooks.Count; i++)
                {
                    if(Application.Workbooks[i].Name == bookName)
                    {
                        if(this.IsActivate == i)
                        {
                            return true;
                        }
                    }
                }
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsActive");
                return false;
            }
        }
        private void Application_WindowResize(Workbook Wb, Window Wn)
        {
            try
            {
                _Error.AddLog(this.ToString() + ".Application_WindowResize ExcelApps : " + Wb.Name);

            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Worksheet_Activate");
            }
        }
        private void Worksheet_Activate(Object Sh)
        {
            try
            {
                _Error.AddLog(this.ToString()+".Worksheet_Activate ExcelApps : " + ((Worksheet)Sh).Name);
                // WorkbookName,SheetName,Address を取得する
                string bookName = ((Workbook)((Worksheet)Sh).Parent).Name;
                string sheetName = ((Worksheet)Sh).Name;
                string address = ((Application)((Workbook)((Worksheet)Sh).Parent).Parent).ActiveCell.Address;
                // Activate Flag をセットする
                SetActiveCellsInfo(bookName,sheetName,address);

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Worksheet_Activate");
            }
        }

        private void Application_SheetSelectionChange(Object Sh,Range Target)
        {
            try
            {
                // WorkbookName,SheetName,Address を取得する
                string bookName = ((Workbook)((Worksheet)Sh).Parent).Name;
                string sheetName = ((Worksheet)Sh).Name;
                string address = Target.Address;
                _Error.AddLog(this.ToString() + ".Application_SheetSelectionChange : ExcelApps : "
                    + bookName + " > " + sheetName + " > " + address);
                // Activate Flag をセットする
                SetActivateFlag(((Workbook)((Worksheet)Sh).Parent).Name, true);
                SetActiveCellsInfo(bookName, sheetName, address);
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Application_SheetSelectionChange");
            }
        }

        private void AppsWindowDeactivate(Workbook wb,Window wn)
        {
            try
            {
                // 別のエクセル Wrokbook が Active になったときに実行される
                _Error.AddLog(this.ToString()+ ".AppsWindowDeactivate");
                // Activate Flag をセットする
                SetActivateFlag(wb.Name, false);
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".AppsWindowDeactivate");
                _Error.ClearError();
                return;
            }
        }

        private void Workbook_WindowDeactivate()
        {
            try
            {
                _Error.AddLog(this.ToString() + ".Workbook_WindowDeactivate");
            } catch ( Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Workbook_WindowDeactivate");
                return;
            }
        }

        private void Workbook_WindowActivate(Window Wn)
        {
            try
            {
                _Error.AddLog(this.ToString()+".Workbook_WindowActivate ExcelApps : " + Wn.Caption);

                string bookName = Wn.Application.ActiveWorkbook.Name;
                string sheetName = ((Worksheet)Wn.Application.ActiveSheet).Name;
                string address = Wn.Application.ActiveCell.Address;
                SetActiveCellsInfo(bookName, sheetName, address);
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Workbook_WindowActivate");
                return;
            }
        }

        private void Appliction_WindowActivate(Workbook Wb,Window Wn)
        {
            try
            {
                string bookName = Wn.Application.ActiveWorkbook.Name;
                string sheetName = ((Worksheet)Wn.Application.ActiveSheet).Name;
                string address = Wn.Application.ActiveCell.Address;
                SetActiveCellsInfo(bookName, sheetName, address);
                _Error.AddLog(this.ToString()+".Appliction_WindowActivate ExcelApps : ");
                _Error.AddLog("  " + Wb.Name + " > " + Wb.ActiveSheet.Name + " > " + address);
                SetActivateFlag(Wb.Name, true);
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Appliction_WindowActivate");
                return;
            }
        }

        public void SetActivateFlag()
        {
            try
            {
                _Error.AddLog("SetActivateFlag");
                if(IsGhost) { return; }
                string wbName = this.GetActiveWorkbookName();
                if (_Error.hasAlert) { _Error.AddLogAlert("GetActiveWorkbookName Failed"); }
                SetActivateFlag(wbName, true);
            } catch (Exception ex)
            {
                _Error.AddException(ex, this, ".SetActivateFlag");
            }
        }

        public void SetActivateFlag(string bookName,bool flag)
        {
            try
            {
                if (GetWorkBooksCount() > 0)
                {
                    for(int i = 1; i<=Application.Workbooks.Count; i++)
                    {
                        if(bookName == Application.Workbooks[i].Name)
                        {
                            if (flag)
                            {
                                IsActivate = i;
                                return;
                            } else
                            {
                                IsActivate = 0;
                                return;
                            }
                        }
                    }
                }
                _Error.AddException(
                    new Exception("WorkbookName Not Exists In Application"),
                    this.ToString() + ".SetActivateFlag",
                    _Error.Constants.TYPE_WARNING);
                IsActivate = 0;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetActivateFlag");
                return;
            }
        }
        public void SetWorkbookEvent()
        {
            try
            {
                if (IsGhost) { return; }
                if (Application == null) { throw new Exception("Application Is Null"); }
                // Excel.Application.Workbooks から FilePath を取得する
                for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                {
                    Application.Workbooks[i].WindowActivate += this.WindowActivate;
                    Application.Workbooks[i].Deactivate += this.WorkbookDeactivate;
                    Application.Workbooks[i].SheetActivate += this.SheetActivate;
                    Application.Workbooks[i].Open += this.WokbookOpen;
                }
                Application.WindowActivate += AppsWindowActivate;
                Application.WindowDeactivate += AppsDeactivate;
                Application.WindowResize += this.Application_WindowResizeHandler;
                Application.WorkbookOpen += this.AppsWorkbookOpen;
                this.Application.WorkbookBeforeClose += AppsWorkbookBeforeClose;

                //Application.SheetSelectionChange -= new AppEvents_SheetSelectionChangeEventHandler(Application_SheetSelectionChange);
                Application.SheetSelectionChange += new AppEvents_SheetSelectionChangeEventHandler(Application_SheetSelectionChange);

                if (_excelEventBridge != null)
                {
                    // Excel.Application.Workbooks にイベントを付与する
                    for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                    {
                        _Error.AddLog(this.ToString()+".SetWorkbookEvent : " + Application.Name + ":" + Application.Workbooks[i].Name);
                        //Application.Workbooks[i].WindowActivate -= this._excelEventBridge.Workbook_WindowActivateEvent;
                        Application.Workbooks[i].WindowActivate += this._excelEventBridge.Workbook_WindowActivateEvent;
                        //Application.Workbooks[i].Deactivate -= this._excelEventBridge.Workbook_DeactivateEvent;
                        Application.Workbooks[i].Deactivate += this._excelEventBridge.Workbook_DeactivateEvent;
                        //Application.Workbooks[i].SheetActivate += this.Worksheet_Activate;
                        //System.Threading.Thread.Sleep(5);
                        //Application.Workbooks[i].SheetActivate -= this._excelEventBridge.WorkSheet_ActivateEvent;
                        Application.Workbooks[i].SheetActivate += this._excelEventBridge.WorkSheet_ActivateEvent;
                    }
                    //Application.WindowActivate -= _excelEventBridge.Application_WindowActivateEvent;
                    Application.WindowActivate += _excelEventBridge.Application_WindowActivateEvent;
                    //Application.WindowDeactivate -= _excelEventBridge.Application_DeactivateEvent;
                    Application.WindowDeactivate += _excelEventBridge.Application_DeactivateEvent;
                    //Application.SheetSelectionChange -= _excelEventBridge.Application_SheetSelectionChangeEvent;
                    Application.SheetSelectionChange += _excelEventBridge.Application_SheetSelectionChangeEvent;
                    Application.WorkbookOpen += _excelEventBridge.Application_WorkbookOpenEvent;
                    Application.WorkbookBeforeClose += _excelEventBridge.Application_WorkbookBeforeCloseEvent;
                    Application.SheetActivate += this._excelEventBridge.Application_SheetActivateEvent;
                }
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetWorkbookEvent");
                return;
            }
        }
        public void CreateApplication()
        {
            try
            {
                Application = new Microsoft.Office.Interop.Excel.Application
                {
                    Visible = true
                };
                //IsFirstRunApplication = true;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".CreateApplication");
                return;
            }
        }

        public void SetAppliction(Application application)
        {
            this.Application = application;
        }


        public bool WorkbookIsActive(AppsInfo info)
        {
            try
            {
                if (info.FileName == Application.ActiveWorkbook.Name)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".WorkbookIsActive");
                return false;
            }
        }

        public string GetActiveCellsAddress()
        {
            try
            {
                _Error.AddLog(this,"GetActiveCellsAddress");
                if (IsGhost) { _Error.AddLog("  Excel.Application is Ghost Process"); return ""; }

                string activeCellAddress = _activeCellsInfo.Address;
                if ((activeCellAddress == null) || (activeCellAddress == ""))
                {
                    activeCellAddress = Application.ActiveCell.Address;
                }
                return activeCellAddress;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetActiveCellsAddress");
                return "";
            }
        }

        public string GetActiveSheetName()
        {
            try
            {
                _Error.AddLog(this, "GetActiveSheetName");
                if (IsGhost) { _Error.AddLog("  GetActiveSheetName IsGhost=true"); return ""; }
                string sheetName = _activeCellsInfo.WorksheetName;
                if(Application.ActiveWorkbook == null) { _Error.AddLogAlert("  Application.ActiveWorkbook == null"); return ""; }
                if ((sheetName != null)||(sheetName != ""))
                {
                    sheetName = ((Worksheet)Application.ActiveWorkbook.ActiveSheet).Name;
                }
                _Error.AddLog(this.ToString()+".GetActiveSheetName : sheetName=" + sheetName);
                return sheetName;
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetActiveSheetName");
                return "";
            }
        }


        public string GetWorkbookPathFromName(string bookName)
        {
            try
            {
                if (IsGhost) { return ""; }
                if (Application == null) { throw new Exception("Application Is Null"); }
                // Excel.Application.Workbooks から FilePath を取得する
                for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                {
                    if (this.Application.Workbooks[i].Name.Contains(bookName))
                    {
                        return Application.Workbooks[i].Path + "\\" + Application.Workbooks[i].Name;
                    }
                }
                return "";
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetWorkbookPathFromName");
                return "";
            }
        }

        // Workbook 名リストを取得する
        public List<string> GetWorkbookNameList()
        {
            List<string> retList = new List<string>();
            try
            {
                if (IsGhost) { return retList; }
                if (Application == null) { throw new Exception("Application Is Null"); }
                // Excel.Application.Workbooks から FilePath を取得する
                for (int i = 1; i <= this.Application.Workbooks.Count; i++)
                {
                    retList.Add(this.Application.Workbooks[i].Name);
                }
                return retList;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetWorkbookNameList");
                return retList;
            }
        }

        // Excel.Application.Hwnd から ProcessId を取得する
        public void SetProcessIdFromExcelApplication()
        {
            this.ProcessId = GetProcessIdFromExcelApplicationHwnd();
        }

        // Excel.Application.Hwnd から ProcessId を取得する
        private int GetProcessIdFromExcelApplicationHwnd()
        {
            int ret = 0;
            try
            {
                if (IsGhost) { return 0; }
                if(this.Application == null) { throw new Exception("Excel.Application Is Null."); }
                //Console.WriteLine("application window activewindow  : " + Application.ActiveWindow.ToString());

                //if (Application.ActiveWindow == null)
                //{
                //    _Error.AddLog("(Application.ActiveWindow is null");
                //} else
                //{
                //    //Console.WriteLine("application window visible : " + Application.Visible);
                //}
                _Error.AddLog("  Application.Visible=" + Application.Visible);
                _ = GetWindowThreadProcessId((IntPtr)this.Application.Hwnd, out uint uintbuf);
                return (int)uintbuf;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetProcessIdFromExcelApplicationHwnd");
                return ret;
            }
        }

        // 開いているか判定する、Update したのちに実行する
        public bool IsOpendFile(string path)
        {
            try
            {
                string opendPath;
                if (this.Application == null) { return false; }
                // Excel.Application.Workbooks から FilePath を取得する
                bool ret = false;
                for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                {
                    opendPath = this.Application.Workbooks[i].Path + "\\" + this.Application.Workbooks[i].Name;
                    if (path.Contains(opendPath))
                    {
                        if (IsFileLocked(path))
                        {
                            ret = true;
                        } else
                        {
                            if (Application.Workbooks[i].ReadOnly)
                            {
                                // ロックされていなくても、
                                // 読み取り専用の場合、ファイルを開いていても、ロックされていないので、開いていない判定になる
                                ret = true;
                            }
                        }
                    }
                }

                for(int i = 1; i < this.Application.Windows.Count + 1; i++)
                {
                    string wncaption = this.Application.Windows[i].Caption;
                    if (wncaption.Contains(System.IO.Path.GetFileName(path)))
                    {
                        if (IsFileLocked(path))
                        {
                            ret = true;
                        }
                    }
                }
                return ret;
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsOpendFile");
                return false;
            }
        }

        public void AddFilePathListIfValueNotExists(string filePath)
        {
            try
            {
                if (FilePathList == null) {
                    FilePathList = new List<string>();
                }
                if (FilePathList.Count < 1) {
                    FilePathList.Add(filePath);
                } else
                {
                    bool flag = false;
                    foreach (string buf in FilePathList)
                    {
                        if (buf.Contains(filePath)) { flag = true; }
                    }
                    if (!flag)
                    {
                        FilePathList.Add(filePath);
                    }
                }

            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".AddFilePathListIfValueNotExists");
                return;
            }
        }

        // path から Workbook を開き、さらに、Excel.Application を取得しセットする
        public void SetApplicationFromFilePath()
        {
            try
            {
                _Error.AddLog(this, "SetApplicationFromFilePath");
                // プロセスのみがある Ghost Process のときは処理しない
                if (IsGhost) { _Error.AddLog(" IsGhost=true"); return; }
                if (FilePathList == null) { throw new Exception("PathList Is Null"); }
                if (FilePathList.Count < 1) { throw new Exception("PathList.Count Is Zero"); }

                foreach (string path in FilePathList)
                {
                    SetApplicationFromFilePath(path);
                    if (_Error.hasError) { throw new Exception("SetApplicationFromFilePath Failed"); }
                    if (this.Application != null) { break; }
                }

            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetApplicationFromFilePath");
                return;
            }
        }

        // path から Excel.Application を取得しセットする（メイン）
        private void SetApplicationFromFilePath(string path)
        {
            try
            {
                var envPath = Environment.ExpandEnvironmentVariables(path);

                if (_TempWorkbook == null)
                {
                    // Workbook を path からまだ開いていない
                    _TempWorkbook = Marshal.BindToMoniker(envPath) as Microsoft.Office.Interop.Excel.Workbook;
                }
                // ExcelApps の Workbook がある
                string buf = _TempWorkbook.Path + "\\" + _TempWorkbook.Name;
                // Workbook の Path が引数 path のものならば、この Workbook から Application をセットする
                if (!buf.Contains(path))
                {
                    // Workbook の Path がほかの path のものならば開きなおす
                    _TempWorkbook = Marshal.BindToMoniker(envPath) as Microsoft.Office.Interop.Excel.Workbook;
                }
                if (_TempWorkbook == null)
                {
                    throw new Exception("path is invalid [" + path + "]");
                }
                this.Application = _TempWorkbook.Application;
                return;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetApplicationFromFilePath_path");
                return;
            }
        }

        // ファイルパスリストをセットする
        // プロセスのハンドル内で保持している(開いている)エクセルファイルパスを取得し
        // その中でファイルロックされているものを保持する
        public void ReSetFileListFromApplication()
        {
            try
            {
                if (IsGhost) {
                    return;
                    //throw new Exception("ExcelApps Is Ghost Process");
                }
                if (Application == null) { throw new Exception("Application Is Null"); }

                _Error.AddLog("** Pid = "+this.ProcessId);

                FilePathList = new List<string>();
                string bufPath = "";
                // Excel.Application.Workbooks から FilePath を取得する
                for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                {
                    bufPath = this.Application.Workbooks[i].Path + "\\" + this.Application.Workbooks[i].Name;
                    if (IsFileLocked(bufPath))
                    {
                        FilePathList.Add(bufPath);
                        _Error.AddLog(this.ToString() + ".ReSetFileListFromApplication path=", bufPath);
                    }
                }
                SetIsGhost();
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetFileListFromApplication");
                return;
            }
        }

        public void UpdateThisInfo()
        {
            try
            {
                _Error.AddLog(this, "UpdateThisInfo");
                SetIsGhost();
            } catch (Exception ex)
            {
                _Error.AddException(ex, this, "UpdateThisInfo");
            }
        }

        private void SetIsGhost()
        {
            try
            {
                _Error.AddLog(this, "SetIsGhost");
                if(this.Application == null) { return; }
                if(this.Application.Workbooks.Count < 1) {
                    // count < 1 true
                    if (!IsGhost)
                    {
                        IsGhost = true; _Error.AddLog(" IsGhost=>true");
                    }
                }                    
                else {
                    // count >= 1 false
                    if (IsGhost)
                    {
                        IsGhost = false; _Error.AddLog(" IsGhost=>false");
                    }
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this, "SetIsGhost");
            }
        }

        // ファイルパスがロックしているか判定する
        private bool IsFileLocked(string filePath)
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


        public string GetActiveWorkbookName()
        {
            try
            {
                _Error.AddLog(this,"GetActiveWorkbookName");
                if (IsGhost) { _Error.AddLogWarning(this.ToString()+".GetActiveWorkbookName IsGhost=true");  return ""; }
                string bookName = _activeCellsInfo.WorkbookName;
                if ((bookName == null) || (bookName == ""))
                {
                    if (Application.Workbooks.Count > 0)
                    {
                        bookName = Application.ActiveWorkbook.Name;
                    } else
                    {
                        _Error.AddLogWarning(this.ToString() + ".GetActiveWorkbookName Application.Workbooks.Count < 1");
                    }
                }
                return bookName;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex,this.ToString()+ ".GetActiveWorkbookName");
                return "";
            }
        }

        public bool IsExistsActiveWorkbook()
        {
            if (this.IsActivate < 0) { return false; }
            return true;
        }

        private object GetActiveInfo(int InfoKind)
        {
            try
            {
                _Error.AddLog(this.ToString()+ ".GetActiveInfo");
                if (IsGhost) { return ""; }
                if (IsExistsActiveWorkbook())
                {
                    for (int i = 1; i <= Application.Workbooks.Count; i++)
                    {
                        if (this.IsActivate == i)
                        {
                            switch (InfoKind)
                            {
                                case 1: // Constants.APPSINFO
                                    AppsInfo info = new AppsInfo
                                    {
                                        FileName = Application.Workbooks[i].Name,
                                        Index = i,
                                        ProcessId = this.ProcessId
                                    };
                                    return info;
                                case 2: // Constants.WORKBOOK_NAME
                                    return Application.Workbooks[i].Name;
                                default:
                                    break;
                            }
                        }
                    }
                } else
                {
                    // IsActivate が 0 以下のとき-1
                    // アプリ起動時、Update 未実行時
                    if (Application == null) { throw new Exception("Application is Null"); }
                    if (Application.Workbooks.Count < 1) { throw new Exception("Application.Workbooks.Count < 1"); }

                    string activeBookName = Application.ActiveWorkbook.Name;

                    for (int i = 1; i <= Application.Workbooks.Count; i++)
                    {
                        if (Application.Workbooks[i].Name.Equals(activeBookName))
                        {
                            switch (InfoKind)
                            {
                                case 1: // Constants.APPSINFO
                                    AppsInfo info = new AppsInfo
                                    {
                                        FileName = Application.Workbooks[i].Name,
                                        Index = i,
                                        ProcessId = this.ProcessId
                                    };
                                    IsActivate = i;
                                    return info;
                                case 2: // Constants.WORKBOOK_NAME
                                    IsActivate = i;
                                    return Application.Workbooks[i].Name;
                                default:
                                    IsActivate = i;
                                    break;
                            }
                        }
                    }
                }

                return null;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetActiveInfo");
                return null;
            }
        }

        public void ActivateWorkbook(AppsInfo info)
        {
            try
            {
                //ExcelApps apps = GetExcelAppsFromAppsInfo(info);
                //if (_Error.HasException()) { return; }
                //ExcelApps apps = this;
                if (!IsActive(info.FileName))
                {
                    this.Application.Workbooks[info.FileName].Activate();
                }
                // ウィンドウを最前面へ、最小化時は元に戻す
                _WindowUtil.WakeupWindow(this.ProcessId);
                if (_Error.HasException()) { return; }

            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ActivateWorkbook");
                return;
            }
        }


            // 最後に追加したファイルを開く
            public void OpenFileLastIndex(int index = -1)
        {
            try
            {
                if (this.Application == null)
                {
                    Application = (Microsoft.Office.Interop.Excel.Application)
                        System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                }
                if (index == -1)
                {
                    index = FilePathList.Count;
                } else if (index > 0)
                    //index = index;
                {
                } else
                {
                    throw new Exception("Index Is Invalid");
                }

                this._TempWorkbook = this.Application.Workbooks.Open(FilePathList[index]);                

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".OpenFile");
                return;
            }
        }

        public void OpenFileAByApplication(string filePath)
        {
            try
            {
                _Error.AddLog(this, "OpenFileAByApplication");
                //this.FilePathList.Add(filePath);
                this._TempWorkbook = this.Application.Workbooks.Open(filePath);
                //this._TempWorkbook = Marshal.BindToMoniker(filePath) as Microsoft.Office.Interop.Excel.Workbook;
                if (IsGhost) { IsGhost = false;  _Error.AddLog(" IsGhost true => false"); }

            } catch (Exception ex)
            {
                _Error.AddException(ex, this, "OpenFileAByApplication");
                return;
            }
        }

        // Workbook をアクティブにする
        public void ActivateWrokbook(string filePath)
        {
            try
            {
                string chkPath;
                for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                {
                    chkPath = this.Application.Workbooks[i].Path + "\\" + this.Application.Workbooks[i].Name;
                    if (filePath.Contains(chkPath))
                    {
                        Application.Workbooks[i].Activate();
                    }
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ActivateWrokbook");
                return;
            }
        }

        public void ActivateWorkbook(string workBookName)
        {
            try
            {
                _Error.AddLog(this, "ActivateWorkbook");
                for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                {
                    if (workBookName.Equals(Application.Workbooks[i].Name))
                    {
                        Application.Workbooks[i].Activate();
                        IsActivate = i;
                    }
                }
                _Error.AddLogWarning("ActivateWorkbook Workbook is nothing. bookName="+workBookName);
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ActivateWrokbook workBookName");
            }
        }
        public bool IsExistsWorkbookInApplication(string WorkbookName)
        {
            try
            {
                if (this.IsExsitsWorkbookOneOrMoreInApplication())
                {
                    for(int i = 1; i<=Application.Workbooks.Count; i++)
                    {
                        if(Application.Workbooks[i].Name == WorkbookName)
                        {
                            return true;
                        }
                    }
                }
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExistsWorkbookInApplication");
                return false;
            }
        }

        // Application 内に Workbook が1つ以上存在しているか
        // EXCEL プロセスが消えていない状態 (Wrokbook なしの Application) を判定する
        public bool IsExsitsWorkbookOneOrMoreInApplication()
        {
            try
            {
                if(this.Application.Workbooks.Count > 0)
                {
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExsitsWorkbookOneOrMoreInApplication");
                return false;
            }
        }
        // 
        public void ApplicationVisible(bool flag)
        {
            try
            {
                this.Application.Visible = flag;
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ApplicationVisible");
                return;
            }
        }

        // 操作元 Wrokbook(WorkbookOperation) に Application.Wrokbook をセットする
        public void SetWorkbookForOperation(string filePath)
        {
            try
            {
                string chkPath;
                for (int i = 1; i < this.Application.Workbooks.Count + 1; i++)
                {
                    chkPath = this.Application.Workbooks[i].Path + "\\" + this.Application.Workbooks[i].Name;
                    if (filePath.Contains(chkPath))
                    {
                        this.OperationWorkbook.MainObject = Application.Workbooks[i];
                    }
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetWorkbookForOperation");
                return;
            }
        }

        public int GetWorkBooksCount()
        {
            try
            {
                if (Application == null) { return 0; }
                return Application.Workbooks.Count;
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetWorkBooksCount");
                return -1;
            }
        }


        public void SelectAddress(string WorkbookName, string worksheetName, string address)
        {
            try
            {
                _Error.AddLog(this.ToString() + ".SelectAddress : " + Application.Workbooks[WorkbookName].Name);

                string sheetName = Application.Workbooks[WorkbookName].Sheets[worksheetName].Name;
                string rangeAddress = ((Worksheet)Application.Workbooks[WorkbookName].Sheets[worksheetName]).Range[address].Address;
                ExcelCells cells = new ExcelCells(_Error);
                //.WriteLine((Application.Workbooks[WorkbookName].Sheets[worksheetName]).Range[address].Value);
                string rangeValue = cells.GetValueCornerAddress(Application, WorkbookName, worksheetName, address);
                
                // Activate
                ((Worksheet)Application.Workbooks[WorkbookName].Sheets[worksheetName]).Activate();
                object obj = ((Worksheet)Application.Workbooks[WorkbookName].Sheets[worksheetName]).Range[address].Select();
                
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                _Error.AddException(ex, this.ToString() + ".SelectAddress COMException");
                return;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SelectAddress");
                return;
            }
        }

        /// <summary>
        /// Close する
        /// </summary>
        public void Close()
        {
            try
            {
                _Error.AddLog(this,"Close");
                if (OperationWorkbook != null)
                {
                    OperationWorkbook.Close();
                    OperationWorkbook = null;
                }
                if (_TempWorkbook != null)
                {
                    _TempWorkbook.Close();
                    _TempWorkbook = null;
                }

                if (Application != null)
                {
                    Worksheet sheet = null;
                    for(int i=1; i<=this.Application.Workbooks.Count; i++)
                    {
                        for(int j=1; j<=Application.Workbooks[i].Sheets.Count; j++)
                        {
                            sheet = Application.Workbooks[i].Worksheets[j];
                            sheet = null;
                        }
                    }
                    Application.Quit();
                    Application = null;
                }


            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Close");
                return;
            }
        }

        public void Dispose()
        {
            try
            {
                
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Dispose");
                return;
            }
        }

    }
}
