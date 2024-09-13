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
        ControlDraggerB _draggerInnerToFrame;
        FormDragger _formDraggerByForm;
        FormDragger _formDraggerByFrame;
        FormDragger _formDraggerByInner;
        TransparentFormSwitch _transparentFormSwitch;
        double _formOpacity = 100;

        //private TransparentFormFunctionWin32B _transparentFormFunction;
        public FormTransportTest()
        {
            InitializeComponent();
            _logger = new AppLogger();
            _dragger = new ControlDraggerB(_logger, pictureBox1, pictureBox1);
            _dragger._trigerKey = Keys.Space;
            _draggerFrame = new ControlDraggerB(_logger, panel1, panel1);

            _formDraggerByForm = new FormDragger(_logger, this, this);
            _formDraggerByFrame = new FormDragger(_logger, this, panel1);

            _formDraggerByInner = new FormDragger(_logger, this, pictureBox1);
            //#
            _dragger._isDragEnable._value = true;
            _formDraggerByInner._isDragEnable._value = !_dragger._isDragEnable._value;
            //
            _draggerInnerToFrame = new ControlDraggerB(_logger, panel1, pictureBox1);
            //
            _transparentFormSwitch = new TransparentFormSwitch(_logger, this, panel1);
            //_transparentFormSwitch.SetDraggerFlags(
            //    ref _formDraggerByFrame._isDragEnable, ref _draggerFrame._isDragEnable);
            _transparentFormSwitch.SetDraggerFlags(
                ref _formDraggerByForm._isDragEnable,
                ref  _formDraggerByFrame._isDragEnable,
                ref  _draggerFrame._isDragEnable,
                ref _dragger._isDragEnable,
                ref _formDraggerByInner._isDragEnable,
                ref _draggerInnerToFrame._isDragEnable);


            // TransparentFormFunctionにthisを渡す
            //_transparentFormFunction = new TransparentFormFunction(_logger, this);

            //タイトルバーを消す
            this.ControlBox = false;
            this.Text = "";
            // フォーム全体を透過する
            //this.TransparencyKey = this.BackColor;
            // フォームの設定
            this.FormBorderStyle = FormBorderStyle.None;  // フォームの境界線を非表示
            //this.BackColor = Color.Magenta;  // フォームの背景色（透明にする色）
            //this.TransparencyKey = this.BackColor;  // Magentaを透明化
            //_transparentFormSwitch.SwitchFlagsByTransparencyKey(
            //    (this.TransparencyKey != this.BackColor));
            _transparentFormSwitch.SwitchFlagsByTransparencyKey(false);
            _transparentFormSwitch.SwitchFormTitleBarVisible(true);

            //_transparentFormSwitch.SwitchDefaultNotFormTitle_B_FreeInner();
            //_transparentFormSwitch.SwitchDefaultNotFormTitle_C_FreeFrame();
            _transparentFormSwitch.SwitchDefaultNotFormTitle_A_MoveWindowMain();

        }


        //protected override void WndProc(ref Message m)
        //{
        //    // TransparentFormFunctionでの処理を適用
        //    _transparentFormFunction.ProcessWndProc(ref m);

        //    // 基底クラスの WndProc を呼び出す
        //    base.WndProc(ref m);
        //}

        //private void SwitchFlagsByTransparencyKey(bool toOn)
        //{
        //    //if (this.TransparencyKey == this.BackColor)
        //    if(!toOn)
        //    {
        //        this.TransparencyKey = Color.Empty;
        //        _draggerFrame._isDragEnable._value = true;
        //        _formDraggerByFrame._isDragEnable._value = false;
        //        _logger.PrintInfo(String.Format("TransparencyKey = {0}", "Empty, False"));
        //    }
        //    else
        //    {
        //        this.TransparencyKey = this.BackColor;
        //        _draggerFrame._isDragEnable._value = false;
        //        _formDraggerByFrame._isDragEnable._value = true;
        //        _logger.PrintInfo(String.Format("TransparencyKey = {0}", "BackColor, True"));
        //    }
        //}

        private void FormTransportTest_Load(object sender, EventArgs e)
        {

        }

        private void FormTransportTest_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Control)
            {

            }
            if (e.KeyCode == Keys.T && e.Control)
            {
                bool toOn = true;
                if (this.TransparencyKey == this.BackColor)
                {
                    toOn = false;
                }
                _transparentFormSwitch.SwitchFlagsByTransparencyKey(toOn);
                //_transparentFormSwitch.SwitchFlagsByTransparencyKey(
                //    (this.TransparencyKey == this.BackColor));
            }
            else if (e.KeyCode == Keys.S)
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
            }
            else if (e.KeyCode == Keys.F)
            {
                if (this.FormBorderStyle == FormBorderStyle.None)
                {
                    //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                    //this.FormBorderStyle = FormBorderStyle.FixedDialog;
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "FixedSingle"));
                }
                else if(this.FormBorderStyle == FormBorderStyle.FixedSingle)
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "Sizable"));
                }
                else if (this.FormBorderStyle == FormBorderStyle.Sizable)
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "None"));
                }
            }
        }
    }
}
