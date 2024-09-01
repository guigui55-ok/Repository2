using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace ImageViewer5.CommonModules
{
    public class LinkControlSize
    {
        protected AppLogger _logger;
        protected Control _innerControl;
        protected Control _parentControl;
        public bool _isEnable;
        public LinkControlSize(AppLogger logger, Control control, Form parentControl, bool isEnable)
        {
            _isEnable = isEnable;
            _logger = logger;
            _innerControl = control;
            _parentControl = parentControl;
            _parentControl.ClientSizeChanged += LinkControlSize_ClientSizeChanged;
        }

        private void LinkControlSize_ClientSizeChanged(object sender , EventArgs e)
        {
            if (_isEnable)
            {
                _innerControl.Size = _parentControl.ClientSize;
            }
        }
    }
}
