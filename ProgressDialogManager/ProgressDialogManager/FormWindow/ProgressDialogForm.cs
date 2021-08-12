using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
// https://dobon.net/vb/dotnet/programing/progressdialogbw.html

namespace ProgressDialog.FormWindow
{
    public partial class ProgressDialogForm : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        protected ErrorManager.ErrorManager _err;
        protected object _result = null;
        protected object workerArgument = null;
        protected int showDialogTime = 3000;
        protected bool IsDoingWork = false;
        public object Result { get { return this._result; } }
        public BackgroundWorker BackgroundWorker { get { return this.backgroundWorker1; } }

        public ProgressDialogForm(ErrorManager.ErrorManager err, DoWorkEventHandler doWorkHandler,object argument,bool isConsole=false)
        {
            _err = err;
            InitializeComponent();
            // コンソールで使うときは AllocConsole を実行する
            if (isConsole) { AllocConsole(); }
            this.workerArgument = argument;
            // ProgressBar Settings
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = 0;
            // BackGroundWorker Settings
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // EventSettings
            this.Button_Cancel.Click += Button_Cancel_Click1;
            this.backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            this.backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            this.backgroundWorker1.DoWork += doWorkHandler;
            this.Shown += ProcessingDialog_Shown;
        }
        // イベントハンドラを削除する
        public void ReleaseDoWorkEventHandler(DoWorkEventHandler doWorkHandler)
        {
            try
            {
                this.backgroundWorker1.DoWork -= doWorkHandler;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "ReleaseDoWorkEventHandler");
            }
        }

        // フォームが表示されたときにバックグラウンド処理を開始
        private void ProcessingDialog_Shown(object sender, EventArgs e)
        {
            if (!IsDoingWork)
            {
                this.backgroundWorker1.RunWorkerAsync(this.workerArgument);
            }
        }

        // Action を実行するが、Dialog は showDialogtime ミリ秒後経過まで表示しない
        // それ以上処理が続く場合 ProgressDialog を表示する
        public void BackGroundWorker_RunWorkerAsync(int showDialogTime = 3000)
        {
            try
            {
                IsDoingWork = true;
                _result = null;
                this.backgroundWorker1.RunWorkerAsync(this.workerArgument);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while(_result == null)
                {
                    if(sw.ElapsedMilliseconds >= (showDialogTime))
                    {
                        this.ShowDialog();
                        break;
                    }
                }
                sw.Stop();
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "BackGroundWorker_RunWorkerAsync");
                this.Close();
            }
        }

        // DoWorker の処理が終了したときに実行される
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IsDoingWork = false;
                _err.AddLog(this, "BackgroundWorker1_RunWorkerCompleted");
                if (e.Error != null)
                {
                    _err.AddException(e.Error, this, "BackgroundWorker1_RunWorkerCompleted : RunWorkerCompletedEventArgs.Error");
                    this.DialogResult = DialogResult.Abort;
                }
                else if (e.Cancelled)
                {
                    _err.AddLog(this, "BackgroundWorker1_RunWorkerCompleted : RunWorkerCompletedEventArgs : e.Cancelled");
                    this.DialogResult = DialogResult.Cancel;
                }
                else
                {
                    _err.AddLog(this, "BackgroundWorker1_RunWorkerCompleted : RunWorkerCompletedEventArgs : DialogResult.OK");
                    this.DialogResult = DialogResult.OK;
                }

                _result = progressBar1.Value;
                                
                this.Close();
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "BackgroundWorker1_RunWorkerCompleted");
            }
        }

        // ReportProgress メソッドが呼び出されたとき
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                this._result = sender;
                //プログレスバーの値を変更する
                if (e.ProgressPercentage < this.progressBar1.Minimum)
                {
                    this.progressBar1.Value = this.progressBar1.Minimum;
                }
                else if (this.progressBar1.Maximum < e.ProgressPercentage)
                {
                    this.progressBar1.Value = this.progressBar1.Maximum;
                }
                else
                {
                    this.progressBar1.Value = e.ProgressPercentage;
                }
                // メッセージのテキストを変更する
                this.ProcessDialogLabel1.Text = (string)e.UserState;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "BackgroundWorker1_ProgressChanged");
            }
        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void Button_Cancel_Click1(object sender, EventArgs e)
        {
            try
            {
                this.Button_Cancel.Enabled = false;
                this.backgroundWorker1.CancelAsync();
                // 次実行したときにボタンを押下できないので true にする
                this.Button_Cancel.Enabled = true;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "Button_Cancel_Click1");
            }
        }

        private void ProcessingDialog_Load(object sender, EventArgs e)
        {

        }

    }
}
