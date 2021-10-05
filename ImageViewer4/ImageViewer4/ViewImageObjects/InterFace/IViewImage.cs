using System;
using System.Drawing;

namespace ViewImageObjects
{
    public interface IViewImage
    {
        int SetPath(String path);
        Image GetImage();

        string GetPath();
    }
}
