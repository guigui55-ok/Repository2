using System;
using System.Drawing;
using System.Windows.Forms;

namespace ViewImageObjects
{
    // PictureBox
    public interface IViewImageControl
    {
        //IViewImageSettings Settings { get; set; }
        //IViewControlState State { get; set; }

        int SetControl(Control ViewControl);
        int SetParentControl(Control ParentControl);
        int Initialize();
        void SetImageWithDispose(Image image);
        void SetImageNotDispose(Image image);
        Size GetSize();
        Point GetLocation();
        void ChangeSize(Size size);
        void SetVisible(bool flag);
        void ChangeLocation(Point point);
        void RefreshPaint();
        void PausePaint(bool flag);
        Control GetParentControl();
        Control GetControl();
        void SuspenLayout();
        void ResumeLayout();
    }
}
