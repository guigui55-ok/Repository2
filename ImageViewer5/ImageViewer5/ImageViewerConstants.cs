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

    }
}
