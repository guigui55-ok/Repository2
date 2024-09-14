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
        Control _sendControl;
        Point _mousePoint;
        public IsDragEnable _isDragEnable;
        public List<Point> _historyList = new List<Point> { };
        public FormDragger(AppLogger logger, Form form, Control sendControl)
        {
            _logger = logger;
            _form = form;
            _sendControl = sendControl;
            _isDragEnable = new IsDragEnable(true);
            //_form.MouseDown += Form1_MouseDown;
            //_form.MouseMove += Form1_MouseMove;
            _sendControl.MouseDown += Form1_MouseDown;
            _sendControl.MouseMove += Form1_MouseMove;
            _sendControl.MouseUp += Form1_MouseUp;
        }

        public void PrintInfo(string value)
        {
            string msg = this.ToString() + " > ";
            msg += _sendControl.Name + " -> " + _form.Name + " > " + value;
            _logger.PrintInfo(msg);
        }

        //Form1のMouseDownイベントハンドラ
        //マウスのボタンが押されたとき
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (_isDragEnable._value)
                {
                    PrintInfo("MouseDown");
                }
                //位置を記憶する
                _mousePoint = new Point(e.X, e.Y);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (_isDragEnable._value)
                {
                    PrintInfo("MouseMove > Up");
                    //_logger.PrintInfo(String.Format(_sendControl.Name + "__MouseMove>Up , {0}", _isDragEnable._value));
                    _historyList.Add(_mousePoint);
                }
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
                    //_logger.PrintInfo(String.Format(_sendControl.Name + "__MouseMove , {0}", _isDragEnable._value));
                    //_historyList.Add(_mousePoint);
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
