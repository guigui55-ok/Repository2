using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControlUtility
{
    public class JudgeClickRightOrLeftChild
    {
        protected ErrorManager.ErrorManager _err;
        protected Control _control;
        protected Control _parentControl;
        public EventHandler ClickRight;
        public EventHandler ClickLeft;
        protected bool IsDrag = false;
        protected bool IsDown = false;
        public JudgeClickRightOrLeftChild(ErrorManager.ErrorManager err, Control control,Control parentControl)
        {
            _err = err;
            _control = control;
            _parentControl = control;
            _control.MouseClick += Control_MouseClick;
            _control.MouseDown += Control_MouseDown;
            _control.MouseUp += Control_MouseUp;
            _control.MouseMove += Control_MouseMove;
        }
        public JudgeClickRightOrLeftChild(ErrorManager.ErrorManager err, Control control, Control parentControl
            , EventHandler clickRight, EventHandler clickLeft)
        {
            _err = err;
            _control = control;
            _parentControl = control;
            ClickRight = clickRight;
            ClickLeft = clickLeft;
            _control.MouseClick += Control_MouseClick;
            _control.MouseDown += Control_MouseDown;
            _control.MouseUp += Control_MouseUp;
            _control.MouseMove += Control_MouseMove;
        }
        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDown) { IsDrag = true; }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsDown) { IsDown = false; }
            if (IsDrag) { 
                IsDrag = false; 
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            IsDown = true;
        }
        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            ControlClicked(_parentControl, sender, e);
        }

        public int ClickPointIsRightSideOnControl(object sender, MouseEventArgs e)
        {
            return ControlClicked(_parentControl, sender, e);
        }

        public int ControlClicked(Control control, object sender, MouseEventArgs e)
        {
            try
            {
                if (IsDrag) { _err.AddLog(this, "ControlClicked , IsDrag=true , return"); return -1; }
                Point po = control.PointToClient(Cursor.Position);
                if (po.X > (control.Width / 2))
                {
                    _err.AddLog(this, "ClickRight");
                    if (ClickRight != null) { ClickRight.Invoke(sender, e); }
                    return 1;
                }
                else
                {
                    _err.AddLog(this, "ClickLeft");
                    if (ClickLeft != null) { ClickLeft.Invoke(sender, e); }
                    return 2;
                }
                // update flag
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
                return 0;
            }
        }

    }
}
