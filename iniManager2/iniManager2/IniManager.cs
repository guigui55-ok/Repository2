using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IniManager
{
    public class IniManager
    {

        /// <summary>
        ///  Error 管理変数。クラスオブジェクトの実行後に使用する。
        /// </summary>
        public ErrorManager.ErrorManager _error;
        IniReader _reader;
        string _path;
        public IniManager(ErrorManager.ErrorManager error)
        {
            _error = error;
            _reader = new IniReader(_error);

        }

        /// <summary>
        ///  Path セットする。存在しない場合はエラーを保持する。
        /// </summary>
        public int SetPath(string path)
        {
            try
            {
                _error.AddLog(this.ToString() + ".SetPath path=" + path);
                if (File.Exists(path)) 
                { _path = path; return 1; } 
                else  
                { throw new Exception("Path Not Exists.\nPath = " + path);}
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString()+".SetPath");
                return 0;
            }
        }

        public string GetErrorMessages() { return _error.GetErrorMessage(); }
        public void ShowErrorMessages() { MessageBox.Show(GetErrorMessages()); }

        /// <summary>
        ///  Path を読み込む、ない場合は作らない。Pathをセットした後に実行する。
        /// </summary>
        public int ReadIni() { return _reader.ReadSettingsFile(_path); }


        /// <summary>
        ///  Path セットし、値を読み込む
        /// </summary>
        public int ReadIni(string path)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".ReadIni path="+path);
                _path = path;
                if (File.Exists(path)) {
                    return _reader.ReadSettingsFile(_path);
                } else
                {
                    throw new Exception(this.ToString() + ".IniReader.ReadSettingsFile Failed.");
                    //Error = _reader.GetError();
                    //return -1;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetPath Failed");
                return 0;
            }
        }

        /// <summary>
        ///  Path を読み込む、ない場合は作る。最初にこのメソッドを実行する。
        /// </summary>
        public int SetPathAndCreateFileIfPathNotExists(string iniPath, string iniFormatString)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetPathAndCreateFileIfPathNotExists");
                int ret;
                // ファイル存在チェック
                ret = this.SetPath(iniPath);
                if (ret < 1)
                {
                    // 存在しないエラー                    
                      _error.AddLog(this.ToString() + ".ReadIni : " +
                        "SetPath Not Exitsts. Create New File. \nIniPath = " + iniPath);

                    // なければフォーマットをしている場合作る
                    ret = new FileForReadWrite().ExcuteWrite(iniPath, iniFormatString);
                    if (ret < 1)
                    {
                        // フォーマットの書き込みエラー
                        throw new Exception(this.ToString() + ".ReadIniSetPath Not Exitsts. Write File Failed. \nIniPath = " + iniPath);
                    }
                    else
                    {
                        _error.AddLog("ExcuteWrite Success. iniPath=" + iniPath);
                    }
                    _path = iniPath;
                }
                return 1;
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".SetPathAndCreateFileIfPathNotExists");
                return 0;
            }
        }

        /// <summary>
        /// Set した Path から値を読み込み、クラス変数にセットする (ReadIni の後に実行)
        /// </summary>
        public int SetSectionFromPath()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetSectionFromPath");
                // 読み込んだ値をReader.IniSectionListへ格納
                int ret = _reader.SetSectionListByAnalyze();
                if (ret == 0)
                {
                    throw new Exception("SetSectionFromPath Failed");
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetPathAndCreateFileIfPathNotExists");
                return 0;
            }
        }

        /// <summary>
        /// sectionName、parameterName と合致した場合、parameterName の値を取得する
        /// </summary>
        public string GetParameterValue(string sectionName, string parametersName)
        {
            return _reader.GetParameterValue(sectionName, parametersName);  
        }

        public string GetParamterValueWhenAddValueIsNothing(string sectionName,string parameterName)
        {
            return GetOrSetParamterValueWhenAddValueIsNothing(sectionName, parameterName, 0);
        }

        public void SetParamterValueWhenAddValueIsNothing(string sectionName,string parameterName,string value)
        {
            _ = GetOrSetParamterValueWhenAddValueIsNothing(sectionName, parameterName, 1,value);
        }

        /// <summary>
        /// sectionName > parameterName の値を取得・設定する。存在しない場合は sectionName , parameterName追加する
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="parametersName"></param>
        /// <returns></returns>
        public string GetOrSetParamterValueWhenAddValueIsNothing(
            string sectionName, string parameterName,int mode,string value = "")
        {
            try
            {
                _error.ClearError();
                _error.AddLog(this.ToString()+ ".GetOrSetParamterValueWhenAddValueIsNothing :Mode="+mode);
                _error.AddLog("  section="+sectionName + " ,parameter="+parameterName+ " ,value="+value);
                if((sectionName == "")||(sectionName == null))
                { throw new Exception(" sectionnName == '' || sectionNmae == null"); }
                if ((parameterName == "") || (parameterName == null))
                { throw new Exception(" parametersName == '' || parametersName == null"); }

                // sectionName が存在するか判定する,0 以上が存在する
                int ret = _reader.IsExistsSection(sectionName);
                if (_error.HasException()) { 
                    _error.AddLog("  IsExistsSection failed"); return "";
                }
                if (ret < 0)
                {
                    // sectionName が存在しないときは、
                    // sectionName, parameterName 作成して終了する
                    _reader.AddNewSection(sectionName, parameterName+"=");
                    if (_error.HasException()) { _error.AddLog("  AddNewSection / " + sectionName + " / " + parameterName); return ""; }
                    return "";
                }
                IniSection sec = _reader.GetIniSection(sectionName);
                if (_error.HasException()) { return""; }
                if (sec == null) {
                    _error.AddLog(" GetIniSection Is null => new List<iniSection>");
                    _reader.IniSectionList = new List<IniSection>();
                }
                // parameterName が存在するか判定する
                if (sec.IsExistsParameterName(parameterName) >= 0)
                {
                    // parameterName が存在する場合
                    switch (mode)
                    {
                        case 0: // Get
                            // parameterName の値を取得する
                            string buf = sec.GetParameterValue(parameterName);
                            if (_error.HasException()) { return ""; }
                            return buf;
                        case 1: // Set
                            sec.SetParameterValue(parameterName, value);
                            if (_error.HasException()) { return ""; }
                            return "";
                        default:
                            throw new Exception("mode is invalid");
                    }
                } else
                {
                    // parameterName が存在しない場合は、Get Set 同じ処理
                    // parameterName ,Value を追加して終了する
                    sec.AddParameter(parameterName + "=" + value);
                    return "";
                }

            }catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetParamterValueWhenAddValueIsNothing");
                return "";
            }
        }



        /// <summary>
        /// sectionName、parameterName と合致した場合、value の値を更新する
        /// </summary>
        public int SetParameterValue(string sectionName,string parameterName,string value)
        {
            return _reader.SetParameterValue(sectionName, parameterName, value);
        }

        /// <summary>
        /// [sectionName] を読み込み、ParameterList を取得する
        /// </summary>
        public List<string> GetSectionValues(string sectionName)
        {
            return  _reader.GetSectionValuesFromIniSectionList(sectionName);
        }

        /// <summary>
        /// [sectionName] をカウントする
        /// </summary>
        public int CountSection(string sectionName) { 
            return _reader.CountSection(sectionName);
        }

        /// <summary>
        /// 今保持しているデータをすべて Path に更新する
        /// </summary>
        public int WriteAllData()
        {
            _error.AddLog(this.ToString()+".WriteAllData");
            // list<string> にする
            List<string> writeList = ConvertSectionDataToListString();
            IniWriter writer = new IniWriter(_error);
            int ret = writer.WriteData(_path,writer.Constants.CREATE_FILE_WHEN_NOT_EXISTS,writeList);
            return ret;
        }
        public List<string> ConvertSectionDataToListString()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".ConvertSectionDataToListString");
                List<IniSection> sectionList = _reader.IniSectionList;

                if (sectionList == null)
                {
                    throw new Exception("sectionList is null");
                }
                if (sectionList.Count < 1)
                {
                    throw new Exception("sectionList Count < 1");
                }

                List<string> readList;
                List<string> writeList = new List<string>();
                string buf;
                foreach(var iniSection in sectionList)
                {                    
                    if ((!(iniSection == null)))
                    {
                        buf = "[" + iniSection.GetSectionName() + "]";
                        _error.AddLog("  " + buf);
                        writeList.Add(buf);
                        readList = iniSection.GetParametersList();
                        if ((!(readList == null))&&(!(readList.Count < 1))) 
                        { 
                            foreach(var value in readList)
                            {
                                _error.AddLog("  " + value);
                                writeList.Add(value);
                            }
                        }
                        else
                        {// list is null or list.count < 1
                            _error.AddLog("list is null or list.count < 1");
                        }
                    } 
                    else
                    {
                        // inisection is null
                        _error.AddLog("inisection is null");
                    }
                }

                return writeList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertSectionDataToListString");
                return null;
            }
        }
    }

    // ====================================================
    class FileForReadWrite
    {
        public string ExcuteRead(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(
                    path, Encoding.GetEncoding("Shift_JIS"));

                string text = sr.ReadToEnd();

                sr.Close();

                //Console.Write(text);
                return text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".ExcuteRead Failed");
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
        public int ExcuteWrite(string path, string writeData)
        {
            try
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter writer =
                  new StreamWriter(path, true, sjisEnc);
                writer.Write(writeData);
                writer.Close();
                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".ExcuteWrite Failed");
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
