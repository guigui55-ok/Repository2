using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using ImageViewer5;
using ImageViewer5.CommonModules;
using ImageViewer5.ImageControl;
using ImageViewer5.ImageControl.Function;

namespace ViewImageModule
{
    /// <summary>
    /// 表示する機能についての処理を扱う
    /// 表示時の振る舞いなどについても扱う
    /// </summary>
    public class ViewImageFunction
    {
        protected AppLogger _logger;
        protected IViewImage _viewImage;
        protected IViewImageControl _viewImageControl;
        protected IViewImageFrameControl _viewImageFrameControl;
        public ImageMainFrame _imageMainFrame;
        public LinkControlSize _linkControlSize;
        public ViewImageSlideShow _viewImageSlideShow;
        //
        public ImagePlayer _imagePlayer;

        public ViewImageFunction(
            AppLogger logger,
            IViewImage viewImage,
            IViewImageFrameControl viewImageFrameControl,
            IViewImageControl viewImageControl,
            ImageMainFrame imageMainFrame)
        {
            _logger = logger;
            _imageMainFrame = imageMainFrame;
            _viewImage = viewImage;
            _viewImageControl = viewImageControl;
            _viewImageFrameControl = viewImageFrameControl;
            _imagePlayer = new ImagePlayer(
                _logger, _viewImage, _viewImageFrameControl, _viewImageControl, imageMainFrame);
        }

        public void InitializeValue()
        {
            _logger.PrintInfo("ViewImageFunction > InitializeValue");
            //
            ImageMainFrame frameControl = (ImageMainFrame)_viewImageControl.GetParentControl();
            Control innerControl = (Control)_viewImageControl.GetControl();
            Form formMain = (Form)frameControl.Parent;
            _linkControlSize = new LinkControlSize(
                _logger, innerControl, frameControl, formMain, frameControl._imageMainFrameSetting._isFitFormMain);
        }

        public void InitializeValue_LoadAfter()
        {
            _logger.PrintInfo("ViewImageFunction > InitializeValue_LoadAfter");
            //_imageMainFrame._formFileListがLoadの後に設定されるので、そのあとでなければならない
            _viewImageSlideShow = new ViewImageSlideShow(_logger, _imageMainFrame);
            _viewImageSlideShow._SlideShowTimer.Tick += _imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectoryEvent;
        }
        
        /// <summary>
        /// ViewImageにセットされたイメージを表示するメイン
        /// </summary>
        public void ViewImageDefault()
        {
            _imagePlayer.ViewImageDefault();
        }

    }//class
}
