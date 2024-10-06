using System;
using System.Drawing;
using System.Windows.Forms;
using AppLoggerModule;

namespace CommonControlUtilityModule
{
    public class ChangeSizeByMouseWheel
    {
        protected AppLogger _logger;
        protected Control _innerControl;
        protected Control _parentControl;
        public EventHandler MouseWheelBegin;
        public EventHandler MouseWheelEnd;
        protected PointF bufPointF;
        protected PointF RatioSizeInnerFromFrame;
        protected Double RatioLocationInnerFromFrameX;
        protected Double RatioLocationInnerFromFrameY;
        public double ExpantionRaito = 1.05;
        public double ShrinkRaito = 0.952;
        public ChangeSizeByMouseWheel(AppLogger logger, Control control, Control parentControl)
        {
            _logger = logger;
            _innerControl = control;
            _parentControl = parentControl;
            _parentControl.MouseWheel += Control_MouseWheel;
        }
        public ChangeSizeByMouseWheel(
            AppLogger logger, 
            Control control,
            Control parentControl,
            EventHandler wheelBegin,
            EventHandler wheelEnd)
        {
            _logger = logger;
            _innerControl = control;
            _parentControl = parentControl;
            MouseWheelBegin = wheelBegin;
            MouseWheelEnd = wheelEnd;
            _parentControl.MouseWheel += Control_MouseWheel;
        }


        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (MouseWheelBegin != null) { MouseWheelBegin.Invoke(sender, e); }
                // 描画を止める
                _innerControl.SuspendLayout();
                _parentControl.SuspendLayout();
                if (e.Delta > 0)
                {
                    // 上回転したとき
                    // 拡大 1.05倍
                    ExpansionSizeViewControl(ExpantionRaito);
                }
                else if (e.Delta < 0)
                {
                    // 下回転したとき
                    // 縮小 0.952倍
                    ExpansionSizeViewControl(ShrinkRaito);
                }
                // 描画を再開
                _innerControl.ResumeLayout();
                _parentControl.ResumeLayout();
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "MouseWheelEvent");
            }
            finally
            {
                if (MouseWheelEnd != null) { MouseWheelEnd.Invoke(sender, e); }
            }
        }
        public void ControlPausePaint(bool flag)
        {
            try
            {
                if (flag)
                {
                    _innerControl.SuspendLayout();
                    _parentControl.SuspendLayout();
                } else
                {
                    _innerControl.ResumeLayout();
                    _parentControl.ResumeLayout();
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "ControlSuspendLayout Failed");
            }
        }

        /// <summary>
        /// コントロールの拡大・縮小
        /// <param name="raito">現在のサイズからの倍率</param>
        /// </summary>
        public void ExpansionSizeViewControl(double raito)
        {
            try
            {
                _logger.AddLog(this, "ExpansionSizeViewControl");
                Size newSize = new Size((int)(_innerControl.Width * raito), (int)(_innerControl.Height * raito));

                // Size ゼロかつ raito が1以上の時はサイズを調整する
                if((newSize.Width == 0)&&(raito > 1.0)) {
                    _logger.AddLog("  ((newSize.Width == 0)||(raito > 1.0))==true => Resize");
                    newSize = new Size(10,10); 
                }

                // 拡大縮小時にLocationを変更のための計算
                Point newLocation = GetLocationByCalcExpansionWhenChangeSize(_innerControl.Size, newSize);
                // Control の描画を停止
                ControlPausePaint(true);
                // サイズ変更
                _innerControl.Size = newSize;
                _logger.AddLog(" Size:"+_innerControl.Size.Width + "," + _innerControl.Size.Height + "=>" 
                    + newSize.Width + "," + newSize.Height);
                // ポジション変更
                _innerControl.Location = newLocation;
                _logger.AddLog(" Location:" + _innerControl.Location.X + "," + _innerControl.Location.Y + "=>"
                    + newLocation.X  + "," + newLocation.Y);
                // Control の描画を再開
                ControlPausePaint(false);
                // 位置とサイズを記憶する
                SaveRaitoSizeAndPositionFromFrameControl();
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "ChangeSizeViewControl Failed");
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
                //if (State.IsFrameSizeChanging) { return; }
                Size frameSize = _parentControl.Size;
                // Size Ratio
                bufPointF.X = (float)_innerControl.Size.Width / (float)_parentControl.Size.Width;
                bufPointF.Y = (float)_innerControl.Size.Height / (float)_parentControl.Size.Height;
                RatioSizeInnerFromFrame = bufPointF;
                // Location Raito
                RatioLocationInnerFromFrameX = (double)_innerControl.Location.X / (double)_parentControl.Size.Width;
                RatioLocationInnerFromFrameY = (double)_innerControl.Location.Y / (double)_parentControl.Size.Height;

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
                _logger.AddException(ex, this, "saveDifferenceSizeAndPositionFromFramecControl Failed");
                return;
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
                _logger.AddException(ex, this, "ChangeLocationWhenChangeSize Failed");
                return new Point();
            }
        }
    }
}
