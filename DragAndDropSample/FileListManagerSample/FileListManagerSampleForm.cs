using CommonUtility.FileListUtility.FileListControl;
using CommonUtility.FileListUtility;
using DragAndDropSample;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        public FileListManagerSampleForm()
        {
            InitializeComponent();
            _err = new ErrorManager.ErrorManager(1);
            _dragAndDropOnControl = new DragAndDropOnControl(_err, this);
            _dragAndDropOnControl.AddRecieveControls(new Control[] { richTextBox1,textBox1,listBox1 });
            _dragAndDropForFile = new DragAndDropForFile(_err, _dragAndDropOnControl);
            _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
            // Files Object
            _fileList = new List<string>();
            _files = new Files(_err, _fileList);
            _fileListManager = new FileListManager(_err, _files);
            _fileListControl = new FileListControlListBox(_err, this.listBox1, _files);
            _fileListManager.UpdateFileListAfterEvent += _fileListControl.UpdateFileListAfterEvent;

            _fileListControl.SelectedItemEvent += _files.SelectedFileEvent;
        }


        private void DragAndDropEventAfterEventForFile(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this, "DragAndDropEventAfterEventForFile");
                if (_dragAndDropForFile.Files == null) { _err.AddLogWarning("Files == null"); return; }
                if (_dragAndDropForFile.Files.Length < 1) { _err.AddLogWarning("Files.Length < 1"); return; }

                _err.AddLog("  GetPath=" + _dragAndDropForFile.Files[0]);
                this.textBox1.Text = _dragAndDropForFile.Files[0];
                AddLog("SetDirectoryPath=" + _dragAndDropForFile.Files[0]);
                textBox1.Text = _dragAndDropForFile.Files[0];
                _fileListManager.SetFilesFromPath(_dragAndDropForFile.Files[0]);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "DragAndDropEventAfterEventForFile");
                _err.ClearError();
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
            textBox1.Text = _dragAndDropForFile.Files[0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _fileListManager.MovePreviousDirectory();
            AddLog("SetDirectoryPath=" + _files.DirectoryPath);
            textBox1.Text = _dragAndDropForFile.Files[0];
        }
    }
}
