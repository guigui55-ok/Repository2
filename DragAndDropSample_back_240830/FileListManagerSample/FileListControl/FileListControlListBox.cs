using CommonUtility.FileListUtility;
using System;
using System.Windows.Forms;
using AppLoggerModule;

namespace CommonUtility.FileListUtility.FileListControl
{
    public class FileListControlListBox : IFileListControl
    {
        public AppLogger _logger;
        protected ErrorManager.ErrorManager _err;
        protected IFiles _files;
        protected ListBox _listBox;
        protected EventHandler _selectedItemEvent;

        public EventHandler SelectedItemEvent { get => _selectedItemEvent; set => _selectedItemEvent = value; }
        IFiles IFileListControl.Files { get => _files; set => _files = value; }
        AppLogger IFileListControl.Logger { get => this._logger; set => this._logger = value; }

        public FileListControlListBox(AppLogger logger, ErrorManager.ErrorManager err,ListBox listBox, IFiles files)
        {
            this._logger = logger;
            _err = err;
            _listBox = listBox;
            _files = files;
            _listBox.Click += ListBox_Click;
        }

        private void ListBox_Click(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this,"_listBox_Click");
                SelectedItemEvent?.Invoke(_listBox.SelectedIndex, EventArgs.Empty);
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "_listBox_Click");
                _logger.ClearError();
            }
        }

        public int SetFilesToControl(IFiles files)
        {
            try
            {
                ClearList();
                if (files.FileList == null) { _logger.AddLogWarning("files.FileList == null"); return -1; }
                _listBox.Items.AddRange(files.FileList.ToArray());
                return 1;

            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "SetFilesToControl");
                return 0;
            }
        }

        public void UpdateFileListAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "UpdateFileListAfterEvent");
                int ret = SetFilesToControl(_files);
                if (_logger.hasError()) { _logger.AddLog(" SetFilesToControl Failed"); _logger.ClearError(); }
                SelectItem(0);
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "UpdateFileListAfterEvent");
                _logger.ClearError();
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

        private bool MatchValueInListBox(object listboxItem, object value)
        {
            try
            {
                if (value.GetType().Equals(typeof(string)))
                {

                    if (((string)value).Equals(_listBox.Items))
                    {
                        return true;
                    }
                    return false;
                }
                else if (value.GetType().Equals(typeof(string)))
                {

                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.AddLogAlert(this, "MatchValueInListBox Failed", ex);
                return false;
            }
        }

        public void SelectItem(object value)
        {
            try
            {
                _logger.AddLog(this, "SelectItem");
                if (_listBox == null) { _logger.AddLog(" listBox1 == null , return"); return; }
                if (_listBox.Items.Count < 1) { _logger.AddLog(" listBox1.Items.Count < 1 , retrun"); return; }
                for (int i = 0; i < _listBox.Items.Count; i++)
                {
                    string item = (string)_listBox.Items[i];
                    //if (i == 0) { Console.WriteLine("listBox1.Items[i].GetType() = " + listBox1.Items[i].GetType()); }
                    if (value.GetType().Equals(typeof(string)))
                    {
                        if (item.Equals(value))
                        {
                            _listBox.SetSelected(i,true);return;
                        }
                    }
                    if (value.GetType().Equals(typeof(int)))
                    {
                        if (i == (int)value)
                        {
                            _listBox.SetSelected(i,true);return;
                        }
                    }
                }
                _logger.AddLog("Item is Nothing");
            }
            catch (Exception ex)
            {
                _logger.AddLogAlert(this, "SelectItem Faile > " + "SelectItem Failed", ex);
            }
        }
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
                _logger.AddException(ex, this, "ClearList");
            }
        }

    }
}
