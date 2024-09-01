using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using System.Drawing;
using ViewImageModule;
using CommonControlUtilityModule;
using ImageViewer5.CommonModules;

namespace ImageViewer5.ImageControl
{
    /// <summary>
    /// 1つのコントロールやイメージ処理を、取りまとめて扱うクラス
    /// ViewImage、外枠UserControl、内側PictureBoxの、機能や連携処理を扱う
    /// </summary>
    public class ImageViwerMainClass
    {
        public PictureBox _pictureBox;
        public Control _parentControl;
        protected AppLogger _logger;
        //# 
        // Member
        public IViewImage _viewImage;
        public IViewImageControl _viewImageControl;
        public IViewImageFrameControl _viewImageFrameControl;
        public ReadFileByDragDrop _readFileByDragDrop;
        public ViewImageFunction _viewImageFunction;

        /// <summary>
        /// ViewImageのコントロールや機能を取りまとめるクラス
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="formMain"></param>
        /// <param name="parentControl"></param>
        /// <param name="pictureBox"></param>
        public ImageViwerMainClass(AppLogger logger,Form formMain, Control parentControl, PictureBox pictureBox)
        {
            _logger = logger;
            _pictureBox = pictureBox;
            _parentControl = parentControl;
            SetImageSettings();
            _viewImage = new ViewImage(_logger);
            _viewImageControl = new ViewImageControlPictureBox(_logger, parentControl, pictureBox);
            _viewImageFrameControl = new ViewImageFrameControlForm(_logger, formMain);
            _viewImageFunction = new ViewImageFunction(_logger, _viewImage, _viewImageFrameControl, _viewImageControl);
        }
        private void SetImageSettings()
        {
            try
            {
                _pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "SetImageSettings");
            }
        }

        /// <summary>
        /// クラス内の状態を初期化する
        /// FormMain_Load（またはそのすぐあと）で実行される想定
        /// 各種イベントメソッドとの紐づけを行っている
        /// </summary>
        /// <param name="initPath"></param>
        public void InitializeValues(List<string> SupportedImageExtList)
        {
            _logger.PrintInfo("ImageViewerMainClass > InitializeValues");
            //_viewImage.SetPath(initPath);
            //_viewImageControl = new ViewImageControlPictureBox(_err, panel1, pictureBox1);
            // 画像を表示する
            //_viewImageControl.SetImageWithDispose(_viewImage.GetImage());
            // 初期化が終わらないとfileListも初期化されず、パス設定の処理に一貫性がなくなる
            //ViewImageFunction.ViewImageDefault();

            // PictureBox をドラッグする
            ControlDragger dragger = new ControlDragger(_logger, _pictureBox, _pictureBox);

            // Panel のクリックされた位置が右側半分か左側半分か判定する
            JudgeClickRightOrLeft judgeClickRightOrLeft = new JudgeClickRightOrLeft(_logger, _parentControl);

            // PictureBox のクリックされた位置が Panel の右側半分か左側半分か判定する
            JudgeClickRightOrLeftChild judgeClickRightOrLeftChild =
                new JudgeClickRightOrLeftChild(_logger, _pictureBox, _parentControl);
            judgeClickRightOrLeftChild.ClickRight += PictureBox_ClickRightEvent;
            judgeClickRightOrLeftChild.ClickLeft += PictureBox_ClickLeftEvent;

            // Panel が MouseWheel を受けたとき、PictureBox の大きさを変更する
            ChangeSizeByMouseWheel changeSizeByMouseWheel = new ChangeSizeByMouseWheel(
                _logger, _pictureBox, _parentControl);

            _viewImageFunction.InitializeValue();
        }

        public void ShowImageThisPath()
        {
            _logger.PrintInfo("ImageViewerMainClass > ShowImageThisPath");
            //_viewImageFunction.ViewImageDefault();
            //
            // ReadFileEventと同じ 240901
            //_viewImage.SetPath(_readFileByDragDrop.Files.GetCurrentValue());
            ImageMainFrame ImageMainFrame1 = (ImageMainFrame)_parentControl;
            string path = ImageMainFrame1._formFileList._files.GetCurrentValue();
            //ファイルパスが対応しているか（未対応）240831
            _viewImage.SetPath(path);
            // 画像を表示する
            //_viewImageControl.SetImageWithDispose(_viewImage.GetImage());
            _viewImageFunction.ViewImageDefault();
        }

        /// <summary>
        /// Initialize処理が終わった後に、最初に画像を表示する
        /// FormMain_Loadの最後か、すぐ後に実行される想定
        /// </summary>
        /// <param name="path"></param>
        public void ShowImageAfterInitialize(string path)
        {
            _logger.PrintInfo("ImageViewerMainClass > ShowImageAfterInitialize");
            _viewImage.SetPath(path);
            //_viewImageControl = new ViewImageControlPictureBox(_logger, panel1, pictureBox1);
            //画像を表示する
            _viewImageControl.SetImageWithDispose(_viewImage.GetImage());
            //初期化が終わらないとfileListも初期化されず、パス設定の処理に一貫性がなくなる
            _viewImageFunction.ViewImageDefault();
        }

        public void PictureBox_ClickRightEvent(object sender, EventArgs e)
        {
            //_readFileByDragDrop.FileListManager.MoveNextFileWhenLastFileNextDirectory();
            ImageMainFrame imageMainFrame = (ImageMainFrame)_parentControl;
            imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectory();
            ReadFileEvent(null, EventArgs.Empty);
        }
        public void PictureBox_ClickLeftEvent(object sender, EventArgs e)
        {
            //_readFileByDragDrop.FileListManager.MoveProviousFileWhenFirstFilePreviousDirectory();
            ImageMainFrame imageMainFrame = (ImageMainFrame)_parentControl;
            imageMainFrame._formFileList._fileListManager.MoveProviousFileWhenFirstFilePreviousDirectory();
            ReadFileEvent(null, EventArgs.Empty);
        }

        public void ReadFileEvent(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "ReadFileEvent");
                //_viewImage.SetPath(_readFileByDragDrop.Files.GetCurrentValue());
                ImageMainFrame ImageMainFrame1 = (ImageMainFrame)_parentControl;
                string path = ImageMainFrame1._formFileList._files.GetCurrentValue();
                //ファイルパスが対応しているか（未対応）240831
                _viewImage.SetPath(path);
                // 画像を表示する
                //_viewImageControl.SetImageWithDispose(_viewImage.GetImage());
                _viewImageFunction.ViewImageDefault();
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, "ReadFileEvent Error");
            }
        }

        public int ChangeImageByPath(String path)
        {
            try
            {
                int flag = _viewImage.SetPath(path);
                SetImageWithDispose(_viewImage.GetImage());
                return 1;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "setPath");
                return 0;
            }
        }

        public void SetImageWithDispose(Image image)
        {
            try
            {
                if (_pictureBox == null)
                {
                    _pictureBox.Image.Dispose();
                }
                _pictureBox.Image = image;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "SetImageWithDispose");
            }
        }

    }
}
