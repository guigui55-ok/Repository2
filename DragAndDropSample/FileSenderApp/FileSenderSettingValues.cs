using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace FileSenderApp
{
    public class FileSenderSettingValues
    {
        AppLogger _logger;
        public Dictionary<string, object> _settingDictBase = new Dictionary<string, object> { };
        Dictionary<string, object> _partsSettingDict = new Dictionary<string, object> { };
        Dictionary<string, object> _tabSettingDict = new Dictionary<string, object> { };
        public FileSenderSettingValues(AppLogger logger)
        {
            _logger = logger;
        }

        // 種類、所属のオブジェクト名、各種状態、子コントロールの値（Dict） をDictで保持する
        public void ReadSettingDict(Dictionary<string, object> settingDict)
        {
            _settingDictBase = settingDict;
        }


        /// <summary>
        /// keyBaseNameを含むDictの値を、リストで取得する（順不同）
        /// ,　値を取得するときに使用する。アプリ起動時設定ファイルからのDict読み込み時に使用する。
        /// 、TabPage*、SendButon*のみを取得する
        /// </summary>
        /// <param name="keyBaseName"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetListMatchValues(string keyBaseName)
        {
            List<Dictionary<string, object>> retList = new List<Dictionary<string, object>> { };
            foreach(string key in _settingDictBase.Keys)
            {
                if (key.IndexOf(keyBaseName) >= 0)
                {
                    //DictのValueだけでなく、Key:Valueそのままの形を格納する
                    Dictionary<string, object> buf = new Dictionary<string, object>(){
                    { key, _settingDictBase[key] }
                };
                    retList.Add(buf);
                }
            }
            return retList;
        }

        public List<Dictionary<string, object>> GetListMatchValues(Dictionary<string,object> valueDict,  string keyBaseName)
        {
            List<Dictionary<string, object>> retList = new List<Dictionary<string, object>> { };
            foreach (string key in valueDict.Keys)
            {
                if (key.IndexOf(keyBaseName) >= 0)
                {
                    //DictのValueだけでなく、Key:Valueそのままの形を格納する
                    Dictionary<string, object> buf = new Dictionary<string, object>(){
                    { key, valueDict[key] }
                };
                    retList.Add(buf);
                }
            }
            return retList;
        }
    }
}
