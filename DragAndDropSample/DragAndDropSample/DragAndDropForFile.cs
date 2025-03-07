﻿using AppLoggerModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DragAndDropSample
{
    public class DragAndDropForFile
    {
        public AppLogger _logger;
        public DragAndDropOnControl DragAndDropOnControl;
        public string[] Files;
        public EventHandler DragAndDropEventAfterEventForFile;
        public DragAndDropForFile(AppLogger logger,DragAndDropOnControl dragAndDropOnControl)
        {
            _logger = logger;
            this.DragAndDropOnControl = dragAndDropOnControl;
            this.DragAndDropOnControl.DragAndDropAfterEvent += DragAndDropAfterEvent;
        }

        private void DragAndDropAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "DragAndDropAfterEvent");
                // DragDrop の e を配列へ
                Files = GetFilesByDragAndDrop((DragEventArgs)e);

            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "DragAndDropAfterEvent");
            } finally
            {
                DragAndDropEventAfterEventForFile?.Invoke(sender, e);
            }
        }

        public string[] GetFilesByDragAndDrop(DragEventArgs e)
        {
            try
            {
                _logger.AddLog(this, "GetFilesByDragAndDrop");
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
