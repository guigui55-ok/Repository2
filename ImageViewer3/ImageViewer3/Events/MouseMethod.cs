using System;
using System.Windows.Forms;
using ErrorLog;

namespace MouseEvents
{
    public class MouseEvents
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected Control _control;
        public MouseEvents(ErrorLog.IErrorLog errorLog,Control control)
        {
            _errorLog = errorLog;
            _control = control;
        }

        public int ClickPointIsRightSideOnControl(MouseEventArgs e)
        {
            return ClickPointIsRightSideOnControl(_control, e);
        }

        public int ClickPointIsRightSideOnControl(Control control, MouseEventArgs e)
        {
            try
            {
                if (e.X > (control.Width / 2))
                {
                    return 1;
                } else
                {
                    return 2;
                }
                // update flag
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
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
                } else
                {
                    _errorLog.AddErrorNotException(this.ToString(), "GetFilesByDragAndDrop GetDataPresent Else");
                    return null;
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
                return null;
            }
        }
    }
}
