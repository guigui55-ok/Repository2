using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonUtilitySample
{
    public class DragAndDrop
    {
        protected ErrorManager.ErrorManager _err;
        protected Control _control;
        public object Sender;
        public DragEventArgs DragEventArgs;
        public object DragDropObject;
        public DragEventHandler DragDropAfterEvent;
        public DragAndDrop(ErrorManager.ErrorManager err,Control control)
        {
            _err = err;
            _control = control;
            if(_control != null)
            {
                _control.AllowDrop = true;
                _control.DragEnter += Control_DragEnter;
                _control.DragDrop += Control_DragDrop;
            }
        }

        public void AddControlsDragEvent(Control[] controls)
        {
            try
            {
                if (controls == null) { return; }
                if (controls.Length < 1) { return; }
                foreach (Control control in controls)
                {
                    control.AllowDrop = true;
                    // FileDragDrop イベントを紐づけする
                    control.DragDrop += Control_DragDrop;
                    control.DragEnter += Control_DragEnter;
                }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "AddControlsDragEvent");
            }
        }

        public string[] getFilesByDragAndDrop(DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // ドラッグ中のファイルやディレクトリの取得
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    return files;
                }
                else
                {
                    _err.AddLogWarning(this, "GetDataPresent :e.Data.GetDataPresent(DataFormats.FileDrop)=false");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "GetFilesByDragAndDrop");
                return null;
            }
        }

        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                _err.AddLog(this, "Control_DragDrop");
                this.Sender = sender;
                this.DragEventArgs = e;
                // ほかのメソッドで受け取るときはこのイベントを介する
                DragDropAfterEvent?.Invoke(sender, e);
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

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            _err.AddLog(this, "Control_DragEnter");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
    }
}
