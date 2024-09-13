
using System;
using System.Windows.Forms;
using AppLoggerModule;
using ImageViewer5;
using ImageViewer5.CommonModules;
using ImageViewer5.ImageControl;
using PlayImageTest;
using ViewImageModule;
using PlayImageModule;
using System.IO;

namespace ImageViewer5.ImageControl.Function
{
    /// <summary>
    /// 画像表示する機能を主に扱う（ファイルパス→イメージ→表示）
    /// </summary>
    public class ImagePlayer
    {
        protected AppLogger _logger;
        protected IViewImage _viewImage;
        protected IViewImageControl _viewImageControl;
        protected IViewImageFrameControl _viewImageFrameControl;
        public ImageMainFrame _imageMainFrame;
        //#
        //画像表示関連
        private PlayGif _playGif;
        public ImagePlayer(
            AppLogger logger,
            IViewImage viewImage,
            IViewImageFrameControl viewImageFrameControl,
            IViewImageControl viewImageControl,
            ImageMainFrame imageMainFrame)
        {
            _logger = logger;
            _viewImage = viewImage;
            _viewImageControl = viewImageControl;
            _viewImageFrameControl = viewImageFrameControl;
            _imageMainFrame = imageMainFrame;
            this.Initialize();
        }

        public void Initialize()
        {
            try
            {
                _playGif = new PlayGif(_logger, (PictureBox)_viewImageControl.GetControl());

            }
            catch (Exception ex)
            {
                _logger.AddLogAlert(this, "Initialize Failed", ex);
            }
        }

        /// <summary>
        /// ViewImageにセットされたイメージを表示するメイン
        /// </summary>
        public void ViewImageDefault()
        {
            try
            {
                ViewImageFunction_FitInnerToFrame obj =
                    new ViewImageFunction_FitInnerToFrame(
                        _logger, _viewImageFrameControl, _viewImageControl, _viewImage);
                obj.FitImageToControl();
                _viewImageControl.SetImageWithDispose(_viewImage.GetImage());

            }
            catch (Exception ex)
            {
                _logger.AddLogAlert(this, "ViewImageDefault Failed", ex);
            }
        }

        /// <summary>
        /// 画像を表示するメイン関数
        /// ファイルパスを取得して、イメージに変換、ViewImageDefaultに渡す
        /// </summary>
        public void ViewImageMain()
        {
            try
            {
                // ReadFileEventと同じ 240901
                ImageMainFrame frameControl = (ImageMainFrame)_viewImageControl.GetParentControl();
                string path = frameControl._formFileList._files.GetCurrentValue();
                //ファイルパスが対応しているか（未対応）240831
                if (!File.Exists(path)){
                    _logger.PrintError(string.Format("ViewImageMain > Path Not Exists. [{0}]", path));
                    _viewImage.DisposeImage();
                    // 240913
                    // ここでreturnするとエラーとなる、
                    /*
                     *    場所 System.Drawing.Image.get_Width()
                           場所 System.Drawing.Image.get_Size()
                           場所 System.Windows.Forms.PictureBox.ImageRectangleFromSizeMode(PictureBoxSizeMode mode)
                           場所 System.Windows.Forms.PictureBox.OnPaint(PaintEventArgs pe)
                     */
                    //return;
                }
                _viewImage.SetPath(path);
                // 画像を表示する

                _playGif.ResetResouce();
                if (path.EndsWith(".gif"))
                {
                    _playGif.DisplayGif(path, 50);
                }
                else
                {
                    this.ViewImageDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.AddLogAlert(this, "ViewImageMain Failed", ex);
            }
        }
    } //End Class
}
