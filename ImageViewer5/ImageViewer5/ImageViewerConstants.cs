using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer5
{
    public static class ImageViewerConstants
    {
        public static readonly List<string> SUPPORTED_IMAGE_EXTENTION_DEFAULT_LIST = new List<string>{
            @"\.jpg$",
            @"\.jpeg$",
            @"\.png$",
            @"\.bmp$",   // Bitmap Image
            @"\.gif$",   // GIF Image
            @"\.tiff$",  // TIFF Image
            @"\.tif$",   // TIFF Image (alternate extension)
            @"\.webp$",   // WebP Image
        };
        // @"\.ico",

        /*
         * 複数のFrameを表示するときの設定値
         */
        // 配置する方向
        public static readonly int TO_RIGHT = 0;
        public static readonly int TO_LEFT = 1;
        public static readonly int TO_BOTTOM = 0;
        public static readonly int TO_TOP = 1;
        // 列方向、行方向どちらを優先して廃止していくか（上記で足りるかも）
        public static readonly int FRAME_PRIORITY_HORIZON = 0;
        public static readonly int FRAME_PRIORITY_VERTICAL = 1;
        // 行の表示Max、列の表示Max
        public static readonly int FRAME_ROW_MAX_DEFAULT = 2;
        public static readonly int FRAME_COL_MAX_DEFAULT = -1;
        // frameを再配置するとき、Frameのサイズを固定する、Formのサイズを固定する
        public static readonly int FIX_FRAME_SIZE = 0;
        public static readonly int FIX_FORM_SIZE = 1;

    }
}
