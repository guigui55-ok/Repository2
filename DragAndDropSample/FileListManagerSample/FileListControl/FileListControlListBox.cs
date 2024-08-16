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
                SelectItem(0);
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
                _err.AddLogAlert(this, "MatchValueInListBox Faile", "MatchValueInListBox Failed", ex);
                return false;
            }
        }

        public void SelectItem(object value)
        {
            try
            {
                _err.AddLog(this, "SelectItem");
                if (_listBox == null) { _err.AddLog(" listBox1 == null , return"); return; }
                if (_listBox.Items.Count < 1) { _err.AddLog(" listBox1.Items.Count < 1 , retrun"); return; }
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
                _err.AddLog("Item is Nothing");
            }
            catch (Exception ex)
            {
                _err.AddLogAlert(this, "SelectItem Faile", "SelectItem Failed", ex);
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
                _err.AddException(ex, this, "ClearList");
            }
        }

    }
}
