using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace TransportForm
{
    public class FormDragger
    {
        AppLogger _logger;
        Form _form;
        Control _recieveControl;
        Point _mousePoint;
        public IsEnableFlag _isDragEnable;
        public FormDragger(AppLogger logger, Form form, Control recieveControl)
        {
            _logger = logger;
            _form = form;
            _recieveControl = recieveControl;
            _isDragEnable = new IsEnableFlag(true);
            //_form.MouseDown += Form1_MouseDown;
            //_form.MouseMove += Form1_MouseMove;
            _recieveControl.MouseDown += Form1_MouseDown;
            _recieveControl.MouseMove += Form1_MouseMove;
        }

        //Form1のMouseDownイベントハンドラ
        //マウスのボタンが押されたとき
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //位置を記憶する
                _mousePoint = new Point(e.X, e.Y);
            }
        }

        //Form1のMouseMoveイベントハンドラ
        //マウスが動いたとき
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (_isDragEnable._value)
                {
                    _form.Left += e.X - _mousePoint.X;
                    _form.Top += e.Y - _mousePoint.Y;
                    //または、つぎのようにする
                    //this.Location = new Point(
                    //    this.Location.X + e.X - mousePoint.X,
                    //    this.Location.Y + e.Y - mousePoint.Y);
                }
            }
        }
    }
}
