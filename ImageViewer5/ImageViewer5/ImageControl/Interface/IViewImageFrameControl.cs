using System.Drawing;
using System.Windows.Forms;

namespace ViewImageModule
{
    /// <summary>
    /// ViewImage、ViewImageFrameの外のForm、FormMainの処理を扱う
    /// 
    /// </summary>
    public interface IViewImageFrameControl
    {
        Size GetSize();

        Control GetControl();

        //Control GetParentControl();
        Form GetParentForm();
    }
}
