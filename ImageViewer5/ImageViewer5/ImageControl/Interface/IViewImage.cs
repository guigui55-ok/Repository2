using System;
using System.Drawing;

namespace ViewImageModule
{
    public interface IViewImage
    {
        int SetPath(String path);
        int SetImage(Image image);
        Image GetImage();

        string GetPath();

        bool ImageIsNull();

        bool DisposeImage();

    }
}
