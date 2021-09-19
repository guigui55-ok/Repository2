using CellsManagerControl.Utility;
using ExcelCellsManager.CellsValuesConrol.CellsValuesList;
using ExcelCellsManager.DataGridStruct;
using ExcelCellsManager.ErrorMessage;
using ExcelCellsManager.ExcelCellsManager;
using ExcelCellsManager.ExcelCellsManager.Event;
using ExcelCellsManager.ExcelWorkbookList;
using ExcelCellsManager.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Utility;

namespace ExcelCellsManager
{
    public partial class ExcelCellsManagerForm : Form
    {
        protected Size _panelSize;
        protected ErrorManager.ErrorManager _error;
        protected ErrorManager.IErrorMessenger _errorMessenger;
        // Main Class
        protected ExcelCellsManagerMain _excelCellsManagerMain;
        // Control Utility Class
        protected IDataGridViewUtility _dataGridUtil;
        protected IDataGridViewItems _dataGridViewItems;
        // Control Event
        protected MouseEvent _cellsManagerMouseMove;
        protected DragDropEvent _cellsManagerDragDropEvent;
        protected DataGridViewKeyEvent _dataGridViewKeyEvent;
        protected DataGridViewFormEventCellsManager _dataGridViewFormEvent;
        protected DataGridViewMouseEventForCellsManager _dataGridViewMouseEventForCellsManager;
        protected ExcellCellsManagerFormKeyEvent _excellCellsManagerFormKeyEvent;
        protected event EventHandler _changeOpenedFilePathEvent;
        protected event EventHandler _openFileEvent;
        // WrokbooksList CheckdListBox Event
        protected IWorkbookListControl _checkedListUtil;
        protected WorkbookList _workbookList;
        // Contorl Relation
        protected ToolStripStatusLabel StatusBarlabel1;
        // Settings
        protected ExcelCellsManagerSettingsValue _appsSettings;
        // etc
        protected ExcelCellsManagerConstants Constants;
        // EdgeForm
        protected MousePointCapture.MousePointCaptureOnScreenEdgeManager _mouseCapture;
        protected string _className = "ExcelCellsManagerForm";
        protected string _thisClassName = "ExcelCellsManagerForm";
        //protected string _ApplicationTitle = "Excel Cells Manager2";
        //protected string _DefalutNewFileName = "*";
        //protected string _DefaultFileType = ".tsv";
        //protected string _iniFileDirectory = "";
        //protected string _dialogFilterType = "TSVファイル(*.tsv)|*.tsv;|すべてのファイル(*.*)|*.*";
        //protected string _iniFileName = "settings.ini";
        public ExcelCellsManagerForm()
        {
            try
            {
                Console.WriteLine("CurrentThread = " + Thread.CurrentThread.ManagedThreadId);
                InitializeComponent();
                // Error,Log記録用クラス
                _error = new ErrorManager.ErrorManager(1, System.IO.Directory.GetCurrentDirectory(), 
                    "Error.Log", "Debug.Log");
                _error.SetLogIndexLimit = 200;
                // ログ記録スタート
                _error.AddLog(_thisClassName + ".ExcelCellsManagerForm Constracta BeginLogRecording");
                // リテラル設定
                Constants = new ExcelCellsManagerConstants(_error)
                {
                    IniFileDirectory = System.IO.Directory.GetCurrentDirectory()
                };

                // エラー表示用 StatusStrip
                // StatusStrip ErrorMessenger
                _errorMessenger = new ErrorMessengerStatusBar(_error, this, statusStrip2,0);
                if (_error.HasAboveWorning()) { _error.AddLogAlert("ErrorMessengerStatusBar.Constracta"); _error.ReleaseErrorState(); }
                _error.Messenger = _errorMessenger;

                // Form 内コンポーネント操作用 Utility
                _dataGridViewItems = new DataGridViewItems(_error);
                _checkedListUtil = new CheckedListBoxUtility(_error, checkedListBox1);
                _dataGridUtil = new DataGridViewUtility(_error, dataGridView1, _dataGridViewItems);

                // 設定の読み込み
                // ※特定のレジストリキーがtrueの時にRegモードとする、初期値はini
                _appsSettings = new ExcelCellsManagerSettingsValue(_error,0);
                if (_error.HasAboveWorning()) { _error.AddLogAlert("ExcelCellsManagerSettingsValue.Constracta"); _error.ReleaseErrorState(); }
                // 初期化
                _excelCellsManagerMain = new ExcelCellsManagerMain(_error,this)
                {
                    CheckdListUtil = this._checkedListUtil,
                    DataGridUtil = this._dataGridUtil,
                    DataGridViewItems = this._dataGridViewItems,
                    OpenedFile = new Utility.OpenedFile(_error),
                    AppsSettings = _appsSettings,
                    AppsState = new ExcelCellsManagerState(),
                    Constants = this.Constants, 
                };
                _excelCellsManagerMain.InitializeSettingsValue = new ExcelCellsManager.Settings.InitializeSettingsValue(_error, _excelCellsManagerMain)
                {
                    // 設定フォーム配置のために、Panel.Width と TabLocation.Y を保存しておく
                    SettingsFormWidth = _excelCellsManagerMain.AppsSettingsFormManager.SetttingsPanelWidth,
                    TabLocationY = _excelCellsManagerMain.AppsSettingsFormManager.TabLocationY
                };
                // 設定の初期値を設定
                _excelCellsManagerMain.InitializeSettingsValue.Excute();
                if (_error.HasAboveWorning()) { _error.AddLogAlert("_appsSettings.Excute"); _error.ReleaseErrorState(); }
                // 設定値の読み込みのためのパスをセットする
                _appsSettings.Initialize(Constants.IniFileDirectory + "\\" + Constants.IniFileName, "");
                if (_error.HasAboveWorning()) { _error.AddLogAlert("_appsSettings.Initialize"); _error.ReleaseErrorState(); }
                // 設定値の読み込み
                _appsSettings.SettingsValueToThisMember();
                if (_error.HasAboveWorning()) { _error.AddLogAlert("_appsSettings.SettingsValueToThisMember"); _error.ReleaseErrorState(); }
                // 設定値を各クラスに反映する
                _excelCellsManagerMain.AppsSettingsFormManager.SettingsList =
                    _excelCellsManagerMain.AppsSettings.SettingsManager.SettingsList;


                // StatusStrip 設定
                StatusBarlabel1 = new ToolStripStatusLabel
                {
                    Text = Constants.StatusBarTextInitialize,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Spring = true
                };
                statusStrip1.Items.Add(StatusBarlabel1);


                // FileOpen クラス設定
                _changeOpenedFilePathEvent = new EventHandler(ChangeOpenedFilePathEvent);
                _excelCellsManagerMain.OpenedFile.ConformEditedTitle = Constants.ApplicationTitle;
                _excelCellsManagerMain.OpenedFile.ChangeFilePathEvent += _changeOpenedFilePathEvent;
                _excelCellsManagerMain.OpenedFile.SaveFileDialogFilter = Constants.DialogFilterType;
                _openFileEvent = new EventHandler(OpenFileEvent);

                // Control.Event の設定
                // Event
                // DataGridView Event
                _dataGridViewKeyEvent = new DataGridViewKeyEvent(_error, _dataGridUtil, dataGridView1);
                _dataGridViewFormEvent = new DataGridViewFormEventCellsManager(
                    _error, _excelCellsManagerMain,_dataGridUtil, dataGridView1);
                dataGridView1.RowsRemoved += DataGridView1_RowsRemoved;
                // ExcellCellsManagerMain を委譲する ControlEvent 実行クラス
                _excelCellsManagerMain.SetDataGridViewAddEvent();
                _dataGridViewMouseEventForCellsManager = new DataGridViewMouseEventForCellsManager(
                    _error, _excelCellsManagerMain, _dataGridUtil, dataGridView1);
                // MainForm KeyEvent
                _excellCellsManagerFormKeyEvent = new ExcellCellsManagerFormKeyEvent(_error, this, _excelCellsManagerMain);
                // DataGridView Event
                _excelCellsManagerMain.DataGridView_EditEvnet += DataList_Edited;
                // MouseEvent を生成代入する
                _cellsManagerMouseMove = new MouseEvent(_error, _className);
                this.MouseMove += _cellsManagerMouseMove.MouseMove;
                // DragDropEvent を生成代入する
                _cellsManagerDragDropEvent = new DragDropEvent(_error, _className);
                this.DragDrop += _cellsManagerDragDropEvent.DragDropEventHandler;
                this.DragEnter += _cellsManagerDragDropEvent.DragEnterEventHandler;

                _excelCellsManagerMain.ChangeActiveCell += Application_SheetSelectionChangeAfter;
                // このクラス内の DragDropAfter を紐づける
                _cellsManagerDragDropEvent.DragDropAfter += DragDropAfter;
                // ActiveCell変更時 StatusStrip を変更するために EventHandler にメソッドを紐づける
                _excelCellsManagerMain.ExcelManager.ExcelEventBridge.Application_SheetSelectionChangeAfterEvent += Application_SheetSelectionChangeAfter;
                _excelCellsManagerMain.ExcelManager.ExcelEventBridge.Application_WindowActivateAfterEvent += Application_WindowActivateAfter;
                _excelCellsManagerMain.ExcelManager.ExcelEventBridge.WorkSheet_ActivateAfterEvent += WorkSheet_ActivateAfterEvent;
                _excelCellsManagerMain.ExcelManager.ExcelEventBridge.Application_SheetActivateAfterEvent += Application_SheetActivateAfterEvent;
                _excelCellsManagerMain.ExcelManager.ExcelEventBridge.Application_WorkbookOpenAfterEvent += Application_WorkbookOpenAfterEvent;
                _excelCellsManagerMain.ExcelManager.ExcelEventBridge.Application_WorkbokBeforeCloseAddEvent += Application_WorkbookBeforeCloseAddEvent;
                _excelCellsManagerMain.ExcelManager.ExcelEventBridge.Application_DeactivateAddEvent += Application_WorkbookDeactivateAddEvent;
                // checkedListBoxEvent
                _workbookList = new WorkbookList(_error, checkedListBox1, _checkedListUtil,_excelCellsManagerMain);
                if (_error.HasAboveWorning()) { _error.AddLogAlert("WorkbookList.Constracta"); }
                //_checkedListBoxEvent = new CheckedListBoxEvent(_error, checkedListBox1);
                //_workbookListEvent = new WorkbookListEvent(_error);
                //_checkedListBoxEvent.ItemRightDoubleClickedAfter = _workbookListEvent.CheckedListBoxItemMouseRightDoubleClickAfterEvent;

                // Initialize ExcelCellsManagerMain
                // DataGiidView の初期化
                // NewFile をセットする
                _excelCellsManagerMain.Initialize();
                if (_error.HasAboveWorning()) { _error.AddLogAlert("_excelCellsManagerMain.Initialize"); }
                // リストを更新する
                // エクセルが一つも開いていないときは Excel.Application を開く
                _excelCellsManagerMain.UpdateList(false);
                if (_error.HasAboveWorning()) { _error.AddLogAlert("_excelCellsManagerMain.UpdateList"); }
                
                //if(_excelCellsManagerMain.ExcelManager.GetExcelAppsList().Count < 1)
                //{
                //    // ExcelAppsList().Count < 1 エクセルが一つも開いていないときは、空の Workbook で Excel.Application をひらく
                //    _excelCellsManagerMain.ExcelManager.UpdateOpendExcelApplication(true);
                //    if (_error.HasAboveWorning()) { _error.AddLogAlert("UpdateOpendExcelApplication Failed"); }
                //}
                // 一つ以上の Workbook が開かれている
                if (_excelCellsManagerMain.ExcelManager.IsOpendWorkbookOneOrMore())
                {
                    // ActiveCell を取得・設定する
                    _excelCellsManagerMain.SetActiveCell();
                    if (_error.HasAboveWorning()) { _error.AddLogAlert("_excelCellsManagerMain.SetActiveCell"); _error.ClearError(); }
                }

                //_error.AddException(new Exception("test Alert"),"test Alert", "Alert messagetoUser");
                if (_error.hasAlert) { _error.Messenger.ShowAlertMessages(); }
                _errorMessenger.ShowErrorMessageEvent += this.ShowErrorMessageEvent;

                this.SizeChanged += ExcelCellsManagerForm_SizeChanged;
                this.ResizeBegin += ExcelCellsManagerForm_ResizeBegin;
                this.ResizeEnd += ExcelCellsManagerForm_ResizeEnd;
                _excelCellsManagerMain.StatusBarChangeTextEvent += ExcelCellsManagerForm__statusBarChangeTextEvent;
                _error.AddLog("******************** Initialize End ********************");
                //_excelCellsManagerMain.AppsState.IsInitialize = false;
                _error.Messenger.ShowMessage(Constants.StatusBarTextInitialize);
            }
            catch (Exception ex)
            {
                string msg = Constants.ErrorMessage.APPLICATION_INITIALIZE;
                _error.AddException(ex,this.ToString()+ ".ExcelCellsManagerForm.Constracta", msg);
                _error.Messenger.ShowAlertMessages();
                MessageBox.Show(msg, "Error! - ExcelCellsManagerForm.Constracta Failed");
            }
        }

        private void DataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            // Visible 時でないと Initialize 時にも数回(3回)メッセージを表示してしまう
            // Visible にすると今回の場合1回で済むので、フラグで管理する
            if (((DataGridView)sender).Visible)
            {
                // Item の Up/Down 時にも反応してしまうため、IsRemoved で管理する
                if (!_excelCellsManagerMain.AppsState.IsInitialize)
                {
                    _error.Messenger.ShowResultSuccessMessage("Rows Removed.");
                }
                else
                {
                    _excelCellsManagerMain.AppsState.IsInitialize = false;
                }
            }
        }

        private void ExcelCellsManagerForm__statusBarChangeTextEvent(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog("ExcelCellsManagerForm__statusBarChangeTextEvent");
                if (sender.GetType() == typeof(string))
                {
                    StatusBarlabel1.Text = (string)sender;
                } else
                {
                    _error.AddLogAlert(" sender is not string Type");
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".ExcelCellsManagerForm__statusBarChangeTextEvent");
            }
        }

        // Event
        // MainForm
        private void ExcelCellsManagerForm_ResizeEnd(object sender, EventArgs e)
        {
            checkedListBox1.ResumeLayout();
        }

        // Event
        // MainForm
        private void ExcelCellsManagerForm_ResizeBegin(object sender, EventArgs e)
        {
            checkedListBox1.SuspendLayout();
        }

        private void ExcelCellsManagerForm_SizeChanged(object sender, EventArgs e)
        {

        }

        

        // Event
        private void ShowErrorMessageEvent(object sender,EventArgs e)
        {
            try
            {
                _error.AddLog(this, "ShowErrorMessageEvent");
                if (sender.GetType() == typeof(int))
                {
                    this.SuspendLayout();
                    _panelSize = panel1.Size;
                    Console.WriteLine("panel.Size.Height = "+_panelSize.Height);
                    int n = (int)sender;
                    if(sender == null) { Console.WriteLine("  *** Sender is Null"); }
                    if (this == null) { Console.WriteLine("  *** this is Null"); }
                    this.Height += (int)sender;
                    statusStrip2.Location = new Point(statusStrip2.Location.X, statusStrip2.Location.Y - n);
                    if (n > 0)
                    {
                    } else
                    {
                    }
                    this.ResumeLayout();
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".ShowErrorMessage Event");
                _error.ClearError();
                this.ResumeLayout();
            }
        }
        // Event
        public void DataList_Edited(object sender,EventArgs e)
        {
            try
            {
                _error.AddLog(_thisClassName+ ".DataList_Edited");

                if (_excelCellsManagerMain.AppsState.IsEdited)
                {
                    if (!this.Text.EndsWith("*"))
                    {
                        this.Text += "*";
                    }
                } else
                {
                    if (this.Text.EndsWith("*"))
                    {
                        this.Text = this.Text.Substring(0, this.Text.Length-1);
                    }
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".DataList_Edited Event");
                _error.ClearError();
            }
        }
        public void OpenFile(string filePath)
        {
            _excelCellsManagerMain.OpenFile(filePath);
        }
        private void OpenFileEvent(object sender, EventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(List<string>))
                {

                }
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".OpenFile Event");
                _error.Messenger.ShowAlertMessages();
            }
        }
        // Event
        // 開いているファイル名が変更された
        private void ChangeOpenedFilePathEvent(object sender,EventArgs e)
        {
            string buf = Constants.ApplicationTitle;
            try
            {
                string path = _excelCellsManagerMain.OpenedFile.GetPath();
                string filename = System.IO.Path.GetFileName(path);
                if (filename != "") { filename = "- " + filename; }
                this.Text = buf + filename;
                _excelCellsManagerMain.AppsSettings.LastOpendFilePath = path;
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".ChangeOpenedFilePathEvent");
                this.Text = buf;
            }
        }

        // ActiveCell 実行後 StatusBar に表示する
        // Application_SheetActivateAfterEvent イベント内で実行する
        private void SetValueToStatusBarTextAfterChangeActiveCell(string[] sender)
        {
            try
            {
                // sender の string[] は {"address > sheet > book","book","sheet","address"}
                StatusBarlabel1.Text = "ActiveCell: " + sender[0];
                _excelCellsManagerMain.SetActiveCellWithEvent(sender[1], sender[2], sender[3]);
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".SetValueToStatusBarTextAfterChangeActiveCell");
                _error.Messenger.ShowAlertMessages();
            }
        }
        // Event
        private void Application_WorkbookDeactivateAddEvent(object sender,EventArgs e)
        {
            try
            {
                _error.AddLog(_thisClassName + ".Application_WorkbookDeactivateAddEvent");

            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".Application_WorkbookDeactivateAddEvent");
                _error.Messenger.ShowAlertMessages();
            } finally {
            }
        }
        // Event
        public void Application_WorkbookBeforeCloseAddEvent(object sender,EventArgs e)
        {
            try
            {
                _error.AddLog(_thisClassName + ".Application_WorkbookBeforeCloseAddEvent");
                _excelCellsManagerMain.AppsState.IsWorkbookBeforeClose = true;
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".Application_WorkbookBeforeCloseAddEvent");
                _error.Messenger.ShowAlertMessages();
            } finally
            {
                //if (_excelCellsManagerMain.AppsSettings.IsUpdateListWhenWorkbookOpendAndClosed)
                //{
                //    _ = Invoke(new WorkbooksListUpdateDelegate(updateWorkbooksList));
                //}
                _error.AddLog(_thisClassName + ".Application_WorkbookBeforeCloseAddEvent : Finally");
            }
        }

        // Event
        delegate void WorkbooksListUpdateDelegate();

        void updateWorkbooksList()
        {
            _excelCellsManagerMain.UpdateList(false);
        }

        // Event
        public void Application_WorkbookOpenAfterEvent(object sender,EventArgs e) 
        {
            try
            {
                _error.AddLog(_thisClassName + ".Application_WorkbookOpenAfterEvent");
                _error.AddLog("CurrentThread = " + Thread.CurrentThread.ManagedThreadId);
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".Application_WorkbookOpenAfterEvent");
                _error.Messenger.ShowAlertMessages();
            }
            finally
            {
                if (_excelCellsManagerMain.AppsSettings.IsUpdateListWhenWorkbookOpendAndClosed)
                {
                    // アプリから開いている時は、Update 中なのでループするので実行しない
                    if (!_excelCellsManagerMain.ExcelManager.IsWorkbookOpening)
                    {
                        _ = Invoke(new WorkbooksListUpdateDelegate(updateWorkbooksList));
                    }
                }
            }
        }
        // Event
        public void Application_SheetActivateAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(_thisClassName+ ".Application_SheetActivateAfterEvent");
                if (sender.GetType() == typeof(string[]))
                {
                    SetValueToStatusBarTextAfterChangeActiveCell(((string[])sender));
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".Application_SheetActivateAfterEvent");
                _error.Messenger.ShowAlertMessages();
            }
        }
        // Event
        public void WorkSheet_ActivateAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(_thisClassName+ ".WorkSheet_ActivateAfterEvent");
                if (sender.GetType() == typeof(string[]))
                {
                    SetValueToStatusBarTextAfterChangeActiveCell(((string[])sender));
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".WorkSheet_ActivateAfterEvent");
                _error.Messenger.ShowAlertMessages();
            }
        }

        // Event
        public void Application_WindowActivateAfter(object sender, EventArgs e)
        {
            try
            {
                // Workbook を 閉じた後
                if (_excelCellsManagerMain.AppsState.IsWorkbookBeforeClose)
                {
                    // このブロックに貼ってすぐにフラグを変更しないとループする
                    _excelCellsManagerMain.AppsState.IsWorkbookBeforeClose = false;
                    // Workbook を 閉じた後のタイミングはここしかない
                    // Workbook を閉じた後に WorkbookList を更新する
                    if (_excelCellsManagerMain.AppsSettings.IsUpdateListWhenWorkbookOpendAndClosed)
                    {
                        _ = Invoke(new WorkbooksListUpdateDelegate(updateWorkbooksList));
                    }
                }
                if (sender.GetType() == typeof(string[]))
                {
                    SetValueToStatusBarTextAfterChangeActiveCell(((string[])sender));
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".Application_WindowActivateAfter");
                _error.Messenger.ShowAlertMessages();
            }
        }
        // Event
        public void Application_SheetSelectionChangeAfter(object sender, EventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(string[]))
                {
                    // sender の string[] は {"address > sheet > book","book","sheet","address"}
                    SetValueToStatusBarTextAfterChangeActiveCell(((string[])sender));
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".Application_SheetSelectionChangeAfter");
                _error.Messenger.ShowAlertMessages();
            }
        }

        // Event
        private void DragDropAfter(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog("DragDropAfter");
                if (sender.GetType() == typeof(System.String[])){
                    _error.AddLog("sender.GetType() == typeof(System.String[]) => true");
                    foreach(string filePath in ((System.String[])sender))
                    {
                        _error.AddLog("DragDropAfter -> Open FilePath : " + filePath);

                        if(_excelCellsManagerMain.ExcelManager.IsExcelType(filePath)){
                            // Excel の拡張子なら Excel でファイルを開く
                            _excelCellsManagerMain.ExcelManager.OpenFile(filePath);
                        } else if (_excelCellsManagerMain.FileTypeIsTsv(filePath))
                        {
                            // Tsv ファイルなら ExcelCellsManager で開く
                            _excelCellsManagerMain.OpenFile(filePath);
                        } else
                        {
                            // .lnk ファイルの場合
                            if (System.IO.Path.GetExtension(filePath).Equals(".lnk"))
                            {
                                ShortcutDynamic shutil = new ShortcutDynamic();
                                string newpath = shutil.GetTargetPath(filePath); 
                                if (_excelCellsManagerMain.ExcelManager.IsExcelType(newpath))
                                {
                                    // Excel の拡張子なら Excel でファイルを開く
                                    _excelCellsManagerMain.ExcelManager.OpenFile(newpath);
                                }
                                else if (_excelCellsManagerMain.FileTypeIsTsv(newpath))
                                {
                                    // Tsv ファイルなら ExcelCellsManager で開く
                                    _excelCellsManagerMain.OpenFile(newpath);
                                } else
                                {
                                    _error.AddLog("DragDropAfter Not Open File : " + filePath);
                                    _error.AddLog("DragDropAfter Not Open File : " + newpath);
                                }
                            } else
                            {
                                _error.AddLog("DragDropAfter Not Open File : " + filePath);
                            }
                        }
                        if (_error.hasAlert)
                        {
                            _error.AddLogAlert(new Exception("ExcelFileOpenByDragDrop Failed : path =" + filePath));
                            _error.Messenger.ShowAlertMessages();
                        }
                    }

                    _excelCellsManagerMain.UpdateList();
                }

            } catch (Exception ex)
            {
                _error.AddException(ex,_thisClassName + ".DragDropAfter");
                _error.Messenger.ShowAlertMessages();
            }
        }
        // Button 更新
        private void Button_UpdateList_Click(object sender, EventArgs e)
        {
            _excelCellsManagerMain.UpdateList(true);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        // Button アドレスを登録する
        private void Button_RegistAddress_Click(object sender, EventArgs e)
        {
            _excelCellsManagerMain.AddList();
        }
        // Button 終了
        private void Button_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Button セルを選択する
        private void Button_SelectCells_Click(object sender, EventArgs e)
        {
            _excelCellsManagerMain.SelectCells();
            _excelCellsManagerMain.ActivateWorkbookWindowActivate();
        }
        // Button Workbook を閉じる
        private void Button_CloseWorkbook_Click(object sender, EventArgs e)
        {
            _excelCellsManagerMain.CloseSave();
            
        }

        private void ExcelCellsManagerForm_Load(object sender, EventArgs e)
        {

        }
        // アプリケーションを閉じる
        private void ExcelCellsManagerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _error.AddLog("******************** ExcelCellsManagerForm_FormClosed Begin ********************");
                // 最後のファイル名設定クラスへ、この値のみ個別で処理を行う、設定保持タイミングが異なるため
                _excelCellsManagerMain.AppsSettings.SetLastFilePathToSettingsObject(
                    _excelCellsManagerMain.OpenedFile.GetPath());

                // 設定を Class メンバフィールドから Register 
                _excelCellsManagerMain.AppsSettings.SetSettingsValueMemberToSettingsClass();
                if (_error.HasException())
                {
                    _error.AddLog("_excelCellsManagerMain.AppsSettings.SetSettingsValueMemberToSettingsClass Failed.");
                    _error.ClearError();
                }

                // 設定を書き込む
                _excelCellsManagerMain.AppsSettings.Register.SaveValuesToFile();
                if (_error.HasException())
                {
                    _error.AddLog("_excelCellsManagerMain.AppsSettings.Register.SaveValuesToFile Failed.");
                    _error.ClearError();
                }

                _error.AddLog(_thisClassName + ".ExcelCellsManagerForm_FormClosed Button Close");
                // エラーログを書き込む
                _error.WriteErrorLog();
                string msg;
                if (_error.hasAlert)
                {
                    // エラーログ書き込み失敗
                    msg = "WriteErrorLog Failed\n" + _error.GetExceptionMessageAndStackTrace();
                    MessageBox.Show(msg, "WriteLogError");
                    return;
                }
                else
                {
                    // エラーログ書き込み成功
                    _error.AddLog("ExcelCellsManagerForm_FormClosed.WriteErrorLog Success [" + _error.GetErrorLogPath + "]");
                }
                // ログ書き込み
                _error.WriteLog();
                if (_error.hasAlert)
                {
                    // ログ書き込み失敗
                    msg = "WriteLog Failed\n" + _error.GetExceptionMessageAndStackTrace();
                    MessageBox.Show(msg, "WriteLogError");
                    return;
                }
                else
                {
                    // ログ書き込み成功
                    _error.AddLog("ExcelCellsManagerForm_FormClosed.WriteLog Success [" + _error.GetLogPath + "]");
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, _thisClassName + ".ExcelCellsManagerForm_FormClosed Failed : WriteLogError");
                _error.Messenger.ShowAlertMessages();
            }
        }

        private void StatusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        // ActiveCell を選択する(Workbook を Activate する)
        private void Button1_Click(object sender, EventArgs e)
        {
            // GetActiveCell
            _ = _excelCellsManagerMain.ExcelManager.GetActivateCell();
            if (_error.hasAlert) { _error.Messenger.ShowAlertMessages(); }
            else { _error.Messenger.ShowResultSuccessMessage("ActiveCell Selected."); }
        }

        // DataList の Item を一つ上に移動する
        private void Button_Up_CheckedListBoxItem_Click(object sender, EventArgs e)
        {
            _excelCellsManagerMain.MoveUpItemInDataGridViewList();
        }
        // DataList の Item を一つ下に移動する
        private void Button_Down_CheckedListBoxItem_Click(object sender, EventArgs e)
        {
            _excelCellsManagerMain.MoveItemDownInDataGridViewList();
        }

        private void ExcelCellsManagerForm_KeyDown(object sender, KeyEventArgs e)
        {

        }
        // Event
        // Form を閉じるとき
        private void ExcellCellsManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // 編集状態の時保存するか確認する
                int ret = _excelCellsManagerMain.ConformWhenDataEdited();
                if (ret == 2)
                {
                    // ret==2 ダイアログキャンセル時は閉じるもキャンセルする
                    e.Cancel = true;
                    return;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString()+ ".ExcellCellsManagerForm_FormClosing");
                _error.Messenger.ShowAlertMessages();
            }
        }

        private void statusStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Button_CopyRangeValue_Click(object sender, EventArgs e)
        {
            _excelCellsManagerMain.CopyCellsValue(true);
        }
    }
}
