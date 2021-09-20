using System;
using System.Collections.Generic;

namespace IniManager
{
    class IniSection
    {
        protected ErrorManager.ErrorManager _error;
        string _sectionName;
        protected List<string> _parameterList;
        public IniSection(ErrorManager.ErrorManager error)
        {
            _error = error;
            _parameterList = new List<string>();
        }
        
        public string GetSectionName() { return _sectionName; }

        public void SetSectionName(string value)　{　_sectionName = value;　}

        /// <summary>
        /// この sectionName の ParameterList へ行を追加
        /// </summary>
        public void AddParameter(string value)
        {
            //if (value == "") { return; }
            //if (value == null) { return; }
            if ((value == "")||(value == null))
            {
                _error.AddLog(this.ToString()+ ".AddParameter: ((value == '')||(value == null)");
                return;
            }
            _parameterList.Add(value);
        }

        /// <summary>
        /// この sectionName の sectionName 以外すべての行数を取得する
        /// </summary>
        public int GetParametersCount()
        {
            if (_parameterList == null) { 
                _error.AddLog(this.ToString() + ".GetParametersCount: parameterList is null"); 
                return -1;
            }
            return _parameterList.Count;
        }

        /// <summary>
        /// この sectionName の sectionName 以外すべてを取得する
        /// </summary>
        public List<string> GetParametersList() { return _parameterList; }

        /// <summary>
        /// 値が一つでも存在しているか。Private 変数 _sectionName の有無で判断している。
        /// </summary>
        public bool IsExistsValues()
        {
            if (_sectionName == "") { return false; }
            if (_sectionName == null) { return false; }
            return true;
        }

        /// <summary>
        /// parameterName が存在するか判定する
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public int IsExistsParameterName(string parameterName)
        {
            try
            {
                int count = 0;
                if (_parameterList == null) { throw new Exception("ParamList Is Null"); }
                if (_parameterList.Count < 1) { throw new Exception("ParamList Count 0"); }

                string buf;
                foreach(string value in _parameterList)
                {
                    buf = this.GetParameterNameFromLine(value);
                    if (buf.Equals(parameterName))
                    {
                        return count;
                    }
                    count++;
                }
                return -1;
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".IsExistsParameterName Failed");
                return -1;
            }
        }

        /// <summary>
        /// この sectionName の ParameterList から、合致した parameterName の値を更新する
        /// </summary>
        public int SetParameterValue(string parameterName,string value)
        {
            try
            {

                List<string> list = _parameterList;
                if (list == null) { throw new Exception("ParamList Is Null");}
                if (list.Count < 1) { throw new Exception("ParamList Count 0");}

                _error.AddLog("  SetParameterValue: section=" + _sectionName + " ,parameter=" + parameterName + " ,value=" + value);
                int count = 0;
                bool flag = false;
                foreach (string val in list)
                {
                    string buf = GetParameterNameFromLine(val);
                    if (buf.CompareTo(parameterName) == 0)
                    {
                        flag = true;
                        break;
                    }
                    count++;
                }
                if (flag) { 
                    _parameterList[count] = parameterName + "=" + value;
                    _error.AddLog("  SetParameterValue Success.");
                    return 1;
                }
                else
                { 
                    throw new Exception("ParameterName Not Exists. [" + parameterName + "]"); 
                }
                //return -1;
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".SetParameterValue Failed");
                return 0;
            }
        }



        /// <summary>
        /// この sectionName の ParameterList から、合致した parameterName の値を取得する
        /// </summary>
        public string GetParameterValue(string parameterName)
        {
            try {

                List<string> list = _parameterList;
                if (list == null) { _error.AddLog("GetParameterValue:ParamList Is Null"); return ""; }
                if (list.Count < 1) { _error.AddLog("GetParameterValue:ParamList Count 0");  return ""; }

                foreach (string value in list)
                {
                    string buf = GetParameterNameFromLine(value);
                    if (buf.CompareTo(parameterName) == 0)
                    {
                        string ret = GetParameterValueFromLine(value);
                        _error.AddLog("  GetParameterValueFromLine: value="+value);
                        return ret;
                    }
                }
                _error.AddLog("GetParameterValue:ParameterName Not Exists. [" + parameterName + "]");
                return "";
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".GetParameterValue Failed");
                return "";
            }
        }

        /// <summary>
        /// param=value 形式の文字列から value を取得する
        /// </summary>
        public string GetParameterValueFromLine(string value)
        {
            try
            {
                if (value == "") { _error.AddLog("GetParameterValueFromLine:value is blank"); return ""; }
                if (value == null) { _error.AddLog("GetParameterValueFromLine:value is null"); return ""; }
                int pos = value.IndexOf('=');
                if (pos > 0)
                {
                    return value.Substring(pos+1, value.Length - pos-1);
                }
                return "";
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, "GetParameterNameFromLine Failed");
                return "";
            }
        }

        /// <summary>
        /// param=value 形式の文字列から param を取得する
        /// </summary>
        public string GetParameterNameFromLine(string value)
        {
            try
            {
                if (value == "") { _error.AddLog("GetParameterValueFromLine:value is blank"); return ""; }
                if (value == null) { _error.AddLog("GetParameterValueFromLine:value is null"); return ""; }
                int pos = value.IndexOf('=');
                if (pos > 0)
                {
                    return value.Substring(0,pos);
                }
                return "";
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, "GetParameterNameFromLine Failed");
                return "";
            }
        }
    }
}
