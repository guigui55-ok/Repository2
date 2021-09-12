using CommonUtility.FileListUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlUtility
{
    public class ReadFileByDragDrop
    {
        protected ErrorManager.ErrorManager _err;
        public IFiles Files;
        public FileListManager FileListManager;
        protected DragAndDropOnControl _dragAndDropOnControl;
        protected DragAndDropForFile _dragAndDropForFile;
        public List<string> FileList;
        //protected IFileListControl _fileListControl;
        public EventHandler DragAndDropEventAfterEvent;

        public ReadFileByDragDrop(ErrorManager.ErrorManager err, Control control,EventHandler UpdateFileAfterEvent)
        {
            _err = err;
            FileList = new List<string>();
            Files = new Files(_err, FileList);
            FileListManager = new FileListManager(_err, Files);
            _dragAndDropOnControl = new DragAndDropOnControl(_err, control);
            _dragAndDropForFile = new DragAndDropForFile(_err, _dragAndDropOnControl);
            // ファイルリストを取得した後にコントロールなどに表示する場合、以下に紐づける
            FileListManager.UpdateFileListAfterEvent += UpdateFileAfterEvent;
            // ファイルを読み込んだ後のイベント
            _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
        }

        private void DragAndDropEventAfterEventForFile(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this, "DragAndDropEventAfterEventForFile");
                if (_dragAndDropForFile.Files == null) { _err.AddLogWarning("Files == null"); return; }
                if (_dragAndDropForFile.Files.Length < 1) { _err.AddLogWarning("Files.Length < 1"); return; }

                _err.AddLog("  GetPath=" + _dragAndDropForFile.Files[0]);
                FileListManager.SetFilesFromPath(_dragAndDropForFile.Files[0]);
                DragAndDropEventAfterEvent?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "DragAndDropEventAfterEventForFile");
                _err.ClearError();
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
                _err.AddException(ex, this, "AddMethodToEventHandler");
            }
        }

        public void Initialize(Control control, EventHandler UpdateFileAfterEvent)
        {
            try
            {
                _err = new ErrorManager.ErrorManager(1);
                // DragDrop をするためのクラス
                control.AllowDrop = true;
                _dragAndDropOnControl = new DragAndDropOnControl(_err, control);
                // Control を追加する
                _dragAndDropOnControl.AddRecieveControl(control);
                // ファイルリストを扱うクラス
                FileList = new List<string>();
                Files = new Files(_err, FileList);
                // ファイルリストを登録するクラス
                FileListManager = new FileListManager(_err, Files);
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
                _err.AddException(ex,this, "Initialize");
            }
        }
    }
}
