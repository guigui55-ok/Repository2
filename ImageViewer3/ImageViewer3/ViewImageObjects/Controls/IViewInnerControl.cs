using System;
using System.Windows.Forms;
using System.Drawing;
using ErrorLog;
using ImageViewer.Values;

namespace ImageViewer.Controls
{
    // Panel3
    public interface IViewInnerControl 
    {
        IViewImageControl ViewImageControl { get; set; }
        IViewImageSettings Settings { get; set; }
        IViewControlState State { get; set; }

        int SetControlState(IViewControlState viewControlState);
        Control GetControl();
        int SetViewImageControl(IViewImageControl viewImageControl);
        int Initialize();
        void SetImageWithDispose(Image image);
        void SetImageNotDispose(Image image);
        Size GetSize();
        Point GetLocation();
        void ChangeSize(Size size);
        void SetVisible(bool flag);
        void ChangeLocation(Point point);
        void RefreshPaint();
        Control GetParentControl();
        void SaveRaitoSizeAndPositionFromFrameControl();

        /// <summary>
        /// コントロールの拡大・縮小
        /// <param name="raito">現在のサイズからの倍率</param>
        /// </summary>
        void ExpansionSizeViewControl(double raito);
        Point GetLocationByCalcExpansionWhenChangeSize(Size beforeSize, Size afterSize);
        void ChangeSizeAndLocation(Size size, Point location);
        void PausePaint(bool flag);

    }
}
