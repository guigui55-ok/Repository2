using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;


//対応ケース１
//https://qiita.com/visyeii/items/9320b0d08e6b132c6541
//Nugetで「ImageProcessor」パッケージをインストールする
//ImageFactoryなどで読み込む

//対応ケース２
//NuGetでSixLabors.ImageSharpとSixLabors.ImageSharp.WebPライブラリをインストール

namespace PlayWebpModule
{
    //internal class PlayWebpClassB
    //{
    //    public void GetImage(string inPutPath, string outPutPath, ImageFormat format)
    //    {
    //        var wf = new WebPFormat();

    //        using (var image = (Bitmap)wf.Load(new FileStream(inPutPath, FileMode.Open, FileAccess.Read)))
    //        {
    //            image.Save(outPutPath, format);
    //        }
    //    }
    //}
}
