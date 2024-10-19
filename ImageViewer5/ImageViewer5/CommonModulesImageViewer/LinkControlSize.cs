using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace ImageViewer5.CommonModulesImageViewer
{
    public class LinkControlSize
    {
        protected AppLogger _logger;
        protected Control _outerControl;
        protected Control _innerControl;
        protected Control _parentControl;
        public bool _isEnable;
        public LinkControlSize(AppLogger logger, Control innerControl, Control outerControl, Form parentControl, bool isEnable)
        {
            _isEnable = isEnable;
            _logger = logger;
            _innerControl = innerControl;
            _outerControl = outerControl;
            _parentControl = parentControl;
            _parentControl.ClientSizeChanged += LinkControlSize_ClientSizeChanged;
        }

        public void DisposeObjects()
        {
            try
            {
                _parentControl.ClientSizeChanged -= LinkControlSize_ClientSizeChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void LinkControlSize_ClientSizeChanged(object sender , EventArgs e)
        {
            if (_isEnable)
            {
                _outerControl.Size = _parentControl.ClientSize;
                _innerControl.Size = _parentControl.ClientSize;
                _innerControl.Location = new System.Drawing.Point(0, 0);
            }
        }
    }
}
