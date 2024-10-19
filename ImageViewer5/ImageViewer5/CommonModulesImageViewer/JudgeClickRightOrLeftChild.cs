using System;
using System.Drawing;
using System.Windows.Forms;
using AppLoggerModule;

namespace CommonControlUtilityModule
{
    public class JudgeClickRightOrLeftChild
    {
        protected AppLogger _logger;
        protected Control _control;
        protected Control _parentControl;
        public EventHandler ClickRight;
        public EventHandler ClickLeft;
        protected bool IsDrag = false;
        protected bool IsDown = false;
        public JudgeClickRightOrLeftChild(
            AppLogger logger,
            Control control,
            Control parentControl)
        {
            InitializeValues(logger, control, parentControl);
        }
        public JudgeClickRightOrLeftChild(
            AppLogger logger,
            Control control,
            Control parentControl,
            EventHandler clickRight,
            EventHandler clickLeft)
        {
            ClickRight = clickRight;
            ClickLeft = clickLeft;
            InitializeValues(logger, control, parentControl);
        }
        private void InitializeValues(
            AppLogger logger,
            Control control,
            Control parentControl)
        {
            _logger = logger;
            _control = control;
            _parentControl = parentControl;
            _control.MouseClick += Control_MouseClick;
            _control.MouseDown += Control_MouseDown;
            _control.MouseUp += Control_MouseUp;
            _control.MouseMove += Control_MouseMove;
            _parentControl.MouseClick += Control_MouseClick;
            _parentControl.MouseDown += Control_MouseDown;
            _parentControl.MouseUp += Control_MouseUp;
            _parentControl.MouseMove += Control_MouseMove;
        }

        public void Dispose()
        {
            try
            {
                _control.MouseClick -= Control_MouseClick;
                _control.MouseDown -= Control_MouseDown;
                _control.MouseUp -= Control_MouseUp;
                _control.MouseMove -= Control_MouseMove;
                _parentControl.MouseClick -= Control_MouseClick;
                _parentControl.MouseDown -= Control_MouseDown;
                _parentControl.MouseUp -= Control_MouseUp;
                _parentControl.MouseMove -= Control_MouseMove;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
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
                if (IsDrag) {
                    _logger.AddLog(this, "ControlClicked , IsDrag=true , return");                    
                    return -1; 
                }
                Point po = control.PointToClient(Cursor.Position);
                if (po.X > (control.Width / 2))
                {
                    _logger.AddLog(this, "ClickRight");
                    if (ClickRight != null) { ClickRight.Invoke(sender, e); }
                    return 1;
                }
                else
                {
                    _logger.AddLog(this, "ClickLeft");
                    if (ClickLeft != null) { ClickLeft.Invoke(sender, e); }
                    return 2;
                }
                // update flag
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
                return 0;
            }
        }

    }
}
