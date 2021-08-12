using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgressDialog.Test
{
    public class ProgressDialogDoWork
    {
        protected ErrorManager.ErrorManager _err;
        public DoWorkEventHandler DoWorkEvent;
        public Action DoWorkAction;
        public int Timeout = 100 * 1000;
        public int TimeUntilShowForm = 3000;
        public BackgroundWorker BackgroundWorker;
        protected object _result;
        public ProgressDialogDoWork(ErrorManager.ErrorManager err)
        {
            _err = err;
            DoWorkEvent += ProgressDialog_DoWork;
        }
        public ProgressDialogDoWork(ErrorManager.ErrorManager err, BackgroundWorker backgroundWorker, Action action)
        {
            _err = err;
            this.BackgroundWorker = backgroundWorker;
            this.DoWorkAction = action;
            DoWorkEvent += ProgressDialog_DoWorkFromAction;
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
                string msg = "Processing";
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

        //private class WorkerObjects
        //{
        //    public BackgroundWorker worker;
        //    public Action action;
        //}

        //private Action GetActionFromSender(object sender)
        //{
        //    try
        //    {
        //        if (sender.GetType().IsArray)
        //        {
        //            object[] ary = (object[])sender;
        //            if (ary.Length < 2)
        //            {
        //                _err.AddLogAlert(this, "GetActionFromSender : sender Length < 2");
        //            }
        //            if (!(ary[0].GetType() == typeof(Action)))
        //            {
        //                _err.AddLogAlert(this, "GetActionFromSender : sender Type is Invalid");
        //            }
        //            return (Action)ary[0];
        //        }
        //        else
        //        {
        //            _err.AddLogAlert(this, "GetActionFromSender : sender IsArray False");
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _err.AddException(ex, this, "GetBackgroundWorkerFromSender");
        //        return null;
        //    }
        //}
        //private BackgroundWorker GetBackgroundWorkerFromSender(object sender)
        //{
        //    try
        //    {
        //        if (sender.GetType().IsArray)
        //        {
        //            object[] ary = (object[])sender;
        //            if(ary.Length < 2)
        //            {
        //                _err.AddLogAlert(this, "GetBackgroundWorkerFromSender : sender Length < 2");
        //            }
        //            if (!(ary[0].GetType() == typeof(BackgroundWorker)))
        //            {
        //                _err.AddLogAlert(this, "GetBackgroundWorkerFromSender : sender Type is Invalid");
        //            }
        //            return (BackgroundWorker)ary[0];
        //        } else
        //        {
        //            _err.AddLogAlert(this, "GetBackgroundWorkerFromSender : sender IsArray False");
        //        }
        //        return null;
        //    } catch (Exception ex)
        //    {
        //        _err.AddException(ex, this, "GetBackgroundWorkerFromSender");
        //        return null;
        //    }
        //}

        // DoWorkEventMethod2
        // この関数を EventHandler に渡す
        // DoWorkEvent を発生時に Action から実行するパターン
        public void ProgressDialog_DoWorkFromAction(object sender, DoWorkEventArgs e)
        {

            string method = "ProgressDialog_DoWorkFromAction";
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
                        _err.AddLogWarning(this, method + ".Task ThreadAbortException : thread=" + Thread.CurrentThread.ManagedThreadId);
                        _err.AddException(ex, this, " ThreadAbortException");
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
                        _err.AddLog(this, method + ".Task Pass "+ TimeUntilShowForm + " mSec");
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
        public void ProgressDialog_DoWork(object sender, DoWorkEventArgs e)
        {
            string method = "ProgressDialog_DoWork1";
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
