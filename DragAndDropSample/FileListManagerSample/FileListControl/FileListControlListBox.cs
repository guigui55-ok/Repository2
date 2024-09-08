using CommonUtility.FileListUtility;
using System;
using System.Windows.Forms;
using AppLoggerModule;

namespace CommonUtility.FileListUtility.FileListControl
{
    public class FileListControlListBox : IFileListControl
    {
        public AppLogger _logger;
        protected IFiles _files;
        protected ListBox _listBox;
        protected EventHandler _selectedItemEvent;
        // リスト内のItemがクリックされて、変更されたときのフラグ
        // filesの選択が変更されたときと、イベントがループしないように管理するためのフラグ
        public bool _isClickedList = false;
        // 外部でfilesの選択が変更されたときのフラグ
        public bool _isRecievedChangeFile = false;

        public EventHandler SelectedItemEvent { get => _selectedItemEvent; set => _selectedItemEvent = value; }
        IFiles IFileListControl.Files { get => _files; set => _files = value; }
        AppLogger IFileListControl.Logger { get => this._logger; set => this._logger = value; }

        public FileListControlListBox(AppLogger logger, ListBox listBox, IFiles files)
        {
            this._logger = logger;
            _listBox = listBox;
            _files = files;
            _listBox.Click += ListBox_Click;
        }

        /// <summary>
        /// _filesが直接変更されたとき、コントロールに反映させる
        /// _files.SelectedItemから呼び出される（RecieveEvent用）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangedFileInFile(object sender, EventArgs e)
        {
            if (_isClickedList) { return; }
            _isRecievedChangeFile = true;
            try
            {
                // senderはファイルパス
                _logger.AddLog(this, "ChangedFileInFile");
                string path = (string)sender;
                SelectItem(path);
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "_listBox_Click");
                _logger.ClearError();
            }
            finally
            {
                _isRecievedChangeFile = false;
            }
        }

        private void ListBox_Click(object sender, EventArgs e)
        {
            _isClickedList = true;
            try
            {
                _logger.AddLog(this,"_listBox_Click");
                SelectedItemEvent?.Invoke(_listBox.SelectedIndex, EventArgs.Empty);
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "_listBox_Click");
                _logger.ClearError();
            }
            finally
            {
                _isClickedList = false;
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
                _logger.PrintInfo("ListJoin = " + _files.StringJoinList());
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
                _logger.AddLogTrace(this, "SelectItem");
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
                            _listBox.SetSelected(i,true);
                            return;
                        }
                    }
                    if (value.GetType().Equals(typeof(int)))
                    {
                        if (i == (int)value)
                        {
                            _listBox.SetSelected(i,true);
                            return;
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
