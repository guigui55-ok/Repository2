using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using CommonModuleImageViewer;
//using CommonModulesProject;
using ImageViewer5.ImageControl;
using System.Reflection;
using ViewImageModule;
using ImageViewer5.AddFunction;
using System.IO;

namespace ImageViewer5
{
    public partial class FormMain : Form
    {
        public AppLogger _logger;
        bool _isFirstPaint = true;
        public ImageMainFrame _nowImageMainFrame;
        public ImageViewerArgs _imageViewerArgs;
        public ApplySettings _applySettings;
        //public List<ImageMainFrame> _imageMainFrameList;
        public MainFrameManager _mainFrameManager;
        public FormMainSetting _formMainSetting;
        // 240626 追加
        public FileSenderFunction _fileSenderFunction;
        public FormMain(string[] args)
        {
            Debugger.DebugPrint("FormMain New");
            InitializeComponent();
            _logger = new AppLogger();
            _logger.LogFileTimeFormat = "";
            // ファイルパスをCurrentDirectoryに設定
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory; 
            string logFilePath = System.IO.Path.Combine(currentDirectory, "__test_log.log");
            Debugger.DebugPrint(string.Format("logFilePath = {0}", logFilePath));
            _logger.LogFileTimeFormat = "";
            _logger.SetFilePath(logFilePath);
            // ログレベルをINFOに設定
            _logger.LoggerLogLevel = LogLevel.INFO;
            // ログをコンソールとファイルに出力するように設定
            _logger.LogOutPutMode = OutputMode.CONSOLE | OutputMode.FILE;
            //
            _logger.isShowThreadId = true;
            //#
            _logger.PrintInfo("#########  FormMain New  #########");
            Assembly myAssembly = Assembly.GetEntryAssembly();
            _logger.PrintInfo(String.Format("myAssembly.Location = {0}", myAssembly.Location));
            //#
            _formMainSetting = new FormMainSetting(_logger, this);
            string settingPath = Path.Combine(Directory.GetCurrentDirectory(), ImageViewerConstants.IMAGE_VIEWER_SETTING_FILE_NAME);
            _formMainSetting.ReadSettingFile(settingPath);
            //#
            _imageViewerArgs = new ImageViewerArgs(_logger, args);
            _imageViewerArgs.ParseArguments(args);
            //#
            //FormMain設定
            //this.Size = Size(300, 400);
            this.Location = new Point(400, 80);
            this.KeyPreview = true;
            //タイトルバーを消す
            //this.ControlBox = false;
            //this.Text = "";
            //
            //#
            _mainFrameManager = new MainFrameManager(_logger, this);
            //
            _fileSenderFunction = new FileSenderFunction(_logger, this);
            _fileSenderFunction.Initialize();
            //#
            //this.imageMainFrame1._logger = _logger;
            //this.imageMainFrame1.Parent = this;
            // ViewImageFrameを複数作るならここらへんで行う
            foreach (ImageMainFrame bufMainFrame in _mainFrameManager._imageMainFrameList)
            {
                bufMainFrame._logger = _logger;
                bufMainFrame.Parent = this;
                bufMainFrame._imageMainFrameSetting._logger = _logger;
                bufMainFrame._imageMainFrameSetting.SetSettingDatFromMain(_formMainSetting);
            }
            //_nowImageMainFrame = imageMainFrame1;
            _nowImageMainFrame = _mainFrameManager._imageMainFrameList[0];
            _nowImageMainFrame.Size = this.ClientSize;
            _nowImageMainFrame.Index = 0;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.PrintInfo("FormMain_Load");
                //
                //FormMain_Load中の処理
                _applySettings = new ApplySettings(_logger, this);
                //
                //#
                //テスト値
                //List<string> ignoreList = new List<string> { "_4a44d3" };
                List<string> ignoreList = new List<string> { "" };
                List<string> filterList = ImageViewerConstants.SUPPORTED_IMAGE_EXTENTION_DEFAULT_LIST;
                string path = @"C:\Users\OK\source\repos\test_media_files\test_jpg";
                //#
                path = (string)_nowImageMainFrame._imageMainFrameSetting._settingDictionary._settingDict[SettingKey.RESTORE_PREV_DIR];
                _nowImageMainFrame.InitializeValues(filterList);
                // タプルを作成して渡す
                var parameters = Tuple.Create(
                    path,
                    filterList,
                    ignoreList,
                    false);
                _nowImageMainFrame.ShowSubFormFileList(parameters);
                _nowImageMainFrame._formFileList._fileListManager.Name = "FileListManager" + _nowImageMainFrame.GetComponentNumber();
                //_nowImageMainFrame._imageViewerMain.ShowImageAfterInitialize(path); //OnPaintFirstで実行する
                //ファイルリスト設定後に実行する
                _nowImageMainFrame._imageViewerMain._viewImageFunction.InitializeValue_LoadAfter();
                //引数設定を適用
                //（スライドショーインスタンス生成などがあるため）
                _applySettings.ApplyArgsImageViewerFrameByDict(_formMainSetting._settingDictionary, 1);
                _applySettings.ApplyArgs(_imageViewerArgs);
                // 1つ目のスライドショーが終わらなくなるため、ON/OFFしておく（原因不明）
                // 240920
                // Anchorがどこかで Top,Left となる（FrameのDraggerが機能しなくなるのでここで設定しなおし）
                _nowImageMainFrame.Anchor = AnchorStyles.None;
                // デザイナの設定と重複する可能性があるので、ログ出力
                _logger.PrintInfo(_nowImageMainFrame.Name + " > Set Anchor.None");
                // 240926
                _fileSenderFunction.AddEventHandler(_nowImageMainFrame);
                _logger.PrintInfo("FormMain_Load End");

            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "FormMain_Load");

            }
            finally
            {
                _logger.PrintInfo("FormMain_Load finally");
                _logger.PrintInfo("FormMain_Load finally End");
            }
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (_isFirstPaint)
                {
                    _logger.PrintInfo(String.Format("FormMain_Paint First"));
                    _logger.PrintInfo(String.Format("FormMain.Size = {0}", this.Size));
                    _applySettings.ApplyArgsForSetting(_formMainSetting._settingDictionary);
                    //最初だけ実行する
                    _nowImageMainFrame._imageViewerMain._viewImageFunction._viewImageOtherFunction.ImageMainFrameAny_MouseDown(this, EventArgs.Empty);
                    // FormMain_Loadの最後だと描画されない
                    _nowImageMainFrame._imageViewerMain.ShowImageAfterInitialize(
                        _nowImageMainFrame._formFileList._files.GetCurrentValue());
                    _mainFrameManager._imageMainFrameList[0] = _nowImageMainFrame;
                    // 241005 FileSenderが表示ONのときに前面に
                    if (_fileSenderFunction._fileSenderApp.Visible)
                    {
                        _fileSenderFunction._fileSenderApp.Activate();
                    }
                    // 241005 暫定対応 画像をFrameにFitさせる
                    _nowImageMainFrame._imageViewerMain._viewImageFunction._viewImageFunction_FitInnerToFrame.FitImageToControl(true);
                    // FileSenderに値をセットする
                    string path = _nowImageMainFrame._formFileList._files.GetCurrentValue();
                    this._fileSenderFunction._fileSenderApp._dataBridgeFromExternal.SetData(path);
                    _isFirstPaint = false;
                }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "FormMain_Paint");
            }
        }

        public ImageMainFrame GetNowImageMainFrame()
        {
            return _nowImageMainFrame;
        }

        private void FormMain_Closed(object sender, FormClosedEventArgs e)
        {
            // 241005 デバッグ・調査用　一時的に抑制する 
            bool IsOutputLog = false;
            IsOutputLog = true;
            //
            _logger.PrintInfo("##########  Form_Closed A_Part  #########");
            _fileSenderFunction._fileSenderApp.isOutputSetting = IsOutputLog;
            this._fileSenderFunction._fileSenderApp.FormFileSenderApp_FormClosed(sender, e);
            _formMainSetting.isOutputSetting = IsOutputLog;
            _formMainSetting.SaveSetting();
            _logger.PrintInfo("##########  Form_Closed B_Part  #########");
            Console.WriteLine("Logger.FilePath");
            Console.WriteLine(_logger.FilePath);
            Console.WriteLine("");
        }
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    _logger.PrintInfo("FormMain_KeyDown  ESC");
                    // 必要なら、イベントを処理済みとしてマーク
                    e.Handled = true;
                    this.Close();
                }
                else if (e.KeyCode == Keys.Right)
                {
                    _logger.PrintInfo("FormMain_KeyDown  Keys.Right");
                    ImageMainFrame imageFrame = _mainFrameManager.GetCurrentFrame();
                    imageFrame._formFileList._fileListManager.MoveNextFile();
                }
                else if (e.KeyCode == Keys.Left)
                {
                    _logger.PrintInfo("FormMain_KeyDown  Keys.Left");
                    ImageMainFrame imageFrame = _mainFrameManager.GetCurrentFrame();
                    imageFrame._formFileList._fileListManager.MovePrevFile();
                }
                else if (e.KeyCode == Keys.A && e.Control)
                {
                    _logger.PrintInfo("FormMain_KeyDown  Ctrl+A");
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Z && e.Control)
                {
                    _logger.PrintInfo("FormMain_KeyDown  Ctrl+Z");
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.I && e.Control)
                {
                    _logger.PrintInfo("FormMain_KeyDown  Ctrl+I");
                    e.Handled = true;
                    _logger.PrintInfo(String.Format("FormMain.Size = {0}", this.Size));
                }
                else if (e.KeyCode == Keys.NumPad0 && e.Control)
                {
                    _logger.PrintInfo("====================");
                    _logger.PrintInfo("FormMain_KeyDown  Ctrl+NumPad0");
                    //Frame（UserControl）を1つ追加して表示する
                    ImageMainFrame bufFrame = _mainFrameManager.CreateImageMainFrame(
                        _mainFrameManager._imageMainFrameList.Count + 1);
                    string path = @"C:\Users\OK\source\repos\test_media_files\test_jpg";
                    List<string> filterList = ImageViewerConstants.SUPPORTED_IMAGE_EXTENTION_DEFAULT_LIST;
                    bufFrame.InitializeValues(filterList);
                    this.Controls.Add(bufFrame);
                    _mainFrameManager._imageMainFrameList.Add(bufFrame);

                    _mainFrameManager.InitializeMainFrame(bufFrame, path, filterList);

                    _mainFrameManager.AdjustSizeAndLocation();
                    _logger.PrintInfo("FormMain_KeyDown  AdjustSizeAndLocation  End");

                    bufFrame._imageViewerMain.ShowImageAfterInitialize(
                        bufFrame._formFileList._files.GetCurrentValue());
                    bufFrame._imageViewerMain._viewImageFunction.InitializeValue_LoadAfter();
                }
                else if (e.KeyCode == Keys.NumPad1 && e.Control)
                {
                    _logger.PrintInfo("====================");
                    _logger.PrintInfo("FormMain_KeyDown  Ctrl+NumPad1");
                    // 表示しているFrame（UserControl）のリストの最後を除去する
                    ImageMainFrame bufFrame = _mainFrameManager._imageMainFrameList[_mainFrameManager._imageMainFrameList.Count - 1];
                    bufFrame._imageViewerMain._viewImage.DisposeImage();
                    bufFrame.Visible = false;
                    this.Controls.Remove(bufFrame);
                    _mainFrameManager._imageMainFrameList.Remove(bufFrame);
                }
                else if (e.KeyCode == Keys.NumPad5 && e.Control)
                {
                    _logger.PrintInfo("FormMain_KeyDown  Ctrl+NumPad5");
                    // 表示しているFrame（UserControl）のサイズに、InnerのPictureBoxのサイズを変更する
                    for (int i = 0; i < _mainFrameManager._imageMainFrameList.Count; i++)
                    {
                        ImageMainFrame bufFrame = _mainFrameManager._imageMainFrameList[i];
                        bufFrame._imageViewerMain._viewImageFunction._viewImageFunction_FitInnerToFrame.FitImageToControl(true);
                    }
                }
                else if (e.KeyCode == Keys.NumPad5)
                {
                    _logger.PrintInfo("FormMain_KeyDown  NumPad5");
                    // 表示しているFrame（UserControl）のサイズに、InnerのPictureBoxのサイズを変更する
                    for (int i = 0; i < _mainFrameManager._imageMainFrameList.Count; i++)
                    {
                        ImageMainFrame bufFrame = _mainFrameManager._imageMainFrameList[i];
                        bufFrame._imageViewerMain._viewImageFunction._viewImageFunction_FitInnerToFrame.FitImageToControl(true);
                    }
                }
                else if (e.KeyCode == Keys.NumPad6 && e.Control)
                {
                    _logger.PrintInfo("FormMain_KeyDown  Ctrl+NumPad6");
                    // WindowにFrame（UserControl）をフィットさせる
                    if (_mainFrameManager._imageMainFrameList.Count == 1)
                    {
                        ImageMainFrame bufFrame = _mainFrameManager._imageMainFrameList[0];
                        bufFrame._imageViewerMain._viewImageFunction._viewImageFunction_FitInnerToFrame.FitImageToControl(true);

                        Size imageSize = bufFrame.Size;
                        Size _frameSize = bufFrame.Size;
                        Size _formSize = this.ClientSize;
                        ViewImageFunction_FitInnerToFrame calcrator = bufFrame._imageViewerMain._viewImageFunction._viewImageFunction_FitInnerToFrame;
                        // サイズ計算 Panel にフィットさせる
                        Size newSize = calcrator.GetSizeFitFrame(_formSize, _frameSize);

                        bufFrame.Size = newSize;
                        // Location 計算
                        // 中央に表示する
                    }
                    else
                    {
                        for (int i = 0; i < _mainFrameManager._imageMainFrameList.Count; i++)
                        {
                            ImageMainFrame bufFrame = _mainFrameManager._imageMainFrameList[i];
                            bufFrame._imageViewerMain._viewImageFunction._viewImageFunction_FitInnerToFrame.FitImageToControl(true);


                            Size imageSize = bufFrame.Size;
                            Size _frameSize = bufFrame.Size;
                            Size _formSize = this.ClientSize;
                            ViewImageFunction_FitInnerToFrame calcrator = bufFrame._imageViewerMain._viewImageFunction._viewImageFunction_FitInnerToFrame;
                            // サイズ計算 Panel にフィットさせる
                            Size newSize = calcrator.GetSizeFitFrame(_formSize, _frameSize);

                            //// Location 計算
                            //// 中央に表示する
                        }
                    }
                }
                else if (e.KeyCode == Keys.NumPad9)
                {
                    _logger.PrintInfo("FormMain_KeyDown  NumPad9");
                    ImageMainFrame imageMainFrame = this._mainFrameManager.GetCurrentFrame();
                    imageMainFrame._imageViewerMain._viewImage.DisposeImage();
                }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "FormMain_KeyDown");
            }
            
        }
    }
}
