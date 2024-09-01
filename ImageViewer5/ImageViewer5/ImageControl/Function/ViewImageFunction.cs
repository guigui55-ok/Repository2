using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace ViewImageModule
{
    public class ViewImageFunction
    {
        protected AppLogger _logger;
        protected IViewImage _viewImage;
        protected IViewImageControl _viewImageControl;
        protected IViewImageFrameControl _viewImageFrameControl;

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
