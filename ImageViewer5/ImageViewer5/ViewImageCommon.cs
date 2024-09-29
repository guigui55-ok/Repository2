using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer5
{
    public class ViewImageCommon
    {
        public ViewImageCommon()
        {

        }

        static public FormMain ConvertToFormMain(Control control)
        {
            return (FormMain)control;
        }
    }
}
