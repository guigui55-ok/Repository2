using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSenderApp.Job
{
    public class JobItem
    {
        // とりあえずDictで扱う
        public Dictionary<string, object> _valueDict;
        public List<string> _keyList;
        public string _undoKeyName = "Undo";
        public JobItem(List<string> keyList)
        {
            _keyList = keyList;

            // JobManagerで処理する
            //List<string> workList = new List<string>();
            //for(int i=0; i< keyList.Count; i++)
            //{
            //    if (workList.Exists(n => n == keyList[i]))
            //    {
            //        continue;
            //    }
            //    workList.Add(keyList[i]);
            //}
            //_keyList = workList;
        }

        public void UndoON()
        {
            _valueDict[_undoKeyName] = true;
        }
        public void UndoOFF()
        {
            _valueDict[_undoKeyName] = false;
        }

        public bool IsUndo()
        {
            return GetItemBool(ConstJobFileSender.KEY_UNDO);
        }

        public string GetString()
        {
            string buf = "";
            foreach(string key in _keyList)
            {
                buf += string.Format("{0}:{1}", key, GetItemString(key));
                if (key != _keyList[_keyList.Count - 1]){ buf += ", "; }
            }
            if (buf != "")
            {
                buf = buf.Substring(0, buf.Length - 2);
            }
            return buf;
        }

        private string GetItemString(string key)
        {
            try
            {
                return (string)GetItem(key);
            }
            catch (Exception)
            {
                return "";
            }
        }
        private bool GetItemBool(string key)
        {
            try
            {
                return (bool)GetItem(key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private object GetItem(string key)
        {
            try
            {
                return _valueDict[key];
            } catch(Exception)
            {
                return null;
            }
        }
    }
}
