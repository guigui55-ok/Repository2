using System;
using ErrorLog;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImageViewer.Events
{
    // ViewImageControl //inner
    // DragDrop MouseClick(Right/Left)用
    public class DragDropEvents
    {
        readonly ErrorLog.IErrorLog _errorLog;
        protected Control _control;
        public ViewImageMouseEventHandler MouseEventHandler;
        //public FileList.FileList FileListForRead;

        protected bool IsDown = false;
        protected bool IsDoClick = true;
        protected bool IsMove=false;

        public DragDropEvents(ErrorLog.IErrorLog errorLog, Control control)
        {
            _errorLog = errorLog;
            _control = control;
            // イベントのインスタンス
            MouseEventHandler = new ViewImageMouseEventHandler();
            // このクラス内のメソッドをイベントへ紐づけ
            _control.DragDrop += Control_DragDrop;
            _control.DragEnter += Control_DragEnter;
            _control.DragOver += Control_DragOver;
        }

        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            //Debug.WriteLine("Control_DragDrop");
            try
            {
                // DragDrop の e を配列へ
                //string[] files = GetFilesByDragAndDrop(e);
                // 配列→Listへ
                MouseEventHandler.DragDrop(null,e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Control_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
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
                    _errorLog.AddErrorNotException(this.ToString(), "GetFilesByDragAndDrop GetDataPresent Else");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
                return null;
            }
        }
    }
}
