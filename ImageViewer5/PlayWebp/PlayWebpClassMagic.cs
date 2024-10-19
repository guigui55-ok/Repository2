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
using ImageMagick;


// NuGetパッケージ管理より、Magick.NET-Q16-AnyCPUを検索してインストールします。
namespace PlayWebp
{
    public class PlayAnimatedWebp
    {
        private PictureBox _pictureBox;
        private Timer _timer;
        private MagickImageCollection _frames;
        private int _currentFrameIndex;

        public PlayAnimatedWebp(PictureBox pictureBox)
        {
            _pictureBox = pictureBox;
            _timer = new Timer();
            _timer.Interval = 100;  // デフォルトのフレーム間隔 (100ms)
            _timer.Tick += OnTick;
        }

        public void Dispose()
        {
            try
            {
                if(_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= OnTick;
                    _timer.Dispose();
                    _timer = null;
                }
                if (_frames != null)
                {
                    _frames.Dispose();
                }
            } catch ( Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }


        public Image GetImageFirstFrameFromFile(string path)
        {
            Image image = null;
            try
            {
                // WebPアニメーションを読み込む
                _frames = new MagickImageCollection(path);

                // 最初のフレームを表示
                _currentFrameIndex = 0;
                image = GetFrame(_currentFrameIndex);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            return image;
        }

        public void SetImageFromFile(string path)
        {
            try
            {
                // WebPアニメーションを読み込む
                _frames = new MagickImageCollection(path);

                // 最初のフレームを表示
                _currentFrameIndex = 0;
                _pictureBox.Image = GetFrame(_currentFrameIndex);

                // アニメーションを開始
                int interval;
                interval = (int)_frames[_currentFrameIndex].AnimationDelay * 10; // 10ms単位
                if (0 < interval)
                {
                    _timer.Interval = interval;
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            // 次のフレームを表示
            _currentFrameIndex = (_currentFrameIndex + 1) % _frames.Count;
            _pictureBox.Image = GetFrame(_currentFrameIndex);

            // 次のフレームの遅延時間に基づいてタイマーの間隔を設定
            _timer.Interval = (int)_frames[_currentFrameIndex].AnimationDelay * 10;
        }

        private Image GetFrameSimple(int index)
        {
            using (var ms = new MemoryStream())
            {
                // フレームをPNG形式でストリームに書き込む
                _frames[index].Write(ms, MagickFormat.Png);
                ms.Position = 0;
                return Image.FromStream(ms); // ストリームからImageを作成
            }
        }

        private Image GetFrame(int index)
        {
            using (var ms = new MemoryStream())
            {
                // フレームをPNG形式でストリームに書き込む
                _frames[index].Write(ms, MagickFormat.Png);
                ms.Position = 0;

                // ストリームから画像を読み込み
                Image originalImage = Image.FromStream(ms);

                // スケーリングして新しいImageを返す
                return ScaleImageToFitPictureBox(originalImage);
            }
        }

        private Image ScaleImageToFitPictureBox(Image originalImage)
        {
            // PictureBoxのサイズを取得
            int pbWidth = _pictureBox.Width;
            int pbHeight = _pictureBox.Height;

            // 画像の元サイズを取得
            int imgWidth = originalImage.Width;
            int imgHeight = originalImage.Height;

            // PictureBoxに収まるようにスケーリングし、縦横比を維持する
            double ratioX = (double)pbWidth / imgWidth;
            double ratioY = (double)pbHeight / imgHeight;
            double ratio = Math.Min(ratioX, ratioY); // 縦横比を維持するために小さい方の比率を使用

            int newWidth = (int)(imgWidth * ratio);
            int newHeight = (int)(imgHeight * ratio);

            // スケーリングした画像を新しく作成
            var scaledImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(scaledImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return scaledImage;
        }
    }
}
