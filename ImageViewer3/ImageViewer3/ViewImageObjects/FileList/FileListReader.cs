using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using ErrorLog;

namespace FileList
{
    public class FileListReader
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected List<string> _fileList;
        protected List<int> _randomList;
        protected List<int> _indexList;
        public int NowIndex = 0;
        protected bool IsRandom = false;
        public bool IsLoopList = true;
        public FileListReader(IErrorLog errorLog, List<string> list)
        {
            _errorLog = errorLog;
            //_fileList = list;
            Initialize();
            this.FileList = list;
        }

        private void Initialize()
        {
            try
            {
                _randomList = new List<int>();
                _indexList = new List<int>();
            }
            catch (Exception ex) { _errorLog.AddException(ex, this.ToString(), "initialize Failed"); return; }
        }

        // MoveNext
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
        // MovePrevious
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
        // GetNowValue
        public string GetNowValue()
        {
            try
            {
                if (_fileList == null) { return""; }
                if (!IsRandom)
                {
                    return _fileList[NowIndex];
                } else
                {
                    return _fileList[_randomList[NowIndex]];
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "GetNowValue Failed");
                return "";
            }
        }



        public List<string> FileList
        {
            get { return _fileList; }
            set
            {
                try
                {
                    _indexList = new List<int>();
                    _fileList = value;
                    if ((_fileList != null) && (_fileList.Count > 0)) {
                        int ret = ResetListOrder();
                        if (ret < 1) { _errorLog.AddErrorNotException(this.ToString(), "FileList Property:resetListOrder"); return; }
                    }
                    NowIndex = 0;

                } catch (Exception ex)
                {
                    _errorLog.AddException(ex, this.ToString(), "FileList Property");
                }
            }
        }

        public List<string> GetList()
        {
            try
            {
                if ((_fileList == null) || (_randomList == null)) { return _fileList; }

                if (IsRandom)
                {
                    List<string> retList = new List<string>();
                    for (int i = 0; i < _fileList.Count; i++)
                    {
                        int now = _randomList[i];
                        retList.Add(_fileList[now]);
                    }
                    return retList;
                } else
                {
                    return _fileList;
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "getListRandom");
                return _fileList;
            }
        }

        //private List<string> MakeList()
        //{
        //    try
        //    {
        //        return _fileList;

        //    } catch (Exception ex)
        //    {
        //        _errorLog.AddException(ex, this.ToString(), "makeList");
        //        return _fileList;
        //    }
        //}

        public int SwitchOrderToRandomOrCorrect()
        {
            try
            {
                int ret=0;
                if (IsRandom)
                {
                    IsRandom = false;
                    return 1;
                }
                else
                {
                    ret = ListOrtderToRandom();
                }
                return ret;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "switchOrderToRandomOrCorrect");
                return 0;
            }
        }

        public int ListOrtderToRandom()
        {
            try
            {
                IsRandom = true;
                _randomList = new MyRandom().ListToRandom(_indexList);
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ListOrtderToRandom");
                return 0;
            }
        }
        private int ResetListOrder()
        {
            try
            {
                for(int i = 0; i < _fileList.Count; i++)
                {
                    _indexList.Add(i);
                }
                if (_indexList.Count != _fileList.Count)
                {
                    _errorLog.AddErrorNotException("List Count Is Difference");
                }
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "makeIndexList");
                return 0;
            }
        }

    }
}
