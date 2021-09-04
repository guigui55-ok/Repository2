using ControlUtility.SelectFiles;
using CotnrolUtility.SelectFiles;
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
        public IFiles files;
        public FileListManager fileListManager;
        public SelectFileByDragDrop fileDragDrop;
        public List<string> list;

        public ReadFileByDragDrop(ErrorManager.ErrorManager err, Control control,EventHandler UpdateFileAfterEvent)
        {
            _err = err;
            list = new List<string>();
            files = new SelectFiles.SingleFile(_err, list);
            fileListManager = new FileListManager(_err,files);
            fileDragDrop = new SelectFileByDragDrop(_err, control);
            // ファイルリストを取得した後にコントロールなどに表示する場合、以下に紐づける
            fileListManager.UpdateFileListAfterEvent += UpdateFileAfterEvent;
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
                    control.DragDrop += fileListManager.RegistFileByDragDrop;
                    fileDragDrop.AddRecieveControl(control);
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
                fileDragDrop = new SelectFileByDragDrop(_err, control);
                // Control を追加する
                fileDragDrop.AddRecieveControl(control);
                // ファイルリストを扱うクラス
                list = new List<string>();
                files = new SingleFile(_err, list);
                // ファイルリストを登録するクラス
                fileListManager = new FileListManager(_err, files);
                // FileDragDrop イベントを紐づけする
                control.DragDrop += fileListManager.RegistFileByDragDrop;
                // ファイルリストを取得した後にコントロールなどに表示する場合、以下に紐づける
                fileListManager.UpdateFileListAfterEvent += UpdateFileAfterEvent;

                // FileListManager settings
                fileListManager.IsReadMatchFirstOnly = true;
                fileListManager.IsReadSourceOfShotcut = false;
                fileListManager.ReadFolderHierarchy = 0;
            } catch (Exception ex)
            {
                _err.AddException(ex,this, "Initialize");
            }
        }
    }
}
