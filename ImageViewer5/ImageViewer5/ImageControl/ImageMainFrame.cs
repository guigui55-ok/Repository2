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
using System.Text.RegularExpressions;

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
        public ImageMainClass _imageViewerMain;
        // ファイルリストを管理するクラス
        public FileListManagerSampleForm _formFileList = null;
        // ImageMainFrame全体の設定（コントロールの振る舞いや、ファイルリストの振る舞いなどなど）
        public ImageMainFrameSetting _imageMainFrameSetting;
        // index
        public int Index = -1;

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
            _logger.PrintInfo(this.Name + " > InitializeValues");
            _imageViewerMain = new ImageMainClass(
                _logger, (Form)this.Parent, this, this.pictureBox_ImageMain, this.GetComponentNumber());
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
            var param = (Tuple<string, List<string>, List<string>, bool>)parameters;
            string path = param.Item1;
            List<string> fileFilterConditionList = param.Item2;
            List<string> fileIgnoreConditionList = param.Item3;
            bool isShowForm = param.Item4;
            //
            _logger.PrintInfo(this.Name + " > ShowSubFormFileList");
            _logger.PrintInfo("  SubThread=" + Thread.CurrentThread.ManagedThreadId);
            if (_formFileList == null)
            {
                this._formFileList = new FileListManagerSampleForm(
                    _logger,
                    dragDropEnable: false,
                    startedThisForm: false);
                this._formFileList.AddEventHandler_UpdateFileListAfterEvent(ChangeFileListEvent);
                this._formFileList.AddEventHandler_SelectedFileEvent(SelectedFile);
            }
            this._formFileList.SetFilesFromPath(path, fileFilterConditionList, fileIgnoreConditionList);
            _logger.PrintInfo(String.Format("  isShowForm = {0}", isShowForm));
            if (isShowForm)
            {
                this._formFileList.Show();
            }

            Form formMain = (Form)this.Parent;
            int x = formMain.Location.X + formMain.Width -13;
            this._formFileList.Location = new Point(x, formMain.Location.Y);
        }

        /// <summary>
        /// 使用していない
        /// 非同期で試したとき用のメソッド
        /// </summary>
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
            _logger.PrintInfo(this.Name + " > SelectedFile  > sender = " + (string)sender);
            _imageViewerMain.ShowImageThisPath();
        }

        private void ChangeFileListEvent(object sender, EventArgs e)
        {
            string[] recvList = (string[])sender;
            _logger.PrintInfo(this.Name + " > ChangeFileListEvent  > sender.Length = " + recvList.Length);
        }

        private void ImageMainFrame_Load(object sender, EventArgs e)
        {
            if (this._logger == null)
            {
                Debugger.DebugPrint(this.Name + " > ImageMainFrame_Load");
            }
            else
            {
                this._logger.PrintInfo(this.Name + " > ImageMainFrame_Load");
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


        public void ChangeSizeAndLocation(Size size, Point location)
        {
            try
            {
                Size beforeSize = this.Size;

                //#

                this.Size = size;
                _imageViewerMain._viewImageControl.ChangeSize(size);
                this.Location = location;

            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "ChangeSizeAndLocation");
            }
        }


        private void CloseApp_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo(this.Name + " > CloseApp_ToolStripMenuItem_Click");
            Form form = (Form)this.Parent;
            form.Close();
        }

        private void ShowFileList_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo(this.Name + " > ShowFileList_ToolStripMenuItem_Click");
            _formFileList.Visible = true;
        }

        private void SlideShowOnOff_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo(this.Name + " > SlideShowOnOff_ToolStripMenuItem_Click");
            _imageViewerMain._viewImageFunction._viewImageSlideShow.ChangeOnOff();
        }

        private void ImageMainFrame_Resize(object sender, EventArgs e)
        {
            try
            {
                if ((_imageViewerMain != null))
                {
                    ResizePictureBoxWithSameRatio(this, _imageViewerMain._viewImageControl.GetControl());
                }
            } catch(Exception ex)
            {
                _logger.AddException(ex, this, "ChangeSizeAndLocation");
            }
        }
        private void ResizePictureBoxWithSameRatio(Control frameControl, Control innerControl)
        {
            // PanelとPictureBoxのサイズの比率を計算
            float widthRatio = (float)innerControl.Width / frameControl.Width;
            float heightRatio = (float)innerControl.Height / frameControl.Height;

            // PanelとPictureBoxのLocationの位置の比率を計算
            float locationXRatio = (float)innerControl.Location.X / frameControl.Width;
            float locationYRatio = (float)innerControl.Location.Y / frameControl.Height;

            // Panelのサイズ変更後に新しいPictureBoxのサイズを計算
            int newPictureBoxWidth = (int)(frameControl.Width * widthRatio);
            int newPictureBoxHeight = (int)(frameControl.Height * heightRatio);

            // Panelのサイズ変更後に新しいPictureBoxの位置を計算
            int newPictureBoxLocationX = (int)(frameControl.Width * locationXRatio);
            int newPictureBoxLocationY = (int)(frameControl.Height * locationYRatio);

            // PictureBoxに新しいサイズと位置を適用
            innerControl.Size = new Size(newPictureBoxWidth, newPictureBoxHeight);
            innerControl.Location = new Point(newPictureBoxLocationX, newPictureBoxLocationY);
        }

        //private void ResizeWithFitPictureBoxInFrame(object sender)
        //{
        //    Control panel = sender as Control;
        //    if (panel == null || panel.Controls.Count == 0)
        //        return;

        //    PictureBox pictureBox = panel.Controls[0] as PictureBox; // PictureBoxがPanelの最初のコントロールと仮定
        //    if (pictureBox == null)
        //        return;

        //    // PictureBoxのアスペクト比を計算
        //    float aspectRatio = (float)pictureBox.Width / pictureBox.Height;

        //    // アスペクト比を保ちながら新しいサイズを計算
        //    int newWidth = panel.Width;
        //    int newHeight = (int)(newWidth / aspectRatio);

        //    //// PictureBoxがPanelに収まるように調整
        //    //if (newHeight > panel.Height)
        //    //{
        //    //    newHeight = panel.Height;
        //    //    newWidth = (int)(newHeight * aspectRatio);
        //    //}

        //    // PictureBoxに新しいサイズを適用
        //    pictureBox.Size = new Size(newWidth, newHeight);

        //    //// OptionでPictureBoxをPanelの中央に配置
        //    //pictureBox.Location = new Point(
        //    //    (panel.Width - pictureBox.Width) / 2,
        //    //    (panel.Height - pictureBox.Height) / 2
        //    //);
        //}

        /// <summary>
        /// ImageMainFrame.Nameの最後の番号を取得する。
        /// 、Frameの名前はすべて「ImageMainFrame1、 ～Frame2...」のようにすること
        /// </summary>
        /// <returns></returns>
        public int GetComponentNumber()
        {
            try
            {
                string pattern = @"\d{1,}$";
                MatchCollection matches = Regex.Matches(this.Name, pattern);
                //foreach (Match match in matches)
                //{
                //    Console.WriteLine(match.Value); // 出力: 123, 456
                //}
                if (matches.Count < 1){return 0;}
                string buf = matches[0].Value;
                return int.Parse(buf);

            } catch(Exception ex)
            {
                _logger.AddException(ex, this, "GetIndexNumber");
            }
            return 0;
        }
    }
}
