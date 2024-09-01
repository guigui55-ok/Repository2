using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using ImageViewer5.CommonModules;
using ImageViewer5.ImageControl;

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
        public LinkControlSize _linkControlSize;

        public ViewImageFunction(
            AppLogger logger,
            IViewImage viewImage,
            IViewImageFrameControl viewImageFrameControl,
            IViewImageControl viewImageControl)
        {
            _logger = logger;
            _viewImage = viewImage;
            _viewImageControl = viewImageControl;
            _viewImageFrameControl = viewImageFrameControl;
        }

        public void InitializeValue()
        {
            _logger.PrintInfo("ViewImageFunction > InitializeValue");
            //
            ImageMainFrame frameControl = (ImageMainFrame)_viewImageControl.GetParentControl();
            Form formMain = (Form)frameControl.Parent;
            _linkControlSize = new LinkControlSize(
                _logger, frameControl, formMain, frameControl._imageMainFrameSetting._isFitFormMain);
        }

        public void ViewImageDefault()
        {
            try
            {
                ViewImageFunction_FitInnerToFrame obj =
                    new ViewImageFunction_FitInnerToFrame(
                        _logger, _viewImageFrameControl, _viewImageControl, _viewImage);
                obj.FitImageToControl();
                _viewImageControl.SetImageWithDispose(_viewImage.GetImage());

            } catch (Exception ex)
            {
                _logger.AddLogAlert(this, "ViewImageDefault Failed"+" > "+ "ViewImageDefault Failed", ex);
            }
        }
    }
}
