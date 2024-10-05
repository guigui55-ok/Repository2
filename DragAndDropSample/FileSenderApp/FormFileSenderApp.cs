using AppLoggerModule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JsonStreamModule;
using System.IO;
using System.Threading;
using FileSenderApp.Job;

namespace FileSenderApp
{
    public partial class FormFileSenderApp : Form
    {
        public AppLogger _logger;
        SenderMainTab _senderMainTab;
        JsonStream _jsonStream;
        public bool isOutputSetting = true; // アプリ起動終了時、setting.jsonをログ・コンソールに出力するか。（長くなるのでフラグで管理＞デバッグ用）
        public FileSenderSettingValues _fileSenderSettingValues;
        public DataBridgeFromExternal _dataBridgeFromExternal;

        //サブフォームとして呼び出すか
        // trueの場合は、Close時 e.Cancel=true、this.Visible=falseとして、オブジェクトを破棄しない（再度呼び出せるように）
        public bool _isSubForm = false;

        //ボタンが押されたときのイベント
        // ファイル移動後
        public EventHandler AnySendButton_Clicked;
        // ファイル移動前
        public EventHandler AnySendButton_Clicked_MoveBefore;
        public bool _isMoveFile = false;
        //
        //public int _waitTime = 50; // m_secontd
        public int _waitTime = 0; // m_secontd
        // 241006 追加
        // ReDo、Undo用
        public JobManager _jobManager;
        public EventHandler ExecuteUndo_After;
        public EventHandler ExecuteRedo_After;
        public EventHandler ExecuteUndo_Before;
        public EventHandler ExecuteRedo_Before;

        public FormFileSenderApp(AppLogger logger=null)
        {
            InitializeComponent();
            if (logger == null){_logger = new AppLogger();} else { _logger = logger; }
            _logger.LogOutPutMode = OutputMode.DEBUG_WINDOW | OutputMode.FILE;
            _logger.FilePath = Directory.GetCurrentDirectory() + "\\" + "__test_log.log";
            _logger.PrintInfo("##############################");
            //#
            //以下の処理は デザイナで設定したタブの設定となる。これは削除予定
            _senderMainTab = new SenderMainTab(_logger, tabControl1, panel1, panelCheckBox, this);
            _senderMainTab.Initialize();
            //#
            //初期化処理①
            textBoxRenameTabPage.Parent = panel1;
            _dataBridgeFromExternal = new DataBridgeFromExternal(_logger);
            //#
            //設定値読み込み初期化
            _jsonStream = new JsonStream();
            SettingFileInitialize();

            this.KeyPreview = true;
            ClearTabControl();
            _senderMainTab = new SenderMainTab(_logger, tabControl1, panel1, panelCheckBox, this);
            _senderMainTab.Initialize();
            //_senderMainTab.AddTabPage();
            AddPageBySetting();
            _logger.PrintInfo("FormFileSenderApp Initialize End");
            _logger.PrintInfo("==========");
            //#
            List<string> jobKeyList = new List<string>()
            {
                ConstJobFileSender.KEY_PROCESS_NAME,
                ConstJobFileSender.KEY_SRC_FILE_PATH,
                ConstJobFileSender.KEY_DST_FILE_PATH,
                ConstJobFileSender.KEY_UNDO
            };
            _jobManager = new JobManager(_logger, jobKeyList);
        }

        private void ApplySetting()
        {
            try
            {
                _logger.PrintInfo("ApplySetting");
                bool isCheckON = (bool)_fileSenderSettingValues._settingDictBase[ConstFileSender.KEY_SETTING_FILE_MOVE_CHECK_BOX];
                if (isCheckON)
                {
                    checkBoxFileMove.Checked = true;
                }
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "ApplySetting");
            }
        }

        public void SetDataFromExternal(object value)
        {
            _logger.PrintInfo(string.Format("SetDataFromExternal [{0}]", value));
            this._dataBridgeFromExternal.SetData(value);
        }

        public void AnyButtonClickedRecieveEvent(object sender , EventArgs e)
        {
            _logger.PrintInfo("AnyButtonClickedRecieveEvent");
            //#
            // 
            try
            {
                SendButton sendButton = (SendButton)sender;
                string distDirPath = sendButton._directoryPath;
                string srdFilePath = _dataBridgeFromExternal.GetData<string>();
                _logger.PrintInfo("srcFilePath = " + srdFilePath.ToString());
                AnySendButton_Clicked_MoveBefore?.Invoke(sender, e);
                bool isExecute = FileMoveOrCopy(srdFilePath, distDirPath, checkBoxFileMove.Checked);
                if (isExecute)
                {
                    if (AnySendButton_Clicked == null) { _logger.PrintInfo("AnySendButton_Clicked==null"); }
                    AnySendButton_Clicked?.Invoke(sender, e);
                    SetUndo(srdFilePath, distDirPath, checkBoxFileMove.Checked);
                    SetButtonEnableRedoUndo();
                }
                else
                {
                    AnySendButton_Clicked?.Invoke(sender, e);
                    //_logger.PrintInfo("FileMoveOrCopy isExcute=false , StopButtonProc");
                }
            } catch (Exception ex)
            {
                _logger.PrintError(ex, "AnyButtonClickedRecieveEvent");
            }
            _logger.PrintInfo(" ***** AnyButtonClickedRecieveEvent END");
        }

        // ##############################
        // # RedoUndo関連 BEGIN

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFilePath"></param>
        /// <param name="distDirPath"></param>
        /// <param name="isFileMove"></param>
        private void SetUndo(string srcFilePath, string distDirPath, bool isFileMove)
        {
            _logger.PrintInfo("SetUndo");
            string distFilePath = GetDistFilePath(srcFilePath, distDirPath);
            Dictionary<string, object> newDict = new Dictionary<string, object>();
            newDict.Add(ConstJobFileSender.KEY_SRC_FILE_PATH, srcFilePath);
            newDict.Add(ConstJobFileSender.KEY_DST_FILE_PATH, distFilePath);
            string proc;
            if (isFileMove) { proc = ConstJobFileSender.VAL_PROCESS_MOVE; }
            else { proc = ConstJobFileSender.VAL_PROCESS_COPY; }
            newDict.Add(ConstJobFileSender.KEY_PROCESS_NAME, proc);
            newDict.Add(ConstJobFileSender.KEY_UNDO, false);
            _jobManager.AddJobItem(newDict);
        }

        private void ExecuteUndoRedo(object undoJobItemObj, int redoOrUndo)
        {
            try
            {
                _logger.PrintInfo(" ********** ");
                _logger.PrintInfo("ExecuteUndoRedo");
                JobItem item = (JobItem)undoJobItemObj;
                string src = (string)item._valueDict[ConstJobFileSender.KEY_SRC_FILE_PATH];
                string dst = (string)item._valueDict[ConstJobFileSender.KEY_DST_FILE_PATH];
                string proc = (string)item._valueDict[ConstJobFileSender.KEY_PROCESS_NAME];
                bool isMove = false;
                if (proc == ConstJobFileSender.VAL_PROCESS_MOVE) { isMove = true; }
                bool undo = (bool)item._valueDict[ConstJobFileSender.KEY_UNDO];
                if (redoOrUndo == ConstFileSender.UNDO)
                {
                    _logger.PrintInfo("Execute UNDO");
                    ExecuteUndo_Before?.Invoke(this, EventArgs.Empty);
                    if (proc == ConstJobFileSender.VAL_PROCESS_COPY)
                    {
                        //コピーの場合は、コピー先を消すだけ
                        File.Delete(dst);
                        _logger.PrintInfo(string.Format("DeleteFile : ", dst));
                    }
                    else if (proc == ConstJobFileSender.VAL_PROCESS_MOVE)
                    {
                        //Moveの場合は、移動先から移動元に再移動する
                        File.Move(dst, src);
                        _logger.PrintInfo(string.Format("MoveFile : {0}  >>  {1}", dst, src));
                    }
                    else
                    {
                        _logger.PrintError(string.Format("ExecuteUndo, InvalidValue Proc={0}", proc));
                    }
                    SetButtonEnableRedoUndo();
                    ExecuteUndo_After?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _logger.PrintInfo("Execute REDO");
                    ExecuteRedo_Before?.Invoke(this, EventArgs.Empty);
                    // redoOrUndo == ConstFileSender.REDO
                    bool isSuccess = FileMoveOrCopy(src, dst, isMove);
                    if (!isSuccess)
                    {
                        _jobManager.ReverseItemJobListToUndo();
                    }
                    SetButtonEnableRedoUndo();
                    //Event
                    ExecuteRedo_After?.Invoke(this, EventArgs.Empty);
                }
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "ExecuteUndo");
            }
        }

        private void SetButtonEnableRedoUndo()
        {
            if (_jobManager.IsUndoEnable())
            {
                buttonUndo.Enabled = true;
            }
            else
            {
                buttonUndo.Enabled = false;
            }
            if (_jobManager.IsRedoEnable())
            {
                buttonRedo.Enabled = true;
            }
            else
            {
                buttonRedo.Enabled = false;
            }
        }

        // # RedoUndo関連 END
        // ##############################

        private string GetDistFilePath(string srcFilePath, string distDirPath)
        {
            return Path.Combine(distDirPath, Path.GetFileName(srcFilePath));
        }


        public bool FileMoveOrCopy(string srcFilePath, string distDirPath, bool isMove)
        {
            string distFilePath = "";
            try
            {
                if (!(File.Exists(srcFilePath)))
                {
                    string msg = string.Format("File Is Not Found [{0}]", srcFilePath);
                    _logger.PrintInfo(msg);
                    return false;
                }
                distFilePath = GetDistFilePath(srcFilePath, distDirPath);
                bool isExecute = false;
                if (File.Exists(distFilePath))
                {
                    string msg = "ファイルが既に存在します。上書きしますか？";
                    DialogResult ret = MessageBox.Show(msg, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (ret == DialogResult.Cancel)
                    {
                        _logger.PrintInfo("FileMoveOrCopy Cancel");
                        isExecute = false;
                    }
                    else
                    {
                        _logger.PrintInfo("FileMoveOrCopy Delete");
                        File.Delete(distFilePath);
                        //await Task.Delay(100);
                        Thread.Sleep(_waitTime);
                        isExecute = true;
                    }
                }
                else
                {
                    isExecute = true;
                }
                if (isExecute)
                {
                    if (isMove)
                    {
                        File.Move(srcFilePath, distFilePath);
                        //CommonModules.CommonGeneral.MoveFile(srcFilePath, distFilePath);
                        //CommonModules.CommonGeneral.MoveFile(srcFilePath, distFilePath);
                        //int errcode = CommonModules.FileMoverWin32.MoveFileExUsingWin32(srcFilePath, distFilePath, 0x2);
                        //if (0 != errcode)
                        //{
                        //    _logger.PrintError("MoveFile Failed");
                        //    if (errcode == 3)
                        //    {
                        //        _logger.PrintError("指定したパスが見つかりません。 ERROR_TOO_MANY_OPEN_FILES");
                        //    }
                        //}
                        _logger.PrintInfo(string.Format("Move ,src={0}, dist={1}", srcFilePath, distDirPath));
                        Thread.Sleep(_waitTime);
                    }
                    else
                    {
                        File.Copy(srcFilePath, distFilePath, true);
                        _logger.PrintInfo(string.Format("Copy ,src={0}, dist={1}", srcFilePath, distDirPath));
                        Thread.Sleep(_waitTime);
                    }
                }
                return isExecute;
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "FileMoveOrCopy Error");
                //#
                // エラー調査用
                // System.IO.IOException : ファイルが別のプロセスで使用されているため、プロセスはファイルにアクセスできません。
                string handlePath = @"C:\Users\OK\source\repos\Repository2_CS\ImageViewer5\ImageViewer5\bin\Debug\Handle\handle.exe";
                if (File.Exists(handlePath))
                {
                    string handleRet = CommonModules.FileLockInfo.GetFileLockingProcess(handlePath, srcFilePath);
                    _logger.PrintInfo(string.Format("handleRet src = {0}", handleRet));
                    handleRet = CommonModules.FileLockInfo.GetFileLockingProcess(handlePath, distFilePath);
                    _logger.PrintInfo(string.Format("handleRet dst = {0}", handleRet));
                }
                else
                {
                    _logger.PrintInfo(string.Format("hendlePath not Founc [{0}]", handlePath));
                }
                //#
                return false;
            }
        }

        public bool IsCheckedMove()
        {
            return checkBoxFileMove.Checked;
        }


        /// <summary>
        /// 設定値を反映させる（設定値を読み取りパーツを配置していく）
        /// , 設定値は関数下のコメントようなものが渡る
        /// </summary>
        private void AddPageBySetting()
        {
            _logger.PrintInfo("AddPageBySetting");
            // 設定のTabPageの数だけ取得する
            List<Dictionary<string, object>> settingList = _fileSenderSettingValues.GetListMatchValues("TabPage");
            foreach(Dictionary<string, object> settingDict in settingList)
            {
                TabPage newTabPage = _senderMainTab.AddTabPage();
                Dictionary<string, object> settingDictB = (Dictionary<string, object>)settingDict;
                _logger.PrintInfo("AddPageBySetting >> SettingDictB");
                CommonModule.CommonGeneral.PrintDict(settingDictB, withDataType:true);
                //#
                // key=TabPageName: TabPageSettingDict となっているはず
                string newTabPageName = settingDict.Keys.ToArray()[0];
                newTabPage.Name = newTabPageName;
                //#
                // 型 'Newtonsoft.Json.Linq.JObject' のオブジェクトを型 'System.Collections.Generic.Dictionary`2[System.String,System.Object]' にキャストできません。
                //_logger.PrintInfo(dictObj.GetType().ToString()); //Newtonsoft.Json.Linq.JObject
                //Dictionary<string, object> newTabPageSettingDict = (Dictionary<string, object>)obj;
                //Dictionary<string, object> newTabPageSettingDict = (Dictionary<string, object>)settingDictB[newTabPageName];
                //Dictionary<string, object> newTabPageSettingDict = jObject.ToObject<Dictionary<string, object>>();
                object dictObj = (object)settingDictB[newTabPageName];
                Dictionary<string, object> newTabPageSettingDict = JsonStream.ConvertJObjectToDict(dictObj);
                // ボタン設定
                ButtonsGroup buttonsGroup = _senderMainTab.GetButtonsGroupMatchName(newTabPage);
                _logger.PrintInfo("AddPageBySetting >> Button");
                CommonModule.CommonGeneral.PrintDict(newTabPageSettingDict);
                //SendButton sendButton = buttonsGroup.AddButton();
                buttonsGroup.DeleteAllButton();
                buttonsGroup.ApplySettingButton(newTabPageSettingDict);
                buttonsGroup.SendButtonClickEvent -= AnyButtonClickedRecieveEvent;
                buttonsGroup.SendButtonClickEvent += AnyButtonClickedRecieveEvent;
            }
            /*
             * 
             * 
  "TabPage0": {
    "SendButton1": {
      "ButtonName": "SendButton1",
      "ButtonText": "SendButton1",
      "DirectoryPath": "",
      "ButtonColorText": "Color [Control]",
      "ButtonColor": "Control"
    },
    "TabPageText": "NewTab1"
  },
  "TabPage1": {
    "SendButton1": {
      "ButtonName": "SendButton1",
      "ButtonText": "SendButton1",
      "DirectoryPath": "",
      "ButtonColorText": "Color [Control]",
      "ButtonColor": "Control"
    },
    "TabPageText": "NewTab2"
  }
             * 
             */
        }


        /// <summary>
        /// TabControlをクリアして、新しいタブコントロールとTabPage1つを追加
        /// </summary>
        private void ClearTabControl()
        {
            _logger.PrintInfo("ClearTabControl");
            TabControl tab = this.tabControl1;
            TabControl newTab = new TabControl();
            string tabName;
            this.Controls.Remove(tab);
            this.Controls.Add(newTab);
            //
            tabName = tab.Name;
            newTab.Name = tab.Name;
            newTab.Anchor = tab.Anchor;
            newTab.Dock = tab.Dock;
            newTab.Location = tab.Location;
            newTab.Size = tab.Size;
            //
            tab.Visible = false;
            tab.Dispose();
            tab = null;
            //
            CreateNewTabPage(newTab, 1);
            //CreateNewTabPage(newTab, 2);
            newTab.TabPages[0].Text = "[+]Add Tab";
            newTab.TabPages[0].Font = new System.Drawing.Font("MS UI Gothic", 9F);
            tabControl1 = newTab;
            _logger.PrintInfo(string.Format("CreateTab = {0}", newTab.Name));
            //#
        }


        /// <summary>
        /// 新しいタブページを追加（ClearTabControlで使用する）
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private TabPage CreateNewTabPage(TabControl tab, int index)
        {
            _logger.PrintInfo("CreateNewTabPage");
            TabPage tabPage1 = new TabPage();
            tab.Controls.Add(tabPage1);
            tabPage1.BackColor = System.Drawing.SystemColors.Control;
            //tabPage1.Controls.Add(this.buttonsGroup1);
            tabPage1.Location = new System.Drawing.Point(4, 22);
            tabPage1.Name = "tabPage" + index;
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(342, 266);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Tab" + index;
            return tabPage1;
        }

        /// <summary>
        /// 設定ファイルの読み込み、
        /// 　読み込みが無効なときはConstからデフォルト値を読み込み、ファイルを作成する。
        /// </summary>
        private void SettingFileInitialize()
        {
            _logger.PrintInfo("SettingFileInitialize");
            //string dirPath = CommonModule.CommonGeneral.GetParentDirectory(Directory.GetCurrentDirectory(), 1);
            string dirPath = Directory.GetCurrentDirectory();
            string path = Path.Combine(dirPath, ConstFileSender.SETTING_FILE_NAME);
            _logger.PrintInfo(string.Format("path = {0}", path));
            if (File.Exists(path))
            {
                _jsonStream._path = path;
                // readSetting
            }
            else
            {
                string json = ConstFileSender.DEFAULT_JSON;
                _jsonStream._path = path;
                Dictionary<string, object> dict = _jsonStream.JsonToDict(json);
                _jsonStream.WriteFile(dict);
                _logger.PrintInfo("Create New Setting File");
            }
            Dictionary<string, object> settingDict = _jsonStream.ReadFile();
            // ここで読み込みエラーがある場合は、ファイルのバックアップを取ってデフォルト値からやり直し
            if (isOutputSetting)
            {
                //CommonModule.CommonGeneral.PrintDict(settingDict);
                string json = _jsonStream.DictToJson(settingDict);
                _logger.PrintInfo("settingDict = ");
                _logger.PrintInfo(json);
            }

            _fileSenderSettingValues = new FileSenderSettingValues(_logger);
            _fileSenderSettingValues.ReadSettingDict(settingDict);
        }

        private void CreateDefaultInfoDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object> { };
            dict = _jsonStream.JsonToDict(ConstFileSender.DEFAULT_JSON);
        }

        private void BackUpFile(string path)
        {
            string newFilePath = CreateNewFile(path);
            System.IO.File.Copy(path, newFilePath, true);
            _logger.PrintInfo("BackUpFile");
            _logger.PrintInfo(string.Format("BackUpFile SrcPath = {0}", path));
            _logger.PrintInfo(string.Format("BackUpFile DistPath = {0}", newFilePath));
        }

        /// <summary>
        /// 新しいファイル名を作成する
        ///  , path.ext > path_1.ext 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string CreateNewFile(string path)
        {
            try
            {
                string dirPath = Directory.GetDirectoryRoot(path);
                string fileName = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);
                int lastNum = GetLastNumber(fileName);
                string numStr = (lastNum + 1).ToString();
                fileName = RemoveSuffix(fileName, "_" + lastNum.ToString());
                string newFileName = fileName + "_" + numStr + ext;
                string newFilePath = Path.Combine(dirPath, newFileName);
                if (File.Exists(newFilePath))
                {
                    return CreateNewFile(newFilePath);
                }
                else
                {
                    return newFilePath;
                }
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "CreateNewFile");
                return path;
            }
        }

        public string RemoveSuffix(string input, string suffix)
        {
            // 文字列がnullや空でないか、またsuffixがnullや空でないかを確認
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(suffix))
            {
                return input;
            }

            // 入力文字列がサフィックスで終わっているか確認
            if (input.EndsWith(suffix))
            {
                // サフィックス部分を除去して返す
                return input.Substring(0, input.Length - suffix.Length);
            }

            // 一致しない場合はそのまま返す
            return input;
        }
        private int GetLastNumber(string value)
        {
            try
            {
                string numStr = GetLastNumberRegix(value);
                return int.Parse(numStr);
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "GetLastNumber");
            }
            return 0;
        }

        private string GetLastNumberRegix(string value)
        {
            // 正規表現で文字列の末尾の連続する数字を抽出
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(value, @"\d+$");

            // マッチする数字がなければ空文字を返す
            if (!match.Success)
            {
                return string.Empty;
            }

            // マッチした数字部分を文字列として返す
            return match.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void TextBoxRenameTab_KeyDown(object sender, KeyEventArgs e)
        {
            _logger.PrintInfo("TextBoxRenameTab_KeyDown");
            if (e.KeyCode == Keys.Enter)
            {
                _logger.PrintInfo("TextBoxRenameTab_KeyDown  Enter");
                ButtonRenameTabOK_Click(buttonRenameTabOK, EventArgs.Empty);
            }
        }

        private void ButtonRenameTabOK_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("ButtonRenameTabOK_Click");
            if (textBoxRenameTabPage.Text != "")
            {
                _senderMainTab._tabControl.TabPages[_senderMainTab._tabControl.SelectedIndex].Text = textBoxRenameTabPage.Text;
            }
            _senderMainTab.UnvisibleRenameControl();
        }

        private void TextBoxRenameTab_KeyPress(object sender, KeyPressEventArgs e)
        {
            //EnterやEscapeキーでビープ音が鳴らないようにする
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
        }

        private void FormFileSenderApp_Load(object sender, EventArgs e)
        {
            _logger.PrintInfo("FormFileSenderApp_Load");
            _logger.PrintInfo("==============================");
            ApplySetting();
        }


        private void FormFileSenderApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            _logger.PrintInfo("FormFileSenderApp_FormClosing");
            if (_isSubForm)
            {
                _logger.PrintInfo("_isSubForm = true, e.Cancel=true, this.Visible=false");
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void FormFileSenderApp_KeyDown(object sender, KeyEventArgs e)
        {
            _logger.PrintInfo("FormFileSenderApp_KeyDown");
            if (e.KeyCode == Keys.NumPad5)
            {
                _logger.PrintInfo("FormFileSenderApp_KeyDown  NumPad5");
                FormFileSenderApp_FormClosing(this, new FormClosingEventArgs(CloseReason.None, false));
            }
            else if (e.KeyCode == Keys.Z && e.Control)
            {
                _logger.PrintInfo("FormFileSenderApp_KeyDown  Ctrl+Z");
                JobItem item = _jobManager.GetUndoItem();
                ExecuteUndoRedo(item, ConstFileSender.UNDO);
            }
            else if (e.Shift && e.Control && e.KeyCode == Keys.Z)
            {
                _logger.PrintInfo("FormFileSenderApp_KeyDown  Ctrl+Shift+Z");
                JobItem item = _jobManager.GetRedoItem();
                ExecuteUndoRedo(item, ConstFileSender.REDO);
            }
        }
        public void FormFileSenderApp_FormClosed(object sender, FormClosedEventArgs e)
        {
            _logger.PrintInfo("******************************");
            _logger.PrintInfo("FormFileSenderApp_FormClosed");
            TabControl tabControl = this.tabControl1;
            Dictionary<string, object> allInfoDict = _senderMainTab.GetValueAll();
            //_senderMainTab.PrintInfoDictAll(allInfoDict);
            //_jsonStream.WriteFile(allInfoDict);
            CommonModule.CommonGeneral.PrintDict(allInfoDict, withDataType: false);
            _jsonStream.WriteFile(allInfoDict);
            if (isOutputSetting)
            {
                //CommonModule.CommonGeneral.PrintDict(settingDict);
                string json = _jsonStream.DictToJson(allInfoDict);
                _logger.PrintInfo("allInfoDict = ");
                _logger.PrintInfo(json);
            }
        }

        private void FormFileSenderApp_Activated(object sender, EventArgs e)
        {
            SetButtonEnableRedoUndo();
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            JobItem item = _jobManager.GetUndoItem();
            ExecuteUndoRedo(item, ConstFileSender.UNDO);
        }

        private void buttonRedo_Click(object sender, EventArgs e)
        {
            JobItem item = _jobManager.GetRedoItem();
            ExecuteUndoRedo(item, ConstFileSender.REDO);
        }
    }
}
