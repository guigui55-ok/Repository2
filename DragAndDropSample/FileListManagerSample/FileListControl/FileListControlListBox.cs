using CommonUtility.FileListUtility;
using System;
using System.Windows.Forms;

namespace CommonUtility.FileListUtility.FileListControl
{
    public class FileListControlListBox : IFileListControl
    {
        protected ErrorManager.ErrorManager _err;
        protected IFiles _files;
        protected ListBox _listBox;
        protected EventHandler _selectedItemEvent;

        public EventHandler SelectedItemEvent { get => _selectedItemEvent; set => _selectedItemEvent = value; }
        IFiles IFileListControl.Files { get => _files; set => _files = value; }

        public FileListControlListBox(ErrorManager.ErrorManager err,ListBox listBox, IFiles files)
        {
            _err = err;
            _listBox = listBox;
            _files = files;
            _listBox.Click += ListBox_Click;
        }

        private void ListBox_Click(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this,"_listBox_Click");
                SelectedItemEvent?.Invoke(_listBox.SelectedIndex, EventArgs.Empty);
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "_listBox_Click");
                _err.ClearError();
            }
        }

        public int SetFilesToControl(IFiles files)
        {
            try
            {
                ClearList();
                if (files.FileList == null) { _err.AddLogWarning("files.FileList == null"); return -1; }
                _listBox.Items.AddRange(files.FileList.ToArray());
                return 1;

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "SetFilesToControl");
                return 0;
            }
        }

        public void UpdateFileListAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this, "UpdateFileListAfterEvent");
                int ret = SetFilesToControl(_files);
                if (_err.hasError) { _err.AddLog(" SetFilesToControl Failed"); _err.ClearError(); }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "UpdateFileListAfterEvent");
                _err.ClearError();
            }
        }

        //public void SelectItem()
        //{
        //    try
        //    {

        //    } catch (Exception ex)
        //    {

        //    }
        //}

        public void ClearList()
        {
            try
            {
                if(_listBox.Items.Count < 1) { return; }
                //foreach(string item in _listBox.Items)
                //{
                //    _listBox.Items.RemoveAt(0);
                //}
                //この列挙子がバインドされている一覧は変更されています。列挙子は、一覧が変更しない場合に限り使用できます。
                int max = _listBox.Items.Count;
                for(int i=0;i<max; i++)
                {
                    _listBox.Items.RemoveAt(0);
                    if(_listBox.Items.Count < 1) { break; } else { i = 0; }
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ClearList");
            }
        }

    }
}
