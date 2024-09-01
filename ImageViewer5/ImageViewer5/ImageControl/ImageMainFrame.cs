using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DragAndDropModule;
using CommonModule;
using AppLoggerModule;
using System.IO;
using System.Threading;
using FileListManagerSample;
using CommonUtility.FileListUtility;

namespace ImageViewer5.ImageControl
{
    public partial class ImageMainFrame : UserControl
    {
        private DragAndDropReciever dragAndDropReciever;
        public AppLogger _logger;
        //ファイルリスト処理・フォームを処理するサブスレッド
        Thread _fileListSubThread;
        //フォームを閉じるときにサブスレッドを終了させるようフラグ
        bool _isFormClosingForFileListThread = false;
        // ViewImageのコントロール、機能について取りまとめているクラス
        public ImageViwerMainClass _imageViewerMain;
        // ファイルリストを管理するクラス
        public FileListManagerSampleForm _formFileList = null;
        // ImageMainFrame全体の設定（コントロールの振る舞いや、ファイルリストの振る舞いなどなど）
        public ImageMainFrameSetting _imageMainFrameSetting;
        public ImageMainFrame()
        {
            Debugger.DebugPrint("ImageMainFrame New");
            InitializeComponent();

            //#
            // 設定ファイルを読み込み反映させる場合はここで行う（未対応） 240901
            _imageMainFrameSetting = new ImageMainFrameSetting();
        }

        public void InitializeValues(List<string> SupportedImageExtList)
        {
            _logger.PrintInfo("ImageMainFrame > InitializeValues");
            _imageViewerMain = new ImageViwerMainClass(
                _logger, (Form)this.Parent, this, this.pictureBox_ImageMain);
            _imageViewerMain.InitializeValues(SupportedImageExtList);
            //this._formFileList.SetFilesFromPath(path);
        }

        public delegate void DelegateUpdateText();
        private void doLoopFileListSubThread()
        {
            try
            {
                //label1.Text = string.Format("{0}", count);
                if (this.InvokeRequired)
                {
                    if (this._isFormClosingForFileListThread) {
                        _logger.PrintInfo("doLoopFileListSubThread End");
                        return;
                    }
                    this.Invoke(new DelegateUpdateText(this.doLoopFileListSubThread));
                    return;
                }
                //label1.Text = string.Format("{0}", count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {

            }
        }

        /// <summary>
        /// _formFileListのイベントをセットして、フォームを表示する
        /// 非同期で実行できるよう引数はobject型になっている（現在非同期では使用していない 240901）
        /// </summary>
        /// <param name="string_path"></param>
        /// <param name="supportedImageExtentionList">
        /// <param name="supportedImageExtentionList">
        /// 対応する拡張子リスト。不要な場合はnullを渡す
        /// nullの場合でも、_formFileList._fileManager._supportedFileExtentionList があればこれを使用する。
        /// 無効にしたい場合は、List string {}を渡す
        /// </param>
        public void ShowSubFormFileList(object parameters)
        {
            //if (SupportedImageExtList == null) { SupportedImageExtList = new List<string> { }; }
            //string path = (string)string_path;
            var param = (Tuple<string, List<string>, List<string>>)parameters;
            string path = param.Item1;
            List<string> fileFilterConditionList = param.Item2;
            List<string> fileIgnoreConditionList = param.Item3;
            //
            _logger.PrintInfo("ShowSubFormFileList");
            _logger.PrintInfo("SubThread=" + Thread.CurrentThread.ManagedThreadId);
            if (_formFileList == null)
            {
                this._formFileList = new FileListManagerSampleForm(
                    _logger, dragDropEnable: false, startedThisForm: false);
                this._formFileList.AddEventHandler_UpdateFileListAfterEvent(ChangeFileListEvent);
                this._formFileList.AddEventHandler_SelectedFileEvent(SelectedFile);
            }
            this._formFileList.SetFilesFromPath(path, fileFilterConditionList, fileIgnoreConditionList);
            this._formFileList.Show();

            Form formMain = (Form)this.Parent;
            int x = formMain.Location.X + formMain.Width -13;
            this._formFileList.Location = new Point(x, formMain.Location.Y);
        }

        private void SubProcMain()
        {
            _logger.PrintInfo("SubThread=" + Thread.CurrentThread.ManagedThreadId);
            // サブスレッドの処理
            //_fileListSubThread = new Thread(new ThreadStart(ShowSubFormFileList));
            //_fileListSubThread.Start();
            //
            //引数がある場合
            _fileListSubThread = new Thread(new ParameterizedThreadStart(ShowSubFormFileList));
            List<string> fileConditionList = new List<string> { };
            List<string> fileIgnoreList = new List<string> { };
            _fileListSubThread.Start(Tuple.Create("your-path-here", fileConditionList, fileIgnoreList));
            //この方法では、ShowSubFormFileList メソッドの引数を object 型として受け取り、必要に応じてキャストして使用します。
        }

        private void SelectedFile(object sender, EventArgs e)
        {
            _logger.PrintInfo("SelectedFile  > sender = " + (string)sender);
            _imageViewerMain.ShowImageThisPath();
        }

        private void ChangeFileListEvent(object sender, EventArgs e)
        {
            string[] recvList = (string[])sender;
            _logger.PrintInfo("ChangeFileListEvent  > sender.Length = " + recvList.Length);
        }

        private void ImageMainFrame_Load(object sender, EventArgs e)
        {
            if (this._logger == null)
            {
                Debugger.DebugPrint("ImageMainFrame_Load");
            }
            else
            {
                this._logger.PrintInfo("ImageMainFrame_Load");
            }
            // ここでは、PictureBox対してドラッグアンドドロップを受け付けます。
            dragAndDropReciever = new DragAndDropReciever(this.pictureBox_ImageMain);
            // イベントにメソッドを紐づける
            dragAndDropReciever.DropDetected += RecieveDropEvent;

            //SubProcMain();
            //this._logger.PrintInfo("_fileListSubThread.ManagedThreadId = " + _fileListSubThread.ManagedThreadId);
            //ShowSubFormFileList("");

        }

        // ドラッグアンドドロップ時にアイテムがドロップされた場合のメソッド
        private void RecieveDropEvent(string[] items)
        {
            foreach (var item in items)
            {
                if (Directory.Exists(item))
                {
                    // ドロップされたアイテムがディレクトリの場合
                    Debugger.DebugPrint($"Dropped directory: {item}");
                    _formFileList.SetFilesFromPath(item, null, null);
                }
                else if (File.Exists(item))
                {
                    // ドロップされたアイテムがファイルの場合
                    Debugger.DebugPrint($"Dropped file: {item}");
                    // ショートカットならその中のパスを取得する
                    string path = _formFileList._fileListManager._filesRegister.GetFilePathFromShortcut(item);
                    _formFileList.SetFilesFromPath(path, null, null);
                }
                else
                {
                    // ドロップされたアイテムが未知の形式の場合
                    Debugger.DebugPrint($"Dropped unknown item: {item}");
                }
                //最初のファイルのみ扱う
                break;
            }
        }

        private void ImageMainFrame_KeyDown(object sender, KeyEventArgs e)
        {
            //フォームのKeyPreviewを切り替える
            //this.KeyPreview = !this.KeyPreview;
            //Console.WriteLine("KeyPreview:" + this.KeyPreview.ToString());
            //this.pictureBox_ImageMain.keyp
        }

        private void Form1_KeyDown(object sender,
            System.Windows.Forms.KeyEventArgs e)
        {
            //受け取ったキーを表示する
            Console.WriteLine(e.KeyCode);
        }

        private void CloseApp_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("CloseApp_ToolStripMenuItem_Click");
            Form form = (Form)this.Parent;
            form.Close();
        }

        private void ShowFileList_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("ShowFileList_ToolStripMenuItem_Click");
            _formFileList.Visible = true;
        }
    }
}
