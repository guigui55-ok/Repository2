using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace SelectFileSample.SelectFile
{
    public class CreateRandomList
    {
        public AppLogger _logger;
        public CreateRandomList(AppLogger logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// リストをランダムにする
        /// </summary>
        /// <returns></returns>
        public List<string> ListOrtderToRandom(List<string> valueList)
        {
            try
            {
                List<int> indexList = CreateIndexList(valueList.Count);
                indexList = new CommonUtility.MyRandom(this._logger).ListToRandom(indexList);
                return GetRandomListFromIndexList(valueList,indexList);
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "ListOrtderToRandom");
                return new List<string>();
            }
        }
        private List<int> CreateIndexList(int max)
        {
            List<int> list = new List<int>();
            for(int i=1; i<=max; i++)
            {
                list.Add(i);
            }
            return list;
        }

        public List<string> GetRandomListFromIndexList(List<string> valueList,List<int> indexList)
        {
            List<string> retList = new List<string>();
            try
            {
                if(indexList == null) { return retList; }
                if(indexList.Count < 1) { return retList; }
                if(valueList == null) { return retList; }
                if(valueList.Count < 1) { return retList; }
                if(valueList.Count != indexList.Count) { return retList; }

                foreach(int n in indexList)
                {
                    retList.Add(valueList[n]);
                }
                return retList;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetRandomListFromIndexList");
                return retList;
            }
        }

        /// <summary>
        /// リストの順番をリセット(通常に)する
        /// </summary>
        /// <returns></returns>
        //private int ResetListOrder()
        //{
        //    try
        //    {
        //        for (int i = 0; i < _fileList.Count; i++)
        //        {
        //            _indexList.Add(i);
        //        }
        //        if (_indexList.Count != _fileList.Count)
        //        {
        //            _logger.AddLogAlert(this, "List Count Is Difference");
        //        }
        //        IsRandom = false;
        //        return 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.AddException(ex, this, "makeIndexList");
        //        return 0;
        //    }
        //    finally
        //    {
        //        ChangedFileList?.Invoke(null, EventArgs.Empty);
        //    }
        //}
    }
}
