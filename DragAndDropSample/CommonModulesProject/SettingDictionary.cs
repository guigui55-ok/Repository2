using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModulesProject
{
    public class SettingDictionary
    {
        public string _name;
        public Dictionary<string, object> _settingDict;
        public SettingDictionary(string name= "SettingDict")
        {
            _name = name;
            _settingDict = new Dictionary<string, object>();
        }

        public void DisposeObjects()
        {
            try
            {
                _settingDict.Clear();
                _settingDict = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void AddValue(string key, object value)
        {
            //_settingDict.Add(key, value);
            //ThrowArgumentException ,同一のキーを含む項目が既に追加されています。
            _settingDict[key] = value;
        }


        /// <summary>
        /// keyIncludeを含むKeyValueをリストで取得する
        /// </summary>
        /// <param name="keyInclude"></param>
        /// <returns></returns>
        public List<object>GetValueList(string keyInclude)
        {
            List<object> retList = new List<object>();
            foreach(string key in this._settingDict.Keys)
            {
                if (key.IndexOf(keyInclude) >= 0)
                {
                    retList.Add(_settingDict[key]);
                }
            }
            return retList;
        }


        /// <summary>
        /// keyIncludeを含むKeyValueをリストで取得する （一致したときの攻勢そのままで返す {key: matchValue}）
        /// </summary>
        /// <param name="keyInclude"></param>
        /// <returns></returns>
        public List<object> GetValueListWithKeyName(string keyInclude)
        {
            List<object> retList = new List<object>();
            foreach (string key in this._settingDict.Keys)
            {
                if (key.IndexOf(keyInclude) >= 0)
                {
                    //retList.Add(_settingDict[key]);
                    Dictionary<string, object> buf = new Dictionary<string, object>();
                    buf.Add(key, _settingDict[key]);
                    retList.Add(buf);
                }
            }
            return retList;
        }

        public object GetValue(string key, object defaultValue = null)
        {
            try
            {
                return _settingDict[key];
            }catch(System.Collections.Generic.KeyNotFoundException ex)
            {
                string buf = ex.Message;
                return  null;
            }
            catch(Exception)
            {
                return null;
            }            
        }

        public string GetValueString(string key, string defaultValue = "")
        {
            object ret = GetValue(key);
            if (ret == null)
            {
                return defaultValue;
            }
            return (string)ret;
        }
        public int GetValueInt(string key, int defaultValue = 0)
        {
            object ret = GetValue(key);
            if (ret == null)
            {
                return defaultValue;
            }
            return (int)ret;
        }
        public bool GetValueBool(string key, bool defaultValue = false)
        {
            object ret = GetValue(key);
            if (ret == null)
            {
                return defaultValue;
            }
            return (bool)ret;
        }

        public Dictionary<string, object> GetSettingDict()
        {
            return _settingDict;
        }
    }
}
