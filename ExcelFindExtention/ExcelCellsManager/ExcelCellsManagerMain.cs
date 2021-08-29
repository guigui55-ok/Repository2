using CellsManagerControl.Utility;
using ExcelCellsManager.CellsValuesConrol.CellsValuesList;
using ExcelCellsManager.ExcelWorkbookList;
using ExcelIO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utility;
using ExcelCellsManager.ExcelCellsManager;
using ExcelCellsManager.Settings;
using ExcelCellsManager.ExcelCellsManager.Event;
using ExcelCellsManager.ExcelCellsManager.Settings;
using System.Threading;
using ExcelCellsManager.ExcelCellsManager.SettingsForm;
using MousePointCapture;
using System.Drawing;

namespace ExcelCellsManager
{
    public class ExcelCellsManagerMain
    {
        protected ErrorManager.ErrorManager _error;
        // Control.Utility
        public IWorkbookListControl CheckdListUtil;
        public IDataGridViewUtility DataGridUtil;
        public IDataGridViewItems DataGridViewItems;
        
        // Excel Relation
        public ExcelManager ExcelManager;
        protected ExcelCellsManager.ExcelCellsManager _excelCellsManager;
        // ExcelEvent ToParentObjectEvent
        protected IExcelAppsEventBridgeInterface _excelEventBridge;
        public EventHandler ChangeActiveCell;
        public EventHandler DataGridView_EditEvnet;
        // MouseCapture Edge
        protected MousePointCapture.MouseCaptureInScreenEdgeManager _mouseCapture;
        protected MouseCaptureInScreenEdgeFormEvents _mouseCaptureEdgeFormEvents;
        // Etc
        public OpenedFile OpenedFile;
        protected string _fileType = ".tsv";
        protected string _saveAsDefaultFileName = "NewFile";
        public string DefaultNewFileName = "NewFile";
        protected char delimiter = '\t';
        // Constants
        public ExcelCellsManagerConstants Constants;
        // settings
        public ExcelCellsManagerSettingsValue AppsSettings;
        public ExcelCellsManagerState AppsState;
        public InitializeSettingsValue InitializeSettingsValue;
        public EcmSettingsFormManager AppsSettingsFormManager;

        public event EventHandler StatusBarChangeTextEvent;
        public ProgressDialog.FormWindow.ProgressDialogManager ProgressDialogManager;
        public ProgressDialog.DoWork.ProgressDialogDoWork ProgresDialogDowork;
        public bool IsWorkbookOpening;
        protected ExcelCellsManagerForm _mainForm;
        
        public ExcelCellsManagerMain(ErrorManager.ErrorManager error,Form form)
        {
            _error = error;
            ExcelManager = new ExcelManager(_error);
            _excelCellsManager = new ExcelCellsManager.ExcelCellsManager(_error, 1);
            _excelEventBridge = new ExcelPublicEventHandlerBredgeForCellManager(_error);
            ExcelManager.ExcelEventBridge =  _excelEventBridge;
            _saveAsDefaultFileName += _fileType;
            // MouseCapture
            // capture <= EdgeForm
            _mouseCapture = new MousePointCapture.MouseCaptureInScreenEdgeManager(_error,form);
            _mouseCaptureEdgeFormEvents = _mouseCapture.EdgeFormEvents;
            // SettingsForm
            AppsSettingsFormManager = new EcmSettingsFormManager(_error, form);
            AppsSettingsFormManager.SettingsForm.VisibleChanged += SettingsForm_VisibleChanged;
            AppsSettingsFormManager.ButtonApply_ClickEvent += SettingsForm_ButtonApplyClick;

            ProgressDialogManager = new ProgressDialog.FormWindow.ProgressDialogManager(_error, form);
            ProgresDialogDowork = new ProgressDialog.DoWork.ProgressDialogDoWork(_error);
            _mainForm = (ExcelCellsManagerForm)form;
        }

        public void SetStatusBarText(string msg)
        {
            try
            {

            } catch (Exception ex)
            {

            }
        }

        public void Test()
        {
            _error.Messenger.ShowAlertMessage("ShowErrorMessage test message\n2line");
        }
        // Event
        private void SettingsForm_ButtonApplyClick(object sender,EventArgs e)
        {
            _error.AddLog(this, "SettingsForm_ButtonApplyClick");
            this.SaveSettings();
        }
        // Event
        // 設定フォームを閉じたときのイベント
        private void SettingsForm_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this, "SettingsForm_VisibleChanged");
                // SettingsObject から ExcellCellsManagerMain.AppsSetting.Value へ格納する
                if (!AppsSettingsFormManager.SettingsForm.Visible)
                {
                    // フォームが閉じられたときに格納する
                    AppsSettings.SettingsValueToThisMemberMain();
                    if (_error.hasAlert) { _error.AddLogAlert("SettingsValueToThisMemberMain Failed"); }
                    // Capture クラスへ反映する
                    SetSettingValueToCaptureForm();
                    if (_error.hasAlert) { _error.AddLogAlert("SetSettingValueToCaptureForm Failed"); }
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SettingsForm_VisibleChanged");
            }
        }

        public void SetDataGridViewAddEvent()
        {
            try
            {
                DataGridUtil.DataGridView_AddRow += DataGridView_EditRow;
                DataGridUtil.DataGridView_InsertRow += DataGridView_EditRow;
                DataGridUtil.DataGridView_DeleteRow += DataGridView_EditRow;
                DataGridUtil.DataGridView_ChangeValue += DataGridView_EditRow;
            } catch(Exception ex)
            {
                { _error.AddException(ex, this.ToString() + ".SetDataGridViewAddEvent"); }
            }
        }

        // IsEdited フラグを false にセットする
        public void ResetEdited()
        {
            try
            {
                _error.AddLog(this.ToString() + ".ResetEdited");
                AppsState.IsEdited = false;
                DataGridView_EditEvnet?.Invoke(null, EventArgs.Empty);
                return;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ResetEdited");
            }
        }

        // Event
        private void DataGridView_EditRow(object sender,EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".DataGridView_EditRow");
                // 編集中でなければ編集中の状態にする
                if (!AppsState.IsEdited)
                {
                    AppsState.IsEdited = true;
                    DataGridView_EditEvnet?.Invoke(sender, e);
                }                    
            } catch (Exception ex)
            { _error.AddException(ex, this.ToString() + ".DataGridView_AddRow"); }
        }

        // アクティブセルをセットした時の追加処理
        public void SetActiveCellWithEvent(string bookName,string sheetName,string address)
        {
            try
            {
                if (_excelCellsManager.ActiveCellsInfo == null)
                {
                    _excelCellsManager.ActiveCellsInfo = new ExcelApps.ActiveCellsInfo();
                }
                // ActiveCell を保持しておく
                _excelCellsManager.ActiveCellsInfo.Address = address;
                _excelCellsManager.ActiveCellsInfo.WorkbookName = bookName;
                _excelCellsManager.ActiveCellsInfo.WorksheetName = sheetName;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetActiveCellWithEvent");
            }
        }
        // CaptureFormに設定値をセットする、settings クラスから
        public void SetSettingValueToCaptureForm()
        {
            try
            {
                TypeUtility util = new TypeUtility(_error);
                _error.AddLog(this.ToString()+ ".SetSettingValueToCaptureForm");

                bool bleft = false, bright = false, btop = false, bbottom = false;
                int[] screenNumbers = new int[4];
                int[] buf;
                buf = AppsSettings.MouseCaptureSettingLeft;
                int[] leftVal = new int[] { buf[1],buf[2] };
                _error.AddLog("MouseCaptureSettingLeft = " + util.ConvertIntArrayToString(leftVal));
                screenNumbers[0] = buf[3];
                if (buf[0] == 1) { bleft = true; }
                buf = AppsSettings.MouseCaptureSettingTop;
                int[] topVal = new int[] { buf[1], buf[2] };
                _error.AddLog("MouseCaptureSettingTop = " + util.ConvertIntArrayToString(topVal));
                screenNumbers[1] = buf[3];
                if (buf[0] == 1) { btop = true; }
                buf = AppsSettings.MouseCaptureSettingRight;
                int[] rightVal = new int[] { buf[1], buf[2] };
                _error.AddLog("MouseCaptureSettingRight = " + util.ConvertIntArrayToString(rightVal));
                screenNumbers[2] = buf[3];
                if (buf[0] == 1) { bright = true; }
                buf = AppsSettings.MouseCaptureSettingBottom;
                int[] bottomVal = new int[] { buf[1], buf[2] };
                _error.AddLog("MouseCaptureSettingBottom = " + util.ConvertIntArrayToString(bottomVal));
                screenNumbers[3] = buf[3];
                if (buf[0] == 1) { bbottom = true; }
                _error.AddLog("screenNumbers = " + util.ConvertIntArrayToString(screenNumbers));

                //_mouseCaptureEdgeFormEvents.SetValidRangeValue()
                _mouseCapture.ResetSettings(screenNumbers, leftVal, topVal, rightVal, bottomVal);
                if (_error.hasAlert) {  _error.AddLogAlert("_mouseCapture.ResetSettings Failed"); }

                _mouseCapture.SetFormVisible(bleft, btop, bright, bbottom);
                if (_error.hasAlert) { _error.AddLogAlert("_mouseCapture.SetFormVisible Failed"); }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetSettingValueToCaptureForm");
            }
        }

        public void Initialize(ExcelCellsManagerSettingsValue settings = null)
        {
            try
            {
                // 設定値読み込み済み
                _error.ReleaseErrorState();
                _error.AddLog(this.ToString() + ".Initialize");
                // フィールド名を取得する
                string[] fieldNames = DataGridViewItems.GetFieldNames();
                // Initialize
                // DataGridView に ColumnName を設定する
                DataGridUtil.Initialize(fieldNames);
                if (_error.hasError) { _error.ReleaseErrorState(); } 

                // DataGridView の ColumnWidth を設定する
                DataGridUtil.SetColumnWidth(AppsSettings.ColumnWidthOfDataList);
                if (_error.hasError) { _error.ReleaseErrorState(); }

                // ここでファイル・設定読み込みなどする？※※
                if (settings != null) { AppsSettings = settings; }
                else
                { 
                    // 引数があれば引数を使う、なければ現在の Object を使う
                    _error.AddLog("Augument settings is null.app Use Already Prepared SetttingsValueClassObject"); 
                }
                // 現在のオブジェクト AppsSetting チェック
                if (this.AppsSettings == null) { 
                    _error.AddException(new Exception("Settings Object Is Null."));
                    _error.ReleaseErrorState();
                }　else
                {
                    // AppsSettings がある状態で実行する
                    InitializeMouseCaptureClass();
                    if (_error.hasError){ 
                        _error.AddLogAlert("InitializeMouseCaptureClass Failed"); 
                        //_error.ReleaseErrorState();
                        return;
                    }
                }

                // SettingsForm の初期化
                AppsSettingsFormManager.PlaceControlSettingsListToForm(0);
                if (_error.hasError) { _error.ReleaseErrorState(); }

                // SettingsFormEvent を設定する

                // 前回開いていたファイルがあればセットする
                InitializeOpenFilePath();
                if (_error.hasError) { _error.ReleaseErrorState(); }

                // 最後にエラーがあればまとめて出力するので、エラーを保持しておく
                //if (_error.hasError) { _error.ReleaseErrorState(); ; }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
            }
        }

        // 設定値を読み込む前に Object をセットする 長くなるので別クラスで実行
        //public void InitializeSettings()
        //{
        //        List<SettingsManager.ISettingsObject> SettingsList = new List<SettingsManager.ISettingsObject>();

        //        //
        //        ISettingsObject temp = new SettingsObject();
        //        // アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする
        //        temp.Name = "IsActivateWorkbookWindowAfterRegistAddress";
        //        temp.Description = "アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする";
        //        temp.ValueType = typeof(bool);
        //        temp.InitialValue = true;　//.......

        //}
        private void InitializeOpenFilePath()
        {
            try
            {
                // NewFile をセットする、DataGridView 設定後に行うこと
                this.OpenedFile.DefaultNewFileName = Constants.DefalutNewFileName + Constants.DefaultFileType;
                // 設定値から前回開いていたのファイルパスを取得する
                string path = this.AppsSettings.LastOpendFilePath;
                if (path.Equals(OpenedFile.DefaultNewFileName))
                {
                    // 新規作成時と同じ値なら以下に書き換え、違う(すでに設定値にパスがある)ときはそのファイルパスを開く
                    path = this.OpenedFile.SaveFileDialogLastDirectory + "\\"
                        + Constants.DefalutNewFileName + Constants.DefaultFileType;
                }
                else
                {
                    // 開く
                    this.OpenFile(path, false);
                }
                // 開いたファイルをセットする
                this.OpenedFile.SetFilePath(path);
                if (_error.hasAlert) { _error.ReleaseErrorState(); }
            } catch(Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".InitializeOpenFilePath");
            }
        }

        private void InitializeMouseCaptureClass()
        {
            try
            {
                // capture settings
                _mouseCapture.Settings.IsValidCapture = AppsSettings.IsActivateFormWhenMoveMouseScreenEdge;
                // 設定値を読み込む Capture クラスへ
                // left
                if (AppsSettings.MouseCaptureSettingLeft[0] == 1) { _mouseCapture.Settings.IsCaputureLeft = true; }
                else { _mouseCapture.Settings.IsCaputureLeft = false; }
                _mouseCapture.Settings.LeftValidRange =
                    new int[] { AppsSettings.MouseCaptureSettingLeft[1], AppsSettings.MouseCaptureSettingLeft[2] };
                // top
                if (AppsSettings.MouseCaptureSettingTop[0] == 1) { _mouseCapture.Settings.IsCaptureTop = true; }
                else { _mouseCapture.Settings.IsCaptureTop = false; }
                _mouseCapture.Settings.TopValidRange =
                    new int[] { AppsSettings.MouseCaptureSettingTop[1], AppsSettings.MouseCaptureSettingTop[2] };
                // right
                if (AppsSettings.MouseCaptureSettingRight[0] == 1) { _mouseCapture.Settings.IsCaptureRight = true; }
                else { _mouseCapture.Settings.IsCaptureRight = false; }
                _mouseCapture.Settings.RightValidRange =
                    new int[] { AppsSettings.MouseCaptureSettingRight[1], AppsSettings.MouseCaptureSettingRight[2] };
                // bottom
                if (AppsSettings.MouseCaptureSettingBottom[0] == 1) { _mouseCapture.Settings.IsCaptureBottom = true; }
                else { _mouseCapture.Settings.IsCaptureBottom = false; }
                _mouseCapture.Settings.BottomValidrange =
                    new int[] { AppsSettings.MouseCaptureSettingRight[1], AppsSettings.MouseCaptureSettingRight[2] };


                // EdgeForm を表示する
                _mouseCapture.InitializeForms();
                if (_error.hasAlert) { _error.AddLogAlert(new Exception("InitializeForms Method Failed")); _error.ReleaseErrorState(); }
                _mouseCapture.SetFlagIsValidCapture(_mouseCapture.Settings.IsValidCapture);
                if (_error.hasAlert) { _error.AddLogAlert(new Exception("SetFlagIsValidCapture Method Failed")); _error.ReleaseErrorState(); }
                // 設定値 IsValidCapture が false の時は表示しない
                if (_mouseCapture.Settings.IsValidCapture)
                {
                    _mouseCapture.ShowForms();
                    if (_error.hasAlert) { _error.AddLogAlert(new Exception("ShowForms Method Failed")); _error.ReleaseErrorState(); }
                }
                if (_error.hasAlert) { _error.AddLogAlert(new Exception("CaptureFormManager Method Failed")); _error.ReleaseErrorState(); }
                // MouseCapture クラスの設定値を AppsSettings クラスから取得し設定する
                SetSettingValueToCaptureForm();
                if (_error.hasAlert) { _error.AddLogAlert(new Exception("SetSettingValueToCaptureForm Method Failed")); _error.ReleaseErrorState(); }
            } catch(Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".InitializeMouseCaptureClass");
            }
        }

        public bool FileTypeIsTsv(string filePath)
        {
            try
            {
                if (System.IO.Path.GetExtension(filePath).Equals(_fileType)) { return true; }
                return false;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".FileTypeIsTsv");
                return false;
            }
        }

        // ExcelManager から ActiveCell を取得して設定する
        public void SetActiveCell()
        {
            try
            {                
                _error.ClearError();
                _error.AddLog(this.ToString() + ".SetActiveCell");
                ExcelApps apps = this.ExcelManager.GetActiveExcelApps();
                if (_error.hasError) {
                    //throw new Exception("ExcelManager.GetActiveExcelApps Faild"); 
                    _error.AddLogAlert("ExcelManager.GetActiveExcelApps Faild");
                    return;
                }
                if (apps == null)
                {
                    //throw new Exception("GetActiveExcelApps.ExcelApps Is Null"); 
                    _error.AddLog("ExcelApps.ExcelApps Is Null");
                    apps = ExcelManager.GetFirstObject();
                    if (apps == null) { throw new Exception("ExcelApps.ExcelApps Is Null (After GetFirstObject)"); }

                } else
                {
                    _error.AddLog(" GetActiveExcelApps Success.");
                }
                string bookname = apps.GetActiveWorkbookName();
                string sheetname = apps.GetActiveSheetName();
                string activecellAddress = apps.GetActiveCellsAddress();
                activecellAddress = activecellAddress.Replace("$", "");
                _error.AddLog(" " + bookname + " > " + sheetname + " > " + activecellAddress);
                if (_error.hasError) { throw new Exception("ExcelManager.GetActiveAddress Faild"); }

                ExcelManager.ChangeActiveCell(
                    apps.Application, bookname, sheetname, activecellAddress);
                if (_error.hasError) { throw new Exception("ExcelManager.ChangeActiveCell Faild"); }
                if (ChangeActiveCell == null) { _error.AddLogWarning("ChangeActiveCell EventHandler Is Null"); }

                string[] sender = new string[] {
                    activecellAddress + " < " + sheetname + " < " + bookname, bookname,sheetname,activecellAddress
                    };
                ChangeActiveCell?.Invoke(sender, EventArgs.Empty);

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetActiveCell");
                _error.Messenger.ShowAlertMessages();
            }
        }

        // ActiveWorkbook を WindowActivate する
        public void ActivateWorkbookWindowActivate()
        {
            try
            {
                
                //_error.ClearError();
                _error.AddLog(this.ToString() + ".ActivateWorkbookWindowActivate");
                ExcelManager.WindowActivate();
                if (_error.hasAlert) { 
                    _error.AddLogAlert("  ExcelManager.WindowActivate failed");
                    _error.ReleaseErrorState();
                    return; 
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ActivateWorkbookWindowActivate");
            }
        }

        // DataList の Item を一つ上に移動する
        public void MoveUpItemInDataGridViewList()
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString() + ".MoveUpItemInDataGridViewList");
                DataGridUtil.MoveItemUpInList();
                if (_error.hasAlert) { _error.Messenger.ShowAlertMessages(); return; }
                else { _error.Messenger.ShowResultSuccessMessage("Item Moved Up."); }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MoveUpItemInDataGridViewList");
                _error.Messenger.ShowAlertLastErrorWhenHasException("MoveUpItemInDataGridViewList Failed");
            }
        }
        // DataList の Item を一つ下に移動する
        public void MoveItemDownInDataGridViewList()
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString() + ".MoveItemDownInDataGridViewList");
                DataGridUtil.MoveItemDownInList();
                if (_error.hasAlert) { _error.Messenger.ShowAlertMessages(); return; }
                else { _error.Messenger.ShowResultSuccessMessage("Item Moved Down."); }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MoveUpItemInDataGridViewList");
                _error.Messenger.ShowAlertLastErrorWhenHasException("MoveItemDownInDataGridViewList Failed");
            }
        }
        
        public void ShowMessageExistingWhenFlagTrue(string msg,bool existing)
        {
            if (existing)
            {
                // エラーがあれば表示する
                if (_error.hasAboveWarning) { _error.Messenger.ShowAlertMessageMessageAddToExisting(_error.GetLastErrorMessagesAsString()); }
                else { _error.Messenger.ShowResultSuccessMessageAddToExisting(msg); }
            }
            else
            {
                // エラーがあれば表示する
                if (_error.hasAboveWarning) { _error.Messenger.ShowAlertMessages(); }
                else { _error.Messenger.ShowResultSuccessMessage(msg); }
            }
        }

        public void UpdateList(bool isCallingThisMethodByOtherMethod, bool showMsgBox = true)
        {
            try
            {
                try
                {
                    if (this.AppsState.IsNowUpdateExcelAppsList)
                    {
                        // Update 中は実行しない
                        _error.AddLogAlert(this, "UpdateList : Tried Excute Update During Update");
                        return;
                    }
                    this.AppsState.IsNowUpdateExcelAppsList = true;
                    _error.ReleaseErrorState();
                    _error.ClearError();
                    _error.AddLog(this.ToString() + ".UpdateList");
                    _error.AddLog("CurrentThread = " + Thread.CurrentThread.ManagedThreadId);
                    // 
                    // WorkbookNameListを取得する、取得した EXCEL.EXE (Application Object) に Workbook がない場合は
                    // GhostProcess として 文字列 "[PID] EXCEL.EXE" をリスト内に格納する
                    // ※このメソッドの中で Update している
                    List<string> excelList = ExcelManager.GetWorkbookNameListAndGhostProcessNameList();
                    if (_error.hasAlert)
                    {
                        _error.AddLogAlert(" GetWorkbookNameListAndGhostProcessNameList Failed");
                        _error.ReleaseErrorState();
                        if (!showMsgBox) { _error.ReleaseErrorState(); }
                        //return;
                    }
                    if (ExcelManager.GetExcelAppsList().Count < 1)
                    {
                        _error.AddLogWarning("  ExcelManager.GetExcelAppsList().Count < 1");
                        ExcelManager.CreateApplicationOnly();
                        if (_error.hasError) { _error.ReleaseErrorState(); }
                    }
                    _error.AddLog("  GetWorkbookNameListAndGhostProcessNameList Success.");

                    // List を CheckedListBox に追加する
                    CheckdListUtil.UpdateItemListAfterClearList(excelList);
                    if (_error.hasAlert) { throw new Exception("UpdateItemListAfterClearList Failed"); }
                    else
                    {
                        _error.AddLog("UpdateList Success.");
                    }
                }
                catch (Exception ex)
                {
                    _error.AddException(ex, this.ToString() + ".UpdateList");
                }
                finally
                {
                    // Update 中の状態を解除する
                    this.AppsState.IsNowUpdateExcelAppsList = false;
                    if (showMsgBox)
                    {
                        ShowMessageExistingWhenFlagTrue("Update WorkbookList.", isCallingThisMethodByOtherMethod);
                    }
                    else
                    {
                        // エラー表示しないときは、エラー情報を保持したままエラー状態を解除する
                        _error.ReleaseErrorState();
                    }
                    // Excel.Application がなければその旨を表示する
                    if (!ExcelManager.AppsListIsValid())
                    {
                        this.StatusBarChangeTextEvent?.Invoke("Excel.Application Is Nothing.", EventArgs.Empty);
                    }
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this,"UpdateList");
                _error.Messenger.ShowAlertLastErrorWhenHasException("UpdateList(bool,bool) Failed");
            }
        }

        // ExcelWrokbookList を更新する
        public void UpdateList(bool showMsgBox = true)
        {
            try
            {
                if (this.AppsState.IsNowUpdateExcelAppsList)
                {
                    // Update 中
                    _error.AddLogAlert(this, "UpdateList : Tried Excute Update During Update");
                    return;
                }
                    this.AppsState.IsNowUpdateExcelAppsList = true;
                _error.ReleaseErrorState();
                _error.ClearError();
                _error.AddLog(this.ToString() + ".UpdateList");
                _error.AddLog("CurrentThread = " + Thread.CurrentThread.ManagedThreadId);
                // 
                // WorkbookNameListを取得する、取得した EXCEL.EXE (Application Object) に Workbook がない場合は
                // GhostProcess として 文字列 "[PID] EXCEL.EXE" をリスト内に格納する
                // ※この中で Update している
                List<string> excelList = ExcelManager.GetWorkbookNameListAndGhostProcessNameList();
                if (_error.hasAlert)
                {
                    _error.AddLogAlert(" GetWorkbookNameListAndGhostProcessNameList Failed");
                    _error.ReleaseErrorState();
                    if (!showMsgBox) { _error.ReleaseErrorState(); }                    
                    //return;
                }
                if (ExcelManager.GetExcelAppsList().Count < 1)
                {
                    _error.AddLogWarning("  ExcelManager.GetExcelAppsList().Count < 1");
                    ExcelManager.CreateApplicationOnly();
                    if (_error.hasError) { _error.ReleaseErrorState(); }
                }
                _error.AddLog("  GetWorkbookNameListAndGhostProcessNameList Success.");

                // List を CheckedListBox に追加する
                CheckdListUtil.UpdateItemListAfterClearList(excelList);
                if (_error.hasAlert) { throw new Exception("UpdateItemListAfterClearList Failed"); }
                else
                {
                    _error.AddLog("UpdateList Success.");
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".UpdateList");
            }
            finally
            {
                this.AppsState.IsNowUpdateExcelAppsList = false;
                if (showMsgBox)
                {
                    // エラーがあれば表示する
                    if (_error.hasAboveWarning) { _error.Messenger.ShowAlertMessages(); }
                    else { _error.Messenger.ShowResultSuccessMessage("Update WorkbookList"); }
                }
                else
                {
                    // エラー表示しないときは、エラー情報を保持したままエラー状態を解除する
                    _error.ReleaseErrorState();
                }
                // Excel.Application がなければその旨を表示する
                if (!ExcelManager.AppsListIsValid())
                {
                    this.StatusBarChangeTextEvent?.Invoke("Excel.Application Is Nothing.", EventArgs.Empty);
                }
            }
        }

        // DataList にデータ(アドレスなど)を追加する
        public void AddList(bool isShowMsg = true)
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString() + ".AddList");

                if (!ExcelManager.AppsListIsValid())
                { throw new Exception("ExcelManager.AppsListIsValid : AppsList Not Invalid"); }

                // ActiveCell 関連情報を取得する
                ExcelApps apps = ExcelManager.GetActiveExcelApps();
                if (_error.hasAlert) { throw new Exception("GetActiveExcelApps Failed"); }
                if (apps == null) { throw new Exception("GetActiveExcelApps Failed. ExcelApps Is Null"); }

                // リストで取得設定しているが、単体で設定する、DataGridItemsをつかう
                // infolist へ格納する
                ExcelCellsInfo2 info;
                info = _excelCellsManager.MakeAddValue(apps.Application, apps.GetActiveWorkbookName(), apps.ProcessId, "");
                if (_error.hasAlert) { throw new Exception("MakeAddValue Failed"); }
                // List,DataGridView へAddする
                if (!DataGridUtil.IsInitialize)
                {
                    string[] fieldNames = DataGridViewItems.GetFieldNames();
                    DataGridUtil.Initialize(fieldNames, info);
                    if (_error.hasAlert) { throw new Exception("DataGridUtil.Initialize Failed"); }
                }
                else
                {
                    DataGridUtil.AddValue(info);
                    if (_error.hasAlert) { throw new Exception("DataGridUtil.AddValue Failed"); }
                }

                // Activate
                if (AppsSettings.IsActivateWorkbookWindowAfterRegistAddress)
                {
                    if (AppsSettings.IsActivateWorkbookWindowAfterRegistAddress)
                    {
                        this.ActivateWorkbookWindowActivate();
                    }
                }
            } catch(Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".AddList");
            }
            finally
            {
                if (isShowMsg)
                {
                    // エラーがあれば表示する
                    if (_error.hasAboveWarning) { _error.Messenger.ShowAlertMessages(); }
                    else
                    {
                        _error.Messenger.ShowResultSuccessMessage("Address added.");
                    }
                }
                else
                {
                    // エラー表示しないときは、エラー情報を保持したままエラー状態を解除する
                    _error.ClearError();
                }
            }
        }

        public ExcelApps GetAppsWithOpenWorkbook(ref ExcelCellsInfo2 cellsInfo,ref AppsInfo appsInfo,ref bool isOpendExcelFile)
        {
            try
            {
                if (appsInfo.ProcessId <= 0)
                {
                    // WorkbookList にはない Workbook → 開いていない → 開く
                    _error.AddLog("Workbook Not Opend.  * Workbook.ProcessID = 0 [" + appsInfo.FileName + "]");
                    ExcelManager.OpenFile(cellsInfo.Path + "\\" + cellsInfo.BookName);
                    if (_error.hasAlert) { throw new Exception("ExcelManager.OpenFile Failed"); }
                    isOpendExcelFile = true;
                    //_error.Messenger.ShowResultSuccessMessageAddToExisting("Open File [" + appsInfo.FileName + "]");
                    //this.OpenFile(cellsInfo.Path + "\\" + cellsInfo.BookName);
                }
                else
                {

                }
                // 開いているか
                // 開いている場合 ExcelApps を取得する
                ExcelApps apps = ExcelManager.GetExcelAppsFromAppsInfo(appsInfo);
                if (_error.hasAlert) { throw new Exception("ExcelManager.GetExcelAppsFromAppsInfo Failed"); }
                // 閉じた場合 Null となる
                if (apps == null)
                {
                    _error.AddLog(appsInfo.FileName + " Is Nothing [" + appsInfo.ProcessId + "]");
                    // ファイルを開く
                    ExcelManager.OpenFile(cellsInfo.Path + "\\" + cellsInfo.BookName);
                    if (_error.hasAlert) { throw new Exception("ExcelManager.OpenFile Failed"); }
                    _error.Messenger.ShowResultSuccessMessageAddToExisting("Open File [" + cellsInfo.BookName + "]");
                    // Index をセットする
                    appsInfo.Index = CheckdListUtil.GetIndexForExcelAppsFromCheckedListBox(cellsInfo.BookName);
                    if (_error.hasAlert)
                    {
                        _error.AddLog("GetIndexForExcelAppsFromCheckedListBox Failed");
                        // チェックリストがない状態でもOK
                        _error.ClearError();
                    }
                    // 再度取得を試みる
                    apps = ExcelManager.GetExcelAppsFromWorkbookName(cellsInfo.BookName);
                    if (_error.hasAlert) { throw new Exception("ExcelManager.GetExcelAppsFromWorkbookName Failed"); }
                }
                else
                {
                    _error.Messenger.ShowResultSuccessMessage("");
                }
                return apps;
            } catch (Exception ex)
            {
                _error.AddException(ex, this, "GetAppsWithOpenWorkbook");
                return null;
            }
        }

        // 登録されているアドレスを選択して WindowActivate する
        public void SelectCells()
        {
            bool isOpendExcelFile = false;
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString() + ".SelectCells");
                _error.Messenger.ShowResultSuccessMessage("Select Cells Excuting.");
                string[] nowData = DataGridUtil.GetCurrentRowData();
                if (_error.hasAlert)
                { throw new Exception("DataGridViewUtility.GetCurrentRowData Failed"); }

                if (nowData == null) { throw new Exception("CurrentData Is Null"); }
                // DataGridView から情報を取得する
                ExcelCellsInfo2 cellsInfo = (ExcelCellsInfo2)DataGridViewItems.ConvertValueFromArray(nowData);
                if (_error.hasAlert) { throw new Exception("DataGridViewItems.ConvertValueFromArray Failed"); }

                // CellsInfo から AppsInfo へ変換、ExcelManager で扱うため
                AppsInfo appsInfo = new AppsInfo
                {
                    ProcessId = 0,
                    FileName = cellsInfo.BookName,
                    Index = CheckdListUtil.GetIndexForExcelAppsFromCheckedListBox(cellsInfo.BookName)
                };

                // pid を WorkBookList から取得する
                List<string> checkdListValueList = CheckdListUtil.GetListAll();
                appsInfo.ProcessId = GetPidFromCheckdListValues(checkdListValueList, cellsInfo.BookName);
                if (_error.hasAlert) {
                        _error.AddLogWarning("GetPidFromCheckdListValues Failed");
                        _error.ClearError();
                }

                ExcelApps apps = GetAppsWithOpenWorkbook(ref cellsInfo,ref appsInfo,ref isOpendExcelFile);
                if (apps == null) { throw new Exception("File Object Is Nothing [" + cellsInfo.BookName + "]"); }

                // Select する
                apps.SelectAddress(cellsInfo.BookName, cellsInfo.SheetName, cellsInfo.Address);
                if (_error.hasAlert) { _error.AddLogAlert("ExcelApps.SelectAddress Failed"); }

                // Window をアクティブにする
                //this.ActivateWorkbookWindowActivate();
                //if (_error.HasException())
                //{ MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "Error"); return; }
                if (isOpendExcelFile) { UpdateList(true,true); }

                if (_error.hasError) { _error.Messenger.ShowUserMessageOnlyAddToExisting(_error.GetLastErrorMessagesAsString()); } 
                else
                {
                    _error.ClearError();
                    _error.Messenger.ShowResultSuccessMessageAddToExisting("Cells Selected.");
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SelectCells");
                _error.Messenger.ShowUserMessageOnlyAddToExisting(_error.GetLastErrorMessagesAsString());
            }
        }

        // 選択している DataGridView の Row から ExcelCellsInfo2 を取得する。
        public ExcelCellsInfo2 GetCellsInfoSelectedRowInDataGridView()
        {
            try
            {
                //_error.ClearError();
                _error.AddLog(this, "GetCellsInfoSelectedRowInDataGridView");
                string[] nowData = DataGridUtil.GetCurrentRowData();
                if (_error.hasAlert) { throw new Exception("DataGridViewUtility.GetCurrentRowData Failed"); }
                if (nowData == null) { throw new Exception("CurrentData Is Null"); }

                // DataGridView から情報を取得する
                ExcelCellsInfo2 cellsInfo = (ExcelCellsInfo2)DataGridViewItems.ConvertValueFromArray(nowData);
                if (_error.hasAlert) { throw new Exception("DataGridViewItems.ConvertValueFromArray Failed"); }
                return cellsInfo;
            } catch (Exception ex)
            {
                _error.AddException(ex, this,"GetCellsInfoSelectedRowInDataGridView");
                return null;
            }
        }

        // 選択している DataGridView の Row から ExcelApps を取得する。Wrokbook が開いていなければ開いてから取得する
        ///
        public ExcelApps GetExcelAppsSelectedRowInDataGridViewAfterOpenWorkbookWhenNotOpened(ExcelCellsInfo2 cellsInfo)
        {
            try
            {
                //_error.ClearError();
                _error.AddLog(this, "GetExcelAppsSelectedRowInDataGridViewAfterOpenWorkbookWhenNotOpened");

                // CellsInfo から AppsInfo へ変換、ExcelManager で扱うため
                AppsInfo appsInfo = new AppsInfo
                {
                    ProcessId = 0,
                    FileName = cellsInfo.BookName,
                    Index = CheckdListUtil.GetIndexForExcelAppsFromCheckedListBox(cellsInfo.BookName)
                };

                // pid を WorkBookList から取得する
                List<string> checkdListValueList = CheckdListUtil.GetListAll();
                appsInfo.ProcessId = GetPidFromCheckdListValues(checkdListValueList, cellsInfo.BookName);
                if (_error.hasAlert) { _error.AddLogWarning("GetPidFromCheckdListValues Failed"); _error.ClearError(); }

                if (appsInfo.ProcessId <= 0)
                {
                    // WorkbookList にはない Workbook → 開いていない →開く
                    _error.AddLog("Workbook Not Opend.  * Workbook.ProcessID = 0 [" + appsInfo.FileName + "]");
                    ExcelManager.OpenFile(cellsInfo.Path + "\\" + cellsInfo.BookName);
                    if (_error.hasAlert) { throw new Exception("ExcelManager.OpenFile Failed"); }
                    // Workbook を開いたがまだ UpdateList していない
                    AppsState.IsNotUpdateExcelAppsListAfterOpenWorkbook = true;
                    _error.Messenger.ShowResultSuccessMessageAddToExisting("Open Excel File.");
                }
                // 開いているか
                // 開いている場合 ExcelApps を取得する
                ExcelApps apps = ExcelManager.GetExcelAppsFromAppsInfo(appsInfo);
                if (_error.hasAlert) { throw new Exception("ExcelManager.GetExcelAppsFromAppsInfo Failed"); }
                // 閉じた場合 Null となる
                if (apps == null)
                {
                    _error.AddLog(appsInfo.FileName + " Is Nothing [" + appsInfo.ProcessId + "]");
                    // ファイルを開く
                    ExcelManager.OpenFile(cellsInfo.Path + "\\" + cellsInfo.BookName);
                    if (_error.hasAlert) { throw new Exception("ExcelManager.OpenFile Failed"); }
                    // Index をセットする
                    appsInfo.Index = CheckdListUtil.GetIndexForExcelAppsFromCheckedListBox(cellsInfo.BookName);
                    if (_error.hasAlert)
                    {
                        _error.AddLog("GetIndexForExcelAppsFromCheckedListBox Failed");
                        // チェックリストがない状態でもOK
                        _error.ClearError();
                    }
                    // 再度取得を試みる
                    apps = ExcelManager.GetExcelAppsFromWorkbookName(cellsInfo.BookName);
                    if (_error.hasAlert) { throw new Exception("ExcelManager.GetExcelAppsFromWorkbookName Failed"); }
                }
                if (apps == null) { throw new Exception("File Object Is Nothing [" + cellsInfo.BookName + "]"); }
                return apps;
            } catch (Exception ex)
            {
                _error.AddException(ex,this, "GetExcelAppsSelectedRowInDataGridViewAfterOpenWorkbookWhenNotOpened");
                return null;
            }
        }

        private void CopyCellsValueMain(ExcelApps apps,ExcelCellsInfo2 info)
        {
            ExcelCellsController controller = new ExcelCellsController(_error);
            controller.Copy(apps.Application, info.BookName, info.SheetName, info.Address);
        }

        // 登録されているアドレスのRange.Valueをコピー状態にする
        public void CopyCellsValue(bool isShowError = true)
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString() + ".CopyCellsValue");
                _error.Messenger.ShowMessage("Copy CellsValue Excuting.");
                // DataList から値を取得する
                ExcelCellsInfo2 cellsInfo = GetCellsInfoSelectedRowInDataGridView();
                if (_error.hasAlert) { throw new Exception("GetCellsInfoSelectedRowInDataGridView Failed"); }

                // 選択している DataGridView の Row から ExcelApps を取得する。Wrokbook が開いていなければ開いてから取得する
                ExcelApps apps = GetExcelAppsSelectedRowInDataGridViewAfterOpenWorkbookWhenNotOpened(cellsInfo);
                if (_error.hasAlert) { throw new Exception("GetExcelAppsSelectedRowInDataGridViewAfterOpenWorkbookWhenNotOpened Failed"); }

                // コピー用オブジェクト作成
                ExcelCopyCellsValue CopyCellsValue = new ExcelCopyCellsValue(_error, cellsInfo, apps);
                CopyCellsValue.Excute();
                if (_error.hasAlert)
                {
                    _error.Messenger.ShowAlertMessageMessageAddToExisting("Copied Cells Values Failed.");
                }
                else
                {
                    _error.Messenger.ShowResultSuccessMessage("Copied Cells Values.");
                }
                // 進捗ダイアログ表示用処理を通して
                // Copy する
                //ProgressDialogManager.ParentForm = (Form)_mainForm;
                //ProgresDialogDowork.DoWorkEvent = ProgresDialogDowork.ProgressDialog_DoWorkWithExcuteActionForNotHasBackGroundWorkerEvent;
                //ProgresDialogDowork.Message = "Now Processing.";
                //ProgressDialogManager.ExcuteProcessForActionForNotHasBackGroundWorker(
                //    CopyCellsValue.Excute, ref ProgresDialogDowork.BackgroundWorker, 0, 3000,"CopyCells");
                //if (_error.hasAlert) { _error.AddLogAlert("ExcelApps.SelectAddress Failed"); }

                //if (ProgresDialogDowork.IsCancelled)
                //{
                //    _error.Messenger.ShowResultSuccessMessageAddToExisting("Copy Cells Cancelled.");
                //}
                //else
                //{
                //    _error.Messenger.ShowResultSuccessMessageAddToExisting("Copied Cells Values.");
                //}
                //ProgressDialogManager.Dispose();

            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".CopyCellsValue");
            }finally
            {
                // ファイルをオープンしたときは UpdateList する
                if (AppsState.IsNotUpdateExcelAppsListAfterOpenWorkbook) { this.UpdateList(true,true); }
                // エラーがあれば表示する
                if (isShowError) { if (_error.hasError) { _error.Messenger.ShowErrorMessageseAddToExisting(); } }
                else { _error.ClearError(); }
            }
        }



        // WrokbookList の選択されている Workbook を Activate する
        public void ActivateWorkbookSelectedCheckedListBox()
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString()+ ".ActivateWorkbookSelectedCheckedListBox");
                // CheckedListBox で Selected されている workbokName を取得する
                string[] item = CheckdListUtil.GetSelectedItem();
                if (_error.hasAlert) { throw new Exception("CheckdListUtil.GetSelectedItem Failed"); }
                if(item == null) { throw new Exception("CheckdListUtil.GetSelectedItem Failed : Item is Null"); }

                // AppsInfo に変換する
                AppsInfo info = ConvetToAppsInfo(item);
                if (_error.hasAlert) { throw new Exception("ConvetToAppsInfo Failed"); }
                // workbook を Activate する
                // Window を Active にする
                ExcelManager.ActivateWorkbook(info);
                if (_error.hasAlert) { throw new Exception("ExcelManager.ActivateWorkbook Failed"); }
            } catch(Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ActivateWorkbookSelectedCheckedListBox");
            }
            finally
            {
                if (_error.hasError) { _error.Messenger.ShowAlertMessages(); }
                else { _error.ClearError(); }
            }
        }
        // WorkbookList から チェックされているものの、ProcessID を取得する
        private int GetPidFromCheckdListValues(List<string> checkdListValues,  string workbookName)
        {
            try
            {
                _error.AddLog(this.ToString() + "GetPidFromCheckdListValues");
                if (checkdListValues == null) { throw new Exception("checkdListValues is null"); }
                if (checkdListValues.Count < 1) { throw new Exception("checkdListValues.Count < 1"); }
                string buf;
                int n;
                int pid = 0;
                foreach(string value in checkdListValues)
                {
                    n = value.IndexOf(']');
                    buf = value.Substring(n + 2, value.Length - (n + 2)); // bookname
                    if (buf.Equals(workbookName))
                    {
                        buf = value.Substring(1, n - 1); // pid
                        if (int.TryParse(buf, out pid)){
                            return pid;
                        } else
                        {
                            return pid;
                        }
                    }
                }
                return 0;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetPidFromCheckdListValues");
                return 0;
            }
        }


        // Workbook を保存して閉じる
        public void CloseSave(bool isMsgShow = true)
        {
            string errmsg;
            try
            {
                _error.ReleaseErrorState();
                _error.ClearError();
                _error.AddLog(this.ToString() + "CloseSave");
                _error.Messenger.ShowResultSuccessMessage("Close(Save) Workbook Excuting.");
                ///// 
                // CheckedBoxList からチェックされている FileName、pid、index を取得する
                List<string[]> infoList2 = CheckdListUtil.GetAppsInfoListFromCheckdItemList();
                if (_error.hasError)
                { throw new Exception("GetAppsInfoListFromCheckdItemList Failed."); }

                if (infoList2.Count < 1) {
                    errmsg = Constants.ErrorMessage.WORKBOOKLIST_IS_NOT_CHECKED;
                    throw new Exception("infoList2.Count < 1");
                    //MessageBox.Show(Constants.ErrorMessage.WORKBOOKLIST_IS_NOT_CHECKED, "Error"); return; 
                }
                // string[] から AppsInfo へ変換する
                List<AppsInfo> appsInfoList = ConvertToAppsInfo(infoList2);
                if (_error.hasAlert) { throw new Exception("ConvertToAppsInfo"); }
                if (appsInfoList.Count < 1) { throw new Exception("ConvertToAppsInfo -> appsInfoList.Count < 1"); }
                /////////
                foreach (AppsInfo info in appsInfoList)
                {
                    // filename、pid、index で指定した Workbook を閉じる
                    ExcelManager.CloseWorkbookByPidAndBookName(
                        info.ProcessId, info.FileName, info.Index, true, false);
                    if (_error.hasAlert) { throw new Exception("ExcelManager.CloseWorkbookByPidAndBookName Failed"); }
                }
                // Update
                UpdateList(true,true);

                
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".CloseSave");
            }
            finally
            {
                if (isMsgShow) { if (_error.hasAlert) { _error.Messenger.ShowAlertMessages(); } }
                else { 
                    _error.ClearError();
                    _error.Messenger.ShowResultSuccessMessage("Workbook Closed.");
                }
            }
        }
        // List<string[]> から List<AppsInfo> に変換する
        private List<AppsInfo> ConvertToAppsInfo(List<string[]> values)
        {
            List<AppsInfo> retList = new List<AppsInfo>();
            try
            {
                if (values == null) { throw new Exception("ConvertToAppsInfo : values is null"); }
                if (values.Count < 1) { throw new Exception("ConvertToAppsInfo : values.Count < 1"); }
                //AppsInfo info;
                foreach (string[] val in values)
                {
                    retList.Add(ConvetToAppsInfo(val));
                }
                return retList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertToAppsInfo");
                return retList;
            }
        }
        // string[] から AppsInfo に変換する
        private AppsInfo ConvetToAppsInfo(string[] value)
        {
            AppsInfo info;
            try
            {
                info = new AppsInfo
                {
                    FileName = value[0]
                };
                if (int.TryParse(value[1], out int pid))
                {
                    info.ProcessId = pid;
                }
                else { info.ProcessId = 0; }
                if (int.TryParse(value[2], out int index))
                {
                    info.Index = index;
                }
                else { info.Index = 0; }
                return info;
            } catch(Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvetToAppsInfo");
                return null;
            }
        }

        public void SaveSettings(bool isShowMsg = true)
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this, "SaveSettings");
                // 最後のファイル名設定クラスへ、この値のみ個別で処理を行う、設定保持タイミングが異なるため
                AppsSettings.SetLastFilePathToSettingsObject(OpenedFile.GetPath());

                // 設定を Class メンバフィールドから Register 
                AppsSettings.SetSettingsValueMemberToSettingsClass();
                if (_error.hasAlert)
                {
                    _error.AddLog("_excelCellsManagerMain.AppsSettings.SetSettingsValueMemberToSettingsClass Failed.");
                }

                // 設定を書き込む
                AppsSettings.Register.SaveValuesToFile();
                if (_error.hasAlert)
                {
                    _error.AddLog("_excelCellsManagerMain.AppsSettings.Register.SaveValuesToFile Failed.");
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this, "SaveSettings");
            }finally
            {
                if (isShowMsg) { if (_error.hasAlert) { _error.Messenger.ShowAlertMessages(); } }
                else { _error.ClearError(); }
            }
        }

        // DataList を保存する(上書き、名前を付けて保存)
        public void SaveDataGridViewData(bool IsSaveAs,bool isShowMsg = true)
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString() + ".SaveDataGridViewData");
                // データを取得する。tsv として保存するのでタブで区切る
                // 連結した string を取得する
                string writeData = DataGridUtil.GetAllData("\t");
                if (_error.hasAlert) { throw new Exception("DataGridUtil.GetAllData Failed"); }
                if (!IsSaveAs)
                {                    
                    // 保存する
                    OpenedFile.SaveData(writeData, _saveAsDefaultFileName, 1);
                } else
                {
                    // 名前を付けて保存
                    OpenedFile.SaveAsData(writeData, _saveAsDefaultFileName, 1);
                }
                if (_error.hasAlert){ throw new Exception("SaveData or SaveAsData Failed : IsSaveAs="+ IsSaveAs); }
                _error.AddLog("SavePath:"+OpenedFile.GetPath());
                // 状態を変更
                AppsState.IsEdited = false;
                ResetEdited();

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SaveDataGridViewData");
            }
            finally
            {
                if (isShowMsg) { if (_error.hasAlert) { _error.Messenger.ShowAlertMessages(); } }
                else { _error.ClearError(); }
            }
        }

        // File を Open して DataList に反映する（開く）
        public void OpenFile(string filePath,bool showMsgBox = true)
        {
            try
            {
                _error.ReleaseErrorState();
                _error.AddLog(this.ToString()+ ".OpenFile : "+filePath);
                // 編集状態の場合は確認する
                int ret = this.ConformWhenDataEdited();
                if (ret == 2) { return; }
                if ((filePath == null)||(filePath == ""))
                {
                    _error.AddLogAlert("FilePath Value is Blank"); return;
                }
                // ファイルパスがある場合に実行する
                List<string> readValues = OpenedFile.GetListByOpenFile(filePath);
                if (_error.hasAlert)
                {
                    _error.AddLogAlert(" OpenedFile.GetListByOpenFile Failed: path="+filePath);
                    throw new Exception(this.ToString() + ".OpenFile : File Open Error : path=" + filePath);
                    //if (showMsgBox) { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "Error"); }
                }
                // path をセットする
                // ExcelCellsManagerForm.ChangeFilePathEvent
                OpenedFile.SetFilePath(filePath);
                if (_error.hasAlert)
                {
                    _error.AddLogAlert(" OpenedFile.SetFilePath Failed: path=" + filePath);
                    throw new Exception(this.ToString() + ".SetFilePath : File Set Error :  path=" + filePath);
                    //if (showMsgBox) { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "Error"); return; }
                }
                // path からデータを取得する
                //List<string> values = this.OpenedFile.GetListByOpenFile(filePath);
                //if (_error.HasException())
                //{
                //    _error.AddLog(" OpenedFile.GetListByOpenFile Failed: path=" + filePath);
                //    if (showMsgBox) { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "Error"); return; }
                //}
                // List<string> を List<object> に変換する
                // object は ExcelCellsInfo2 と同じ構造の string[]
                List<string[]> valesObject = 
                    DataGridUtil.DataGridViewValueConverter.ConvertArrayStringListFromStringList(readValues, delimiter);
                // List,DataGridView へAddする
                if (!DataGridUtil.IsInitialize)
                {
                    string[] fieldNames = DataGridViewItems.GetFieldNames();
                    DataGridUtil.Initialize(fieldNames);
                    if (_error.hasAlert)
                    {
                        _error.AddLogAlert(" DataGridViewItems.GetFieldNames or Initialize Failed");
                        throw new Exception("DataGridViewItems.GetFieldNames or Initialize Failed");
                        //if (showMsgBox) { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "Error"); return; }
                    }
                }
                // List<string> を DataGridView へ
                DataGridUtil.ResetValue(valesObject);
                if (_error.hasAboveWarning)
                {
                    _error.AddLogAlert(" DataGridUtil.ResetValue or Initialize Failed");
                    throw new Exception("DataGridUtil.ResetValue or Initialize Failed");
                    //if (showMsgBox) { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "Error"); return; }
                }
                // 状態を変更
                AppsState.IsEdited = false;
                ResetEdited();
                if (_error.hasAboveWarning) { _error.AddLogAlert(new Exception(this.ToString()+".ResetEdited Failed")); }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".OpenFile",Constants.ErrorMessage.FILE_OPEN_ERROR);
            }
            finally
            {
                if (showMsgBox)
                {
                    // エラーがあれば表示する
                    if (_error.hasAboveWarning) { _error.Messenger.ShowAlertMessages(); }
                }
                else
                {
                    // エラー表示しないときは、エラー情報を保持したままエラー状態を解除する
                    _error.ReleaseErrorState();
                }
            }
        }
        // データ編集状態の時に保存するか確認する
        // 戻り値は
        public int ConformWhenDataEdited(bool isShowMsg=true)
        {
            try
            {
                _error.AddLog(this,"ConformWhenDataEdited");
                if (AppsState.IsEdited)
                {
                    int ret = OpenedFile.ConformSaveWhenFileEdited(AppsState.IsEdited, OpenedFile.GetFileName());
                    switch (ret)
                    {
                        case 0: break; // 保存しないを選択→何もしない
                        case 1: // 保存する
                            this.SaveDataGridViewData(false); break;
                        case 2: _error.AddLog("Select CANCEL Of Dialog"); return ret; // Cancel
                        case -1: _error.AddLogAlert("Save Conform Dialog Error"); return ret; // Exception
                        case -2: _error.AddLogAlert("DialogResult Value is Unexcepted"); return ret; // 
                    }
                    if (_error.hasAlert)
                    {
                        _error.AddLogAlert("OpenedFile.ConformSaveWhenFileEdited Failed : result="+ret);
                        return ret; 
                    }
                }
                return 0;
            } catch (Exception ex)
            {
                _error.AddException(ex, this, "ConformWhenDataEdited");
                return -3;
            }
        }
            

        // 新しいファイルを開く(新規作成)
        public void NewFile()
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this, "NewFile");

                // 編集状態の場合は確認する
                int ret = this.ConformWhenDataEdited();
                if (ret == 2) { _error.AddLog(" NewFile End"); return; }

                // 開いているファイルが編集状態でない場合はそのまま NewFile する
                // path をセットする
                OpenedFile.SetFilePath(Constants.DefalutNewFileName + Constants.DefaultFileType);
                if (_error.hasAlert){ throw new Exception("OpenedFile.SetFilePat Failed"); }

                List<string> values = new List<string>
                {
                    "\t\t\t\t\t"
                };
                // List<string> を List<object> に変換する
                // object は ExcelCellsInfo2 と同じ構造の string[]
                List<string[]> valesObject = DataGridUtil.DataGridViewValueConverter.ConvertArrayStringListFromStringList(values, delimiter);
                // List,DataGridView へAddする
                if (!DataGridUtil.IsInitialize)
                {
                    string[] fieldNames = DataGridViewItems.GetFieldNames();
                    DataGridUtil.Initialize(fieldNames);
                    if (_error.hasAlert) { throw new Exception("DataGridUtil.Initialize Failed"); }
                }
                // List<string> を DataGridView へ
                DataGridUtil.ResetValue(valesObject);
                if (_error.hasAlert) { throw new Exception("DataGridUtil.ResetValue Failed"); }

                // 状態を変更
                AppsState.IsEdited = false;
                ResetEdited();

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".NewFile");
            }
        }

        public void Close()
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString() + ".Close");
                _error.WriteErrorLog();
                _error.WriteLog();
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Close");
            }
        }
    } // Class End
}
