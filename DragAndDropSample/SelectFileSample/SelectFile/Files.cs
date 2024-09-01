using System;
using System.Collections.Generic;
using AppLoggerModule;

namespace ControlUtility.SelectFiles
{
    // FileListMain
    public class Files : IFiles
    {
        public AppLogger _logger;
        protected List<string> _fileList;
        public int NowIndex = 0;
        EventHandler _changeFiles;
        public EventHandler ChangedFileList { get => _changeFiles; set => _changeFiles = value; }

        public Files(AppLogger logger, List<string> list)
        {
            _logger = logger;
            Initialize();
            this.FileList = list;
        }

        private void Initialize()
        {
            try
            {
            }
            catch (Exception ex) { _logger.AddException(ex, this, "initialize Failed"); return; }
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
