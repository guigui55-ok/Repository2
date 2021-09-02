using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtility.CloseHandleUtil;

namespace CommonUtility
{
    static class ClassMain
    {
        static void Main()
        {
            List<string> _TypeList = new List<string>() { ".xlsx"};
            try
            {
                Handles handles = new Handles();
            }
            finally
            {
                Console.ReadKey();
            }
        }



        // ProcessId が保持している Handle の Type.Event と Type.File が保持している Name を
        // 取得してその中から _ExcelTypeList が含まれるものを抜粋する
        // メイン処理関数
        static public List<string> GetListExcelFilePathFromPid_Main(int pid)
        {
            List<string> pathList = new List<string>();
            try
            {
                //if (_ExcelTypeList == null) { throw new Exception("ExcelTypeList Is Null"); }
                //if (_ExcelTypeList.Count < 0) { throw new Exception("ExcelTypeList.Count Is Zero"); }

                IEnumerable<HandleInfo> enumInfo = Handles.EnumProcessHandles(pid);
                if (enumInfo == null)
                {
                    Console.WriteLine("Handles.EnumProcessHandles [" + pid + "] Is Null");
                }
                Console.WriteLine("");

                foreach (HandleInfo hi in enumInfo)
                {

                    foreach (string type in _TypeList)
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
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, "GetListExcelFilePathFromPid Failed.");
                return pathList;
            }
        }
    }
}
