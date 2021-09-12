using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlUtility
{
    public class DragAndDropForFile
    {
        protected ErrorManager.ErrorManager _err;
        public DragAndDropOnControl DragAndDropOnControl;
        public string[] Files;
        public EventHandler DragAndDropEventAfterEventForFile;
        public DragAndDropForFile(ErrorManager.ErrorManager err,DragAndDropOnControl dragAndDropOnControl)
        {
            _err = err;
            this.DragAndDropOnControl = dragAndDropOnControl;
            this.DragAndDropOnControl.DragAndDropAfterEvent += DragAndDropAfterEvent;
        }

        private void DragAndDropAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this, "DragAndDropAfterEvent");
                // DragDrop の e を配列へ
                Files = GetFilesByDragAndDrop((DragEventArgs)e);

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "DragAndDropAfterEvent");
            } finally
            {
                DragAndDropEventAfterEventForFile?.Invoke(sender, e);
            }
        }

        public string[] GetFilesByDragAndDrop(DragEventArgs e)
        {
            try
            {
                _err.AddLog(this, "GetFilesByDragAndDrop");
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
    }
}
