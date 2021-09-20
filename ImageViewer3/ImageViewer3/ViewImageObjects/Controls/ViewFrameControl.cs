using System;
using System.Windows.Forms;
using System.Drawing;
using ImageViewer.Values;
using System.Diagnostics;

namespace ImageViewer.Controls
{
    // Panel2
    public class ViewFrameControl : IViewFrameControl
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected Panel _framePanel;
        protected Panel _parentControl;
        IViewInnerControl _viewInnerControl;

        protected IViewControlState _state;
        protected IViewImageSettings _settings;
        private PointF bufPointF = new PointF();

        public IViewControlState State { get { return _state; } set { _state = value; } }
        public IViewInnerControl ViewInnerControl { get { return _viewInnerControl; } set { _viewInnerControl = value; } }

        public ViewFrameControl(ErrorLog.IErrorLog errorlog,
            Panel parentControl, Panel framePanel,IViewImageSettings settings,IViewControlState state) 
        { 
            _errorLog = errorlog; 
            _framePanel = framePanel; 
            _parentControl = parentControl;
            _settings = settings;
            _state = state;
        }

        public Control GetControl() { return _framePanel; }
        public Size GetSize() { return _framePanel.Size; }
        public Point GetLocation() { return _framePanel.Location; }
        public void ChangeSize(Size size)
        {
            try
            {
                _framePanel.Size = size;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setPrentControl Failed");
                return;
            }
        }

        public void ChangeLocation(Point point)
        {
            try
            {
                _framePanel.Location = point;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setPrentControl Failed");
                return;
            }
        }
        public void PausePaint(bool flag)
        {
            if (flag)
            {
                _framePanel.SuspendLayout();
            }
            else
            {
                _framePanel.ResumeLayout();
            }
            ViewInnerControl.PausePaint(flag);
        }

        // コントロールのサイズと位置変更
        public void ChangeSizeAndLocation(Size size, Point location)
        {
            try
            {
                // Control の描画を停止
                this.PausePaint(true);
                //this.SetVisible(false);
                // サイズ変更
                this.ChangeSize(size);
                // ポジション変更
                this.ChangeLocation(location);
                // Control の描画を再開
                //this.SetVisible(true);
                this.PausePaint(false);
                // 位置とサイズを記憶する
                this.SaveRaitoSizeAndPositionFromContentsControl(false);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeSizeViewControl Failed");
            }
        }

        public void SetVisible(bool flag)
        {
            if (_framePanel == null) { _errorLog.AddErrorNotException("setVisible"); }
            _framePanel.Visible = flag;
        }

        /// <summary>
        /// 親Controlに対してのLocationとサイズの比率を保存する
        /// 画像表示時、マウスドラッグでコントロール移動時、拡大縮小時、初期化時に記録する
        /// <param name="IsContentsSizeChanging">親ControlがSizeChangedかどうか</param>
        /// </summary>
        public void SaveRaitoSizeAndPositionFromContentsControl(bool IsParentControlSizeChanging)
        {
            try
            {
                // 親コントロールが変化しているときは、記録しない
                if (IsParentControlSizeChanging) { return; }
                //Size ContentsSize = _parentControl.Size;
                // Size Ratio
                bufPointF.X = (float)_framePanel.Size.Width / (float)_parentControl.Size.Width;
                bufPointF.Y = (float)_framePanel.Size.Height / (float)_parentControl.Size.Height;
                State.RaitoSizeFrameFromContents = bufPointF;
                // Location Raito
                State.RaitoLocationFrameFromContentsX = (double)_framePanel.Location.X / (double)_parentControl.Size.Width;
                State.RaitoLocationFrameFromContentsY = (double)_framePanel.Location.Y / (double)_parentControl.Size.Height;

                Debug.WriteLine("save size ratio = " + State.RaitoSizeFrameFromContents.X + " , " + State.RaitoSizeFrameFromContents.Y);
                //Debug.WriteLine("save pos  ratio = " + State.RaitoLocationFrameFromContentsX + " , " + State.RaitoLocationFrameFromContentsY);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "SaveRaitoSizeAndPositionFromContentsControl Failed");
                return;
            }
        }
    }
}
