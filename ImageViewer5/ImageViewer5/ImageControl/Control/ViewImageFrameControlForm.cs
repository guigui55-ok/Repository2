using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace ViewImageModule
{
    /// <summary>
    /// ViewImage、ViewImageFrameの外のForm、FormMainの処理を扱う
    /// 
    /// </summary>
    public class ViewImageFrameControlForm : IViewImageFrameControl
    {
       AppLogger _logger;
        Form _form;
        public ViewImageFrameControlForm(AppLogger logger, Form form)
        {
            _logger = logger;
            _form = form;
        }

        public Control GetControl()
        {
            return _form;
        }

        public Size GetSize()
        {
            if(_form != null)
            {
                return _form.Size;
            } else
            {
                _logger.AddLogAlert(this, "GetSize , _form != null");
                return new Size(0,0);
            }
        }
    }
}
