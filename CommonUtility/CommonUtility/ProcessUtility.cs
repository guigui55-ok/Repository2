using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonUtility
{
    public class ProcessUtility
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        protected ErrorManager.ErrorManager _Error;


        public void _GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId)
        {
            GetWindowThreadProcessId(hWnd,out lpdwProcessId);
        }

        public ProcessUtility(ErrorManager.ErrorManager error)
        {
            _Error = error;
        }

        // プロセス名と合致した PID を取得する
        public List<int> GetPidListContainsProcessNameInNow(string processName)
        {
            List<int> retList = new List<int>();
            try
            {//ローカルコンピュータ上で実行されているすべてのプロセスを取得
                System.Diagnostics.Process[] ps =
                    System.Diagnostics.Process.GetProcesses();
                //"machinename"という名前のコンピュータで実行されている
                //すべてのプロセスを取得するには次のようにする。
                //System.Diagnostics.Process[] ps =
                //    System.Diagnostics.Process.GetProcesses("machinename");

                //配列から1つずつ取り出す
                foreach (System.Diagnostics.Process p in ps)
                {
                    try
                    {
                        if (p.ProcessName.Contains(processName))
                        {
                            //プロセス名を出力する
                            _Error.AddLog("プロセス名:"+ p.ProcessName + "[" + p.Id + "]");
                            retList.Add(p.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        //.WriteLine("エラー: {0}", ex.Message);
                        _Error.AddException(ex, this.ToString()+ ".GetPidListContainsProcessNameInNow foreach");
                    }
                }
                return retList;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExistsProcessNameInNow");
                return retList;
            }
        }

        // プロセス名と合致した Process を取得する
        public List<Process> GetProcessListMatchToProcessName(System.Diagnostics.Process[] processes,string processName)
        {
            List<Process> retList = new List<Process>();
            try
            {
                if (processes.Length < 1)
                {
                    throw new Exception(this.ToString() + ".IsExistsProcessNameInNow : processes Length Is Zero");
                }

                //配列から1つずつ取り出す
                foreach (System.Diagnostics.Process p in processes)
                {
                    try
                    {
                        if (p.ProcessName.Contains(processName))
                        {
                            retList.Add(p);
                        }
                    }
                    catch (Exception ex)
                    {
                        _Error.AddException(ex, this.ToString() + ".IsExistsProcessNameInNow foreach");
                    }
                }
                return retList;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExistsProcessNameInNow");
                return retList;
            }
        }

        // プロセス名と合致した PID を取得する
        public List<int> GetPidListContainsProcessNameInNow(Process[] processes,string processName)
        {
            List<int> retList = new List<int>();
            try
            {//ローカルコンピュータ上で実行されているすべてのプロセスを取得
                System.Diagnostics.Process[] ps = processes;
                //"machinename"という名前のコンピュータで実行されている
                //すべてのプロセスを取得するには次のようにする。
                //System.Diagnostics.Process[] ps =
                //    System.Diagnostics.Process.GetProcesses("machinename");

                if (ps.Length < 1) {
                    throw new Exception(this.ToString() + ".IsExistsProcessNameInNow : processes Length Is Zero");
                }

                //配列から1つずつ取り出す
                foreach (System.Diagnostics.Process p in ps)
                {
                    try
                    {
                        if (p.ProcessName.Contains(processName))
                        {
                            //プロセス名を出力する
                            _Error.AddLog("プロセス名:" + p.ProcessName + "[" + p.Id + "]");
                            retList.Add(p.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _Error.AddException(ex, this.ToString() + ".GetPidListContainsProcessNameInNow foreach");
                    }
                }
                return retList;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExistsProcessNameInNow");
                return retList;
            }
        }

        public int GetPidToMatchWindowTitle(Process[] processes,string WindowTitle,int CompareParameter = 0)
        {
            try
            {

                if (processes.Length < 1)
                {
                    throw new Exception(this.ToString() + ".IsExistsProcessNameInNow : processes Length Is Zero");
                }

                //配列から1つずつ取り出す
                foreach (System.Diagnostics.Process p in processes)
                {
                    try
                    {
                        if (p.MainWindowTitle.Length > 0)
                        {
                            //.WriteLine(p.MainWindowTitle);
                        }
                        if (p.MainWindowTitle.Equals(WindowTitle))
                        {
                            //プロセス名を出力する*
                            return p.Id;
                        }
                        if (string.Compare(p.MainWindowTitle,WindowTitle)==0)
                        {
                            //プロセス名を出力する*
                            return p.Id;
                        }
                        if (p.MainWindowTitle.IndexOf(WindowTitle)>0)
                        {
                            //プロセス名を出力する 修正
                            return p.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        _Error.AddException(ex, this.ToString() + ".GetPidToMatchWindowTitle foreach");
                    }
                }
                return -1;
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetPidToMatchWindowTitle");
                return 0;
            }
        }

        public bool ProcessClose(int pid)
        {
            try
            {    // プロセス ID が 0 のプロセスを取得する
                System.Diagnostics.Process hProcess = System.Diagnostics.Process.GetProcessById(pid);
                // 取得できたプロセスのプロセス名を表示する
                if (hProcess != null)
                {
                    _Error.AddLog(this.ToString()+".ProcessClose = " + hProcess.ProcessName);
                }

                // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
                hProcess.Close();
                hProcess.Dispose();
                return true;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ProcessClose");
                return false;
            }
        }

        public bool KillProcess(int pid)
        {
            try
            {
                _Error.AddLog(this,"KillProcess");
                System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcesses();
                bool isMatch = false;
                foreach (Process prs in process)
                {
                    if (prs.Id == pid)
                    {
                        prs.Kill();
                        _Error.AddLog(this.ToString()+ ".KillProcess : " + pid);
                        isMatch = true;
                        return true;
                        //break;
                    }
                }
                if (!isMatch)
                {
                    // プロセスが実行されていない場合、見つからない場合
                    _Error.AddLogWarning("Pid is Not Exists pid="+pid); return true;
                }
                try
                {
                    Process prs = Process.GetProcessById(pid);
                    prs.Kill();
                } catch(ArgumentException)
                {
                    // プロセスが実行されていない場合は ArgumentException が発生する
                    // この場合は true とする
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".KillProcess");
                return false;
            }
        }
    }
}
