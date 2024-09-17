using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using SkiaSharp;


// SkiaSharpは直接WebPのアニメーション再生をサポートしていません
namespace PlayWebp
{
    public class PlayWebpSikaSharp
    {
        //AppLogger _logger;
        PictureBox _pictureBox;
        public PlayWebpSikaSharp(PictureBox pictureBox)
        {
            //_logger = logger;
            _pictureBox = pictureBox;
        }

        public void SetImageFromFile(string path)
        {
            try
            {
                Image image = ReadImageFromFile(path);
                // 以前の画像を破棄
                if (_pictureBox.Image != null)
                {
                    _pictureBox.Image.Dispose();
                    _pictureBox.Image = null;
                }
                _pictureBox.Image = image;
            } catch (Exception ex)
            {
                Debug.WriteLine("Error");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public Image ReadImageFromFile(string path)
        {
            Image _image = null;
            try
            {
                // SkiaSharpを使ってWebP画像を読み込む
                using (var inputStream = File.OpenRead(path))
                {
                    var skBitmap = SKBitmap.Decode(inputStream);
                    using (var skImage = SKImage.FromBitmap(skBitmap))
                    using (var data = skImage.Encode(SKEncodedImageFormat.Png, 100)) // PictureBox用にPNG形式に変換
                    using (var ms = new MemoryStream(data.ToArray()))
                    {
                        _image = Image.FromStream(ms); // SKImageをSystem.Drawing.Imageに変換
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            if (_image != null)
            {
                Debug.WriteLine(string.Format("image.size = {0},{1}", _image.Width, _image.Height));
                Debug.WriteLine(string.Format("PictureBox.Location = {0}", _pictureBox.Location));
            }
            return _image;
        }
    }
}
