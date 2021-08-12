using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace ProgressDialog.FormWindow
{
    public class ProgressDialogManager
    {
        protected ErrorManager.ErrorManager _err;
        public ProgressDialogForm _processingDialog;
        public Form ParentForm;
        public Form ProcessingDialog { get => _processingDialog; set => _processingDialog = (ProgressDialogForm)value; }
        public string FormText = "Now Processing...";
        public ProgressDialogManager(ErrorManager.ErrorManager err,Form parentForm)
        {
            _err = err;
            if(parentForm != null)
            {
                this.ParentForm = parentForm;
            }

            
        }
        protected int _timeout = 30 * 1000;
        public int Timeout { get => _timeout; set => _timeout = value; }
        public void Initialize(DoWorkEventHandler doWorkEventHandler,int timeout = 30 * 1000)
        {
            try
            {
                _processingDialog = new ProgressDialogForm(_err, doWorkEventHandler,timeout,false);
                _processingDialog.SizeChanged += _processingDialog_SizeChanged;
                if(this.ParentForm != null)
                {
                    this.ParentForm.SizeChanged += ParentForm_SizeChanged;
                }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "Initialize");
            }
        }

        private void ParentForm_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (ParentForm.WindowState == FormWindowState.Normal)
                {
                    _processingDialog.WindowState = FormWindowState.Normal;
                }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "ParentForm_SizeChanged");
            }
        }

        private void _processingDialog_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.ParentForm != null)
                {
                    if(_processingDialog.WindowState == FormWindowState.Minimized)
                    {
                        ParentForm.WindowState = FormWindowState.Minimized;
                    } else if (_processingDialog.WindowState == FormWindowState.Normal)
                    {
                        ParentForm.WindowState = FormWindowState.Normal;
                    }
                }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "_processingDialog_MinimumSizeChanged");
            }
        }

        public DialogResult ExcuteProcess(Action action,ref BackgroundWorker backgroundWorker,int timeout=30 * 1000,int showDialogTime = 3000)
        {
            Test.ProgressDialogDoWork doWork = null;
            try
            {
                // メイン処理を行うクラス
                doWork = new Test.ProgressDialogDoWork(_err);
                doWork.DoWorkAction = action;
                // Initialize
                Initialize(doWork.ProgressDialog_DoWorkFromAction,timeout);
                if (_err.hasError) { _err.AddLogAlert("ExcuteProcess.Initialize Failed"); }

                doWork.BackgroundWorker = this._processingDialog.BackgroundWorker;
                backgroundWorker = this._processingDialog.BackgroundWorker;

                return RunProcess(showDialogTime);

            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "RunProcess(Action action)");
                return DialogResult.None;
            }
            finally
            {
                if (doWork != null) { doWork.Dispose(); }
                if (_processingDialog != null) {
                    if (!_processingDialog.BackgroundWorker.CancellationPending)
                    {
                        _processingDialog.BackgroundWorker.CancelAsync();
                    }
                    _processingDialog.Dispose();
                    _processingDialog = null;
                }
                GC.Collect();
            }
        }

        public DialogResult ExcuteProcess(DoWorkEventHandler doWorkEventHandler)
        {
            try
            {
                Initialize(doWorkEventHandler);
                if (_err.hasError) { _err.AddLogAlert("ExcuteProcess.Initialize Failed"); }

                return RunProcess();

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "RunProcess(Action action)");
                return DialogResult.None;
            }
        }

        public DialogResult RunProcess(int showDialogTime = 0)
        {
            try
            {
                if (showDialogTime <= 0)
                {
                    return ShowDialog();
                } else
                {
                    _processingDialog.BackGroundWorker_RunWorkerAsync();
                    return _processingDialog.DialogResult;
                }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "RunProcess(Action action)");
                return DialogResult.None;
            }
        }

        public DialogResult ShowDialog()
        {
            try
            {
                _err.AddLog(this, "ShowDialog ThreadId="+Thread.CurrentThread.ManagedThreadId);
                if(_processingDialog == null) { _err.AddLogAlert("_processingDialog == null"); return DialogResult.None; }

                DialogResult result = _processingDialog.ShowDialog();
                if(result == DialogResult.Abort)
                {
                    _err.AddLogAlert(this, "ShowDialog : _processingDialog.ShowDialog DialogResult.Abort");
                } else if(result == DialogResult.OK)
                {
                    _err.AddLog(this, "ShowDialog : _processingDialog.ShowDialog DialogResult.OK");

                    //結果を取得する
                    int stopTime = (int)_processingDialog.Result;
                    _err.AddLog(this, "ShowDialog : _processingDialog.ShowDialog Success! stopTime="+stopTime);
                } else
                {
                    //if(result == null) { result = DialogResult.None; }
                    _err.AddLog(this, "ShowDialog : _processingDialog.ShowDialog DialogResult="+result);

                    //結果を取得する
                    int stopTime = (int)_processingDialog.Result;
                    _err.AddLog(this, "ShowDialog : _processingDialog.ShowDialog Success! stopTime=" + stopTime);
                }
                return result;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "ShowDialog");
                return DialogResult.None;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_processingDialog != null)
                {
                    _processingDialog.Dispose();
                    _processingDialog = null;
                }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "Dispose");
            }
        }

    }
}
