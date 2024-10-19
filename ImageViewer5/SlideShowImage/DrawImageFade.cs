using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections;
using System.Drawing.Imaging;

namespace SlideShowImage
{    
    public class DrawImageFade
    {
        AppLogger _logger;
        //DrawImage
        //メインのイメージ（このイメージを中心に、描画などを変えていく）
        public Image _drawMainImage;
        //メインのイメージがパスより設定されたとき
        public string _imagePath;

        //PaintImageSettingObject
        public PointF[] _drawPoints;
        // イメージの領域（描画用、SetImage時に更新される）
        Rectangle _readImageRegion;
        //PaintImageAsPictureBoxInPanel:IPaintImage
        Image _imageCanvas;
        // イメージを描画するメインコントロール（コンストラクタ生成時にセットしておく）
        PictureBox _imageControl;
        //計算用
        CalcImageSizeWithControl _calclator;

         
        public DrawImageFade(AppLogger logger, PictureBox imageControl)
        {
            _logger = logger;
            _imageControl = imageControl;
            _calclator = new CalcImageSizeWithControl(_logger);
        }

        public void DisposeAll()
        {
            try
            {
                if (_drawMainImage != null)
                {
                    _drawMainImage.Dispose();
                    _drawMainImage = null;
                }
                if (_imageCanvas != null)
                {
                    _imageCanvas.Dispose();
                    _imageCanvas = null;
                }
                if (_calclator != null)
                {
                    _calclator?.Dispose();
                }
            } catch(Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }


        //DrawImage
        public void DisposeImage()
        {
            _drawMainImage?.Dispose();
        }

        public void SetImageToControl(Image image)
        {
            //if (_imageControl.Image != null)
            //{
            //    _imageControl.Image.Dispose(); // 既存の画像を解放
            //    _drawMainImage?.Dispose(); // 前の画像を解放
            //}

            _imageControl.Image = (Image)image.Clone(); // クローンしてセット
            _drawMainImage = _imageControl.Image;

            //if (_drawMainImage != null)
            //{
            //    _drawMainImage.Dispose(); // 前の画像を解放
            //}
            //_drawMainImage = (Image)image.Clone(); 
        }

        /// <summary>
        /// PictureBoxに設定されているGIFアニメーションを静止画に変換するメソッド。
        /// 指定したフレーム番号の静止画を取得します。
        /// </summary>
        /// <param name="image">GIF画像が設定されたImage</param>
        /// <param name="frameIndex">取得したいフレーム番号（0から開始）</param>
        /// <returns>指定されたフレームを表す静止画のImageオブジェクト</returns>
        public Image ConvertGifToStaticImage(Image image, int frameIndex = 0)
        {
            // GIFかどうか確認
            if (image.RawFormat.Equals(ImageFormat.Gif))
            {
                // GIFのフレーム数を取得
                FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
                int frameCount = image.GetFrameCount(dimension);

                // フレーム番号が有効な範囲内か確認
                if (frameIndex < 0 || frameIndex >= frameCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(frameIndex), "フレーム番号が範囲外です。");
                }

                // 指定されたフレームを選択
                image.SelectActiveFrame(dimension, frameIndex);

                // 指定されたフレームの静止画を返す
                _logger.PrintInfo("convert GifAnime to static");
                return (Image)image.Clone(); // Cloneして元のImageを保持したまま新しいImageとして返す
            }

            // GIFでない場合、そのままのImageを返す
            return image;
        }

        /// <summary>
        /// Imageが同じかどうか判定する
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public bool AreImagesEqualByHash(Image img1, Image img2)
        {
            // 画像をメモリストリームに保存
            using (MemoryStream ms1 = new MemoryStream())
            using (MemoryStream ms2 = new MemoryStream())
            {
                // 画像形式を指定して保存（ここでは PNG を使用）
                img1.Save(ms1, ImageFormat.Png);
                img2.Save(ms2, ImageFormat.Png);

                // MD5ハッシュを計算
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash1 = md5.ComputeHash(ms1.ToArray());
                    byte[] hash2 = md5.ComputeHash(ms2.ToArray());

                    // ハッシュ値が同一かどうかを比較
                    return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
                }
            }
        }


        /// <summary>
        /// GIFファイルを静止画として読み込むメソッド。指定されたフレーム番号のフレームを取得します。
        /// </summary>
        /// <param name="gifPath">GIFファイルのパス</param>
        /// <param name="frameIndex">取得したいフレーム番号（0から開始）</param>
        /// <returns>指定されたフレームを表す Image オブジェクト</returns>
        public Image LoadGifFrameAsStaticImage(string gifPath, int frameIndex=0)
        {
            _logger.PrintInfo("LoadGifFrameAsStaticImage");
            // 画像ファイルをロード
            Image gifImage = Image.FromFile(gifPath);

            // GIF形式かどうかを確認
            if (gifImage.RawFormat.Equals(ImageFormat.Gif))
            {
                // GIFのフレーム数を取得
                FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
                int frameCount = gifImage.GetFrameCount(dimension);

                // フレーム数が有効な範囲内か確認
                if (frameIndex < 0 || frameIndex >= frameCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(frameIndex), "フレーム番号が範囲外です。");
                }

                // 指定されたフレームを選択
                gifImage.SelectActiveFrame(dimension, frameIndex);
            }

            // 指定されたフレームの静止画を返す
            return (Image)gifImage.Clone(); // Cloneして元のImageを保持したまま新しいImageとして返す
        }


        /// <summary>
        /// イメージをセットする
        /// PictureBoxにはセットしない
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(Image image)
        {
            if ( object.ReferenceEquals(_drawMainImage, image))
            {
                _readImageRegion = new Rectangle(0, 0, image.Size.Width, image.Size.Height);
                _logger.PrintInfo("image equals true");
                return;
            }
            else
            {
                _drawMainImage?.Dispose();
                _drawMainImage = (Image)image.Clone();
                _readImageRegion = new Rectangle(0, 0, image.Size.Width, image.Size.Height);
            }
        }

        /// <summary>
        /// イメージをパスからセットする
        /// PictureBoxにはセットしない
        /// </summary>
        /// <param name="path"></param>
        public void SetImageByPath(string path)
        {
            _imagePath = path;
            try
            {
                if (!File.Exists(path))
                {
                    _logger.PrintError(this.ToString() + ".SetImageByPath > FileNotFound");
                    return;
                }
                _drawMainImage?.Dispose();
                _logger.PrintInfo(string.Format("ImageFileName = {0}", Path.GetFileName(path)));
                if (Path.GetExtension(path) == ".gif")
                {
                    _drawMainImage = LoadGifFrameAsStaticImage(path);
                }
                else
                {
                    _drawMainImage = Image.FromFile(path);
                }
                SetImage(_drawMainImage);
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "SetImageByPath");
            }

        }

        //Original
        public Bitmap GetBitmapNowImage()
        {
            return new Bitmap(_imageControl.Size.Width, _imageControl.Size.Height);
        }

        //ImageSetting
        /// <summary>
        /// Imageの設定をする
        /// <para></para>
        /// Imageの生成＞設定変更＞PictureBoxへ渡す（現在のImage.Dispose新たなImageに差し替え）という流れとなる
        /// </summary>
        /// <param name="effectSetting"></param>
        public void ApplyDrawSetting(EffectFadeSetting effectSetting)
        {
            Graphics g = null;
            Bitmap canvas = null;
            try
            {
                //ImageのNullチェック(未実装）
                if (_drawMainImage == null)
                {
                    _logger.PrintInfo("ApplyDrawSetting > image is null");
                    return;
                }
                canvas = GetBitmapNowImage();
                Image image = _drawMainImage;

                //mgのGraphicsオブジェクトを取得
                g = Graphics.FromImage(canvas);
                //画像描画領域
                PointF[] drawPoints = CalcDrawRegion();
                //画像読み込み領域
                //Rectangle destRect = _readImageRegion;
                Rectangle destRect = new Rectangle(0, 0, _imageControl.Width, _imageControl.Height);
                //canvasへ描画実行
                //g.DrawImage(image, destRect, drawPoints.X, drawPoints.Y, destRect.Width, destRect.Height, GraphicsUnit.Pixel, effectSetting._drawImageAttributes);
                g.DrawImage(image, drawPoints, destRect, GraphicsUnit.Pixel, effectSetting._drawImageAttributes);

                _imageCanvas = canvas;
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "ApplyDrawSetting");
            }
            finally
            {
                // Graphicsオブジェクトを解放
                g?.Dispose();
                // Bitmapオブジェクトを解放
                if (canvas != _imageControl.Image)
                {
                    canvas?.Dispose();
                }
            }
        }


        public void ApplyDrawSettingB(EffectFadeSetting effectSetting)
        {
            Graphics g = null;
            Bitmap canvas = null;
            try
            {
                //ImageのNullチェック(未実装）
                if (_drawMainImage == null)
                {
                    _logger.PrintInfo("ApplyDrawSettingB > image is null");
                    return;
                }
                // PictureBox.SizeMode = Zoom にしておくこと
                Rectangle rect = _calclator.CalcImageSizeFitImageSizeKeepAspect(_drawMainImage, _imageControl);

                //Rectangle rect = new Rectangle(0, 0, _drawMainImage.Width, _drawMainImage.Height);
                canvas = new Bitmap((int)(rect.Width), (int)(rect.Height));
                g = Graphics.FromImage(canvas);
                //g.DrawImage(_drawMainImage, rect);//単に描画するとき
                // GIFかどうかを確認し、GIFの場合は静止画に変換する
                _drawMainImage = ConvertGifToStaticImage(_drawMainImage);
                g.DrawImage(
                    _drawMainImage,                                  // 描画するイメージ
                    rect, // 描画する領域
                    0, 0,                                   // 画像の描画開始位置（元画像の左上）
                    _drawMainImage.Width,                            // 画像の元の幅
                    _drawMainImage.Height,                           // 画像の元の高さ
                    GraphicsUnit.Pixel,                     // 単位（ピクセル単位で指定）
                    effectSetting._drawImageAttributes // ImageAttributes（エフェクト設定を適用）
                );

                SetImage(canvas);
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, "ApplyDrawSettingB");
            }
            finally
            {
                // Graphicsオブジェクトを解放
                g?.Dispose();
                // Bitmapオブジェクトを解放
                if (canvas != _imageControl.Image)
                {
                    canvas?.Dispose();
                }
            }
        }

        public Bitmap ChangeImageOpacity(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 透明度を設定するためのColorMatrix
                System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix
                {
                    Matrix33 = opacity // アルファ値（透明度）を設定
                };
                System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                // 画像を描画し直す
                g.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        public PointF[] GetDrawRegionAsPoints()
        {
            //PaintImageSettingObject
            //'画像描画領域計算 DrawWidth DrawHeightに格納
            //'calcDrawRectangleByImage(DrawImage, ArgPictureBox, 1)
            //'Dim srcRect As Rectangle
            //'画像描画領域
            //'destRect = New Rectangle(0, 0, DrawWidth, DrawHeight)
            return _drawPoints;
        }

        //PaintImageSettingChangeRoutaition
        public void calcRegionDefaultToReadAndDraw()
        {

        }


        public PointF[] CalcDrawRegion(int flag=0)
        {
            PointF[] drawPoints= { };
            if (flag == 1)
            {
                //90度回転？
                PointF[] buf = {
                    new PointF(_imageControl.Width, 0),
                    new PointF(_imageControl.Width, _imageControl.Height),
                    new PointF(0, 0)
                };
                drawPoints = buf;
            }else
            {
                //通常
                PointF[] buf = {
                    new PointF(0, 0),
                    new PointF(_imageControl.Width, 0),
                    new PointF(_imageControl.Width, _imageControl.Height)
                };
                drawPoints = buf;
            }
            return drawPoints;
        }
}
}
