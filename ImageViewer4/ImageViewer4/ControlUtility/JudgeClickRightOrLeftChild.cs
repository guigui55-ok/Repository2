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
        public JudgeClickRightOrLeftChild(ErrorManager.ErrorManager err, Control control,Control parentControl)
        {
            _err = err;
            _control = control;
            _parentControl = control;
            _control.MouseClick += Control_MouseClick;
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
                Point po = control.PointToClient(Cursor.Position);
                if (po.X > (control.Width / 2))
                {
                    _err.AddLog(this, " ClickRight");
                    this.ClickRight?.Invoke(sender, e);
                    return 1;
                }
                else
                {
                    _err.AddLog(this, " ClickLeft");
                    this.ClickRight?.Invoke(sender, e);
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

        public string[] GetFilesByDragAndDrop(DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // ドラッグ中のファイルやディレクトリの取得
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    return files;
                }
                else
                {
                    _err.AddLogAlert(this, "GetFilesByDragAndDrop GetDataPresent Else");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
                return null;
            }
        }
    }
}
