using System;
using System.Drawing;
using System.Windows.Forms;
using ErrorLog;
using System.Diagnostics;
using ImageViewer.Values;

namespace ImageViewer.Controls
{
    // Panel3
    public class ViewInnerControl : IViewInnerControl
    {
        protected Panel _parentControl;
        protected Panel _innerControl;
        protected ErrorLog.IErrorLog _errorLog;
        protected IViewControlState _state;
        protected IViewImageSettings _settings;
        protected IViewImageControl _viewImageControl;

        //protected Point bufPoint;
        private PointF bufPointF;
        public IViewImageControl ViewImageControl { get => _viewImageControl; set => _viewImageControl = value; }
        public IViewImageSettings Settings { get => _settings; set => _settings = value; }
        public IViewControlState State { get => _state; set => _state = value; }
        

        public ViewInnerControl(IErrorLog erorlog,
            Panel parentControl,Panel innerControl,IViewImageSettings settings,IViewControlState state)
        {
            _errorLog = erorlog;
            _parentControl = parentControl;
            _innerControl = innerControl;
            _settings = settings;
            _state = state;
        }

        public Control GetControl() { return (Control)_innerControl; }


        public int SetViewImageControl(IViewImageControl viewImageControl)
        {
            try
            {
                if (!(viewImageControl is null))
                {
                    _viewImageControl = viewImageControl;
                    _state = _viewImageControl.State;
                    _settings = _viewImageControl.Settings;
                    return 1;
                }
                else
                {
                    _errorLog.AddErrorNotException(this.ToString(), "setViewImageControl");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setParentControl");
                return 0;
            }
        }


        public void ChangeLocation(Point point)
        {
            try
            {
                _innerControl.Location = point; 
            } catch (Exception ex)
            { _errorLog.AddException(ex, this.ToString(), "changeLocation"); }
        }

        public void ChangeSize(Size size)
        {
            try 
            {
                _innerControl.Size = size;
            }
            catch (Exception ex)
            { _errorLog.AddException(ex, this.ToString(), "changeSize"); }
        }

        public Point GetLocation()
        {
            try
            {
                if (_innerControl == null) { 
                    _errorLog.AddErrorNotException(this.ToString(), "setControl is not Panel"); return new Point(); }
                return _innerControl.Location;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "getLocation");
                return new Point();
            }
        }

        public Control GetParentControl()
        {
            try
            {
                if (_parentControl == null)
                {
                    _errorLog.AddErrorNotException(this.ToString(), "getParentControl is null"); return null;
                }
                return _parentControl;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "getParentControl");
                return null;
            }
        }

        public Size GetSize()
        {
            try
            {
                if (_innerControl == null)
                {
                    _errorLog.AddErrorNotException(this.ToString(), "getSize innerControl is null"); return new Size();
                }
                return _innerControl.Size;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "getSize");
                return new Size();
            }
        
        }

        public int Initialize()
        {
            try {
                return 1;
            } catch (Exception ex) {
                _errorLog.AddException(ex, this.ToString(), "Initialize");
                return 0;
            }
        }


        public int SetControlState(IViewControlState viewControlState)
        {
            _state = viewControlState;
            return 1;
        }


        public void SetImageNotDispose(Image image)
        {
            _viewImageControl.SetImageNotDispose(image);
        }

        public void SetImageWithDispose(Image image)
        {
            _viewImageControl.SetImageWithDispose(image);
        }


        public void SetVisible(bool flag)
        {
            if (_innerControl is null) { return; }
            _innerControl.Visible = flag;
            _viewImageControl.SetVisible(flag);
        }

        public void RefreshPaint()
        {
            _viewImageControl.RefreshPaint();
        }

        public void PausePaint(bool flag)
        {
            if (flag)
            {
                _innerControl.SuspendLayout();
                
            } else
            {
                _innerControl.ResumeLayout();
            }
            _viewImageControl.PausePaint(flag);
        }

        /// <summary>
        /// コントロールの拡大・縮小
        /// <param name="raito">現在のサイズからの倍率</param>
        /// </summary>
        public void ExpansionSizeViewControl(double raito)
        {
            try
            {
                Size newSize = new Size((int)(_innerControl.Width * raito), (int)(_innerControl.Height * raito));

                // 拡大縮小時にLocationを変更のための計算
                Point newLocation = GetLocationByCalcExpansionWhenChangeSize(this.GetSize(), newSize);
                // Control の描画を停止
                this.PausePaint(true);
                //this.SetVisible(false);
                // サイズ変更
                this.ChangeSize(newSize);
                // ポジション変更
                this.ChangeLocation(newLocation);
                // Control の描画を再開
                //this.SetVisible(true);
                this.PausePaint(false);
                // 位置とサイズを記憶する
                this.SaveRaitoSizeAndPositionFromFrameControl();
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeSizeViewControl Failed");
            }
        }

        /// <summary>
        /// コントロールの拡大・縮小時の位置変更のための計算
        /// <param name="beforeSize">以前サイズ</param>
        /// <param name="afterSize">変化後のサイズ</param>
        /// </summary>
        public Point GetLocationByCalcExpansionWhenChangeSize(Size beforeSize, Size afterSize)
        {
            try
            {
                int x = (int)(beforeSize.Width - afterSize.Width) / 2;
                int y = (int)(beforeSize.Height - afterSize.Height) / 2;
                x += _innerControl.Location.X;
                y += _innerControl.Location.Y;
                return new Point(x, y);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeLocationWhenChangeSize Failed");
                return new Point();
            }
        }

        /// <summary>
        /// コントロールのサイズと位置変更
        /// </summary>
        public void ChangeSizeAndLocation(Size size, Point location)
        {
            try
            {
                // Control の描画を停止
                this.SetVisible(false);
                //this.PausePaint(true);
                // サイズ変更
                this.ChangeSize(size);
                // ポジション変更
                this.ChangeLocation(location);
                // Control の描画を再開
                //this.PausePaint(false);
                this.SetVisible(true);
                // 位置とサイズを記憶する
                this.SaveRaitoSizeAndPositionFromFrameControl();
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeSizeViewControl Failed");
            }
        }

        // 画像表示時、マウスドラッグでコントロール移動時、拡大縮小時に記録する
        public void SaveRaitoSizeAndPositionFromFrameControl()
        {
            try
            {

                // 意図的に InnerSize を変えたときには保存する
                // 外枠 FrameControl での InnerControl.Location と InnerControl.Size を保存
                // InnerControlが変化した際に保存するが、FrameControlがともに変化しているときは保存しない
                if (State.IsFrameSizeChanging) { return; }
                Size frameSize = _parentControl.Size;
                // Size Ratio
                bufPointF.X = (float)_innerControl.Size.Width / (float)_parentControl.Size.Width;
                bufPointF.Y = (float)_innerControl.Size.Height / (float)_parentControl.Size.Height;
                State.RatioSizeInnerFromFrame = bufPointF;
                // Location Raito
                State.RatioLocationInnerFromFrameX = (double)_innerControl.Location.X / (double)_parentControl.Size.Width;
                State.RatioLocationInnerFromFrameY = (double)_innerControl.Location.Y / (double)_parentControl.Size.Height;

                //Debug.WriteLine("save size ratio = " + State.RatioSizeInnerFromFrame.X  + " , " + State.RatioSizeInnerFromFrame.Y);
                //Debug.WriteLine("save pos  ratio = " + State.RatioLocationInnerFromFrameX + " , " + State.RatioLocationInnerFromFrameY);

                // Size Difference
                //bufPoint.X = frameSize.Width - _innerControl.Width;
                //bufPoint.Y = frameSize.Height - _innerControl.Height;
                //State.DifferenceSizeFromContents = bufPoint;

                //// Position
                //State.DifferencePositionFromContents = _innerControl.Location;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "saveDifferenceSizeAndPositionFromFramecControl Failed");
                return;
            }
        }

    }
}
