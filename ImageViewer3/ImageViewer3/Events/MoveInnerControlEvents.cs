using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ErrorLog;

namespace ImageViewer.Events
{
    // InnerContrl ControlMove用　ChangeLocation
    public class MoveInnerControlEvents
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected Control _control;
        protected Control _recieveControl;
        public Events.ViewImageMouseEventHandler MouseEventHandler;
        protected bool IsDown = false;
        protected bool IsMove = false;
        private Point mpBegin;

        
        
        public MoveInnerControlEvents(
            ErrorLog.IErrorLog errorLog, Control control,Control recieveEventControl)
        {
            _errorLog = errorLog;
            _control = control;
            _recieveControl = recieveEventControl;
            // イベントのインスタンスを生成
            MouseEventHandler = new ViewImageMouseEventHandler();
            // このクラス内のメソッドをイベントへ紐づけ
            _recieveControl.MouseMove += Control_MouseMove;
            _recieveControl.MouseDown += Control_MouseDown;
            _recieveControl.MouseUp += Control_MouseUp;
        }

        private void ChangeLocationByMouse(MouseEventArgs e,Point mp)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    _control.Left += e.X - mp.X;
                    _control.Top += e.Y - mp.Y;
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "changeLocationByMouse");
            }
        }
        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            IsMove = true;
            try
            {
                if (IsDown)
                {
                    ChangeLocationByMouse(e, mpBegin);
                }
                MouseEventHandler.MouseMove(sender, e);
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "Control_MouseMove");
            } finally
            {
                IsMove = false;
            }
        }
        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Control_MouseDown");
            IsDown = true;
            try
            {
                mpBegin = e.Location;
                MouseEventHandler.MouseDown(sender, e);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "Control_MouseDown");
            }
            finally
            {
                //IsDown = false;
            }
        }
        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            IsDown = false;
            try
            {
                MouseEventHandler.MouseUp(sender, e);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "Control_MouseUp");
            }
            finally
            {
                //IsDown = false;
            }
        }
    }
}
