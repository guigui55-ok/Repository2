using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelWorkbookList
{
    public class UpdateControlAsync
    {
        ErrorManager.ErrorManager _err;
        Form _form;
        public Action UpdateControlAction;
        public UpdateControlAsync(ErrorManager.ErrorManager err, Form form)
        {
            _err = err;
            _form = form;
        }
        // 非同期でコントロールにアクセスするための delegate を実装する
        public delegate void DelegateUpdateControl();

        // 非同期でコントロールにアクセスするための Method を実装する
        private void UpdateControlBySubThreadMain()
        {
            try
            {
                _err.AddLog(this, "UpdateControlBySubThreadMain");
                if (_form.InvokeRequired)
                {
                    _err.AddLog("InvokeRequired=true");
                    _form.Invoke(new DelegateUpdateControl(this.UpdateControlAction));
                    return;
                }
                else
                {
                    _err.AddLog("InvokeRequired=false");
                }
                if (UpdateControlAction != null)
                {
                    UpdateControlAction();
                }
                else
                {
                    _err.AddLog(this, "UpdateControlAction == null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        // 実行メソッド
        public void ExcuteUpdateControlBySubThread()
        {
            _err.AddLog(this, "ExcuteUpdateControlBySubThread");
            UpdateControlBySubThreadMain();
        }

    }
}
