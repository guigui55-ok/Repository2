using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppLoggerModule;

namespace CommonUtility.Async
{
    public class DoMethodAsync
    {
        public AppLogger _logger;
        protected ErrorManager.ErrorManager _err;
        public int Timeout = 60 * 1000;
        public bool IsCancelled = false;

        public object sender;
        public EventArgs e;
        public DoMethodAsync(AppLogger logger, ErrorManager.ErrorManager err)
        {
            _logger = logger;
            _err = err;
        }

        //public void DoMethodAsyncExcute(Action<object,EventArgs> action)
        //{

        //}

        public void DoMethodAsyncExcute(Action<object , EventArgs> action)
        {

            string method = "DoMethodAsyncExcute";
            IsCancelled = false;
            Thread subThread = null;
            bool waitEndThreadAlive = false;
            try
            {
                _logger.AddLog(this, method + " : threadId=" + Thread.CurrentThread.ManagedThreadId);
                Action<object, EventArgs> ac = action;
                if (ac == null) { throw new Exception(" DoWorkAction is Null"); }
                _logger.AddLog(this, "  this.Timeout =" + Timeout);

                // 非同期処理をCancelするためのTokenを取得.
                var tokenSource = new CancellationTokenSource();
                var cancelToken = tokenSource.Token;

                Task task = Task.Run(() =>
                {
                    try
                    {
                        subThread = Thread.CurrentThread;
                        _logger.AddLog(this, method + ".Task ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                        // ※action はここで実行される
                        ac(this.sender,this.e);
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine("OperationCanceledException");
                        Console.WriteLine(ex.Message);
                    }
                    catch (ThreadAbortException ex)
                    {
                        _logger.AddLogWarning(this, method + ".Task ThreadAbortException : thread=" + Thread.CurrentThread.ManagedThreadId, ex);
                        //_logger.AddException(ex, this, " ThreadAbortException");
                    }
                    catch (Exception ex)
                    {
                        _logger.AddException(ex, this, method + ".Task.Run");
                    }
                    finally
                    {
                    }
                }, tokenSource.Token);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                //bool IsCancelled = false;
                //int n = 0;
                //string dot = "";
                bool isPass3sec = false;
                bool isPass1sec = false;
                // Task が終了するまで待つ
                while (task.IsCompleted == false)
                {
                    if (Timeout > 0)
                    {
                        // Timeout 経過でもキャンセルする
                        if (sw.ElapsedMilliseconds == (Timeout))
                        {
                            _logger.AddLog(this, method + ".Task Timeout");
                            IsCancelled = true;
                            break;
                        }
                    }
                    // TimeUntilShowForm ミリ秒(初期値 3 秒)経過後に処理が終了していない場合、Form を表示する
                    if (sw.ElapsedMilliseconds == 3000)
                    {
                        if (!isPass3sec)
                        {
                            _logger.AddLog(this, method + ".Task Pass " + 3000 + " mSec");
                            isPass3sec = true;
                        }
                    }
                    // form の情報を更新する
                    // 1 秒ごとに更新する
                    if (sw.ElapsedMilliseconds % 1000 == 0)
                    {
                        if (!isPass1sec)
                        {
                            // 1秒ごとに実行される
                            isPass1sec = true;
                        }
                    }
                    else { isPass1sec = false; }

                    if (task.IsCanceled)
                    {
                        //キャンセルされたとき
                        IsCancelled = true;
                        break;
                    }
                }

                sw.Stop();
                // Task が終了した
                if (task.IsCompleted)
                {
                    // Task が終了していたら OK
                    _logger.AddLog(this, method + ".Task End.");
                }
                else
                {
                    // タスクをキャンセルする
                    _logger.AddLog(this, method + ".tokenSource.Cance. task.IsCompleted=" + task.IsCompleted);
                    tokenSource.Cancel();
                }
                if (subThread != null)
                {
                    // ExitThread
                    subThread.Abort();
                    if (waitEndThreadAlive)
                    {
                        _logger.AddLog(this, method + " Waiting SubThread Abort");
                        subThread.Join();
                        while (subThread.IsAlive) { }
                    }
                    subThread = null;
                    _logger.AddLog(this, method + " Task End. SubThread End");
                }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, method);
                subThread = null;
            }
        }
    }
}
