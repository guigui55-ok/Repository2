using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewImageObjects
{
    /// <summary>
    /// ViewImage、ViewImageFrameの外のForm、FormMainの処理を扱う
    /// 
    /// </summary>
    public class ViewImageFrameControlForm : IViewImageFrameControl
    {
        ErrorManager.ErrorManager _err;
        Form _form;
        public ViewImageFrameControlForm(ErrorManager.ErrorManager err,Form form)
        {
            _err = err;
            _form = form;
        }

        public Size GetSize()
        {
            if(_form != null)
            {
                return _form.Size;
            } else
            {
                _err.AddLogAlert(this, "GetSize , _form != null");
                return new Size(0,0);
            }
        }
    }
}
