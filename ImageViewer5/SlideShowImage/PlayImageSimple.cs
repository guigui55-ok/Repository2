using System;
using System.Drawing;
using System.Windows.Forms;
using AppLoggerModule;
using SlideShowImage;

namespace PlayImageModule
{
    public class PlayImageSimple
    {
        private AppLogger _logger;
        private Control _control;
        private Image _image;
        public bool isFadeOn = false;

        // コンストラクタでControlを指定する
        public PlayImageSimple(AppLogger logger, Control control)
        {
            _logger = logger;
            _control = control;
        }

        // 静止画（JPG/PNGなど）をControlに表示するメソッド
        public void Display(string imagePath)
        {
            if (_control == null || string.IsNullOrEmpty(imagePath))
            {
                return;
            }

            try
            {
                // 以前の画像を解放する
                if (_image != null)
                {
                    _image.Dispose();
                }

                // 新しい画像をロード
                _image = Image.FromFile(imagePath);

                // Controlのサイズに合わせて、縦横比を保持しながらスケーリング
                var scaleFactor = Math.Min((float)_control.Width / _image.Width, (float)_control.Height / _image.Height);
                var width = (int)(_image.Width * scaleFactor);
                var height = (int)(_image.Height * scaleFactor);
                var offsetX = (_control.Width - width) / 2;
                var offsetY = (_control.Height - height) / 2;

                // 描画用のバッファを作成して画像をスケーリングして描画
                using (var bufferedImage = new Bitmap(_control.Width, _control.Height))
                using (var g = Graphics.FromImage(bufferedImage))
                {
                    g.Clear(_control.BackColor); // 背景をクリア
                    g.DrawImage(_image, new Rectangle(offsetX, offsetY, width, height));

                    // Controlに画像を表示（背景に描画）
                    _control.BackgroundImage?.Dispose();
                    _control.BackgroundImage = new Bitmap(bufferedImage);
                }
            }
            catch (Exception ex)
            {
                _logger.PrintError($"画像の表示に失敗しました: {ex.Message}");
            }
        }

        // 現在の画像を取得するメソッド
        public Image GetCurrentImage()
        {
            return _image;
        }

        // 画像の表示を停止するメソッド
        public void ClearImage()
        {
            if (_control.BackgroundImage != null)
            {
                _control.BackgroundImage.Dispose();
                _control.BackgroundImage = null;
            }
            if (_control.GetType() == typeof(PictureBox))
            {
                PictureBox pix = (PictureBox)_control;
                if (pix.Image != null)
                {
                    pix.Image.Dispose();
                    pix.Image = null;
                }
            }

            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }
        }
    }
}
