using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IniManager
{
    class IniReader
    {

        // エラー出力、保存用クラス
        public ErrorManager.ErrorManager _error;
        // デバッグモード 1 で ConeleWriteline する
        protected int DebugMode = 1;

        // [Section]のリスト 配置でないものを想定
        protected List<List<string>> _sections = new List<List<string>>();
        // [Section]内の値のリスト
        public List<string> Section = new List<string>();
        // []を除外したもの
        public string _sectionName;
        // Section の行
        List<string> _lines;
        protected string _clName;

        // SectionList クラス 読み込み時に設定
        public List<IniSection> IniSectionList;
        public IniReader(ErrorManager.ErrorManager error)
        {
            _error = error;
            DebugMode = error.DebugMode;
            _clName = this.ToString();
        }

        public List<string> GetReadLinesAll() { return _lines; }
        //public Error GetError() { return _error; }

        /// <summary>
        /// [Section] をカウントする
        /// </summary>
        public int CountSection(string sectionName)
        {
            try
            {
                int ret = 0;
                sectionName = "[" + sectionName + "]";
                for (int i = 0; i < _lines.Count - 1; i++)
                {
                    if (string.Compare(sectionName, _lines[i].ToString()) == 0)
                    {
                        ret++;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, "CountSection Failed");
                return -1;
            }
        }


        /// <summary>
        /// [sectionName]、parameterName と合致した場合、param=value の value を取得する
        /// </summary>
        public string GetParameterValue(string sectionName,string parameterNme)
        {
            try
            {
                _error.AddLog(_clName + ".GetParameterValue:sectionName="+sectionName + " ,parameterName="+parameterNme);
                IniSection iniSection= GetIniSection(sectionName);
                if (iniSection == null) { throw new Exception("sectionName Not Exists.("+sectionName+")"); }
                string buf = iniSection.GetParameterValue(parameterNme);
                return buf;
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".GetParameterValue Failed");
                return "";
            }
        }

        // ひとまずiniManagerで実装
        //public string GetParamterValueWhenAddValueIsNothing(string sectionName, string parametersName)
        //{
        //    try
        //    {

        //    } catch (Exception ex)
        //    {
        //        _error.SaveException(ex, "GetParamterValueWhenAddValueIsNothing Failed");
        //        return "";
        //    }
        //}

        /// <summary>
        /// Section 名が存在するか判定する
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public int IsExistsSection(string sectionName)
        {
            try
            {
                if (IniSectionList == null) { _error.AddLog(_clName + ".IsExistsSection:IniSectionList Is Null"); return -1; }
                if (IniSectionList.Count < 1) { _error.AddLog(_clName + ".IsExistsSection:IniSectionList.Count<1"); return -2; }

                int count = 0;
                foreach (IniSection sec in IniSectionList)
                {
                    if (sec.GetSectionName().Equals(sectionName))
                    {
                        return count;
                    }
                    count++;
                }
                return -3;
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, _clName + ".IsExistsSection Failed");
                return -4;
            }
        }
        /// <summary>
        /// [sectionName]、parameterName と合致した場合、param=value の value を更新する
        /// </summary>
        public int SetParameterValue(string sectionName,string parameterName,string value)
        {
            try
            {
                _error.AddLog(this.ToString() + ".SetParameterValue:sectionName=" + sectionName 
                    + " ,parameterName=" + parameterName + " ,value="+value);
                IniSection iniSection = GetIniSection(sectionName);
                if (iniSection == null)
                {
                    throw new Exception("sectionName Not Exists.(" + sectionName + ")");
                    //return 1;
                }
                return iniSection.SetParameterValue(parameterName,value);
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString()+ ".SetParameterValue Failed");
                return 0;
            }
        }

        /// <summary>
        /// sectionName から IniSection を取得する
        /// </summary>
        public IniSection GetIniSection(string sectionName)
        {
            try
            {
                List<IniSection> list = this.IniSectionList;
                if (list == null) {　throw new Exception("IniSectionList Is Null."); }
                if (list.Count < 1) { throw new Exception("IniSectionList Count 0."); }

                foreach (IniSection iniSection in list)
                {
                    if (sectionName.CompareTo(iniSection.GetSectionName()) == 0)
                    {
                        return iniSection;
                    }
                }
                _error.AddLog(this.ToString()+ ".GetIniSection : sectionName is Not Exists.  sectionName="+sectionName);
                return null;
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".GetIniSection Failed");
                return null;
            }
        }

        /// <summary>
        /// sectionName から parameterList を取得する
        /// </summary>
        public List<string> GetParametersList(string sectionName)
        {
            try
            {
                _error.AddLog(this.ToString()+".GetParametersList");
                List<IniSection> list = this.IniSectionList;
                if (list == null) { _error.AddLog(" list is null. sectionName="+sectionName);  return null; }
                if (list.Count < 1) { _error.AddLog(" list.Count<1. sectionName=" + sectionName); return null; }
                foreach (IniSection iniSection in list)
                {
                    if (sectionName.CompareTo(iniSection.GetSectionName()) == 0)
                    {
                        return iniSection.GetParametersList();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".GetParametersList Failed");
                return null;
            }
        }


        public List<string> GetSectionValuesFromIniSectionList(string sectionName)
        {
            try
            {
                _error.AddLog(this.ToString() + ".GetSectionValues");
                // リストリストの1つ目で判断する
                foreach (var sec in this.IniSectionList)
                {
                    if (!(sec == null))
                    {
                        if (!(sec.GetParametersCount() < 1))
                        {
                            if (sec.GetSectionName().Equals(sectionName))
                            {
                                return sec.GetParametersList();
                            }
                        }
                        else
                        {
                            // lines.Count < 1
                            _error.AddLog(" IniSection.Count<1 / sectionName="+ sec.GetSectionName());
                        }
                    }
                    else
                    {
                        // lines is null
                        _error.AddLog(" IniSection is Null");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".GetSectionValues Failed");
                return null;
            }
        }

        /// <summary>
        /// sectionName から ParameterList を取得する
        /// </summary>
        public List<string> GetSectionValues(string sectionName)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".GetSectionValues");
                // リストリストの1つ目で判断する
                foreach( var lines in _sections)
                {
                    if (!(lines == null))
                    {
                        if (!(lines.Count < 1))
                        {
                            if (lines[0].CompareTo(sectionName) == 0)
                            {
                                return lines;
                            }
                        } else
                        {
                            // lines.Count < 1
                            _error.AddLog(" Lines.Count<1");
                        }
                    } else
                    {
                        // lines is null
                        _error.AddLog(" Lines is Null");
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".GetSectionValues Failed");
                return null;
            }
        }

        /// <summary>
        /// sectionName の値を読み込む。count は 1 が 1つ目
        /// </summary>
        private void ReadSection(string sectionName,int count)
        {
            List<string> list;
            try
            {
                list = new List<string>();
                int nowcount = 1;
                bool start = true;
                _sectionName = "[" + sectionName + "]";
                for (int i=0; i<_lines.Count - 1; i++)
                {
                    if (string.Compare(_sectionName, _lines[i]) == 0)
                    {
                        if (nowcount == count)
                        {
                            for (int j=i; j < _lines.Count - 1; j++)
                            {
                                if (start)
                                {
                                    start = false;
                                } else
                                {
                                    if (_lines[j].StartsWith("[") == true)
                                    {
                                        break;
                                    }
                                }
                                list.Add(_lines[j]);
                            }
                        }
                        if (nowcount >= count)
                        {
                            break;
                        }
                        nowcount++;
                    }
                }
                Section = list;
                _sections.Add(list);
            }
            catch (Exception ex)
            {
                SaveException(ex, this.ToString() + ".ReadSection Failed");
            }
        }

        private void SaveException(Exception exception,string funcName)
        {
            _error.SaveException(exception, funcName);
        }


        /// <summary>
        /// セットした iniPath のすべての値を読み込み IniSectionList へ格納する
        /// </summary>
        public int SetSectionListByAnalyze()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetSectionListByAnalyze");
                IniSectionList = new List<IniSection>();
                if (_lines == null) {
                    _error.AddLog("Readlines is null");
                    return -1; 
                }
                if (_lines.Count < 1)
                {
                    _error.AddLog("Readlines.Count < 1");
                    return -2; 
                }

                IniSection nowSection = new IniSection(_error);
                bool Reading = false;
                foreach (var value in _lines)
                {
                    if (value.StartsWith("[") && value.EndsWith("]"))
                    {
                        // 読み取り行が [sectionName] の形式であるとき
                        // すでに値があるかチェックする
                        if (nowSection.IsExistsValues())
                        {
                            // IniSection にすでに値が入っていたら 2週目以降、保存して次へ
                            IniSectionList.Add(nowSection);
                        } // else nowSection に値が一つも入っていない
                        // nowSection にまだ値が一つも入っていない場合、new して sectionName を格納する
                        Reading = true;
                        nowSection = new IniSection(_error);
                        // [sectionName]
                        nowSection.SetSectionName(value.Substring(1,value.Length-2));
                    } else
                    {
                        // 読み取り行が [sectionName] の形式ではないとき
                        // [SectionName] 以前の値は無視することとする
                        if (Reading)
                        {
                            // 値を保存
                            nowSection.AddParameter(value);
                        }
                    }
                } // else [sectionName] 形式ではないときは、なにもしない

                // 2週目以降は、保存しているが、最後が保存されていないので
                if (nowSection.IsExistsValues())
                {
                    IniSectionList.Add(nowSection);
                }
                _error.AddLog("  SetParameterListByAnalyze Success");
                return 1;
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".SetParameterListByAnalyze Failed");
                return 0;
            }
        }

        public void AddNewSection(string sectionName,string parameterName)
        {
            try
            {
                _error.AddLog(this.ToString() + ".AddNewSection : section=" + sectionName + " ,parameter=" + parameterName);
                // sectionName, parameterName 作成して終了する
                IniSection newSection = new IniSection(_error);
                // sectionName を追加する
                newSection.SetSectionName(sectionName);
                // parameterName を追加する
                newSection.AddParameter(parameterName);
                // リストへ追加する
                this.IniSectionList.Add(newSection);
            } catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".AddNewSection Failed");
            }
        }


        /// <summary>
        /// セットした IniPath を読み込む。読み込んだものすべてはいったん _lines へ格納する
        /// </summary>
        public int ReadSettingsFile(string filepath)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".ReadSettingsFile");
                string line = "";
               _lines = new List<string>();

                // using System.IO;
                using (StreamReader sr = new StreamReader(
                    filepath, Encoding.GetEncoding("Shift_JIS")))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        _lines.Add(line);
                        // デバッグ用
                        if (_error.DebugMode == 1) { System.Diagnostics.Debug.WriteLine(line); }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                SaveException(ex, this.ToString()+".ReadSettingsFile Failed");
                return 0;
            }
        }

    }
}
