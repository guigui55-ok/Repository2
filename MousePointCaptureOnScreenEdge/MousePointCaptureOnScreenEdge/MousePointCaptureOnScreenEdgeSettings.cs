using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MousePointCapture
{
    public class MousePointCaptureOnScreenEdgeSettings
    {
        public bool IsValidCapture = true;
        public bool IsCaputureLeft;
        public bool IsCaptureTop;
        public bool IsCaptureRight;
        public bool IsCaptureBottom;

        public int[] LeftValidRange;
        public int[] TopValidRange;
        public int[] RightValidRange;
        public int[] BottomValidrange;

        public int[] ScreenNumbers = new int[] {1,1,1,1};
    }
}
