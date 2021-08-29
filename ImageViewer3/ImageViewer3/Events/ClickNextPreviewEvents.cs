using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ImageViewer.Events
{
    public class ClickNextPreviewEvents
    // ViewImageControl //inner
    // MouseClick(Right/Left)用
    {
        protected ErrorLog.IErrorLog _errorLog;
        readonly Control _control;
        public Events.ViewImageMouseEventHandler MouseEventHandler;

        protected bool IsDown = false;
        protected bool IsDoClick = true;
        protected bool IsMove = false;

        public ClickNextPreviewEvents(ErrorLog.IErrorLog errorLog, Control control)
        {
            _errorLog = errorLog;
            _control = control;
            MouseEventHandler = new ViewImageMouseEventHandler();
            // このクラス内のメソッドをイベントへ紐づけ
            _control.MouseClick += Control_MouseClick;
            _control.Click += Control_Click;
            _control.MouseWheel += Control_MouseWheel;
            _control.MouseDown += Control_MouseDown;
            _control.MouseUp += Control_MouseUp;
            _control.MouseMove += Control_MouseMove;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            this.IsMove = true;
            try
            {
                if (IsDown)
                {
                    // マウスドラッグしたときは、クリック扱いしない
                    IsDoClick = false;
                }
            }
            catch { }
            finally
            {
                IsMove = false;
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            IsDown = true;
        }
        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            IsDown = false;
            try { }
            catch { }
            finally
            {
                IsDoClick = true;
            }
        }

        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseEventHandler.MouseWheel(sender, e);
        }

        private void Control_Click(object sender, EventArgs e)
        {
            //Debug.WriteLine("Control_Click");
        }
        // MouseClick Event
        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine("Control_MouseClick");
            try
            {
                if (!IsDoClick) { return; }
                int flag = ClickPointIsRightSideOnControl(_control, e);
                if (flag == 1)
                {
                    // RightClick
                    MouseEventHandler.RightClick(null, e);

                }
                else if (flag == 2)
                {
                    // LeftClick
                    MouseEventHandler.LeftClick(null, e);
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "Control_MouseClick");
            }
        }
        // クリックされた場所がコントロールの右側化左側か
        public int ClickPointIsRightSideOnControl(Control control, MouseEventArgs e)
        {
            try
            {
                if (e.X > (control.Width / 2))
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
                return 0;
            }
        }
    }
}
