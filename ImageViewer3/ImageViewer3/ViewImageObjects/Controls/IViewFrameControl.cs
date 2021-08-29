using ImageViewer.Values;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Controls
{
    // Panel2
    public interface IViewFrameControl
    {
        IViewInnerControl ViewInnerControl { get; set; }
        IViewControlState State { get; set; }

        Control GetControl();
        void ChangeSize(Size size);
        void ChangeLocation(Point point);
        void PausePaint(bool flag);
        void SetVisible(bool flag);

        void ChangeSizeAndLocation(Size size, Point location);
        //void SaveRaitoSizeAndPositionFromContentsControl();
        void SaveRaitoSizeAndPositionFromContentsControl(bool IsParentControlSizeChanging);
        Size GetSize();
        Point GetLocation();
    }
}
