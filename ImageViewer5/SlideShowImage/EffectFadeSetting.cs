using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using AppLoggerModule;
using System.Drawing.Imaging;

namespace SlideShowImage
{
    //PaintImageEffectSetting
    public class EffectFadeSetting:IDisposable
    {
        AppLogger _logger;
        public ImageAttributes _drawImageAttributes;
        public double _alphaPercent = 100;
        private bool _isDisposed = true;
        public ColorMatrix _matrix;
        public EffectFadeSetting(AppLogger logger)
        {
            _logger = logger;
            SetColorMatrix(_alphaPercent);
        }
        public EffectFadeSetting(AppLogger logger, double alphaPercent)
        {
            _logger = logger;
            _alphaPercent = alphaPercent;
            SetColorMatrix(alphaPercent);
        }

        public void logColorMatrix(ColorMatrix matrix)
        {
            _logger.PrintInfo($"ColorMatrix values:");
            _logger.PrintInfo($"Matrix00: {matrix.Matrix00}, Matrix11: {matrix.Matrix11}, Matrix22: {matrix.Matrix22}, Matrix33: {matrix.Matrix33}, Matrix44: {matrix.Matrix44}");
        }


        public void changeAlphaPercent(double alphaPercent)
        {
            // アルファ値を設定し、描画に反映させる
            _alphaPercent = alphaPercent;
            SetColorMatrix(_alphaPercent);  // ここで新しいアルファ値に基づいた ColorMatrix を設定する
            _logger.PrintInfo($"Alpha percent updated to: {_alphaPercent}");
        }

        public void SetColorMatrix(double alphaPercent)
        {
            Dispose_DrawImageAttributes();
            _drawImageAttributes = new ImageAttributes();
            _matrix = GetColorMatrix(alphaPercent);
            _drawImageAttributes.SetColorMatrix(_matrix);
        }

        private ColorMatrix GetColorMatrix(double argMatrix33)
        {
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Matrix00 = 1;
            colorMatrix.Matrix11 = 1;
            colorMatrix.Matrix22 = 1;
            colorMatrix.Matrix33 = (float)argMatrix33 * 0.01F;
            colorMatrix.Matrix44 = 1;
            return colorMatrix;
        }

        public void Dispose_DrawImageAttributes()
        {
            _drawImageAttributes?.Dispose();
            _drawImageAttributes = null;
        }

        public void Dispose()
        {
            //Dispose();
            GC.SuppressFinalize(this);
        }

        //protected override void Dispose(bool disposing)
        //{
        //}
    }
}
