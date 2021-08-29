using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Functions
{
    public class InvokeFunctionViewImageObjects
    {
        public int MaintainSizeAndPositionRatioContentsAndFrame(ViewImageObjects viewImageObjects)
        {
            // 描画をやめる
            viewImageObjects.Functions.ForControl.PasePaint(true);
            // FramePanel の位置とサイズを復元する
            viewImageObjects.Functions.ForControl.MaintainSizeAndPositionRatioContentsAndFrame(
                viewImageObjects.MainContentsControl.Size);
            // 描画を再開
            viewImageObjects.Functions.ForControl.PasePaint(false);
            return 1;
        }
    }
}
