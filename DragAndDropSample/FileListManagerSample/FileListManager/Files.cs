using System;
using System.Collections.Generic;
using AppLoggerModule;
using System.IO;

namespace CommonUtility.FileListUtility
{
    // FileListMain
    public class Files : IFiles
    {
        public AppLogger _logger;
        protected List<string> _fileList;
        //public List<string> _supportedImageExtensionList;'外部で絞込処理をする為コメントアウトするが、クラス内部で必要になるかもしれない 240901
        protected string _directoryPath = "";
        public int NowIndex = 0;
        EventHandler _changeFilesEvent;
        EventHandler _selectedFileEvent;
        //外部からファイル変更されたときのフラグ（このクラス内で変更されたときと処理が競合するのを防止するためのフラグ）
        public bool _isRecievedChangeFileFromOut = false;
        //このクラスからファイルが変更されたときのフラグ
        public bool _isChangeFileInThis = false;
        // リストの
        public bool _isDoLoopWhenOverListMaxMin = true;

        public EventHandler ChangedFileListEvent { get => _changeFilesEvent; set => _changeFilesEvent = value; }

        //検討中
        EventHandler IFiles.SelectedFileEvent { get => _selectedFileEvent; set => _selectedFileEvent= value; }

        //List<string> IFiles.SupportedImageExtensionList { get => _supportedImageExtensionList; set => _supportedImageExtensionList = value; }

        //List<string> SupportedImageExtensionList { get=> _supportedImageExtensionList; set => _supportedImageExtensionList=value; }
        public string DirectoryPath { get => _directoryPath; set => _directoryPath = value; }
        public AppLogger Logger { get => this._logger; set => this._logger = value; }
        public Files(AppLogger logger, List<string> list)
        {
            this._logger = logger;
            this.FileList = list;
        }

        /// <summary>
        /// 外部でfilesが変更されたときに、このクラスのindexも変更する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeFileRecieve(object sender, EventArgs e)
        {
            _isRecievedChangeFileFromOut = true;
            try
            {
                // senderはfiles.listのindexが送られる
                if (sender.GetType().Equals(typeof(int)))
                {
                    this.Move((int)sender);
                }
                else
                {
                    _logger.AddLogWarning("sender type invalid");
                }
            }
            catch (Exception ex)
            {
                _logger.AddLogAlert(this, "SelectedFile", ex);
            }
            finally
            {
                _isRecievedChangeFileFromOut = false;
            }
        }
        public void SelectedFileEventInThis(object sender, EventArgs e)
        {
            _isRecievedChangeFileFromOut = true;
            try
            {
                if (sender.GetType().Equals(typeof(int)))
                {
                    this.Move((int)sender);
                }
                else
                {
                    _logger.AddLogWarning("sender type invalid");
                }
            }
            catch (Exception ex)
            {
                _logger.AddLogAlert(this, "SelectedFile", ex);
            }
            finally
            {
                _isRecievedChangeFileFromOut = false;
            }
        }

        public int Count()
        {
            if (_fileList == null) { return 0; }
            return _fileList.Count;
        }

        public bool IsLastIndex()
        {
            if(_fileList == null) { return true; }
            if(_fileList.Count < 1) { return true; }
            if(NowIndex >= (_fileList.Count - 1)) { return true; }
            else { return false; }
        }

        public bool IsFirstIndex()
        {
            if (_fileList == null) { return true; }
            if (_fileList.Count < 1) { return true; }
            if (NowIndex <= 0) { return true; }
            else { return false; }
        }

        public void ResetList(bool withEvent = true)
        {
            try
            {
                _logger.AddLog(this, "ResetList");
                _fileList = new List<string>();
                DirectoryPath = "";
                NowIndex = 0;
                if (withEvent)
                {
                    _changeFilesEvent?.Invoke(null, EventArgs.Empty);
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex,this, "ResetList");
            }
        }

        public void Move(string value)
        {
            try
            {
                if (_fileList == null) { _logger.AddLog(this, "Move(string value) _fileList==null"); return; }
                if ((value == null) || (value == "")) { return; }
                //for(int i=0; i<_fileList.Count; i++)
                //{
                //    if (_fileList[i].Equals(value))
                //    {
                //        Move(i);
                //        return;
                //    }
                //}
                //_logger.AddLog(this, "Move(string value) value is Nothing");
                // 240830
                // 処理を変更
                int i = _fileList.IndexOf(value);
                if (i < 0)
                {
                    string msg;
                    msg = String.Format("Move(string Value) Value Is Nothing [{0}]", value);
                    _logger.AddLog(this, msg);
                    return;
                }
                Move(i);
                return;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "Move(string value)");
            }
        }

        private void RaiseSelectedFileEvent()
        {
            //if (_isRecievedChangeFileFromOut) { return; } //複数のイベントが紐づけられている可能性があり、この場合、一部のイベントが除外されるため、利用する側でイベントループを抑制する。
            _isChangeFileInThis = true;
            _selectedFileEvent?.Invoke(this.GetCurrentValue(), EventArgs.Empty);
            _isChangeFileInThis = false;
        }

        public void Move(int index)
        {
            if (_fileList == null) { return; }
            if ((index <= _fileList.Count - 1)&&(index >= 0))
            {
                NowIndex = index;
                _logger.AddLog(this, "Move(int index) index=" + NowIndex);
                RaiseSelectedFileEvent();
            }
            else
            {
                _logger.AddLogWarning(this, "Move(int index) index is invalid");
            }
        }

        /// <summary>
        /// List の Index をひとつ次へ移動する、最大値を超えたとき 0 に戻る
        /// </summary>
        public void MoveNext()
        {
            if (_fileList == null) { return; }
            if (NowIndex >= _fileList.Count - 1)
            {
                if (_isDoLoopWhenOverListMaxMin)
                {
                    NowIndex = 0;
                }
                else
                {
                    _logger.AddLog(this, "MoveNext CountMax (Loop=false) =" + NowIndex);
                    return;
                }
            } else
            {
                NowIndex++;
                _logger.AddLog(this, "MoveNext index=" + NowIndex);
            }
            RaiseSelectedFileEvent();
        }
        /// <summary>
        /// List の Index をひとつ前へ移動する。最小値を下回ったときは最後に移動する。
        /// </summary>
        public void MovePrevious()
        {
            if (_fileList == null) { return; }
            if (NowIndex <= 0)
            {
                if (_isDoLoopWhenOverListMaxMin)
                {
                    NowIndex = _fileList.Count - 1;
                }
                else
                {
                    _logger.AddLog(this, "MoveNext CountMin (Loop=false) =" + NowIndex);
                    return;
                }
            } else
            {
                NowIndex--;
                _logger.AddLog(this, "MovePrevious index=" + NowIndex);
            }
            RaiseSelectedFileEvent();
        }
        /// <summary>
        /// CurrentIndex の値を取得する。
        /// </summary>
        /// <returns></returns>
        public string GetCurrentValue()
        {
            try
            {
                if (_fileList == null) { return""; }
                if (_fileList.Count < 1) { _logger.AddLog(this,"GetCurrentValue : FileList.Count<1"); return ""; }
                return _fileList[NowIndex];
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetCurrentValue Failed");
                return "";
            }
        }
        /// <summary>
        /// ファイルリストを取得する
        /// </summary>
        public List<string> FileList
        {
            get { return _fileList; }
            set
            {
                try
                {
                    _logger.AddLog(this, "FileList:PropertySet");
                    _fileList = value;
                    if ((_fileList != null) && (_fileList.Count > 0)) {
                        //int ret = ResetListOrder();
                        //if (ret < 1) { _logger.AddLogAlert(this, "FileList Property:resetListOrder"); return; }
                        //NowIndex = 0;
                    }
                    else
                    {

                    }
                    //240830
                    // 現状では、ファイルが空の時は SelectedItem が実行されないが
                    // 実行されるようにするかは検討中
                    Move(0);
                    _changeFilesEvent?.Invoke(this.DirectoryPath, EventArgs.Empty);

                } catch (Exception ex)
                {
                    _logger.AddException(ex, this, "FileList Set Property");
                }
            }
        }

        /// <summary>
        /// ファイルリストを取得する。
        /// </summary>
        /// <returns></returns>
        public List<string> GetList()
        {
            try
            {
                if (_fileList == null)  { return _fileList; }
                return _fileList;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetList");
                return _fileList;
            }
        }

        public void UpdateFileList(List<string> list)
        {
            this.FileList = list;
        }

        public string StringJoinList(int index=-1)
        {
            //#
            List<string> bufList = new List<string> { };
            if (0 < index)
            {
                //長いとき用
                bufList = _fileList.GetRange(0, index);
            }
            foreach (string buf in _fileList)
            {
                bufList.Add(Path.GetFileName(buf));
            }
            //#
            string bufJoin = String.Join(",", bufList);
            //_logger.PrintInfo("fileNameList = " + bufJoin);
            return bufJoin;
        }

        public int GetCurrentIndex()
        {
            return NowIndex;
        }
    }
}
