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
using CommonModule;
//using CommonModulesProject;
using ImageViewer5.ImageControl;

namespace ImageViewer5
{
    public partial class FormMain : Form
    {
        private AppLogger _logger;
        bool _isFirstPaint = true;
        public ImageMainFrame _nowImageMainFrame;
        public FormMain()
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
            this.imageMainFrame1._logger = _logger;
            this.imageMainFrame1.Parent = this;
            _nowImageMainFrame = imageMainFrame1;
            _nowImageMainFrame.Size = this.ClientSize;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.PrintInfo("FormMain_Load");
                //
                //FormMain_Load中の処理
                //
                _logger.PrintInfo("FormMain_Load End");

            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "FormMain_Load");

            }
            finally
            {
                //#
                //テスト値
                //List<string> ignoreList = new List<string> { "_4a44d3" };
                List<string> ignoreList = new List<string> { "" };
                string path = @"C:\Users\OK\source\repos\test_media_files\test_jpg";
                List<string> filterList = ImageViewerConstants.SUPPORTED_IMAGE_EXTENTION_DEFAULT_LIST;
                //#
                _nowImageMainFrame.InitializeValues(filterList);
                // タプルを作成して渡す
                var parameters = Tuple.Create(
                    path,
                    filterList,
                    ignoreList);
                _nowImageMainFrame.ShowSubFormFileList(parameters);
                _nowImageMainFrame._imageViewerMain.ShowImageAfterInitialize(path);
            }
        }


        private void FormMain_Closed(object sender, FormClosedEventArgs e)
        {
            _logger.PrintInfo("##########  Form_Closed  #########");
            Console.WriteLine("Logger.FilePath");
            Console.WriteLine(_logger.FilePath);
            Console.WriteLine("");
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (_isFirstPaint)
                {
                    _logger.PrintInfo(String.Format("FormMain_Paint First"));
                    _logger.PrintInfo(String.Format("FormMain.Size = {0}", this.Size));
                    // FormMain_Loadの最後だと描画されない
                    _nowImageMainFrame._imageViewerMain.ShowImageAfterInitialize(
                        _nowImageMainFrame._formFileList._files.GetCurrentValue());
                    //_nowImageMainFrame._imageViewerMain.ShowImageThisPath();
                    _isFirstPaint = false;
                }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "FormMain_Paint");
            }

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
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "FormMain_KeyDown");
            }
            
        }
    }
}
