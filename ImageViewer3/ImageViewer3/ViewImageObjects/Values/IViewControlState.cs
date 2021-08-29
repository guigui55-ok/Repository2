using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Values
{
    public interface IViewControlState
    {
        bool IsInitializeControl { get; set; }
        bool IsFrameSizeChanging { get; set; }
        bool IsActiveControl { get; set; }
        bool NowSizeUpdate { get; set; }
        bool IsPausePaint { get; set; }
        bool IsMouseHover { get; set; }

        PointF DifferenceSizeFromContents { get; set; }
        PointF DifferencePositionFromContents { get; set; }
        Point DifferenceSizeInnerFromFrame { get; set; }
        Point DifferencePositionInnerInFrame { get; set; }

        PointF RatioSizeInnerFromFrame { get; set; }
        //PointF RatioLocationInnerFromFrame { get; set; }
        double RatioLocationInnerFromFrameX { get; set; }
        double RatioLocationInnerFromFrameY { get; set; }


        PointF RaitoSizeFrameFromContents { get; set; }
        double RaitoLocationFrameFromContentsX { get; set; }
        double RaitoLocationFrameFromContentsY { get; set; }
        }
}
