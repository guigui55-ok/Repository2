using System;
using System.Drawing;

namespace ImageViewer.Values
{
    public interface IViewImage
    {
        int SetPath(String path);
        Image GetImage();

        string GetPath();
    }
}
