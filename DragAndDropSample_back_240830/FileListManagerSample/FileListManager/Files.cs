using System;
using System.Collections.Generic;
using AppLoggerModule;

namespace CommonUtility.FileListUtility
{
    // FileListMain
    public class Files : IFiles
    {
        public AppLogger _logger;
        protected ErrorManager.ErrorManager _err;
        protected List<string> _fileList;
        protected string _directoryPath = "";
        public int NowIndex = 0;
        EventHandler _changeFilesEvent;
        public EventHandler ChangedFileListEvent { get => _changeFilesEvent; set => _changeFilesEvent = value; }
        public string DirectoryPath { get => _directoryPath; set => _directoryPath = value; }
        public AppLogger Logger { get => this._logger; set => this._logger = value; }
        public Files(AppLogger logger, ErrorManager.ErrorManager err, List<string> list)
        {
            this._logger = logger;
            _err = err;
            this.FileList = list;
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
        public void SelectedFileEvent(object sender,EventArgs e)
        {
            try
            {
                if (sender.GetType().Equals(typeof(int)))
                {
                    this.Move((int)sender);
                } else
                {
                    _logger.AddLogWarning("sender type invalid");
                }
            } catch (Exception ex)
            {
                _logger.AddLogAlert(this, "SelectedFile" ,ex);
            }
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
                for(int i=0; i<_fileList.Count; i++)
                {
                    if (_fileList[i].Equals(value))
                    {
                        Move(i);
                        return;
                    }
                }
                _logger.AddLog(this, "Move(string value) value is Nothing");
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "Move(string value)");
            }
        }

        public void Move(int index)
        {
            if (_fileList == null) { return; }
            if ((index <= _fileList.Count - 1)&&(index >= 0))
            {
                NowIndex = index;
                _logger.AddLog(this, "Move(int index) index=" + NowIndex);
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
                NowIndex = 0;
            } else
            {
                NowIndex++;
                _logger.AddLog(this, "MoveNext index=" + NowIndex);
            }
        }
        /// <summary>
        /// List の Index をひとつ前へ移動する。最小値を下回ったときは最後に移動する。
        /// </summary>
        public void MovePrevious()
        {
            if (_fileList == null) { return; }
            if (NowIndex <= 0)
            {
                NowIndex = _fileList.Count -1;
            } else
            {
                NowIndex--;
                _logger.AddLog(this, "MovePrevious index=" + NowIndex);
            }
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
                    }
                    NowIndex = 0;

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
        
    }
}
