using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using AppLoggerModule;

namespace PlayImageModule
{
    public class PlayGif
    {
        public AppLogger _logger;
        private PictureBox _pictureBox;
        private Image _gifImage;
        private FrameDimension _frameDimension;
        private int _currentFrame;
        private Timer _timer;
        private Image _currentFrameImage; // 現在のフレームのイメージを保持

        // コンストラクタでPictureBoxを指定する
        public PlayGif(AppLogger logger, PictureBox pictureBox)
        {
            _logger = logger;
            _pictureBox = pictureBox;
            _timer = new Timer();
            _timer.Tick += new EventHandler(OnFrameChanged);
        }

        public void Dispose()
        {
            try
            {
                if (_gifImage != null)
                {
                    _gifImage.Dispose();
                    _gifImage = null;
                }
                if (_frameDimension != null)
                {
                    _frameDimension = null;
                }
                if (_currentFrameImage != null)
                {
                    _currentFrameImage.Dispose();
                    _currentFrameImage = null;
                }
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= OnFrameChanged;
                    _timer.Dispose();
                    _timer = null;
                }
            } catch ( Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// // 現在のタイマーを停止し、フレームとリソースをリセット
        /// </summary>
        public void ResetResouce()
        {
            _timer.Stop();
            _currentFrame = 0;

            if (_gifImage != null)
            {
                _gifImage.Dispose();
            }
        }

        // GIF画像をPictureBoxに表示し、再生速度を設定するメソッド
        public void DisplayGif(string gifPath, int speed = 100)
        {
            if (_pictureBox == null || string.IsNullOrEmpty(gifPath))
            {
                return;
            }

            try
            {
                // 現在のタイマーを停止し、フレームとリソースをリセット
                ResetResouce();

                _gifImage = Image.FromFile(gifPath);
                _frameDimension = new FrameDimension(_gifImage.FrameDimensionsList[0]);

                // タイマーを設定してGIFの再生を開始
                _timer.Interval = speed; // スピード調整 (ミリ秒単位)
                _timer.Start();
            }
            catch (System.Exception ex)
            {
                _logger.PrintError($"GIF画像の表示に失敗しました: {ex.Message}");
            }
        }

        // GIF画像のフレームを変更するイベントハンドラ
        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (_gifImage != null)
            {
                _gifImage.SelectActiveFrame(_frameDimension, _currentFrame);

                // PictureBoxのサイズに合わせて、縦横比を保持しながらスケーリング
                var scaleFactor = Math.Min((float)_pictureBox.Width / _gifImage.Width, (float)_pictureBox.Height / _gifImage.Height);
                var width = (int)(_gifImage.Width * scaleFactor);
                var height = (int)(_gifImage.Height * scaleFactor);
                var offsetX = (_pictureBox.Width - width) / 2;
                var offsetY = (_pictureBox.Height - height) / 2;

                // 描画用のバッファを作成してGIFをスケーリングして描画
                using (var bufferedImage = new Bitmap(_pictureBox.Width, _pictureBox.Height))
                using (var g = Graphics.FromImage(bufferedImage))
                {
                    g.Clear(_pictureBox.BackColor); // 背景をクリア
                    g.DrawImage(_gifImage, new Rectangle(offsetX, offsetY, width, height));
                    _pictureBox.Image?.Dispose(); // 古いイメージを解放
                    _pictureBox.Image = new Bitmap(bufferedImage);
                    _currentFrameImage?.Dispose(); // 古いフレームイメージを解放
                    _currentFrameImage = new Bitmap(bufferedImage); // 現在のフレームイメージを保存
                }

                _currentFrame = (_currentFrame + 1) % _gifImage.GetFrameCount(_frameDimension);
            }
        }

        // 現在のフレームイメージを取得するメソッド
        public Image GetCurrentFrameImage()
        {
            return _currentFrameImage;
        }

        // GIF画像の再生を停止するメソッド
        public void StopGif()
        {
            _timer.Stop();

            if (_pictureBox.Image != null)
            {
                _pictureBox.Image.Dispose();
                _pictureBox.Image = null;
            }

            if (_gifImage != null)
            {
                _gifImage.Dispose();
                _gifImage = null;
            }

            if (_currentFrameImage != null)
            {
                _currentFrameImage.Dispose();
                _currentFrameImage = null;
            }
        }
    }
}
