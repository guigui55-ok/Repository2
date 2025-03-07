﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using ImageViewer5;
using ImageViewer5.CommonModulesImageViewer;
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
        //外部からの移譲
        public ImageMainFrame _imageMainFrame;
        protected AppLogger _logger;
        protected IViewImage _viewImage;
        protected IViewImageControl _viewImageControl;
        protected IViewImageFrameControl _viewImageFrameControl;
        // クラス内で生成
        public LinkControlSize _linkControlSize;
        public ViewImageSlideShow _viewImageSlideShow;
        //
        public ImagePlayer _imagePlayer;
        public ViewImageFunction_FitInnerToFrame _viewImageFunction_FitInnerToFrame;
        public ViewImageOtherFunction _viewImageOtherFunction;
        //複数同時に実行されることがあるため
        public string Name;

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
            _viewImageFunction_FitInnerToFrame = new ViewImageFunction_FitInnerToFrame(
                _logger, _viewImageFrameControl, _viewImageControl, _viewImage);
            this.Name = "ViewImageFunction" + imageMainFrame.GetComponentNumber();
            _viewImageOtherFunction = new ViewImageOtherFunction(_logger, imageMainFrame);
        }

        public void Dispose()
        {
            try
            {
                _linkControlSize.DisposeObjects();
                _linkControlSize = null;
                _viewImageSlideShow.Dispose();
                _viewImageSlideShow = null;
                _imagePlayer.Dispose();
                _imagePlayer = null;
                _viewImageFunction_FitInnerToFrame.Dispose();
                _viewImageFunction_FitInnerToFrame = null;
                _viewImageOtherFunction.Dispose();
                _viewImageOtherFunction = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void InitializeValue()
        {
            _logger.PrintInfo(this.Name + " > InitializeValue");
            //
            ImageMainFrame frameControl = (ImageMainFrame)_viewImageControl.GetParentControl();
            Control innerControl = (Control)_viewImageControl.GetControl();
            Form formMain = (Form)frameControl.Parent;
            _linkControlSize = new LinkControlSize(
                _logger, innerControl, frameControl, formMain, frameControl._imageMainFrameSetting._isFitFormMain);
        }

        public void InitializeValue_LoadAfter()
        {
            _logger.PrintInfo(this.Name + " > InitializeValue_LoadAfter");
            //_imageMainFrame._formFileListがLoadの後に設定されるので、そのあとでなければならない
            _viewImageSlideShow = new ViewImageSlideShow(_logger, _imageMainFrame);
            _viewImageSlideShow.Initialize_LoadAfter();
            //_viewImageSlideShow._SlideShowTimer.Tick += _imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectoryEvent;
            _viewImageOtherFunction.Initialize();
            ForDebug();
        }

        public void ForDebug()
        {
            _imageMainFrame.BackColor = Color.AliceBlue;
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
