using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragAndDropSample
{
    public class DragAndDropOnControl
    {
        protected ErrorManager.ErrorManager _err;
        protected Control _control;
        public EventHandler DragAndDropAfterEvent;
        public DragAndDropOnControl(ErrorManager.ErrorManager err, Control recieveEventControl)
        {
            _err = err;
            _control = recieveEventControl;
            _control.DragDrop += Control_DragDrop;
            _control.DragEnter += Control_DragEnter;
        }

        public void AddRecieveControls(Control[] controls)
        {
            try
            {
                _err.AddLog(this, "AddRecieveControls");
                if(controls == null) { _err.AddLogAlert("  controls == null"); return; }
                if (controls.Length < 1) { _err.AddLogAlert("  controls.Length < 1"); return; }

                foreach(Control con in controls)
                {
                    con.AllowDrop = true;
                    con.DragEnter += Control_DragEnter;
                    con.DragDrop += Control_DragDrop;
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "AddRecieveControls");
            }
        }

        public void AddRecieveControl(Control control)
        {
            try
            {
                _err.AddLog(this, "AddRecieveControls");
                control.AllowDrop = true;
                control.DragEnter += Control_DragEnter;
                control.DragDrop += Control_DragDrop;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "AddRecieveControl");
            }
        }

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            _err.AddLog(this, "Control_DragEnter");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                _err.AddLog(this, "Control_DragDrop");
                // 受け取った EventArgs はほかのクラスで処理する
                DragAndDropAfterEvent?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "Control_DragDrop");
            }
            finally
            {
                _err.AddLog(this, "Control_DragDrop Finally");
            }
        }

    }
}
