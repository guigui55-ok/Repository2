using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Imaging.Filters.EdgeDetection;
using ImageProcessor.Imaging.Filters.Photo;


// NuGetパッケージ管理にて、SkiaSharpを検索して、SkiaSharpとSkiaSharp.Views.WindowsFormsの2つのパッケージをインストールします。
namespace PlayWebp
{
    public class PlayWebpImageProcessor
    {
        //AppLogger _logger;
        PictureBox _pictureBox;
        public PlayWebpImageProcessor(PictureBox pictureBox)
        {
            //_logger = logger;
            _pictureBox = pictureBox;
        }

        public void SetImageFromFile(string path)
        {
            try
            {
                Image image = ReadImageFromFile(path);
                _pictureBox?.Dispose();
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
                // ファイルの読み込み
                byte[] source = File.ReadAllBytes(path);
                using (MemoryStream inStream = new MemoryStream(source))
                {
                    _image = new ImageFactory()
                      .Load(inStream)
                      .Image;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            return _image;
        }
    }
}
