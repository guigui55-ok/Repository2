using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer5
{
    public class FormMainSetting
    {
        public int formMainCount = 2;
        //#
        //Frame の配置の決め方（Frameが複数あるときに、配置するときに使用する）
        // ToRight , ToLeft
        // ToDonw , ToUp
        // Priority, Horizon, Vertical
        // FixFrameSize(Formのサイズを大きくする）、FixFormSize（Window内でFrameのサイズを調整する）
        //#
        public int frameDirectionHorizon = ImageViewerConstants.TO_RIGHT;
        public int frameDirectionVertical = ImageViewerConstants.TO_BOTTOM;
        public int framePreferredDirection = ImageViewerConstants.FRAME_PRIORITY_HORIZON;
        public int frameRowMax = ImageViewerConstants.FRAME_ROW_MAX_DEFAULT;
        public int frameColMax = ImageViewerConstants.FRAME_COL_MAX_DEFAULT;
        public int iIsFixFrameSize = ImageViewerConstants.FIX_FRAME_SIZE;

    }
}
