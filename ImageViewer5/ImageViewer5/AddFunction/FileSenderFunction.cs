using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;
using FileSenderApp;
using ImageViewer5.ImageControl;

namespace ImageViewer5.AddFunction
{
    public class FileSenderFunction
    {
        AppLogger _logger;
        public FormFileSenderApp _fileSenderApp;
        FormMain _formMain;
        public FileSenderFunction(AppLogger logger, FormMain formMain)
        {
            _logger = logger;
            _formMain = formMain;
        }

        /// <summary>
        /// 初期化処理
        /// （FormFileSenderAppの生成、ログが長いので注意）
        /// </summary>
        public void Initialize()
        {
            _logger.PrintInfo(" ============================================================ ");
            _logger.PrintInfo("FileSenderFunction > Initialize");
            // コンストラクタで表示される
            _fileSenderApp = new FormFileSenderApp(_logger);　 //別ログにしても良いかもしれない
            _fileSenderApp._isSubForm = true;
            string buf = _fileSenderApp.ToString();
            //ファイルリストのカレントファイルが変更されたときに、FileSenderAppのBridgeValueを変更する
            //　=値（パス）の入れ替え時は、fileSender.DataBridgeも更新（テキストボックス更新＞DataBridge更新）
            _logger.PrintInfo(" ============================================================ ");
        }

        public void AddEventHandler(ImageMainFrame imageMainFrame)
        {
            imageMainFrame._formFileList.AddEventHandler_SelectedFileEvent(ChangeFileEventRecieve);
            _fileSenderApp.AnySendButton_Clicked += FileSenderApp_ClickedButton_RecieveEvent;
        }

        private void FileSenderApp_ClickedButton_RecieveEvent(object sender, EventArgs e)
        {
            //ボタンがクリックされた（ファイルが、削除orコピーされた）
            // _mainFrameManager.GetCurrentFrameのファイルが削除orコピーされた
            _logger.PrintInfo("FileSenderFunction > FileSenderApp_ClickedButton_RecieveEvent");
            //#
            //削除・移動ならリスト更新、コピーなら、＋1する
            if (_fileSenderApp.IsCheckedMove())
            {
                // UpdateFilePath
                //_formMain._mainFrameManager.GetCurrentFrame()._formFileList._fileListManager.
                _formMain._mainFrameManager.GetCurrentFrame().UpdateFileList();
            }
            else{
                _formMain._mainFrameManager.GetCurrentFrame()._formFileList._files.MoveNext();
            }
            //string filePath = _filechangeSimple.GetCurrentPath();
            string filePath = _formMain._mainFrameManager.GetCurrentFrame()._formFileList._files.GetCurrentValue();
            //_fileSenderApp._dataBridgeFromExternal.SetData(filePath);
            _fileSenderApp.SetDataFromExternal(filePath);
            //textBox1.Text = filePath;
        }

        private void ChangeFileEventRecieve(object sender, EventArgs e)
        {
            //カレントファイルが更新された（ファイルが選択された）
            _logger.PrintInfo("FileSenderFunction > ChangeFileEventRecieve");
            //string filePath = _filechangeSimple.GetCurrentPath();
            string filePath = _formMain._mainFrameManager.GetCurrentFrame()._formFileList._files.GetCurrentValue();
            //_fileSenderApp._dataBridgeFromExternal.SetData(filePath);
            _fileSenderApp.SetDataFromExternal(filePath);
            //textBox1.Text = filePath;
        }
    }
}
