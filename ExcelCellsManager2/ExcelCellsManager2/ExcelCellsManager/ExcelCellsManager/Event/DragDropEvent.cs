using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelCellsManager.Event
{
    public class DragDropEvent
    {
        protected ErrorManager.ErrorManager _error;
        protected string _formName = "";
        public string[] AllowFileTypes;
        public DragEventHandler DragEnterEventHandler;
        public DragEventHandler DragDropEventHandler;
        public event EventHandler DragDropAfter;
        public DragDropEvent(ErrorManager.ErrorManager error, string formName)
        {
            _error = error;
            _formName = formName;
            DragEnterEventHandler = new DragEventHandler(DragEnter);
            DragDropEventHandler = new DragEventHandler(DragDrop);
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            _error.AddLog(this.ToString() + ".DragEnter");
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {

                    // ドラッグ中のファイルやディレクトリの取得
                    string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                    foreach (string d in drags)
                    {
                        if (!System.IO.File.Exists(d))
                        {
                            // ファイル以外であればイベント・ハンドラを抜ける
                            return;
                        }
                    }
                    e.Effect = DragDropEffects.Copy;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, _formName + "." + this.ToString() + ".DragEnter");
            }
        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString() + ".DragDrop");
                // ドラッグ＆ドロップされたファイル
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                //files // System.String[]
                DragDropAfter?.Invoke(files, EventArgs.Empty);

            } catch (Exception ex)
            {
                _error.AddException(ex, _formName + "." + this.ToString() + ".DragDrop");
            }
        }
    }
}
