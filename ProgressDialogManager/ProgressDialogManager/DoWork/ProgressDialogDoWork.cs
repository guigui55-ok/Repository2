using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgressDialog.DoWork
{
    public class ProgressDialogDoWork
    {
        protected ErrorManager.ErrorManager _err;
        public DoWorkEventHandler DoWorkEvent;
        public Action DoWorkAction;
        public int Timeout = 100 * 1000;
        public int TimeUntilShowForm = 3000;
        public BackgroundWorker BackgroundWorker;
        public string Message = "Now Processing";
        public string WindowTitle = "";
        protected object _result;
        public bool IsCancelled = false;
        public ProgressDialogDoWork(ErrorManager.ErrorManager err)
        {
            _err = err;
            DoWorkEvent += ProgressDialog_DoWorkWithExcuteActionEvent;
        }
        public ProgressDialogDoWork(ErrorManager.ErrorManager err, BackgroundWorker backgroundWorker, Action action)
        {
            _err = err;
            this.BackgroundWorker = backgroundWorker;
            this.DoWorkAction = action;
            DoWorkEvent += ProgressDialog_DoWorkWithExcuteActionEvent;
        }
        public ProgressDialogDoWork(ErrorManager.ErrorManager err, 
            BackgroundWorker backgroundWorker,DoWorkEventHandler doWorkMethod, Action action)
        {
            _err = err;
            this.BackgroundWorker = backgroundWorker;
            this.DoWorkAction = action;
            DoWorkEvent += doWorkMethod;
        }


        // BackGroundWorker クラスに進捗を報告する
        public void SetBackGroundWorker_ReportProgress(int percentProgress,object userState)
        {
            try
            {
                if (this.BackgroundWorker == null)
                {
                    _err.AddLog(this, ".SetBackGroundWorker_ReportProgress:BackgroundWorker == null");
                } else
                {
                    //結果を設定する
                    _result = percentProgress;
                    this.BackgroundWorker.ReportProgress(percentProgress, userState);
                }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "SetBackGroundWorker_ReportProgress");
            }
        }

        public void DoWorkAction_SampleHeavyMethodNotHasBackGroundWorker()
        {
            string method = "DoWorkAction_SampleHeavyMethodNotHasBackGroundWorker";
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                _err.AddLog(method + " ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                bool isPass2sec = false;
                while (true)
                {
                    // 負荷のかかる処理
                    if (sw.Elapsed.Seconds % 2000 == 0)
                    {
                        if (!isPass2sec) { Console.Write("."); isPass2sec = true; }
                    } else { isPass2sec = false; }
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, method);
            }
            finally
            {
                sw.Reset();
                _err.AddLog(method + " finally");
            }
        }
        // TestActionMethod
        // 外部の Action を実装時の見本
        public void DoWorkAction_SampleHeavyMethod()
        {
            string method = "DoWorkAction_SampleHeavyMethod";
            Stopwatch sw = new Stopwatch();
            try
            {
                // ※BackgroundWorker は取得済みであること
                BackgroundWorker bw = this.BackgroundWorker;

                sw.Start();
                int n = 0;
                int m = 0;
                string dot = "";
                string msg = this.Message;
                bool flag = false;
                _err.AddLog(method + " ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                while (true)
                {
                    if (sw.Elapsed.Seconds % 2 == 0)
                    {
                        // メイン処理
                        if (!flag)
                        {
                            // ProgressBar 更新用
                            n++;
                            if (n > 100) { n = 0; }
                            // 文字列に追加する用
                            m++;
                            if (m > 5) { m = 0; }
                            if (m > 0) { for (int i = 0; i < m; i++) { dot += "."; } } else { dot = ""; }

                            Console.Write(".");
                            // コントロールの表示を変更する
                            // ※進捗を表示する処理を追加すること
                            SetBackGroundWorker_ReportProgress(n , msg + dot);
                            flag = true;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, method);
            }
            finally
            {
                sw.Reset();
                _err.AddLog(method + " finally");
            }
        }

        // TestActionMethod3
        // 外部の Action を実装時の見本
        // BackGroundWorker を持たない Action メソッドをつかうとき
        public void ProgressDialog_DoWorkWithExcuteActionForNotHasBackGroundWorkerEvent(object sender, DoWorkEventArgs e)
        {

            string method = "ProgressDialog_DoWorkWithExcuteActionForNotHasBackGroundWorkerEvent";
            IsCancelled = false;
            Thread subThread = null;
            bool waitEndThreadAlive = false;
            try
            {
                _err.AddLog(this, method + " : threadId=" + Thread.CurrentThread.ManagedThreadId);

                // ※sender から BackgroundWorker を受け取っておくこと
                BackgroundWorker bw = (BackgroundWorker)sender;
                Action ac = this.DoWorkAction;

                if (ac == null) { throw new Exception(" DoWorkAction is Null"); }

                this.Timeout = (int)e.Argument;
                _err.AddLog(this, "  this.Timeout = (int)e.Argument = " + Timeout);

                // 非同期処理をCancelするためのTokenを取得.
                var tokenSource = new CancellationTokenSource();
                var cancelToken = tokenSource.Token;

                Task task = Task.Run(() =>
                {
                    try
                    {
                        subThread = Thread.CurrentThread;
                        _err.AddLog(this, method + ".Task ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                        // ※action はここで実行される
                        ac();
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine("OperationCanceledException");
                        Console.WriteLine(ex.Message);
                    }
                    catch (ThreadAbortException ex)
                    {
                        _err.AddLogWarning(this.ToString(), method + ".Task ThreadAbortException : thread=" + Thread.CurrentThread.ManagedThreadId,ex);
                        //_err.AddException(ex, this, " ThreadAbortException");
                    }
                    catch (Exception ex)
                    {
                        _err.AddException(ex, this, method + ".Task.Run");
                    }
                    finally
                    {
                    }
                }, tokenSource.Token);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                bool IsCancelled = false;
                int n = 0;
                string dot = "";
                bool isPass3sec = false;
                bool isPass1sec = false;
                // Task が終了するまで待つ
                while (task.IsCompleted == false)
                {
                    //キャンセルされたか調べる
                    if (bw.CancellationPending)
                    {
                        IsCancelled = true;
                        break;
                    }
                    if (Timeout > 0)
                    {
                        // Timeout 経過でもキャンセルする
                        if (sw.ElapsedMilliseconds == (Timeout))
                        {
                            _err.AddLog(this, method + ".Task Timeout");
                            IsCancelled = true;
                            break;
                        }
                    }
                    // TimeUntilShowForm ミリ秒(初期値 3 秒)経過後に処理が終了していない場合、Form を表示する
                    if (sw.ElapsedMilliseconds == (TimeUntilShowForm))
                    {
                        if (!isPass3sec)
                        {
                            _err.AddLog(this, method + ".Task Pass " + TimeUntilShowForm + " mSec");
                            isPass3sec = true;
                        }
                    }
                    // form の情報を更新する
                    // 1 秒ごとに更新する
                    if (sw.ElapsedMilliseconds % 1000 == 0)
                    {
                        if (!isPass1sec)
                        {
                            //_err.AddLog("  sw.Elapsed.Seconds % 1000 == 0 : n="+n);
                            if (n > 100) { n = 0; } else { n++; }
                            if (dot.Length <= 6) { dot += "."; } else { dot = ""; }
                            SetBackGroundWorker_ReportProgress(n, this.Message + dot);
                            isPass1sec = true;
                        }
                    } else { isPass1sec = false; }
                }
                //結果を設定する
                e.Result = sw.ElapsedMilliseconds;

                sw.Stop();
                if (IsCancelled)
                {
                    //キャンセルされたとき
                    e.Cancel = true;
                    IsCancelled = true;
                }
                // Task が終了した
                if (task.IsCompleted)
                {
                    // Task が終了していたら OK
                    _err.AddLog(this, method + ".Task End.");
                }
                else
                {
                    // タスクをキャンセルする
                    _err.AddLog(this, method + ".tokenSource.Cance. task.IsCompleted=" + task.IsCompleted);
                    tokenSource.Cancel();
                }
                if (subThread != null)
                {
                    // ExitThread
                    subThread.Abort();
                    if (waitEndThreadAlive)
                    {
                        _err.AddLog(this, method + " Waiting SubThread Abort");
                        subThread.Join();
                        while (subThread.IsAlive) { }
                    }
                    subThread = null;
                    _err.AddLog(this, method + " Task End. SubThread End");
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, method);
                subThread = null;
                //結果を設定する
                e.Result = -1;
            }
        }

        // DoWorkEventMethod2
        // この関数を EventHandler に渡す
        // DoWorkEvent を発生時に Action から実行するパターン
        public void ProgressDialog_DoWorkWithExcuteActionEvent(object sender, DoWorkEventArgs e)
        {

            string method = "ProgressDialog_DoWorkWithExcuteActionEvent";
            IsCancelled = false;
            Thread subThread = null;
            bool waitEndThreadAlive = false;
            try
            {
                _err.AddLog(this, method + " : threadId=" + Thread.CurrentThread.ManagedThreadId);

                // ※sender から BackgroundWorker を受け取っておくこと
                BackgroundWorker bw = (BackgroundWorker)sender;
                Action ac = this.DoWorkAction;

                if (ac == null) { throw new Exception(" DoWorkAction is Null"); }

                this.Timeout = (int)e.Argument;
                _err.AddLog(this, "  this.Timeout = (int)e.Argument = " + Timeout);

                // 非同期処理をCancelするためのTokenを取得.
                var tokenSource = new CancellationTokenSource();
                var cancelToken = tokenSource.Token;

                Task task = Task.Run(() =>
                {
                    try
                    {
                        subThread = Thread.CurrentThread;
                        _err.AddLog(this, method + ".Task ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                        // ※action はここで実行される
                        ac();
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine("OperationCanceledException");
                        Console.WriteLine(ex.Message);
                    }
                    catch (ThreadAbortException ex)
                    {
                        _err.AddLogWarning(this.ToString(), method + ".Task ThreadAbortException : thread=" + Thread.CurrentThread.ManagedThreadId,ex);
                        //_err.AddException(ex, this, " ThreadAbortException");
                    }
                    catch (Exception ex)
                    {
                        _err.AddException(ex, this, method + ".Task.Run");
                    }
                    finally
                    {
                    }
                }, tokenSource.Token);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                //bool IsCancelled = false;
                bool isPass3sec = false;
                // Task が終了するまで待つ
                while (task.IsCompleted == false)
                {
                    //キャンセルされたか調べる
                    if (bw.CancellationPending)
                    {
                        IsCancelled = true;
                        break;
                    }
                    if (Timeout > 0)
                    {
                        // Timeout 経過でもキャンセルする
                        if (sw.ElapsedMilliseconds == (Timeout))
                        {
                            _err.AddLog(this, method + ".Task Timeout");
                            IsCancelled = true;
                            break;
                        }
                    }
                    // TimeUntilShowForm ミリ秒(初期値 3 秒)経過後に処理が終了していない場合、Form を表示する
                    if (sw.ElapsedMilliseconds == (TimeUntilShowForm))
                    {
                        if (!isPass3sec)
                        {
                            _err.AddLog(this, method + ".Task Pass " + TimeUntilShowForm + " mSec");
                            isPass3sec = true;
                        }
                    }
                }
                //結果を設定する
                e.Result = sw.ElapsedMilliseconds;

                sw.Stop();
                if (IsCancelled)
                {
                    //キャンセルされたとき
                    e.Cancel = true;
                }
                // Task が終了した
                if (task.IsCompleted)
                {
                    // Task が終了していたら OK
                    _err.AddLog(this, method + ".Task End.");
                }
                else
                {
                    // タスクをキャンセルする
                    _err.AddLog(this, method + ".tokenSource.Cance. task.IsCompleted=" + task.IsCompleted);
                    tokenSource.Cancel();
                }
                if (subThread != null)
                {
                    // ExitThread
                    subThread.Abort();
                    if (waitEndThreadAlive)
                    {
                        _err.AddLog(this, method + " Waiting SubThread Abort");
                        subThread.Join();
                        while (subThread.IsAlive) { }
                    }
                    subThread = null;
                    _err.AddLog(this, method + " Task End. SubThread End");
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, method);
                subThread = null;
                //結果を設定する
                e.Result = -1;
            }
        }

        // DoworkEventMethod1
        // Action Method を使用せず、そのまま実装する場合
        public void ProgressDialog_DoWorkEvent(object sender, DoWorkEventArgs e)
        {
            string method = "ProgressDialog_DoWork1";
            IsCancelled = false;
            try
            {
                _err.AddLog(this, method + " : threadId=" + Thread.CurrentThread.ManagedThreadId);
                // ※sender から BackgroundWorkerを受け取っておくこと
                BackgroundWorker bw = (BackgroundWorker)sender;

                // ※パラメータを取得する
                int stopTime = (int)e.Argument;

                // 時間のかかる処理を開始する
                for (int i = 1; i <= 100; i++)
                {
                    // キャンセルされたか調べる
                    if (bw.CancellationPending)
                    {
                        _err.AddLog(this, method + " : BackgroundWorker.CancellationPending=" + bw.CancellationPending);
                        // キャンセルされたとき
                        e.Cancel = true;
                        IsCancelled = true;
                        break;
                    }

                    // 指定された時間待機する
                    System.Threading.Thread.Sleep(stopTime);

                    // ProgressChanged イベントハンドラを呼び出し、
                    // コントロールの表示を変更する
                    // ※進捗を表示する処理を追加すること
                    bw.ReportProgress(i, i.ToString() + "% 終了しました");
                }

                _err.AddLog(this, method + " : DoWorkEventArgs.Result=" + e.Result);
                // ※結果を設定する
                e.Result = stopTime * 100;
            }
            catch (Exception ex)
            {
                _err.AddException(ex,this, "ProgressDialog_DoWork");
            }
        }

        public void Dispose()
        {
            //protected ErrorManager.ErrorManager _err;
            //public DoWorkEventHandler DoWorkEvent;
            DoWorkEvent = null;
            //public Action DoWorkAction;
            DoWorkAction = null;
            //public int Timeout;
            //public BackgroundWorker BackgroundWorker;
            BackgroundWorker = null;
            //protected object _result;
            _result = null;
            GC.Collect();
        }
    }
}
