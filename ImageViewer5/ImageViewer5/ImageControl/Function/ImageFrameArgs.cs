using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace ImageViewer5.ImageControl.Function
{
    class ImageFrameArgs
    {
        AppLogger _logger;
        ImageMainFrame _imageMainFrame;
        public ImageFrameArgs(AppLogger logger, ImageMainFrame imageMainFrame)
        {
            _logger = logger;
            _imageMainFrame = imageMainFrame;
        }

        public void ExecuteFunctionByArgs(Dictionary<string, object> frameArgsDictionary)
        {

        }
    }
}
