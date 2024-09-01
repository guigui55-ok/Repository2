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
        protected ErrorManager.ErrorManager _err;        
        protected DragAndDropOnControl _dragAndDropOnControl;
        protected DragAndDropForFile _dragAndDropForFile;
        // Files Class
        protected IFiles _files;
        protected FileListManager _fileListManager;
        protected List<string> _fileList;
        protected IFileListControl _fileListControl;
        // 
        protected AppLogger _logger;
        public FileListManagerSampleForm()
        {
            InitializeComponent();
            this._logger = new AppLogger();
            this._logger.SetDefaultValues();
            _err = new ErrorManager.ErrorManager(1);
            _dragAndDropOnControl = new DragAndDropOnControl(this._logger, _err, this);
            _dragAndDropOnControl.AddRecieveControls(new Control[] { richTextBox1,textBox1,listBox1 });
            _dragAndDropForFile = new DragAndDropForFile(this._logger, _err, _dragAndDropOnControl);
            _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
            // Files Object
            _fileList = new List<string>();
            _files = new Files(this._logger, _err, _fileList);
            _fileListManager = new FileListManager(this._logger, _err, _files);
            _fileListControl = new FileListControlListBox(this._logger, _err, this.listBox1, _files);
            _fileListManager.UpdateFileListAfterEvent += _fileListControl.UpdateFileListAfterEvent;

            _fileListControl.SelectedItemEvent += _files.SelectedFileEvent;
        }


        private void DragAndDropEventAfterEventForFile(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "DragAndDropEventAfterEventForFile");
                if (_dragAndDropForFile.Files == null) { _logger.AddLogWarning("Files == null"); return; }
                if (_dragAndDropForFile.Files.Length < 1) { _logger.AddLogWarning("Files.Length < 1"); return; }

                _logger.AddLog("  GetPath=" + _dragAndDropForFile.Files[0]);
                this.textBox1.Text = _dragAndDropForFile.Files[0];
                AddLog("SetDirectoryPath=" + _dragAndDropForFile.Files[0]);
                textBox1.Text = _dragAndDropForFile.Files[0];
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
            richTextBox1.AppendText(value + sepaleteValue);
            richTextBox1.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _fileListManager.MoveNextDirectory();
            AddLog("SetDirectoryPath=" + _files.DirectoryPath);
            if (_dragAndDropForFile.Files == null)
            {
                AddLog("_dragAndDropForFile.Files == null");
                return;
            }
            textBox1.Text = _dragAndDropForFile.Files[0];
            if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(0); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _fileListManager.MovePreviousDirectory();
            AddLog("SetDirectoryPath=" + _files.DirectoryPath);
            if (_dragAndDropForFile.Files == null)
            {
                AddLog("_dragAndDropForFile.Files == null");
                return;
            }
            textBox1.Text = _dragAndDropForFile.Files[0];
            if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(0); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _fileListManager.MoveProviousFileWhenFirstFilePreviousDirectory();
            //AddLog("SetDirectoryPath=" + _files.DirectoryPath);
            if(_dragAndDropForFile.Files == null) { _logger.AddLog("_dragAndDropForFile.Files == null"); return; }
            if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(_files.GetCurrentValue()); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _fileListManager.MoveNextFileWhenLastFileNextDirectory();
            //AddLog("SetDirectoryPath=" + _files.DirectoryPath);
            if (_dragAndDropForFile.Files == null) { _logger.AddLog("_dragAndDropForFile.Files == null"); return; }
            if (_dragAndDropForFile.Files.Length > 0) { _fileListControl.SelectItem(_files.GetCurrentValue()); }
        }
    }
}
