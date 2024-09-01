using System;
using System.Drawing;

namespace ViewImageModule
{
    public interface IViewImage
    {
        int SetPath(String path);
        Image GetImage();

        string GetPath();

        bool ImageIsNull();

    }
}
