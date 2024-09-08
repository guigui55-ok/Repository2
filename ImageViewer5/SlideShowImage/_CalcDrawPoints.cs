using System;
using System.Drawing;
using System.Windows.Forms;
using AppLoggerModule;


/// <summary>
/// マウスホイールで拡大縮小関連
/// マウスポインタとコントロールとイメージの関係から座標や大きさを変更する
/// 検討中
/// </summary>
public class PointsForDrawSquareImage
{
    private PointF[] PointsForDraw;
    public Point _controlSize;
    public double _nowRaito;
    // Public ResultPoint As RectangleF

    private double _imageWidth;
    private double _imageHeight;
    //private double _afterImageWidth;
    //private double _AfterImageHeight;

    private AppLogger _logger;

    public PointsForDrawSquareImage(AppLogger logger)
    {
        _logger = logger;
    }

    public PointsForDrawSquareImage(
        double x,
        double y,
        double width, 
        double height,
        AppLogger logger) : this(logger)
    {
        SetPoints(x, y, width, height);
    }

    public PointF[] GetPoints()
    {
        return PointsForDraw;
    }

    public void SetPoints(double x, double y, double width, double height)
    {
        try
        {
            PointsForDraw = new[]
            {
                new PointF((float)x, (float)y),
                new PointF((float)width, (float)y),
                new PointF((float)x, (float)height)
            };
            _imageWidth = width - x;
            _imageHeight = height - y;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".SetPoints");
        }
    }

    public void SetHorizontalAlign(int flagInt, PointF[] argPoints, double frameWidth)
    {
        try
        {
            double topPos = argPoints[0].X;
            double leftPos = argPoints[0].Y;
            double rightPos = argPoints[1].X;
            double bottomPos = argPoints[2].Y;

            if (flagInt == 0)
            {
                // Center
                double addPos = (frameWidth - rightPos) / 2;
                leftPos += addPos;
                rightPos += addPos;
            }
            else if (flagInt == 1)
            {
                // Left
            }
            else if (flagInt == 2)
            {
                // Right
                rightPos = frameWidth;
                leftPos = frameWidth - rightPos;
            }

            PointsForDraw = new[]
            {
                new PointF((float)leftPos, (float)topPos),
                new PointF((float)rightPos, (float)topPos),
                new PointF((float)leftPos, (float)bottomPos)
            };
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".SetHorizontalAlign");
        }
    }

    public void SetVerticalAlign(int flagInt, PointF[] argPoints, double frameHeight)
    {
        try
        {
            double topPos = argPoints[0].Y;
            double leftPos = argPoints[0].X;
            double rightPos = argPoints[1].X;
            double bottomPos = argPoints[2].Y;

            if (flagInt == 0)
            {
                // Center
                double addPos = (frameHeight - bottomPos) / 2;
                topPos += addPos;
                bottomPos += addPos;
            }
            else if (flagInt == 1)
            {
                // Top
            }
            else if (flagInt == 2)
            {
                // Bottom
                topPos = frameHeight - bottomPos;
                bottomPos = frameHeight;
            }

            PointsForDraw = new[]
            {
                new PointF((float)leftPos, (float)topPos),
                new PointF((float)rightPos, (float)topPos),
                new PointF((float)leftPos, (float)bottomPos)
            };
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".SetVerticalAlign");
        }
    }

    // 移動
    //public PointF CalcRaitoImageByPictureBox(Image argBaseImage, PictureBox argPictureBox)
    //{
    //    try
    //    {
    //        double newWidth;
    //        double newHeight;

    //        double verticalRaito = (double)argPictureBox.Height / argBaseImage.Height;
    //        double horizontalRaito = (double)argPictureBox.Width / argBaseImage.Width;

    //        if (verticalRaito < horizontalRaito)
    //        {
    //            newHeight = argPictureBox.Height;
    //            newWidth = argBaseImage.Width * verticalRaito;
    //            _nowRaito = verticalRaito;
    //        }
    //        else
    //        {
    //            newHeight = argBaseImage.Height * horizontalRaito;
    //            newWidth = argPictureBox.Width;
    //            _nowRaito = horizontalRaito;
    //        }

    //        return new PointF((float)newWidth, (float)newHeight);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.PrintError(ex, this.ToString() + ".CalcRaitoImageByPictureBox");
    //        return new PointF(0, 0);
    //    }
    //}


    /// <summary>
    /// イメージをPictureBoxのサイズに合わせる（イメージ比を保持）
    /// </summary>
    /// <param name="argBaseImage"></param>
    /// <param name="argPictureBox"></param>
    /// <returns></returns>
    public PointF CalcRaitoImageByPictureBox(Size argBaseImage, Size argPictureBox)
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
                _nowRaito = verticalRaito;
            }
            else
            {
                newHeight = argBaseImage.Height * horizontalRaito;
                newWidth = argPictureBox.Width;
                _nowRaito = horizontalRaito;
            }

            return new PointF((float)newWidth, (float)newHeight);
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".CalcRaitoImageByPictureBox");
            return new PointF(0, 0);
        }
    }

    public string GetDataForDebug()
    {
        string rtn = "";
        rtn += " x:" + PointsForDraw[0].X + Environment.NewLine;
        rtn += " y:" + PointsForDraw[0].Y + Environment.NewLine;
        rtn += " x:" + PointsForDraw[1].X + Environment.NewLine;
        rtn += " y:" + PointsForDraw[1].Y + Environment.NewLine;
        rtn += " x:" + PointsForDraw[2].X + Environment.NewLine;
        rtn += " y:" + PointsForDraw[2].Y + Environment.NewLine;
        return rtn;
    }

    public void OutPutRectF(RectangleF rect)
    {
        try
        {
            string rtn = "";
            rtn += " x:" + rect.X;
            rtn += " y:" + rect.Y + Environment.NewLine;
            rtn += " w:" + rect.Width + Environment.NewLine;
            rtn += " h:" + rect.Height + Environment.NewLine;
            _logger.AddLog(rtn);
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".OutPutRectF");
        }
    }


    /// <summary>
    /// 指定した画面上のポイントを計算してクライアント座標を算出
    /// </summary>
    /// <param name="picbox"></param>
    /// <returns></returns>
    public bool IsCursorOnControl(Control control)
    {
        try
        {
            Point mp;
            // 指定した画面上のポイントを計算してクライアント座標を算出
            mp = control.PointToClient(System.Windows.Forms.Cursor.Position);

            if ((mp.X <= control.Width) && (mp.Y <= control.Height) &&
                (mp.X >= 0) && (mp.Y >= 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".IsCursorOnControl");
            return false;
        }
    }


    public int SetPointsForImageResize(
        PictureBox argPictureBox,
        double nowWidth,
        double nowHeight,
        double argNowRaito,
        double argAfterRaito,
        int verticalAlign,
        int horizontalAlign)
    {
        try
        {
            SetPointsForImageResize(
                argPictureBox.Width,
                argPictureBox.Height,
                nowWidth,
                nowHeight,
                argNowRaito,
                argAfterRaito,
                verticalAlign,
                horizontalAlign,
                Cursor.Position,
                IsCursorOnControl(argPictureBox));
            return 1;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".SetPointsForImageResize");
            return -1;
        }
    }

    public int SetPointsForImageResize(double picBoxWidth, double picBoxHeight, double nowWidth, double nowHeight, double argNowRaito, double argAfterRaito, int verticalAlign, int horizontalAlign)
    {
        try
        {
            SetPointsForImageResize(picBoxWidth, picBoxHeight, nowWidth, nowHeight, argNowRaito, argAfterRaito, verticalAlign, horizontalAlign, Cursor.Position, false);
            return 1;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".SetPointsForImageResize");
            return -1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nowWidth"></param>
    /// <param name="nowHeight"></param>
    /// <param name="argAfterRaito"></param>
    /// <returns></returns>
    public int SetPointsForImageResizeLeftTop(
        double nowWidth, 
        double nowHeight,
        double argAfterRaito)
    {
        try
        {
            double afterWidth = nowWidth * argAfterRaito;
            double afterHeight = nowHeight * argAfterRaito;

            RectangleF newRect = new RectangleF(0, 0, (float)afterWidth, (float)afterHeight);

            PointsForDraw = new[]
            {
                new PointF(newRect.X, newRect.Y),
                new PointF(newRect.Width, newRect.Y),
                new PointF(newRect.X, newRect.Height)
            };
            return 1;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".SetPointsForImageResizeLeftTop");
            return -1;
        }
    }

    public int SetPointsForImageResize(
        double drawWidth,
        double drawHeight,
        double nowWidth,
        double nowHeight,
        double argNowRaito,
        double argAfterRaito,
        int verticalAlign,
        int horizontalAlign,
        Point mp, bool mpInPicBox)
    {
        try
        {
            double afterWidth = nowWidth * argAfterRaito;
            double afterHeight = nowHeight * argAfterRaito;
            _nowRaito = argAfterRaito;

            RectangleF newRect = new RectangleF();

            if (mpInPicBox)
            {
                newRect = CalcSizeHeightForMethodSetPointsForImageResizeByCursor(
                    drawWidth, drawHeight, afterWidth, afterHeight, newRect, mp);
            }
            else
            {
                newRect = CalcSizeWidthForMethodSetPointsForImageResize(horizontalAlign, drawWidth, afterWidth, newRect);
                newRect = CalcSizeHeightForMethodSetPointsForImageResize(verticalAlign, drawHeight, afterHeight, newRect);
            }

            PointsForDraw = new[]
            {
                new PointF(newRect.X, newRect.Y),
                new PointF(newRect.Width, newRect.Y),
                new PointF(newRect.X, newRect.Height)
            };
            return 1;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".SetPointsForImageResize");
            return -1;
        }
    }

    /// <summary>
    /// コントロールとImageの高さからtop,bottomを設定
    /// マウスホイールで拡大縮小用
    /// </summary>
    /// <param name="flagInt"></param>
    /// <param name="controlHeight"></param>
    /// <param name="nowHeight"></param>
    /// <param name="argRectangle"></param>
    /// <returns></returns>
    private RectangleF CalcSizeHeightForMethodSetPointsForImageResizeByCursor(
        double controlWidth,
        double controlHeight,
        double nowWidth,
        double nowHeight,
        RectangleF argRectangle,
        Point mp)
    {
        try
        {
            double afterWidth = nowWidth;
            double afterHeight = nowHeight;
            double top = 0;
            double left = 0;
            double right = afterWidth;
            double bottom = afterHeight;

            double newX = mp.X - (controlWidth / 2);
            double newY = mp.Y - (controlHeight / 2);

            top += newX;
            left += newY;
            right += newX;
            bottom += newY;

            RectangleF newRect = new RectangleF();
            if (afterWidth < controlWidth)
            {
                newRect = CalcSizeWidthForMethodSetPointsForImageResize(
                    0, controlWidth, afterWidth, newRect);
            }
            else
            {
                if (left < 0)
                {
                    left = 0;
                    right = afterWidth;
                }
                if (right > controlWidth)
                {
                    right = afterWidth;
                    left = afterWidth - controlWidth;
                }
            }

            if (afterHeight < controlHeight)
            {
                newRect = CalcSizeHeightForMethodSetPointsForImageResize(
                    0, controlHeight, afterHeight, newRect);
            }
            else
            {
                if (top < 0)
                {
                    top = 0;
                    bottom = afterHeight;
                }
                if (bottom > controlHeight)
                {
                    bottom = afterHeight;
                    top = afterHeight - controlHeight;
                }
            }

            OutPutRectF(new RectangleF((float)left, (float)top, (float)right, (float)bottom));
            return new RectangleF((float)left, (float)top, (float)right, (float)bottom);
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".CalcSizeHeightForMethodSetPointsForImageResizeByCursor");
            return argRectangle;
        }
    }

    /// <summary>
    /// コントロールとImageの高さからtop,bottomを設定
    /// マウスホイールで拡大縮小用
    /// </summary>
    /// <param name="flagInt"></param>
    /// <param name="controlHeight"></param>
    /// <param name="nowHeight"></param>
    /// <param name="argRectangle"></param>
    /// <returns></returns>
    private RectangleF CalcSizeHeightForMethodSetPointsForImageResize(
        int flagInt, double controlHeight, double nowHeight, RectangleF argRectangle)
    {
        try
        {
            double minorsMargin;
            double top;
            double bottom;

            if (flagInt == 0)
            {
                minorsMargin = (controlHeight - nowHeight) / 2;
                top = minorsMargin;
                bottom = minorsMargin + nowHeight;
            }
            else if (flagInt == 1)
            {
                top = 0;
                bottom = nowHeight;
            }
            else
            {
                top = controlHeight - nowHeight;
                bottom = controlHeight;
            }

            return new RectangleF(argRectangle.X, (float)top, argRectangle.Width, (float)bottom);
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".CalcSizeHeightForMethodSetPointsForImageResize");
            return argRectangle;
        }
    }

    private RectangleF CalcSizeWidthForMethodSetPointsForImageResize(
        int flagInt,
        double controlWidth,
        double nowWidth,
        RectangleF argRectangle)
    {
        try
        {
            double minorsMargin;
            double left;
            double right;

            if (flagInt == 0)
            {
                minorsMargin = (controlWidth - nowWidth) / 2;
                left = minorsMargin;
                right = nowWidth + minorsMargin;
            }
            else if (flagInt == 1)
            {
                left = 0;
                right = nowWidth;
            }
            else
            {
                left = controlWidth - nowWidth;
                right = controlWidth;
            }

            return new RectangleF((float)left, argRectangle.Y, (float)right, argRectangle.Height);
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".CalcSizeWidthForMethodSetPointsForImageResize");
            return argRectangle;
        }
    }

    public double GetDrawImageWidth(PointF[] point)
    {
        try
        {
            return point[1].X - point[0].X;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".GetDrawImageWidth");
            return 0;
        }
    }

    public double GetDrawImageHeight(PointF[] point)
    {
        try
        {
            return point[2].Y - point[0].Y;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".GetDrawImageHeight");
            return 0;
        }
    }


    /// <summary>
    /// imagePoint が frameWidth または frameHeight より大きいか判定する
    /// </summary>
    /// <param name="imagePoint"></param>
    /// <param name="frameWidth"></param>
    /// <param name="frameHeight"></param>
    /// <returns></returns>
    public bool IsImageBigThanFrame(PointF[] imagePoint, double frameWidth, double frameHeight)
    {
        try
        {
            if (GetDrawImageWidth(imagePoint) > frameWidth)
            {
                return true;
            }
            if (GetDrawImageHeight(imagePoint) > frameHeight)
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, this.ToString() + ".IsImageBigThanFrame");
            return false;
        }
    }
}
