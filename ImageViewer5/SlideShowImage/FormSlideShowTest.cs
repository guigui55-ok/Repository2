using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using System.IO;
using PlayImageModule;

namespace SlideShowImage
{
    public partial class FormSlideShowTest : Form
    {
        AppLogger _logger;
        private List<string> _pathList;
        private int _currentIndex;
        //PlayImage _playImage;
        PlayImageWithFade _playImage;

        SlideShowFunction _slideShowFunction;

        EffectFadeSetting _effectFadeSetting;
        DrawImageFade _drawImageFade;


        public FormSlideShowTest()
        {
            InitializeComponent();
        }

        private void FormSlideShowTest_Load(object sender, EventArgs e)
        {

            _logger = new AppLogger();
            pictureBox1.Dock = DockStyle.Fill;
            //_playGif = new PlayGif(_logger, pictureBox1);
            //_playImage = new PlayImage(_logger, pictureBox1);
            _playImage = new PlayImageWithFade(_logger, pictureBox1);
            _playImage.isFadeOn = true;
            _playImage.isFadeOn = false;
            _logger.PrintInfo(string.Format("_playImage.isFadeOn = {0}", _playImage.isFadeOn));
            _slideShowFunction = new SlideShowFunction(_logger, pictureBox1);
            _slideShowFunction.SetTimerTick(ShowNext);
            //
            _effectFadeSetting = new EffectFadeSetting(_logger);
            _drawImageFade = new DrawImageFade(_logger, pictureBox1);

            // 指定ディレクトリからGIFのパスリストを取得;
            // GIFファイルが保存されているディレクトリのパスを指定してください
            string defaultDirPath = @"J:\ZMyFolder_2\jpgbest\gif_png_bmp\gif";
            _pathList = GetGifPathList(defaultDirPath);
        }

        private void ShowNext(object sender, EventArgs e)
        {

            // インデックスを次に進めるが、最大を超えないようにする
            if (_currentIndex < _pathList.Count - 1)
            {
                _currentIndex++;
                LoadGifAtCurrentIndex();
            }
            else
            {
                _logger.PrintInfo("ShowNext MaxIndex");
            }
        }

        // 現在のインデックスに基づいてGIFをロードする
        private void LoadGifAtCurrentIndex()
        {
            if (_pathList.Count > 0 && _currentIndex >= 0 && _currentIndex < _pathList.Count)
            {
                //_playImage.Display(_pathList[_currentIndex]);
                _ = _playImage.DisplayAsync(_pathList[_currentIndex]);
            }
        }

        // キーイベントの処理
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Right)
            {
                // インデックスを次に進めるが、最大を超えないようにする
                if (_currentIndex < _pathList.Count - 1)
                {
                    _currentIndex++;
                    LoadGifAtCurrentIndex();
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                // インデックスを前に戻すが、最小を超えないようにする
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                    LoadGifAtCurrentIndex();
                }
            }

            else if (e.KeyCode == Keys.Up)
            {
                _slideShowFunction.StartTimer();
            }
            else if (e.KeyCode == Keys.Down)
            {
                _slideShowFunction.StopTimer();
            }
            else if (e.KeyCode == Keys.Z)
            {
                _playImage.isFadeOn = !_playImage.isFadeOn;
                _logger.PrintInfo(string.Format("_playImage.isFadeOn = {0}", _playImage.isFadeOn));
            }
            else if (e.KeyCode == Keys.A)
            {
                //透明度を下げる
                //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                //_drawImageFade.SetImageByPath(_pathList[_currentIndex]);
                //Image image = _drawImageFade.ConvertGifToStaticImage(pictureBox1.Image);
                //_drawImageFade.SetImage(image);
                //double val = _effectFadeSetting._alphaPercent - 10;
                //EffectFadeSetting setting = new EffectFadeSetting(_logger, val);
                //_effectFadeSetting.changeAlphaPercent(val);
                //_drawImageFade.ApplyDrawSettingB(_effectFadeSetting);
                //_effectFadeSetting.logColorMatrix(_effectFadeSetting._matrix);
                ////
                //int alpha = (int)val;
                //_logger.PrintInfo(String.Format("@alpha = {0}", alpha));
                //// 透明度を適用した画像を作成
                ////Bitmap originalImage = new Bitmap(_pathList[_currentIndex]);
                ////originalImage = _drawImageFade.ChangeImageOpacity(originalImage, alpha / 255f);
                //_drawImageFade.SetImageToControl(_drawImageFade._drawMainImage);
                // 新しいファイル取得後、PictureBox.Image, _drawMainImage にセットするときにエラーとなる
                //###
                //#
                // 上記ではできず

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                double val = _effectFadeSetting._alphaPercent - 10;
                _effectFadeSetting.changeAlphaPercent(val);
                //
                int alpha = (int)val;
                _logger.PrintInfo(String.Format("@alpha = {0}", alpha));
                // 透明度を適用した画像を作成
                Bitmap originalImage = new Bitmap(_pathList[_currentIndex]);
                originalImage = _drawImageFade.ChangeImageOpacity(originalImage, alpha / 255f);
                _drawImageFade.SetImageToControl(originalImage);
            }            
            else if (e.KeyCode == Keys.S)
            {
                //透明度をあげる
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                double val = _effectFadeSetting._alphaPercent + 10;
                _effectFadeSetting.changeAlphaPercent(val);
                //
                int alpha = (int)val;
                _logger.PrintInfo(String.Format("@alpha = {0}", alpha));
                // 透明度を適用した画像を作成
                Bitmap originalImage = new Bitmap(_pathList[_currentIndex]);
                originalImage = _drawImageFade.ChangeImageOpacity(originalImage, alpha / 255f);
                _drawImageFade.SetImageToControl(originalImage);
            }
        }

        // 指定されたディレクトリからGIFファイルのパスリストを取得するメソッド
        private List<string> GetGifPathList(string defaultDirPath)
        {
            List<string> gifPaths = new List<string>();

            if (Directory.Exists(defaultDirPath))
            {
                // ディレクトリ内のGIFファイルのみを取得
                gifPaths = Directory.GetFiles(defaultDirPath, "*.gif", SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                _logger.PrintError($"指定されたディレクトリが存在しません: {defaultDirPath}");
            }

            return gifPaths;
        }

        private void FormSlideShowTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                _logger.Dispose();
                _pathList.Clear();
                _pathList = null;
                _playImage.Dispose();
                _slideShowFunction.Dispose();
                _effectFadeSetting.Dispose();
                _drawImageFade.DisposeAll();

            } catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
