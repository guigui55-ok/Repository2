using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgressDialog
{
    public class MethodProcessConsoleSuspender
    {
        protected ErrorManager.ErrorManager _err;
        protected int timeOut = 5000;  
        public MethodProcessConsoleSuspender(ErrorManager.ErrorManager err)
        {
            _err = err;
        }

        public void TestLightMethod()
        {
            Console.WriteLine("TestLightMethod Excute");
            Console.WriteLine("TestLightMethod ThreadId=" + Thread.CurrentThread.ManagedThreadId);
        }

        public void TestHeavyMethod()
        {
            try
            {
                Stopwatch sw = new Stopwatch();

                _err.AddLog("TestHeavyMethod");
                sw.Start();
                bool flag = false;
                _err.AddLog("TestHeavyMethod ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                while (true)
                {
                    if (sw.Elapsed.Seconds % 2 == 0)
                    {
                        if (!flag)
                        {
                            Console.Write(".");
                            flag = true;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                _err.AddLogWarning(this, "TestHeavyMethod ThreadAbortException : thread="+Thread.CurrentThread.ManagedThreadId);
                _err.AddException(ex, this, " ThreadAbortException");
            }
            catch (Exception ex)
            {
                _err.AddException(ex,this, "TestHeavyMethod");
            }
        }

        public void ExcuteMethodEndsWithTimeout(Action action,bool waitEndThreadAlive = false)
        {
            string method = "ExcuteMethodEndsWithTimeout";
            Thread subThread = null;
            try
            {
                _err.AddLog(this, method +" : threadId=" + Thread.CurrentThread.ManagedThreadId);

                // 非同期処理をCancelするためのTokenを取得.
                var tokenSource = new CancellationTokenSource();
                var cancelToken = tokenSource.Token;

                Task ret = Task.Run(() =>
                {
                    try
                    {
                        subThread = Thread.CurrentThread;
                        _err.AddLog(this,method +".Task ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                        action();
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine("OperationCanceledException");
                        Console.WriteLine(ex.Message);
                    }
                    catch (ThreadAbortException ex)
                    {
                        _err.AddLogWarning(this, method + ".Task ThreadAbortException : thread=" + Thread.CurrentThread.ManagedThreadId);
                        _err.AddException(ex,this, " ThreadAbortException");
                    }
                    catch (Exception ex)
                    {
                        _err.AddException(ex,this,method +".Task.Run");
                    }
                    finally
                    {
                    }
                }, tokenSource.Token);

                // タスクが終了するまで待機する 5秒
                ret.Wait(timeOut);
                if (ret.IsCompleted)
                {
                    // Task が終了していたら OK
                    _err.AddLog(this,method +".Task End.");
                    subThread.Abort();
                    
                    subThread = null;
                }
                else
                {
                    // タスクをキャンセルする
                    _err.AddLog(this,method +".Task Cancel.");
                    tokenSource.Cancel();
                }
                if (subThread != null)
                {
                    // ExitThread
                    subThread.Abort();
                    if (waitEndThreadAlive)
                    {
                        _err.AddLog(this, method + " Waiting SubThread Abort");
                        while (subThread.IsAlive) { }
                    }
                    subThread = null;
                    _err.AddLog(this,method +".subThread End.");
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, ex.Source);
                subThread = null;
            }
        }


        public void ContinueToExcuteMethodUntilAnyPressKey(Action action, bool waitEndThreadAlive = false)
        {
            string method = "ContinueToExcuteMethodUntilAnyPressKey";
            Thread subThread = null;
            try
            {
                _err.AddLog(this, method + " : threadId=" + Thread.CurrentThread.ManagedThreadId);
                // 非同期処理をCancelするためのTokenを取得.
                var tokenSource = new CancellationTokenSource();
                var cancelToken = tokenSource.Token;

                Task ret = Task.Run(() =>
                {
                    try
                    {
                        subThread = Thread.CurrentThread;
                        _err.AddLog(this, method + "TaskThread CurrentThread=" + Thread.CurrentThread.ManagedThreadId);
                        action();
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine("OperationCanceledException");
                        Console.WriteLine(ex.Message);
                    }
                    catch (ThreadAbortException)
                    {
                        _err.AddLogWarning(this, method + ".Task ThreadAbortException : thread=" + Thread.CurrentThread.ManagedThreadId);
                    }
                    catch (Exception ex)
                    {
                        _err.AddException(ex, this, method +".Task.Run");
                    }
                    finally
                    {
                    }
                }, tokenSource.Token);

                ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();

                Console.WriteLine("Continue Excuting Method Untill Any Press Enter Key.");
                while (keyInfo.Key != ConsoleKey.Enter)
                {
                    keyInfo = Console.ReadKey();
                }
                if (ret.IsCompleted)
                {
                    // Task が終了していたら OK
                    _err.AddLog(this, method + ".Task End.");
                }
                else
                {
                    // タスクをキャンセルする
                    _err.AddLog(this, method + ".Task Cancel.");
                    tokenSource.Cancel();
                }
                if (subThread != null)
                {
                    // ExitThread
                    subThread.Abort();
                    if (waitEndThreadAlive)
                    {
                        _err.AddLog(this,method + " Waiting SubThread Abort");
                        subThread.Join();
                        while (subThread.IsAlive) { }
                    }
                    subThread = null;
                    _err.AddLog(this, method + " Task End. subThreadEnd");
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, ex.Source);
                subThread = null;
            }
        }
    }
}
