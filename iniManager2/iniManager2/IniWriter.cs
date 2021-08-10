using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IniManager
{
    class IniWriter
    {
        readonly ErrorManager.ErrorManager _error;
        public readonly Constants Constants = new Constants();
        protected List<string> writeList = new List<string>();
        public IniWriter(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        /// <summary>
        /// 保持しているデータを path に書き込む\n
        /// 書き込みモード\n
        /// CREATE_FILE_WHEN_NOT_EXISTS : path がないときはファイルを新たに作成する\n
        /// NOT_CREATE_FILE_WHEN_NOT_EXISTS : path がないときはファイルを作成しない\n
        /// </summary>
        /// <param name="path">書き込みファイルパス</param>
        /// <param name="mode">書き込みモード</param>
        /// <param name="writeList">Listに列挙された書き込みデータ</param>
        public int WriteData(string path,int mode ,List<string> writeList)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".WriteData");
                int ret;
                if (!(File.Exists(path)))
                {
                    // not exists
                    if (mode == Constants.CREATE_FILE_WHEN_NOT_EXISTS)
                    {
                        ret = new Common().ExcuteWrite(path,"");
                        if (ret < 1)
                        {
                            throw new Exception("ExcuteWrite Failed.");
                            //return -1;
                        }
                    } else if(mode == Constants.NOT_CREATE_FILE_WHEN_NOT_EXISTS)
                    {
                        throw new Exception("Path Not Exists.\nPath = "+ path);
                        //return -2;
                    }
                }
                IniReader reader = new IniReader(_error);
                // 読み込み
                ret = reader.ReadSettingsFile(path);
                if (ret < 0) {
                    return -3; 
                }

                // 値をセットする
                ret = SetValueForWrite(reader, writeList);
                if (ret < 1) { return -4; }

                // 1つの文字列に書き込み用
                string writeData = ListToString(writeList);
                if (_error.HasException()) { return -5; }

                // 書き込み
                ret = ExcuteOverWrite(path, writeData);
                if (ret < 1) { return -6; }
                _error.AddLog("ExcuteOverWrite Success. Path= "+path);

                return 1;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".WriteData Failed.");
                return 0;
            }
        }
        private int ExcuteOverWrite(string path, string writeData)
        {
            try
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter writer = new StreamWriter(path, false, sjisEnc);
                writer.Write(writeData);
                writer.Close();
                return 1;
            }
            catch (Exception ex)
            {
                _error.SaveException(ex, this.ToString() + ".ExcuteOverWrite");
                return 0;
            }
        }

        private string ListToString(List<string> writeList)
        {
            try
            {
                if (writeList == null)
                { throw new Exception("writeList is null.");}
                if (writeList.Count < 1)
                { throw new Exception("writeList count is 0."); }

                string writedata="";
                foreach(var value in writeList)
                {
                    writedata += value + "\n";
                }
                return writedata;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ListToString Failed.");
                return "";
            }
        }

        private int SetValueForWrite(IniReader reader,List<string> writelists)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetValueForWrite");
                List<string> lines = reader.GetReadLinesAll();

                if (lines is null)
                {
                    //throw new Exception("lines is null."); 
                    _error.AddLog(" lines is null. => Make New List<string>");
                    lines = new List<string>();
                }
                if (lines.Count < 1)
                {
                    //throw new Exception("lines count is 0."); 
                    _error.AddLog(" lines count is 0.");
                }

                if (writelists is null)
                {
                    //throw new Exception("writelists is null."); 
                    _error.AddLog("writelists is null.  Stop Writing"); return -1;
                }
                if (writelists.Count < 1)
                { 
                    //throw new Exception("writelists count is 0.");
                    _error.AddLog("writelists count is 0.  Stop Writing"); return -1;
                }


                List<string> newList = new List<string>();
                string newData = "";
                bool writeFlag = false;
                bool sectionFlag = false;
                string sectionName = "";
                string paramWrite = "";
                string paramRead = "";
                IniSection iniSection = new IniSection(_error);
                int readCount = 0;
                foreach (var writeValue in writelists)
                {

                    if (writeValue.StartsWith("[") && writeValue.EndsWith("]"))
                    {
                        // Section が来るまでは無視
                        if (!writeFlag) { writeFlag = true; }                    
                        sectionName = writeValue;
                        newList.Add(writeValue);
                    } else
                    {

                        // 空文字ならそのまま残す
                        if (writeValue is null) { newData = writeValue;  break; }
                        if (writeFlag)
                        {
                            sectionFlag = false;
                            foreach (var readvalue in lines)
                            {
                                readCount = 0;
                                // section であればチェック
                                if (readvalue.StartsWith("[") && readvalue.EndsWith("]"))
                                {
                                    // sectionName 合致するか
                                    if (readvalue.CompareTo(sectionName) == 0) { sectionFlag = true; }
                                    else
                                    {
                                        // 違う sectionName なら false
                                        sectionFlag = false;
                                    }
                                }
                                else if (sectionFlag)
                                {
                                    // 同じ sectionName 内であれば値をチェック
                                    paramRead = iniSection.GetParameterNameFromLine(readvalue);
                                    paramWrite = iniSection.GetParameterNameFromLine(writeValue);
                                    if (paramRead.CompareTo(paramWrite) == 0)
                                    {
                                        // parameterName が合致する場合は ReadValue 変更
                                        //{"コレクションが変更されました。列挙操作は実行されない可能性があります。"}
                                        //lines[readCount] = paramRead + "=" + iniSection.GetParameterValueFromLine(writeValue);
                                        newData = paramRead + "=" + iniSection.GetParameterValueFromLine(writeValue);
                                        break;
                                    }

                                } else
                                {
                                    // 違う sectionName かつ [] ではない行の文字列、何もしない
                                }
                                readCount++;
                            } // foreach readList
                        } // writeFlag
                        else
                        {
                            newData = writeValue;
                        }
                        newList.Add(newData);
                    } // not start section 
                } // foreach writeList

                // すべて終わったら変更を終えた list を保存する
                this.writeList = newList;
                return 1;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueForWrite Failed.");
                return 0;
            }
        }
    }
    public class Constants
    {
        public readonly int CREATE_FILE_WHEN_NOT_EXISTS = 1;
        public readonly int NOT_CREATE_FILE_WHEN_NOT_EXISTS = 2;
    }
}
