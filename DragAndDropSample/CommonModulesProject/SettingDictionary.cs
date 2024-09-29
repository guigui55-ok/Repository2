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

        public void AddValue(string key, object value)
        {
            //_settingDict.Add(key, value);
            //ThrowArgumentException ,同一のキーを含む項目が既に追加されています。
            _settingDict[key] = value;
        }

        public object GetValue(string key)
        {
            return _settingDict[key];
        }

        public string GetValueString(string key)
        {
            return (string)GetValue(key);
        }
        public int GetValueInt(string key)
        {
            return (int)GetValue(key);
        }
        public bool GetValueBool(string key)
        {
            return (bool)GetValue(key);
        }

        public Dictionary<string, object> GetSettingDict()
        {
            return _settingDict;
        }
    }
}
