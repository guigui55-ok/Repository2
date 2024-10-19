using CommonUtility.FileListUtility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AppLoggerModule;

namespace CommonControlUtilityModule
{
    /// <summary>
    /// DragAndDropでファイルやフォルダを読み込むクラス
    /// 1つのコントロールとファイル更新時のイベントが紐づけられる
    /// FileListManager、Fileクラスで処理する
    /// 他に依存クラスは、DragAndDropOnControl ,DragAndDropForFile
    /// </summary>
    public class ReadFileByDragDrop
    {
        protected AppLogger _logger;
        public IFiles Files;
        public FileListManager FileListManager;
        protected DragAndDropOnControl _dragAndDropOnControl;
        protected DragAndDropForFile _dragAndDropForFile;
        public List<string> FileList;
        //protected IFileListControl _fileListControl;
        public EventHandler DragAndDropEventAfterEvent;

        public ReadFileByDragDrop(AppLogger logger, Control control,EventHandler UpdateFileAfterEvent)
        {
            _logger = logger;
            FileList = new List<string>();
            Files = new Files(_logger, FileList);
            FileListManager = new FileListManager(_logger, Files);
            _dragAndDropOnControl = new DragAndDropOnControl(_logger, control);
            _dragAndDropForFile = new DragAndDropForFile(_logger, _dragAndDropOnControl);
            FileListManager.IsReadSourceOfShotcut = true;
            // ファイルリストを取得した後にコントロールなどに表示する場合、以下に紐づける
            FileListManager.UpdateFileListAfterEvent += UpdateFileAfterEvent;
            // ファイルを読み込んだ後のイベント
            _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
        }

        public void Dispose()
        {
            try
            {
                _dragAndDropOnControl.Dispose();
                _dragAndDropForFile.Dispose();
                FileList.Clear();
                FileList = null;
                Files = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void DragAndDropEventAfterEventForFile(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "DragAndDropEventAfterEventForFile");
                if (_dragAndDropForFile.Files == null) { _logger.AddLogWarning("Files == null"); return; }
                if (_dragAndDropForFile.Files.Length < 1) { _logger.AddLogWarning("Files.Length < 1"); return; }

                string path = FileListManager.GetFilePathFromShortcut(_dragAndDropForFile.Files[0]);
                _logger.AddLog("  GetPath=" + path);
                FileListManager.SetFilesFromPath(path);
                if (DragAndDropEventAfterEvent != null) { DragAndDropEventAfterEvent.Invoke(sender, e); }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "DragAndDropEventAfterEventForFile");
                _logger.ClearError();
            }
        }
        public void AddMethodToEventHandler(Control[] controls)
        {
            try
            {
                if(controls == null) { return; }
                if(controls.Length < 1) { return; }
                foreach(Control control in controls)
                {
                    control.AllowDrop = true;
                    // FileDragDrop イベントを紐づけする
                    control.DragDrop += FileListManager.ChangedFileListEvent;
                    _dragAndDropOnControl.AddRecieveControl(control);
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "AddMethodToEventHandler");
            }
        }

        public void Initialize(AppLogger logger, Control control, EventHandler UpdateFileAfterEvent)
        {
            try
            {
                _logger = logger;
                // DragDrop をするためのクラス
                control.AllowDrop = true;
                _dragAndDropOnControl = new DragAndDropOnControl(_logger, control);
                // Control を追加する
                _dragAndDropOnControl.AddRecieveControl(control);
                // ファイルリストを扱うクラス
                FileList = new List<string>();
                Files = new Files(_logger, FileList);
                // ファイルリストを登録するクラス
                FileListManager = new FileListManager(_logger, Files);
                // FileDragDrop イベントを紐づけする
                control.DragDrop += FileListManager.ChangedFileListEvent;
                // ファイルリストを取得した後にコントロールなどに表示する場合、以下に紐づける
                FileListManager.UpdateFileListAfterEvent += UpdateFileAfterEvent;

                // FileListManager settings
                FileListManager.IsReadMatchFirstOnly = true;
                FileListManager.IsReadSourceOfShotcut = false;
                FileListManager.ReadFolderHierarchy = 0;
            } catch (Exception ex)
            {
                _logger.AddException(ex,this, "Initialize");
            }
        }
    }
}
