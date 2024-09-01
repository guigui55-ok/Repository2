using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonModule;
using System.Windows.Forms;
using System.IO;

namespace DragAndDropModule
{
    public class DragAndDropReciever
    {
        private Control _control;
        // ドラッグアンドドロップを受け付けたときに発生するイベント
        public event Action<string[]> DropDetected;
        public event Action<string> NonFileDropDetected;

        public DragAndDropReciever(Control control)
        {
            _control = control;
            _control.AllowDrop = true;
            _control.DragEnter += new DragEventHandler(Control_DragEnter);
            _control.DragDrop += new DragEventHandler(Control_DragDrop);
        }

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);

                // イベントを発火させ、外部メソッドを呼び出す
                Debugger.DebugPrint($"Files Dropped: {items.Length}", "Files Drop");
                DropDetected?.Invoke(items);
            }
            else
            {
                // ファイル以外がドロップされた場合
                string droppedData = e.Data.GetData(DataFormats.Text)?.ToString() ?? "Unknown data format";
                Debugger.DebugPrint($"Dropped: {droppedData}", "Drop Unknown data format");
                NonFileDropDetected?.Invoke(droppedData);
            }
        }

        //private void Control_DragDrop(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);

        //        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //        foreach (var file in files)
        //        {
        //            Debugger.DebugPrint($"File Dropped: {file}", "File Drop");
        //            // イベントを発火させ、外部メソッドを呼び出す
        //            FilesDropDetected?.Invoke(files);
        //        }


        //        // 最初のアイテムを取得
        //        string firstItem = items.Length > 0 ? items[0] : null;

        //        if (!string.IsNullOrEmpty(firstItem))
        //        {
        //            if (Directory.Exists(firstItem))
        //            {
        //                // ドロップされたアイテムがディレクトリの場合
        //                Debugger.DebugPrint($"Directory Dropped: {firstItem}", "Directory Drop");
        //                DirectoryDropDetected?.Invoke(firstItem);
        //            }
        //            else if (File.Exists(firstItem))
        //            {
        //                // ドロップされたアイテムがファイルの場合
        //                FileDropDetected?.Invoke(firstItem);
        //                Debugger.DebugPrint($"File Dropped: {firstItem}", "File Drop");
        //                // イベントを発火させ、外部メソッドを呼び出す
        //                FileDropDetected?.Invoke(firstItem);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // ファイル以外がドロップされた場合
        //        string droppedData = e.Data.GetData(DataFormats.Text)?.ToString() ?? "Unknown data format";
        //        Debugger.DebugPrint($"Dropped: {droppedData}", "Drop Unknown data format");
        //        NonFileDropDetected?.Invoke(droppedData);
        //    }
        //}
    }
}