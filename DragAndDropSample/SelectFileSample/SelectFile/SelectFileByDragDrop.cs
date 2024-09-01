using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace CotnrolUtility.SelectFiles
{
    public class SelectFileByDragDrop
    {
        public AppLogger _logger;
        protected Control _control;
        public EventHandler RecieveFileAfterDragDropEvent;
        public bool IsNowProcessing = false;
        public SelectFileByDragDrop(AppLogger logger, Control recieveEventControl)
        {
            this._logger = logger;
            _control = recieveEventControl;
            _control.DragDrop += Control_DragDrop;
            _control.DragEnter += Control_DragEnter;
        }

        public void AddRecieveControl(Control control)
        {
            try
            {
                control.DragEnter += Control_DragEnter;
                control.DragDrop += Control_DragDrop;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "AddRecieveControl");
            }
        }

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            _logger.AddLog(this, "Control_DragEnter");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                _logger.AddLog(this, "Control_DragDrop Begin , IsNowProcessing=true");
                _logger.AddLog("  Thread=" + Thread.CurrentThread.ManagedThreadId);
                IsNowProcessing = true;
                // DragDrop の e を配列へ
                string[] files = GetFilesByDragAndDrop(e);
                // ほかのメソッドで受け取るときはこのイベントを介する
                Task task = Task.Run(() => {
                    // SelectFileSampleForm.DragDrop += fileListManager.RegistFileListByDragDrop;
                    RecieveFileAfterDragDropEvent?.Invoke(files, e);
                });
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "Control_DragDrop");
            }
            finally
            {
                IsNowProcessing = false;
                _logger.AddLog(this, "Control_DragDrop Finally , IsNowProcessing=false");
            }
        }

        public string[] GetFilesByDragAndDrop(DragEventArgs e)
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
                    _logger.AddLogWarning(this, "GetDataPresent :e.Data.GetDataPresent(DataFormats.FileDrop)=false");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetFilesByDragAndDrop");
                return null;
            }
        }
    }
}
