using SelectFileSample.SelectFile;
using System;
using System.Collections.Generic;

namespace ControlUtility.SelectFiles
{
    // FileListMain
    public class SingleFile　: IFiles
    {
        protected ErrorManager.ErrorManager _err;
        protected string _filePath = "";
        public int CurrentIndex = 0;
        EventHandler _changeFiles;
        public EventHandler ChangedFileList { get => _changeFiles; set => _changeFiles = value; }

        public SingleFile(ErrorManager.ErrorManager err, List<string> list)
        {
            _err = err;
            Initialize();
            this.FileList = list;
        }

        private void Initialize()
        {
            try
            {
                
            }
            catch (Exception ex) { _err.AddException(ex, this, "initialize Failed"); return; }
        }

        /// <summary>
        /// List の Index をひとつ次へ移動する、最大値を超えたとき 0 に戻る
        /// </summary>
        public void MoveNext()
        {
        }
        /// <summary>
        /// List の Index をひとつ前へ移動する。最小値を下回ったときは最後に移動する。
        /// </summary>
        public void MovePrevious()
        {
        }
        /// <summary>
        /// CurrentIndex の値を取得する。
        /// </summary>
        /// <returns></returns>
        public string GetCurrentValue()
        {
            try
            {
                return _filePath;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "GetCurrentValue Failed");
                return "";
            }
        }
        /// <summary>
        /// ファイルリストを取得する
        /// </summary>
        public List<string> FileList
        {
            get { return new List<string> { _filePath }; }
            set
            {
                try
                {
                    _err.AddLog(this, "FileList:PropertySet");
                    List<string> _fileList;
                    _fileList = value;
                    if ((_fileList != null) && (_fileList.Count > 0))
                    {
                        //int ret = ResetListOrder();
                        //if (ret < 1) { _err.AddLogAlert(this, "FileList Property:resetListOrder"); return; }
                        _filePath = _fileList[0];
                        if(_fileList.Count >= 2)
                        {
                            _err.AddLogWarning("  SetFileList.Count >= 2");
                        }
                    }
                    else
                    {
                        _err.AddLogAlert("((_fileList != null) && (_fileList.Count > 0))==false");
                    }
                }
                catch (Exception ex)
                {
                    _err.AddException(ex, this, "FileList Property");
                }
            }
        }
        /// <summary>
        /// ファイルリストを取得する。IsRandom=true 時はランダム順序のリストを取得する。
        /// </summary>
        /// <returns></returns>
        public List<string> GetList()
        {
            try
            {
                //if ((_fileList == null) || (_randomList == null)) { return _fileList; }

                //if (IsRandom)
                //{
                //    List<string> retList = new List<string>();
                //    for (int i = 0; i < _fileList.Count; i++)
                //    {
                //        int now = _randomList[i];
                //        retList.Add(_fileList[now]);
                //    }
                //    return retList;
                //}
                //else
                //{
                //    return _fileList;
                //}
                return new List<string> { _filePath };
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "GetList");
                return new List<string>();
            }
        }
    }
}
