using System;
using System.Drawing;
using System.Windows.Forms;

namespace ViewImageObjects
{
    public class ViewImageFunction_FitInnerToFrame
    {
        protected ErrorManager.ErrorManager _err;
        protected IViewImageControl _viewImageControl;
        protected IViewImageFrameControl _viewimageFrameControl;
        protected IViewImage _viewImage;

        public ViewImageFunction_FitInnerToFrame(ErrorManager.ErrorManager err,
            IViewImageFrameControl viewImageFrameControl, IViewImageControl viewImageControl,IViewImage viewImage)
        {
            _err = err;
            _viewImageControl = viewImageControl;
            _viewimageFrameControl = viewImageFrameControl;
            _viewImage = viewImage;
        }

        public void FitImageToControl()
        {
            try
            {
                _err.AddLog(this, "FitImageToControl");
                if (!ControlIsValid()) { return; }

                Size imageSize = _viewImage.GetImage().Size;
                Size innerSize = _viewImageControl.GetSize();
                Size frameSize = _viewimageFrameControl.GetSize();
                if (ControlSizeIsNull(frameSize, innerSize)) { _err.AddLogWarning("  SizeIsNull=true"); }

                // サイズ計算 Panel にフィットさせる
                Size newSize = GetSizeFitFrame(frameSize, imageSize);

                // ※イメージがControlより小さい場合、中央に表示する、コントロールにFitさせる
                //// Location
                //if (viewImageObjects.Controls.ViewInnerControl.Settings.IsViewAlwaysCenter)
                //{
                //    // 常に中央に表示する場合
                //    // サイズ変更、中央計算
                //}
                //else
                //{
                //    // PictureBox 非固定
                //}

                // Location 計算
                // 中央に表示する
                Point newPoint = GetLocationFrameCenter(frameSize, newSize);

                _viewImageControl.SetVisible(false);
                _viewImageControl.ChangeLocation(newPoint);
                _viewImageControl.ChangeSize(newSize);
                _viewImageControl.SetVisible(true);

            } catch (Exception ex)
            {
                _err.AddLogAlert(this, "FitImageToControl Failed", "FitImageToControl Failed",ex);
            }
        }

        // Image を Frameに合わせる
        public Size GetSizeFitFrame(Size frameSize, Size imageSize)
        {
            try
            {
                // ※ sizeIsNull を外部で実行しておく
                //'PictureBoxのサイズをPanelサイズにFitさせる
                //'＝PictureBoxのサイズをPanelサイズからはみ出さないようにする

                //'サイズを合わせるための基準 Criteria for matching sizes
                //'Public Overloads Sub setFlagCriteriaForMatchSize(argFlag As Integer)
                //'0 Image OuterFit
                //'1 Inner_Horizontal 幅はInnerで高さにOuterの比率をかける
                //'2 Inner_Vertical　高さはInnerで幅にOuterの比率をかける
                //'FlagCriteriaForMatchSize = argFlag
                // 縦横比 1:AspectRaito
                double AspectRaito = (double)imageSize.Width / imageSize.Height;
                // Frame と Image の縦比
                double VerticalRaito = (double)frameSize.Height / imageSize.Height;
                // Frame と Image の縦比
                double HorizontalRaito = (double)frameSize.Width / imageSize.Width;

                double retRaito = 0, NewHeight = 0, NewWidth = 0;
                // 倍率が小さいほうを設定する
                if (VerticalRaito < HorizontalRaito)
                {
                    // Frame の縦に合わせる
                    //NewHeight = frameSize.Height * 1;
                    //retRaito = NewHeight / imageSize.Height; // 倍率を計算する
                    //NewWidth = imageSize.Width * retRaito; //* AspectRaito; // 上記倍率から縦横比を掛けて Width を決定

                    // 縦のほうが倍率が小さい→横の余白が長い→縦に合わせる
                    NewHeight = frameSize.Height * 1;
                    // 縦の倍率を求める imageSize が frameSize になる
                    retRaito = (double)frameSize.Height / imageSize.Height; // VerticalRaito
                                                                            // 倍率を Width にかける
                    NewWidth = imageSize.Width * retRaito;
                }
                else
                {
                    // Frame の横に合わせる
                    NewWidth = frameSize.Width * 1;
                    retRaito = NewWidth / frameSize.Width; // 倍率を計算する
                    NewHeight = imageSize.Height * retRaito; // 上記倍率から縦横比を掛けて Height を決定
                }
                return new Size((int)NewWidth, (int)NewHeight);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + " getSizeFitFrame");
                return new Size();
            }
        }

        public Point GetLocationFrameCenter(Size frameSize,Size imageSize)
        {
            try
            {
                // sizeIsNull を外部で実行しておく
                double width = (frameSize.Width - imageSize.Width) / 2;
                double height = (frameSize.Height - imageSize.Height) / 2;
                return new Point((int)width, (int)height);
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + " GetLocationFrameCenter");
                return new Point();
            }
        }

        // ImageSize が FrameSize よりおおきい (Width ,Height のいずれか)
        private bool IsImageSizeMoreThanFrameSize(Size frameSize,Size ImageSize)
        {
            try
            {
                if((frameSize.Width < ImageSize.Width)||(frameSize.Height < ImageSize.Height)){
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + " IsImageSizeMoreThanFrameSize");
                return false;
            }
        }

        private int InnerControlSizeFitFrameControl(
            Size frameSize,Size innerSize,Size imageSize)
        {
            return 0;
        }

        public bool ControlIsValid()
        {
            try
            {
                if (_viewImage == null) { _err.AddLogAlert("_viewImage == null"); return false; }
                if (_viewImageControl == null) { _err.AddLogAlert("_viewImageControl == null"); return false; }
                return true;
            } catch (Exception ex)
            {
                _err.AddLogAlert(this, "ControlIsValid Failed", "ControlIsValid Failed", ex);
                return false;
            }
        }

        public bool ControlSizeIsNull(Size frameSize, Size imageSize)
        {
            try
            {
                if (frameSize == null)
                {
                    _err.AddLogAlert(this, " FrameSize == null");
                    return true;
                }
                if (imageSize == null)
                {
                    _err.AddLogAlert(this, " imageSize == null");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "SizeIsNull");
                return true;
            }
        }
    }
}
