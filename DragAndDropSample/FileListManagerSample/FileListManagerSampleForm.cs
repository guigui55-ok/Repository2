using CommonUtility.FileListUtility.FileListControl;
using CommonUtility.FileListUtility;
using DragAndDropSample;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AppLoggerModule;

namespace FileListManagerSample
{
    public partial class FileListManagerSampleForm : Form
    {
        protected DragAndDropOnControl _dragAndDropOnControl;
        protected DragAndDropForFile _dragAndDropForFile;
        // Files Class
        public IFiles _files;
        public CommonUtility.FileListUtility.FileListManager _fileListManager;
        protected List<string> _fileList;
        public IFileListControl _fileListControl;
        //public List<string> _supportedFileExtentionList = new List<string> { };
        // 
        protected AppLogger _logger;
        // 外部フォームから使用したいときで、ドラッグドロップ処理をやめたいとき
        public bool _dragDropEnabled;
        // 起動開始時のフォームがこのフォームかどうかを保持する（ダイアログクローズ時オブジェクトを破棄しないようにするため）
        public bool _startedThisForm = true;
        //#
        //設定フォーム（メンバに設定値を持つ）
        public FileListManagerSettingForm _fileListManagerSettingForm;
        //#
        //string Name = "FileListManagerSampleForm";
        public FileListManagerSampleForm(AppLogger logger=null, bool dragDropEnable=true, bool startedThisForm=true)
        {
            InitializeComponent();
            _startedThisForm = startedThisForm;
            if (logger == null)
            {
                _logger = new AppLogger();
                _logger.SetDefaultValues();
            }
            else
            {
                _logger = logger;
            }
            this._dragDropEnabled = dragDropEnable;
            if (_dragDropEnabled)
            {
                _dragAndDropOnControl = new DragAndDropOnControl(_logger, this);
                _dragAndDropOnControl.AddRecieveControls(new Control[] { richTextBox_Log, textBox_DirectoryPath, listBox_FileList });
                _dragAndDropForFile = new DragAndDropForFile(this._logger, _dragAndDropOnControl);
                _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
            }
            // Files Object
            _fileList = new List<string>();
            _files = new Files(_logger, _fileList);
            _fileListManager = new FileListManager(_logger, _files);
            _fileListControl = new FileListControlListBox(_logger, this.listBox_FileList, _files);
            _fileListManager.UpdateFileListAfterEvent += _fileListControl.UpdateFileListAfterEvent;
            _fileListManager.UpdateFileListAfterEvent += FileListManagerForm_UpdateFileListAfterEvent;
            // _filesのリストが直接変更されたとき、コントロールに反映させる
            _files.SelectedFileEvent += _fileListControl.ChangedFileInFile;
            // コントロールの一覧がクリックされたとき、_filesの選択に反映させる
            _fileListControl.SelectedItemEvent += _files.ChangeFileRecieve;
            //Setting
            _fileListManagerSettingForm = new FileListManagerSettingForm(_logger, _fileListManager._fileListManagerSetting);
            //this.listBox_FileList.SelectedIndexChanged += listBox_FileList_ChangedItem;
        }

        public void DisposeObjects()
        {
            try
            {
                _fileListControl.SelectedItemEvent -= _files.ChangeFileRecieve;
                _files.SelectedFileEvent -= _fileListControl.ChangedFileInFile;
                _files.FileList.Clear();
                _files.FileList = null;
                _files = null;
                _fileListManager.UpdateFileListAfterEvent -= _fileListControl.UpdateFileListAfterEvent;
                _fileListManager.UpdateFileListAfterEvent -= FileListManagerForm_UpdateFileListAfterEvent;
                _fileListManager.Dispose();
                _fileListManager = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void FileListManagerForm_UpdateFileListAfterEvent(object sender, EventArgs e)
        {
            try
            {
                this.textBox_DirectoryPath.Text = _files.DirectoryPath;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "FileListManagerForm_UpdateFileListAfterEvent");
            }
        }

        /// <summary>
        /// ファイルパスから、ファイルリストをセットする（外部から操作する用）
        /// <para></para>
        /// FilterList, IgnoreList は FileListManager._filesRegister に保存される
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="supportedImageExtentionList">
        /// 対応する拡張子リスト。不要な場合はnullを渡す
        /// nullの場合でも、_fileListManagerのメンバ変数 _supportedImageExtentionList に設定されていたらこれを使用する
        /// 無効にするときは List string {} を渡す
        /// </param>
        public void SetFilesFromPath(string filePath, List<string> fileFilterConditionList, List<string> fileIgnoreConditionList)
        {
            _fileListManager.SetFilesFromPath(filePath, fileFilterConditionList, fileIgnoreConditionList);
        }
        /// <summary>
        /// カレントファイルが更新された後のイベントに紐づけする
        /// </summary>
        public void AddEventHandler_SelectedFileEvent(EventHandler eventMethod)
        {
            _files.SelectedFileEvent += eventMethod;
        }

        /// <summary>
        /// ファイルリストが更新された後のイベントに紐づけする
        /// </summary>
        public void AddEventHandler_UpdateFileListAfterEvent(EventHandler eventMethod)
        {
            _fileListManager.UpdateFileListAfterEvent += eventMethod;
        }

        private void DragAndDropEventAfterEventForFile(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "DragAndDropEventAfterEventForFile");
                if (_dragAndDropForFile.Files == null) { _logger.AddLogWarning("Files == null"); return; }
                if (_dragAndDropForFile.Files.Length < 1) { _logger.AddLogWarning("Files.Length < 1"); return; }

                _logger.AddLog("  GetPath=" + _dragAndDropForFile.Files[0]);
                this.textBox_DirectoryPath.Text = _dragAndDropForFile.Files[0];
                AddLog("SetDirectoryPath=" + _dragAndDropForFile.Files[0]);
                textBox_DirectoryPath.Text = _dragAndDropForFile.Files[0];
                _fileListManager.SetFilesFromPath(_dragAndDropForFile.Files[0]);
                if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(_files.GetCurrentValue()); }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "DragAndDropEventAfterEventForFile");
                _logger.ClearError();
            }
        }
        private void AddLog(string value,string sepaleteValue = "\n")
        {
            richTextBox_Log.AppendText(value + sepaleteValue);
            richTextBox_Log.ScrollToCaret();
        }

        private void FileListManagerForm_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _fileListManager.MoveNextDirectory();
            AddLog("SetDirectoryPath=" + _files.DirectoryPath);
            if (_dragDropEnabled)
            {
                if (_dragAndDropForFile.Files == null)
                {
                    AddLog("_dragAndDropForFile.Files == null");
                    return;
                }
                textBox_DirectoryPath.Text = _dragAndDropForFile.Files[0];
                if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(0); }
            }
            else
            {
                if (0 < _files.FileList.Count)
                {
                    _fileListControl.SelectItem(0);
                }
                textBox_DirectoryPath.Text = _files.GetCurrentValue();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _fileListManager.MovePreviousDirectory();
            AddLog("SetDirectoryPath=" + _files.DirectoryPath);

            if (_dragDropEnabled)
            {
                if (_dragAndDropForFile.Files == null)
                {
                    AddLog("_dragAndDropForFile.Files == null");
                    return;
                }
                textBox_DirectoryPath.Text = _dragAndDropForFile.Files[0];
                if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(0); }
            }
            else
            {
                if (0 < _files.FileList.Count)
                {
                    _fileListControl.SelectItem(0);
                }
                textBox_DirectoryPath.Text = _files.GetCurrentValue();
            }
        }

        private void button_MovePreviousFile_Click(object sender, EventArgs e)
        {
            _fileListManager.MoveProviousFileWhenFirstFilePreviousDirectory();
            if (_dragDropEnabled)
            {
                //AddLog("SetDirectoryPath=" + _files.DirectoryPath);
                if (_dragAndDropForFile.Files == null) { _logger.AddLog("_dragAndDropForFile.Files == null"); return; }
                if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(_files.GetCurrentValue()); }
            }
            else
            {
                if (0 < _files.FileList.Count)
                {
                    _fileListControl.SelectItem(_files.GetCurrentValue());
                }
                textBox_DirectoryPath.Text = _files.GetCurrentValue();
                string path = "";
                if (listBox_FileList.SelectedItem == null)
                {
                    path = "null";
                }
                else
                {
                    path = listBox_FileList.SelectedItem.ToString();
                }
                AddLog("PrevFIle:" + path);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _fileListManager.MoveNextFileWhenLastFileNextDirectory();
            if (_dragDropEnabled)
            {
                //AddLog("SetDirectoryPath=" + _files.DirectoryPath);
                if (_dragAndDropForFile.Files == null) { _logger.AddLog("_dragAndDropForFile.Files == null"); return; }
                if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(_files.GetCurrentValue()); }
            }
            else
            {
                if (0 < _files.FileList.Count)
                {
                    _fileListControl.SelectItem(_files.GetCurrentValue());
                }
                textBox_DirectoryPath.Text = _files.GetCurrentValue();
                string path = "";
                if (listBox_FileList.SelectedItem == null)
                {
                    path = "null";
                }
                else
                {
                    path = listBox_FileList.SelectedItem.ToString();
                }
                AddLog("NextFIle:" + path);
            }
        }

        private void FileListManagerForm_Closing(object sender, FormClosingEventArgs e)
        {
            AddLog("FileListManagerSampleForm > Closing  _startedThisForm=" + _startedThisForm.ToString());
            // 起動元が自身の FileListManagerSampleForm以外なら終了させない
            if (!_startedThisForm)
            {
                this.Visible = false;
                e.Cancel = true;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            _fileListManagerSettingForm._isPshedShowButton = true;
            _fileListManagerSettingForm.Show();
        }

        private void listBox_FileList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Down)
            {
                _logger.PrintInfo("listBox_FileList_ChangedItem KeyDown");
                string value = listBox_FileList.SelectedItem.ToString();
                _files.Move(value);
            } else if(e.KeyCode == Keys.Up)
            {
                _logger.PrintInfo("listBox_FileList_ChangedItem KeyUp");
                string value = listBox_FileList.SelectedItem.ToString();
                _files.Move(value);
            }
        }

        //private void listBox_FileList_ChangedItem(object sender, EventArgs e)
        //{
        //　ここで（_files.moveで）ファイル更新するとループするので注意 240911
        //}
    }
}
