using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace FileSenderApp.Job
{
    public class JobManager
    {
        AppLogger _logger;
        List<JobItem> _jobItemList = new List<JobItem>();
        List<JobItem> _redoUndoList = new List<JobItem>();
        public int _listMax = 30;
        List<string> _keyList = new List<string>();
        public JobManager(AppLogger logger, List<string> keyList)
        {
            _logger = logger;
            _keyList = DuplicationDeleteForfKeyList(keyList);
        }

        /// <summary>
        /// 重複値を削除する
        /// </summary>
        /// <param name="argList"></param>
        /// <returns></returns>
        public List<string> DuplicationDeleteForfKeyList(List<string> argList)
        {
            List<string> workList = new List<string>();
            for (int i = 0; i < argList.Count; i++)
            {
                if (workList.Exists(n => n == argList[i]))
                {
                    continue;
                }
                workList.Add(argList[i]);
            }
            return workList;
        }

        public bool IsUndoEnable()
        {
            if (0 < _jobItemList.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsRedoEnable()
        {
            if (0 < _redoUndoList.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddJobItem(Dictionary<string, object> addDict)
        {
            try
            {
                Dictionary<string, object> newDict = new Dictionary<string, object>();
                object buf;
                foreach(string key in _keyList)
                {
                    buf = addDict[key];
                    newDict.Add(key, buf);
                }
                JobItem newJob = new JobItem(this._keyList);
                newJob._valueDict = newDict;
                this.AddJob(newJob);
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "AddJobItem");
            }
        }

        public void AddJob(JobItem jobItem)
        {
            if(_listMax < _jobItemList.Count)
            {
                _jobItemList.RemoveAt(0);
            }
            _jobItemList.Add(jobItem);
            if (0 < _redoUndoList.Count) {
                _redoUndoList.Clear();
            }
            _logger.PrintInfo(string.Format("AddJob, value={0}", jobItem.GetString()));
            _logger.PrintInfo(string.Format("AddJob, ListCount={0}", _jobItemList.Count));
        }

        public JobItem GetUndoItem()
        {
            //Undo
            //JobItem ret = null;
            //List<JobItem> workList = new List<JobItem>(_jobItemList.ToArray());
            //if (workList.Count<1)
            //{
            //    return null;
            //}
            //for (int i = workList.Count; 0<=i; i--)
            //{
            //    if (!(workList[i].IsUndo()))
            //    {
            //        ret = workList[i];
            //        break;
            //    }
            //    else
            //    {
            //        workList.RemoveAt(i);
            //    }
            //}
            //_jobItemList = workList;
            //_logger.PrintInfo(string.Format("PopLastItem, LitCount={0}", _jobItemList.Count));
            //#
            // JobItemListからRedoUndoListに移動
            if (_jobItemList.Count < 1)
            {
                return null;
            }
            int index = _jobItemList.Count - 1;
            JobItem bufJobItem = _jobItemList[index];
            bufJobItem.UndoON();
            _redoUndoList.Add(bufJobItem);
            _jobItemList.RemoveAt(index);
            return bufJobItem;
        }

        public JobItem GetRedoItem()
        {
            //JobItem ret = null;
            //#
            // RedoUndoListからJobItemListに移動
            if (_redoUndoList.Count < 1)
            {
                return null;
            }
            int index = _redoUndoList.Count - 1;
            JobItem bufJobItem = _redoUndoList[index];
            bufJobItem.UndoOFF();
            _jobItemList.Add(bufJobItem);
            _redoUndoList.RemoveAt(index);
            return bufJobItem;
        }

        public void ReverseItemJobListToUndo()
        {
            // Redoに失敗したときに実行する
            GetUndoItem();
            //if (_jobItemList.Count < 1)
            //{
            //    return;
            //}
            //int index = _jobItemList.Count - 1;
            //JobItem bufJobItem = _jobItemList[index];
            //bufJobItem.UndoOFF();
            //_redoUndoList.Add(bufJobItem);
            //_jobItemList.RemoveAt(index);
            //return;
        }
    }
}
