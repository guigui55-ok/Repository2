using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelIO
{
    public class ExcelIO
    {
        protected ErrorManager.ErrorManager mError;
        protected Microsoft.Office.Interop.Excel.Application mApp = null;
        protected Microsoft.Office.Interop.Excel.Workbook mWorkBook = null;
        protected ProcessUtility.ProcessUtility _ProcUtil;
        protected string _ExcelProcName = "EXCEL";
        //
        protected List<int> _PidList;
        protected List<string> _ProcessNameList;
        //
        protected string _FilePath;
        protected Process[] _ProcessList;
        protected List<Process> _ExcelProcList;
        protected List<string> _ExcelFileNameOpenedList;
        protected int _DelayTime = 500;
        protected int _Pid=0;
        protected Microsoft.Office.Interop.Excel.Application _ExcelApp = null;

        public ExcelIO(ErrorManager.ErrorManager error)
        {
            mError = error;
            _ProcUtil = new ProcessUtility.ProcessUtility(error);
        }

        private void SetExcelFileNameOpendList(List<Process> excelProcList)
        {
            try
            {

                //配列から1つずつ取り出す
                foreach (System.Diagnostics.Process p in excelProcList)
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("エラー: {0}", ex.Message);
                    }
                }
            } catch (Exception ex)
            {
                mError.AddException(ex, this.ToString() + ".SetExcelFileNameOpendList");
                Console.WriteLine(ex.StackTrace);
                return;
            }
        }

        // GetActiveObject メソッドから Excel.Application を取得しセットする
        public bool SetExcelApplicationByGetActiveObjectMethod()
        {
            try
            {
                _ExcelApp = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                return true;
            } catch(Exception ex)
            {
                mError.AddException(ex, this.ToString() + ".Open");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        // GetActiveObject メソッドから Excel.Application を取得する
        public Excel.Application GetExcelApplicationFromGetActiveObject()
        {
            try
            {
                return (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch (Exception ex)
            {
                mError.AddException(ex, this.ToString() + ".GetExcelApplicationFromGetActiveObject");
                return null;
            }
        }

        public bool Open(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    return false;
                }
                _FilePath = filePath;
                _ProcessList = Process.GetProcesses();
                // 開く前に現在のPidリストを取得しておく
                _PidList = _ProcUtil.GetPidListContainsProcessNameInNow(_ProcessList,_ExcelProcName);

                _ExcelProcList = _ProcUtil.GetProcessListMatchToProcessName(_ProcessList, _ExcelProcName);

                _ExcelApp = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");

                Console.WriteLine("** workbook ");
                foreach(Excel.Workbook workbook in _ExcelApp.Workbooks)
                {
                    Console.WriteLine(workbook.Name);
                }

                // IsFileLocked
                bool flag = IsFileLocked(filePath);
                if (!flag)
                {
                    mApp = new Microsoft.Office.Interop.Excel.Application
                    {
                        Visible = true
                    };
                    // ロックされていない
                    // filePath が相対パスのとき例外が発生するので fullPath に変換
                    string fullPath = System.IO.Path.GetFullPath(filePath);
                    mWorkBook = mApp.Workbooks.Open(fullPath);

                    //Console.WriteLine("Delay1");
                    //await Task.Delay(_DelayTime);
                    //Console.WriteLine("Delay2");
                    System.Threading.Thread.Sleep(_DelayTime);
                    // オープンしたのち取得しておいたPidリストと比較して開いたPidを保存する
                    SetPid();
                    Console.WriteLine("Pid = " + _Pid);
                } else
                {
                    // ロックされている
                    // 既存のファイルを開いている
                    // このときは、ウィンドウタイトルの比較からPidをセットする
                    _Pid = _ProcUtil.GetPidToMatchWindowTitle(
                        _ProcessList, Path.GetFileName(_FilePath));
                    if (_Pid > 0)
                    {
                        // 開いている Pid が取得できた

                        var myExcelFilePath = Environment.ExpandEnvironmentVariables(filePath);

                        mWorkBook = Marshal.BindToMoniker(myExcelFilePath) as Microsoft.Office.Interop.Excel.Workbook;

                        Console.WriteLine(mWorkBook.GetType());
                        Console.WriteLine(mWorkBook.Parent.GetType());
                        //Console.WriteLine(mApp.GetType());
                        Console.WriteLine(typeof(Excel.Application) == mWorkBook.Parent.GetType());
                        //mApp = (Microsoft.Office.Interop.Excel.Application)mWorkBook.Parent();
                        //mApp = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                        mApp = mWorkBook.Application;

                    }
                    else
                    {
                        throw new Exception("Open Failed.(Failed Get Pid When File Rocked)");
                    }

                }
                return true;
            } catch (Exception ex)
            {
                mError.AddException(ex, this.ToString() + ".Open");
                Console.WriteLine(ex.StackTrace);
                //Close();
                return false;
            }
        }


        private bool IsFileLocked(string path)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }


        /// <summary>
        /// 開いているワークブックとエクセルのプロセスを閉じる.
        /// </summary>
        public bool Close()
        {
            try
            {
                if (mWorkBook != null)
                {
                    foreach(var sheet in mWorkBook.Sheets)
                    {
                        Marshal.FinalReleaseComObject(sheet);
                    }
                    //mWorkBook.Close(false);
                    mWorkBook.Close();
                    //Marshal.ReleaseComObject(mWorkBook);
                    Marshal.FinalReleaseComObject(mWorkBook);
                    mWorkBook = null;
                }

                if (mApp != null)
                {
                    //mApp.ActiveWindow.Close(false);
                    //mApp.Application.Quit();
                    //Process prs = Process.GetProcessById(_Pid);
                    //prs.Kill
                    mApp.Quit();
                    Marshal.FinalReleaseComObject(mApp);
                    mApp = null;
                } else
                {
                }

                bool ret = _ProcUtil.KillProcess(_Pid);
                if (!ret)
                {
                    Console.WriteLine("KillProcess Failed");
                }
                else
                {
                    Console.WriteLine("KillProcess Success");
                }
                return true;
            } catch (Exception ex)
            {
                mError.AddException(ex, this.ToString() + ".Close");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool SetPid(){
            try
            {
                // 新たに Pid リストを得る
                List<int> newList = _ProcUtil.GetPidListContainsProcessNameInNow(_ExcelProcName);

                // Open 前のリストと、Open 後のリストを比較して
                // Open 前リストにない Pid を保存する
                foreach(int val in newList)
                {
                    bool flag = false;
                    if (_PidList.Count > 0)
                    {
                        foreach (int nowVal in _PidList)
                        {
                            if (nowVal == val)
                            {
                                flag = true;
                            }
                        }
                    }

                    if (!flag)
                    {
                        if (_Pid != 0)
                        {
                        }
                        _Pid = val;
                    }
                    
                    // pid が2つ以上あった場合、一度に2つ起動した
                    // windowタイトルから選ばせる？、起動ファイルはわかっているので
                }
                return true;

            } catch (Exception ex)
            {
                mError.AddException(ex, this.ToString() + ".SetPid");
                return false;
            }
        }
    } // Class End
}
