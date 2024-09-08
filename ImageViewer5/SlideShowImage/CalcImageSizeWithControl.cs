using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace SlideShowImage
{
    public class CalcImageSizeWithControl
    {
        AppLogger _logger;
        //PointF[] _drawPoints = { };
        Control _control;
        Image _image;
        Size _imageSize;
        public CalcImageSizeWithControl(AppLogger logger)
        {
            _logger = logger;
        }

        public void SetControlAndImage(Control control, Image image)
        {
            _control = control;
            _image = image;
        }

        ////////////////
        // 240907 new
        
        /// <summary>
        /// PictureBoxのサイズにImageのサイズを合わせる
        /// </summary>
        /// <param name="image"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public Rectangle CalcImageSizeFitImageSizeKeepAspect(Image image, PictureBox control)
        {
            Rectangle ret = new Rectangle();
            if (image == null || control == null)
                return ret;

            // PictureBoxのサイズ
            int picBoxWidth = control.ClientSize.Width;
            int picBoxHeight = control.ClientSize.Height;

            // 画像の元のサイズ
            int imgWidth = image.Width;
            int imgHeight = image.Height;

            // 縦横比を維持したまま画像の表示サイズを計算
            float picBoxAspectRatio = (float)picBoxWidth / picBoxHeight;
            float imgAspectRatio = (float)imgWidth / imgHeight;

            int drawWidth, drawHeight;

            if (imgAspectRatio > picBoxAspectRatio)
            {
                // 横幅に合わせて高さを調整
                drawWidth = picBoxWidth;
                drawHeight = (int)(picBoxWidth / imgAspectRatio);
            }
            else
            {
                // 高さに合わせて横幅を調整
                drawHeight = picBoxHeight;
                drawWidth = (int)(picBoxHeight * imgAspectRatio);
            }

            // 画像を中央に表示するためのオフセット
            int offsetX = (picBoxWidth - drawWidth) / 2;
            int offsetY = (picBoxHeight - drawHeight) / 2;

            return new Rectangle(offsetX, offsetY, drawWidth, drawHeight);
            //// PictureBoxのGraphicsオブジェクトを取得
            //using (Graphics g = control.CreateGraphics())
            //{
            //    // 画像をPictureBoxに描画
            //    g.DrawImage(image, new Rectangle(offsetX, offsetY, drawWidth, drawHeight));
            //}
        }


        ////////////////////

        /// <summary>
        /// イメージサイズをコントロールサイズに合わせる、イメージの比率を保持
        /// </summary>
        /// <returns></returns>
        public Size ImageSizeChangeToMatchControlSize()
        {
            double newWidth;
            double newHeight;
            //double ratio;
            double verticalRatio = _control.Size.Height / _image.Size.Height;
            double horizontalRatio = _control.Size.Width / _image.Size.Height;
            //倍率が小さいほうを設定
            if (verticalRatio < horizontalRatio)
            {
                newHeight = _control.Size.Height * 1;
                newWidth = _image.Size.Width * verticalRatio;
                //_nowRatio = verticalRatio;
            } else
            {
                newHeight = _image.Size.Height * horizontalRatio;
                newWidth = _control.Size.Width * 1;
                //ratio = horizontalRatio;
            }
            _imageSize = new Size((int)newWidth, (int)newHeight);
            return _imageSize;
        }

        //上と同じ
        public PointF CalcRaitoImageByPictureBox(Image argBaseImage, PictureBox argPictureBox)
        {
            try
            {
                double newWidth;
                double newHeight;
                double verticalRaito = (double)argPictureBox.Height / argBaseImage.Height;
                double horizontalRaito = (double)argPictureBox.Width / argBaseImage.Width;

                if (verticalRaito < horizontalRaito)
                {
                    newHeight = argPictureBox.Height;
                    newWidth = argBaseImage.Width * verticalRaito;
                    //_nowRaito = verticalRaito;
                }
                else
                {
                    newHeight = argBaseImage.Height * horizontalRaito;
                    newWidth = argPictureBox.Width;
                    //_nowRaito = horizontalRaito;
                }

                return new PointF((float)newWidth, (float)newHeight);
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".CalcRaitoImageByPictureBox");
                return new PointF(0, 0);
            }
        }
    }
}
