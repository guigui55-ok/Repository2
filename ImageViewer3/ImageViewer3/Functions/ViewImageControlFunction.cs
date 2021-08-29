using ImageViewer.Controls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Functions
{
    public class ViewImageControlFunction
    {
        protected ErrorLog.IErrorLog _errorLog;
        public IViewImageControl ViewImageControl;
        public IViewInnerControl ViewInnerControl;
        public IViewFrameControl ViewFrameControl;
        public ViewImageObjects ViewImageObjects;

        public ViewImageControlFunction(
            ErrorLog.IErrorLog errorlog,
            IViewImageControl viewImgeControl,IViewInnerControl viewInnerControl, IViewFrameControl viewFrameControl,ViewImageObjects viewImageObjects)
        {
            _errorLog = errorlog;
            ViewImageControl = viewImgeControl;
            ViewInnerControl = viewInnerControl;
            ViewFrameControl = viewFrameControl;
            ViewImageObjects = viewImageObjects;

            ViewFrameControl.GetControl().LocationChanged += FrameControl_LocationChanged;
            ViewFrameControl.GetControl().SizeChanged += FrameControl_SizeChanged;

            ViewInnerControl.GetControl().LocationChanged += InnerControl_LocationChanged;
            ViewInnerControl.GetControl().SizeChanged += InnerControl_SizeChanged;
        }


        public void MouseWheelEvent(object sender, MouseEventArgs e)
        {
            try
            {
                // 描画を止める
                //ViewImageObjects.Functions.ForControl.PasePaint(true);
                this.SetVisible(false);
                if (e.Delta > 0)
                {
                    // 上回転したとき
                    // 拡大 1.05倍
                    ChangeSizeViewControl(ImageViewerConstants.EXPANTION_RAITO);
                }
                else if (e.Delta < 0)
                {
                    // 下回転したとき
                    // 縮小 0.952倍
                    ChangeSizeViewControl(ImageViewerConstants.SHRINK_RAITO);
                }
                // 描画を再開
                //ViewImageObjects.Functions.ForControl.PasePaint(false);
                this.SetVisible(true);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " MouseWheelEvent");
            }
        }


        /// <summary>
        /// FrameControlとInnerControlのサイズと位置比率を維持する
        /// Maintain the System.Drawing.Size and position ratio of Frame Control and Inner Control
        /// 外部から実行する用
        /// </summary>
        public int MaintainSizeAndPositionRatioContentsAndFrame(ViewImageObjects viewImageObjects)
        {
            viewImageObjects.Functions.ForControl.MaintainSizeAndPositionRatioContentsAndFrame(viewImageObjects.MainContentsControl.Size);
            return 1;
        }
        /// <summary>
        /// FrameControlとInnerControlのサイズと位置比率を維持する
        /// Maintain the System.Drawing.Size and position ratio of Frame Control and Inner Control
        /// </summary>
        public void MaintainSizeAndPositionRatioContentsAndFrame(Size ContentsSize)
        {
            try
            {
                // 初期化時はnull
                if (ViewImageObjects is null) { return; }
                Debug.WriteLine("MaintainSizeAndPositionRatio ContentsAndFrame");
                // 今の ContentsSize サイズ
                Size nowOuterSize = ContentsSize;

                if (ViewImageObjects.Controls.ViewFrameControl.State.RatioSizeInnerFromFrame.X <= 0)
                { _errorLog.AddErrorNotException(this.ToString(), "MaintainSizeAndPositionRatioFrameAndInner Failed"); }
                if (ViewImageObjects.Controls.ViewFrameControl.State.RatioSizeInnerFromFrame.Y <= 0)
                { _errorLog.AddErrorNotException(this.ToString(), "MaintainSizeAndPositionRatioFrameAndInner Failed"); }

                // 記録した比率からSize算出
                double ratioW = (double)nowOuterSize.Width 
                    * (double)ViewImageObjects.Controls.ViewFrameControl.State.RaitoSizeFrameFromContents.X;
                double ratioH = (double)nowOuterSize.Height 
                    * (double)ViewImageObjects.Controls.ViewFrameControl.State.RaitoSizeFrameFromContents.Y;
                // 新しいサイズ
                Size afterSize = new Size((int)ratioW, (int)ratioH);

                // 保存した比率からLocation算出
                Point nowPos = ViewFrameControl.GetLocation();
                double newX = ViewImageObjects.Controls.ViewFrameControl.State.RaitoLocationFrameFromContentsX
                    * (double)ViewImageObjects.MainContentsControl.Size.Width;
                double newY = ViewImageObjects.Controls.ViewFrameControl.State.RaitoLocationFrameFromContentsY
                    * (double)ViewImageObjects.MainContentsControl.Size.Height;
                // 新しいLocation
                Point newPos = new Point((int)newX, (int)newY);


                //Debug.WriteLine("before Size = " + nowOuterSize.Width + ", " + nowOuterSize.Height);
                //Debug.WriteLine("before pos  = " + nowPos.X + ", " + nowPos.Y);
                Debug.WriteLine("nowOuterSize = " + nowOuterSize.Width + " , " + nowOuterSize.Height);
                Debug.WriteLine("new Size = " + afterSize.Width + " , " + afterSize.Height);
                Debug.WriteLine("new pos  = " + newPos.X + " , " + newPos.Y );
                Debug.WriteLine("-------------");

                ViewFrameControl.ChangeSizeAndLocation(afterSize, newPos);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "MaintainSizeAndPositionRatioFrameAndInner Failed");
            }
        }



        /// <summary>
        /// FrameControlとInnerControlのサイズと位置比率を維持する
        ///  画像表示時、マウスドラッグでコントロール移動時、拡大縮小時に記録する
        /// </summary>
        public void SaveRaitoSizeAndPositionInnerFromFrameControl()
        {
            ViewInnerControl.SaveRaitoSizeAndPositionFromFrameControl();
        }
        /// <summary>
        /// FrameControlとInnerControlのサイズと位置比率を維持する
        /// Maintain the Size and position ratio of Frame Control and Inner Control
        /// </summary>
        public void MaintainSizeAndPositionRatioFrameAndInner(object sender, EventArgs e)
        {
            try
            {             
                Debug.WriteLine("MaintainSizeAndPositionRatioFrameAndInner");
                // 今の FrameControl サイズ
                Size nowSize = ViewFrameControl.GetSize();

                if (ViewImageObjects.Controls.ViewInnerControl.State.RatioSizeInnerFromFrame.X <= 0)
                {
                    _errorLog.AddErrorNotException( this.ToString(), "MaintainSizeAndPositionRatioFrameAndInner Failed");
                }
                if (ViewImageObjects.Controls.ViewInnerControl.State.RatioSizeInnerFromFrame.Y <= 0)
                {
                    _errorLog.AddErrorNotException( this.ToString(), "MaintainSizeAndPositionRatioFrameAndInner Failed");
                }
                // 記録した比率からSize算出
                double ratioW = (double)nowSize.Width * (double)ViewImageObjects.Controls.ViewInnerControl.State.RatioSizeInnerFromFrame.X;
                double ratioH = (double)nowSize.Height * (double)ViewImageObjects.Controls.ViewInnerControl.State.RatioSizeInnerFromFrame.Y;
                Size afterSize = new Size((int)ratioW, (int)ratioH);
                    
                // 保存した比率からLocation算出
                Point nowPos = ViewInnerControl.GetLocation();
                Point differentPos = ViewInnerControl.State.DifferencePositionInnerInFrame;
                double newX = ViewImageObjects.Controls.ViewInnerControl.State.RatioLocationInnerFromFrameX
                    * (double)ViewFrameControl.GetSize().Width;
                double newY = ViewImageObjects.Controls.ViewInnerControl.State.RatioLocationInnerFromFrameY
                    * (double)ViewFrameControl.GetSize().Height;

                Point newPos = new Point((int)newX,(int)newY);


                Debug.WriteLine("before Size = " + nowSize.Width + ", " + nowSize.Height);
                Debug.WriteLine("before pos  = " + nowPos.X + ", " + nowPos.Y);

                Debug.WriteLine("new Size = " + afterSize.Width + ", " + afterSize.Height);
                Debug.WriteLine("new pos  = " + newPos.X + ", " + newPos.Y);

                ViewInnerControl.ChangeSizeAndLocation(afterSize, newPos);

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "MaintainSizeAndPositionRatioFrameAndInner Failed");
            }
        }

        // Frame コントロールのサイズ変更、位置変更
        public void ChangeSizeFrameControl(Size size)
        {
            try
            {
                ViewFrameControl.ChangeSize(size);
                // 意図的に FrameSize を変えたときには保存する
                // 外枠 ContentsControl での Location と FrameControl.Size を保存
                // FrameControlが変化した際に保存するが、ContensControlがともに変化しているときは保存しない
                ViewFrameControl.SaveRaitoSizeAndPositionFromContentsControl(
                    ViewImageObjects.MainControls.ContentsControl.MainformManager.State.IsContentsControlSizeChanged);
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeSizeFrameControl Failed");
            }
        }

        public void SaveRatioFromContentscControl()
        {
            Debug.WriteLine("SaveRatioFromContentscControl");
            // 初期化時はnull
            if (ViewImageObjects is null) { return; }
            // 意図的に FrameSize を変えたときには保存する
            // 外枠 ContentsControl での Location と FrameControl.Size を保存
            // FrameControlが変化した際に保存するが、ContensControlがともに変化しているときは保存しない
            ViewFrameControl.SaveRaitoSizeAndPositionFromContentsControl(
                    ViewImageObjects.MainControls.ContentsControl.MainformManager.State.IsContentsControlSizeChanged);
        }

        // サイズ変更
        public void ChangeSizeViewControl(Size newSize)
        {
            try
            {
                // Control の描画を停止
                ViewInnerControl.SetVisible(false);
                //ViewInnerControl.PausePaint(true);
                // サイズ変更
                ViewInnerControl.ChangeSize(newSize);
                // ポジション変更
                //ViewInnerControl.changeLocation(newLocation);
                // Control の描画を再開
                //ViewInnerControl.PausePaint(false);
                ViewInnerControl.SetVisible(true);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeSizeViewControl Failed");
            }
        }

        // コントロールの拡大_縮小
        public void ChangeSizeViewControl(double raito)
        {
            try
            {
                // 描画を止める
                //ViewImageObjects.Functions.ForControl.PasePaint(true);
                // コントロールの拡大縮小
                ViewInnerControl.ExpansionSizeViewControl(raito);
                // 描画を再開
                //ViewImageObjects.Functions.ForControl.PasePaint(false);
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeSizeViewControl Failed");
            }
        }

        public void SetVisible(bool flag)
        {
            ViewFrameControl.SetVisible(flag);
            ViewInnerControl.SetVisible(flag);
        }

        public void PasePaint(bool flag)
        {
            ViewFrameControl.PausePaint(flag);
            ViewInnerControl.PausePaint(flag);
            
        }
        private void FrameControl_LocationChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("FrameControl_LocationChanged");
            // Frame と ContentsPanel との位置関係、Frame のサイズを保存する
            ViewFrameControl.SaveRaitoSizeAndPositionFromContentsControl(
                ViewImageObjects.MainControls.MainFormManger.State.IsContentsControlSizeChanged);
            //ViewInnerControl.SaveRaitoSizeAndPositionFromFrameControl();

        }
        private void FrameControl_SizeChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("FrameControl_SizeChanged");
            // Inner のサイズを記録しない用のフラグ
            ViewFrameControl.State.IsFrameSizeChanging = true;
            // Frame と ContentsPanel との位置関係、Frame のサイズを保存する
            //ViewFrameControl.SaveRaitoSizeAndPositionFromContentsControl();
            // 描画を止める
            ViewImageObjects.Functions.ForControl.PasePaint(true);
            // Inner のサイズと位置を外側の拡大縮小の際に、以前のものと相対的に同じにする
            ViewImageObjects.Functions.ForControl.MaintainSizeAndPositionRatioFrameAndInner(sender, e);
            // 描画を再開
            ViewImageObjects.Functions.ForControl.PasePaint(false);
            // Inner のサイズを記録しない用のフラグ
            ViewFrameControl.State.IsFrameSizeChanging = false;
        }
        private void InnerControl_LocationChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("InnerControl_LocationChanged");
            // 描画を止める
            //ViewImageObjects.Functions.ForControl.PasePaint(true);
            //ViewFrameControl.GetControl().Visible = false;
            // Inner と Frame との位置関係、Inner のサイズを保存する
            ViewInnerControl.SaveRaitoSizeAndPositionFromFrameControl();
            // 描画を再開
            //ViewImageObjects.Functions.ForControl.PasePaint(false);
            //ViewFrameControl.GetControl().Visible = true;

        }
        private void InnerControl_SizeChanged(object sender, EventArgs e)
        {
            // 描画を止める
            //ViewImageObjects.Functions.ForControl.PasePaint(true);
            // Inner と Frame との位置関係、Inner のサイズを保存する
            ViewInnerControl.SaveRaitoSizeAndPositionFromFrameControl();
            // 描画を再開
            //ViewImageObjects.Functions.ForControl.PasePaint(false);
        }
    }
}
