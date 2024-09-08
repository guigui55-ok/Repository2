using AppLoggerModule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TransportForm
{
    public partial class FormTransportTest : Form
    {
        AppLogger _logger;
        ControlDraggerB _dragger;
        ControlDraggerB _draggerFrame;
        FormDragger _formDragger;
        double _formOpacity = 100;


        private TransparentFormFunction _transparentFormFunction;
        public FormTransportTest()
        {
            InitializeComponent();
            _logger = new AppLogger();
            _dragger = new ControlDraggerB(_logger, pictureBox1, pictureBox1);
            //_draggerFrame = new ControlDraggerB(_logger, panel1, panel1);
            _formDragger = new FormDragger(_logger, this, panel1);

            // TransparentFormFunctionにthisを渡す
            //_transparentFormFunction = new TransparentFormFunction(_logger, this);

            //タイトルバーを消す
            this.ControlBox = false;
            this.Text = "";
            // フォーム全体を透過する
            //this.TransparencyKey = this.BackColor;
            // フォームの設定
            this.FormBorderStyle = FormBorderStyle.None;  // フォームの境界線を非表示
            this.BackColor = Color.Magenta;  // フォームの背景色（透明にする色）
            this.TransparencyKey = this.BackColor;  // Magentaを透明化


        }


        //protected override void WndProc(ref Message m)
        //{
        //    // TransparentFormFunctionでの処理を適用
        //    _transparentFormFunction.ProcessWndProc(ref m);

        //    // 基底クラスの WndProc を呼び出す
        //    base.WndProc(ref m);
        //}

        private void FormTransportTest_Load(object sender, EventArgs e)
        {

        }

        private void FormTransportTest_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode == Keys.I && e.Control)
            {

            }else if (e.KeyCode == Keys.S)
            {
                _formOpacity += 10;
                this.Opacity = _formOpacity;
                _logger.PrintInfo(String.Format("_formOpacity = {0}", _formOpacity));
                this.Refresh();
            }else if (e.KeyCode == Keys.A)
            {
                _formOpacity -= 10;
                this.Opacity = _formOpacity;
                _logger.PrintInfo(String.Format("_formOpacity = {0}", _formOpacity));
                this.Refresh();
            }
            else if (e.KeyCode == Keys.D)
            {
                if (this.TransparencyKey == this.BackColor)
                {
                    this.TransparencyKey = Color.Empty;
                    _logger.PrintInfo(String.Format("TransparencyKey = {0}", "Empty"));
                }
                else
                {
                    this.TransparencyKey = this.BackColor;
                    _logger.PrintInfo(String.Format("TransparencyKey = {0}", "BackColor"));
                }
            }
            else if (e.KeyCode == Keys.F)
            {
                if (this.FormBorderStyle == FormBorderStyle.Sizable)
                {
                    //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                    //this.FormBorderStyle = FormBorderStyle.FixedDialog;
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "FixedSingle"));
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "Sizable"));
                }
            }
        }
    }
}
