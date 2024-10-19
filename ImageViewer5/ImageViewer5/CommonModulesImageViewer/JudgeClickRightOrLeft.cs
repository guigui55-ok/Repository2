using System;
using System.Windows.Forms;
using AppLoggerModule;

namespace CommonControlUtilityModule
{
    public class JudgeClickRightOrLeft
    {
        protected AppLogger _logger;
        protected Control _control;
        public EventHandler ClickRight;
        public EventHandler ClickLeft;
        protected bool IsMove = false;
        protected bool IsDrag = false;
        protected bool IsDown = false;
        public JudgeClickRightOrLeft(AppLogger logger,Control control)
        {
            _logger = logger;
            _control = control;
            _control.MouseClick += Control_MouseClick;
            _control.MouseDown += Control_MouseDown;
            _control.MouseUp += Control_MouseUp;
            _control.MouseMove += Control_MouseMove;
        }

        public JudgeClickRightOrLeft(
            AppLogger logger,
            Control control,
            EventHandler clickRight,
            EventHandler clickLeft)
        {
            _logger = logger;
            _control = control;
            ClickRight = clickRight;
            ClickLeft = clickLeft;
            _control.MouseClick += Control_MouseClick;
            _control.MouseDown += Control_MouseDown;
            _control.MouseUp += Control_MouseUp;
            _control.MouseMove += Control_MouseMove;
        }

        public void Dispose()
        {
            try
            {
                _control.MouseClick -= Control_MouseClick;
                _control.MouseDown -= Control_MouseDown;
                _control.MouseUp -= Control_MouseUp;
                _control.MouseMove -= Control_MouseMove;
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
                IsDrag = false; _logger.AddLog(this, "Control_MouseUp IsDrag true => false"); 
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            IsDown = true;
        }


        private void Control_MouseClick(object sender, MouseEventArgs e)
        {
            ControlClicked(_control,sender, e);
        }

        public int ClickPointIsRightSideOnControl(object sender,MouseEventArgs e)
        {
            return ControlClicked(_control,sender, e);
        }

        public int ControlClicked(Control control,object sender, MouseEventArgs e)
        {
            try
            {
                if (IsDrag) { _logger.AddLog(this, "ControlClicked , IsDrag=true , return"); return -1; }
                if (e.X > (control.Width / 2))
                {
                    _logger.AddLog(this," ClickRight");
                    if (ClickRight != null) { ClickRight.Invoke(sender, e); }
                    return 1;
                } else
                {
                    _logger.AddLog(this, " ClickLeft");
                    if (ClickRight != null) { ClickRight.Invoke(sender, e); }
                    return 2;
                }
                // update flag
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "ClickPointIsRightSideOnControl");
                return 0;
            }
        }

    }
}
