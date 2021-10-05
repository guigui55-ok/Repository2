using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewImageObjects
{
    public class ViewImageFunction
    {
        protected ErrorManager.ErrorManager _err;
        protected IViewImage _viewImage;
        protected IViewImageControl _viewImageControl;
        protected IViewImageFrameControl _viewImageFrameControl;

        public ViewImageFunction(ErrorManager.ErrorManager err,
            IViewImage viewImage,IViewImageFrameControl viewImageFrameControl,IViewImageControl viewImageControl)
        {
            _err = err;
            _viewImage = viewImage;
            _viewImageControl = viewImageControl;
            _viewImageFrameControl = viewImageFrameControl;
        }

        public void ViewImageDefault()
        {
            try
            {
                ViewImageFunction_FitInnerToFrame obj =
                    new ViewImageFunction_FitInnerToFrame(_err, _viewImageFrameControl, _viewImageControl, _viewImage);
                obj.FitImageToControl();

                _viewImageControl.SetImageWithDispose(_viewImage.GetImage());

            } catch (Exception ex)
            {
                _err.AddLogAlert(this, "ViewImageDefault Failed", "ViewImageDefault Failed", ex);
            }
        }
    }
}
