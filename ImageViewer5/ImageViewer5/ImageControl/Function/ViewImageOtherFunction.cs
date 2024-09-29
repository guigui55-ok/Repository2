using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace ImageViewer5.ImageControl.Function
{
    /// <summary>
    /// ImageMainFrameのための、その他のクラス
    /// 　、とりあえず実装したい機能などもここで扱う
    /// </summary>
    public class ViewImageOtherFunction
    {
        AppLogger _logger;
        ImageMainFrame _imageMainFrame;

        public ViewImageOtherFunction(AppLogger logger, ImageMainFrame imageMainFrame)
        {
            _logger = logger;
            _imageMainFrame = imageMainFrame;            
        }

        public void Initialize()
        {
            _logger.PrintInfo("ViewImageOtherFunction > Initialize > " + _imageMainFrame.Name);
            _imageMainFrame._imageViewerMain._parentControl.MouseDown += this.ImageMainFrameAny_MouseDown;
            _imageMainFrame._imageViewerMain._pictureBox.MouseDown += this.ImageMainFrameAny_MouseDown;
        }

        public void ImageMainFrameAny_MouseDown(object sender , EventArgs e)
        {
            _logger.PrintInfo("ViewImageOtherFunction > ImageMainFrameAny_MouseDown > " + _imageMainFrame.Name);
            FormMain formMain = ViewImageCommon.ConvertToFormMain(_imageMainFrame.Parent);
            formMain._mainFrameManager.ClearFrameFocusFlag();
            _imageMainFrame._imageViewerMain._isFocusFrame = true;
        }
    }
}
