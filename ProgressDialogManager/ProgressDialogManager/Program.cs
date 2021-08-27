using ProgressDialog.FormWindow;
using ProgressDialog.DoWork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgressDialog
{
    class Program
    {
        static void Main()
        {
            try
            {
                ErrorManager.ErrorManager _err = new ErrorManager.ErrorManager(1);
                MethodProcessConsoleSuspender suspender = new MethodProcessConsoleSuspender(_err);

                //Console.WriteLine("TestLightMethod");
                //suspender.ExcuteMethodEndsWithTimeout(suspender.TestLightMethod);
                //Console.WriteLine("\n -----\nTestHeavyMethod");
                //suspender.ExcuteMethodEndsWithTimeout(suspender.TestHeavyMethod);

                //Console.WriteLine("\n -----\nContinueToExcuteMethodUntilAnyPressKey");
                //suspender.ContinueToExcuteMethodUntilAnyPressKey(suspender.TestHeavyMethod,true);

                Console.WriteLine("ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("\n -----\n");
                ProgressDialogManager dialog = new ProgressDialogManager(_err,null);

                // DoWork 実行クラス
                ProgressDialogDoWork doWork = new ProgressDialogDoWork(_err);
                // DoWorkEvent が実行メソッド
                // DoWorkEventHandler を フォームに紐づける 
                // Initialize 内 -> doWork.DoWorkEvent += doWork.ProgressDialog_DoWork
                // this.backgroundWorker1.DoWork += doWorkHandler;
                dialog.Initialize(doWork.DoWorkEvent);
                //進行状況ダイアログを表示する
                //結果を取得、ログへ出力
                //フォームが表示されたときにバックグラウンド処理を開始
                //this.backgroundWorker1.RunWorkerAsync(this.workerArgument); <= Dialog.Shown Event
                DialogResult result = dialog.ShowDialog();
                // DoWorkEventHandler を解除する
                dialog._processingDialog.ReleaseDoWorkEventHandler(doWork.DoWorkEvent);

                Console.WriteLine("\n -----\n");
                // DoWork Class Settings
                // DoWork に Action を紐づける
                doWork.DoWorkAction = doWork.DoWorkAction_SampleHeavyMethod;
                //doWork.DoWorkEvent -= doWork.ProgressDialog_DoWork; // 以前のDoWorkEventがある場合は注意する
                doWork.DoWorkEvent = doWork.ProgressDialog_DoWorkWithExcuteActionEvent;
                // BackgroundWorker を紐づける
                doWork.BackgroundWorker = dialog._processingDialog.BackgroundWorker;

                // ProcessDialogManager Class Settings
                // DoWorkEventHandler を フォームに紐づける
                dialog._processingDialog.BackgroundWorker.DoWork += doWork.DoWorkEvent;
                // 実行する
                dialog.RunProcess();

                //後始末
                dialog.Dispose();

                //doWork.DoWorkEvent?.Invoke()

            } catch (Exception ex)
            {
                Console.WriteLine("RiseException Program.Main");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Program End. Any Press Key.");
                Console.ReadKey();
            }
        }


    }
}
