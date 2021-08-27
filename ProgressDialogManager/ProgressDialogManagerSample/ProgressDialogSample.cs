using ErrorManager;
using ProgressDialog.FormWindow;
using ProgressDialog.DoWork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgressDialog
{
    public partial class ProgressDialogSample : Form
    {
        protected ErrorManager.ErrorManager _err;
        protected ProgressDialogManager _progressDialog;
        protected ProgressDialogDoWork _doWork;
        public ProgressDialogSample()
        {
            InitializeComponent();
            this.FormClosed += ProgressDialogSample_FormClosed;

            _err = new ErrorManager.ErrorManager(1);
            _progressDialog = new ProgressDialogManager(_err,this);
            // メイン処理を行うクラス
            _doWork = new ProgressDialogDoWork(_err);
            // DoWorkEventHandler を フォームに紐づける 
            // Initialize 内 -> doWork.DoWorkEvent += doWork.ProgressDialog_DoWork
            // this.backgroundWorker1.DoWork += doWorkHandler;
            //_progressDialog.Initialize(_doWork.DoWorkEvent);

            //_doWork = new ProgressDialogDoWork(_err, _progressDialog.f)
        }

        private void ProgressDialogSample_FormClosed(object sender, FormClosedEventArgs e)
        {
            _progressDialog.Dispose();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 処理を開始する
                // 進行状況ダイアログを表示する
                // 結果を取得、ログへ出力
                // フォームが表示されたときにバックグラウンド処理を開始
                //this.backgroundWorker1.RunWorkerAsync(this.workerArgument); <= Dialog.Shown Event
                //DialogResult result = _progressDialog.ShowDialog();
                // 実行する
                // ActionMethod を使用せずにクラス内で実装するパターン
                _progressDialog.ExcuteProcess(_doWork.DoWorkEvent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
            finally
            {
                // DoWorkEventHandler を解除する
                _progressDialog._processingDialog.ReleaseDoWorkEventHandler(_doWork.DoWorkEvent);
            }
        }

        private void MethodProcessSuspenderSample_Load(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            // 実行する
            // ActionMethod を使用するパターン
            // BackGroundWorker を ActionMethod ないで更新・通知しているパターン
            //_doWork.DoWorkAction = _doWork.DoWorkAction_SampleHeavyMethod;
            //_doWork.DoWorkEvent = _doWork.ProgressDialog_DoWorkFromAction;
            //_progressDialog.ExcuteProcess(_doWork.ProgressDialog_DoWorkFromAction);
            _progressDialog.ExcuteProcess(_doWork.DoWorkAction_SampleHeavyMethod, ref _doWork.BackgroundWorker, 0,3000);
            //_progressDialog.RunProcess(3000);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // ActionMethod を使用する
            // BackGroundWorker は使用しないパターン

            // doWork.DoWorkAction = _doWork.DoWorkAction_SampleHeavyMethodNotHasBackGroundWorker
            // this.backgroundWorker1.DoWork += _doWork.DoWorkAction_SampleHeavyMethodNotHasBackGroundWorker;
            _doWork.DoWorkEvent = _doWork.ProgressDialog_DoWorkWithExcuteActionForNotHasBackGroundWorkerEvent;
            _progressDialog.ExcuteProcessForActionForNotHasBackGroundWorker(
                _doWork.DoWorkAction_SampleHeavyMethodNotHasBackGroundWorker, ref _doWork.BackgroundWorker, 0, 3000);
        }
    }
}
