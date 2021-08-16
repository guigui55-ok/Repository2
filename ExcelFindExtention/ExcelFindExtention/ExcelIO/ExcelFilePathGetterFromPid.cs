using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelIO
{
    public class ExcelFilePathGetterFromPid
    {
        protected ErrorManager.ErrorManager _Error;
        protected List<string> _ExcelTypeList;
        public ExcelFilePathGetterFromPid(ErrorManager.ErrorManager error,List<string> ExcelTypeList)
        {
            _Error = error;
            _ExcelTypeList = ExcelTypeList;
        }

        // ProcessId が保持している Handle の Type.Event と Type.File が保持している Name を
        // 取得してその中から _ExcelTypeList が含まれるものを抜粋する
        // その後、List 内に同値がある場合は消去し、
        // エクセルの作業中の backup ファイルの文字列 ~$ を含むものも消去する
        public List<string> GetListExcelFilePathFromPid(int pid)
        {
            try
            {
                List<string> pathList = GetListExcelFilePathFromPid_Main(pid);
                if(pathList.Count < 1) { return pathList; }
                pathList = RemoveSameValue(pathList);
                pathList = RemoveElementIncludeValue(pathList, "~$");
                return pathList;
            } catch (Exception ex)
            {
                _Error.AddException(ex, "GetListExcelFilePathFromPid Failed.");
                return new List<string>();
            }
        }

        // ProcessId が保持している Handle の Type.Event と Type.File が保持している Name を
        // 取得してその中から _ExcelTypeList が含まれるものを抜粋する
        // メイン処理関数
        public List<string> GetListExcelFilePathFromPid_Main(int pid)
        {
            List<string> pathList = new List<string>();
            try
            {
                if (_ExcelTypeList == null) { throw new Exception("ExcelTypeList Is Null"); }
                if (_ExcelTypeList.Count < 0) { throw new Exception("ExcelTypeList.Count Is Zero"); }

                foreach (Utility.HandleInfo hi in Utility.Handles.EnumProcessHandles(pid))
                {

                    foreach (string type in _ExcelTypeList)
                    {
                        string buf = hi.Name;
                        if (buf != null)
                        {
                            if (buf.Contains(type))
                            {
                                pathList.Add(hi.Name);
                            }
                        }
                    }
                }
                return pathList;
            } catch (Exception ex)
            {
                _Error.AddException(ex, "GetListExcelFilePathFromPid Failed.");
                return pathList;
            }
        }

        // List 内から value が含まれる要素を削除する
        private List<string> RemoveElementIncludeValue(List<string> list, string value)
        {
            List<string> retList = new List<string>();
            try
            {
                if (value == "") { return list; }
                if (list == null) { throw new Exception("List Is Null"); }
                if (list.Count < 1) { throw new Exception("List.Count Is Zero"); }
                foreach (string buf in list) { retList.Add(buf); }

                int max = list.Count;
                for (int i = 0; i < max; i++)
                {
                    if (retList[i].IndexOf(value) > 1)
                    {
                        // include
                        retList.RemoveAt(i);
                        max = retList.Count;
                        if (i >= max)
                        {
                            break;
                        }
                        else
                        {
                            i--;
                        }
                    }
                }

                return retList;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex,this.ToString()+ ".RemoveElementIncludeValue");
                return retList;
            }
        }
        // リスト内に同値があれば削除する
        private List<string> RemoveSameValue(List<string> list)
        {
            List<string> retList = new List<string>();
            try
            {
                if (list == null) { throw new Exception("List Is Null"); }
                if (list.Count < 1) { throw new Exception("List.Count Is Zero"); }
                foreach (string value in list) { retList.Add(value); }

                foreach (string value in list)
                {
                    int max = retList.Count;
                    bool flag = false;
                    for (int i = 0; i < max; i++)
                    {
                        if (value == retList[i])
                        {
                            if (flag)
                            {
                                // 2度目
                                retList.RemoveAt(i);
                                max = retList.Count;
                                if (i >= max)
                                {
                                    break;
                                }
                                else
                                {
                                    i--;
                                }
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                    }
                }
                return retList;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex,this.ToString()+ ".RemoveSameValue");
                return retList;
            }
        }

    }
}
