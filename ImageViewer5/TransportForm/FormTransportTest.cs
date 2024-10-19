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
        protected ControlDraggerB _draggerInnerToInner;
        protected ControlDraggerB _draggerInnerToFrame;
        protected ControlDraggerB _draggerFrameToFrame;
        protected FormDragger _draggerFrameToForm;
        protected FormDragger _draggerInnerToForm;
        protected FormDragger _draggerFormToForm;
        double _formOpacity = 100;

        private TransparentFormSwitch _transparentFormSwitch;

        //private TransparentFormFunction _transparentFormFunction;
        public FormTransportTest()
        {
            InitializeComponent();
            _logger = new AppLogger();
            _draggerInnerToInner = new ControlDraggerB(_logger, pictureBox1, pictureBox1, new SwitchKeys(Keys.Space));
            _draggerInnerToFrame = new ControlDraggerB(_logger, panel1, pictureBox1, null);
            _draggerInnerToForm = new FormDragger(_logger, this, pictureBox1);
            //
            _draggerFrameToFrame = new ControlDraggerB(_logger, panel1, panel1, new SwitchKeys(Keys.None, Keys.Control, true));
            _draggerFrameToForm = new FormDragger(_logger, this, panel1);
            //
            _draggerFormToForm = new FormDragger(_logger, this, this);

            // TransparentFormFunctionにthisを渡す
            //_transparentFormFunction = new TransparentFormFunction(_logger, this);
            _transparentFormSwitch = new TransparentFormSwitch(_logger, this, panel1);
            _transparentFormSwitch.SetDraggerFlags(
                ref _draggerFormToForm._isDragEnable,
                ref _draggerFrameToForm._isDragEnable,
                ref _draggerFrameToFrame._isDragEnable,
                ref _draggerInnerToInner._isDragEnable,
                ref _draggerInnerToForm._isDragEnable,
                ref _draggerInnerToFrame._isDragEnable);
            _transparentFormSwitch._moveInnerKey = _draggerInnerToInner._switchKeys;
            _transparentFormSwitch._moveFrameKey = _draggerFrameToFrame._switchKeys;
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
            //_transparentFormSwitch.SwitchFlagsByTransparencyKey(false);
            //_transparentFormSwitch.SwitchFormTitleBarVisible(true);

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
                if (this.TransparencyKey == this.BackColor)
                {
                    _transparentFormSwitch.SwitchFlagsByTransparencyKey(false);
                }
                else
                {
                    _transparentFormSwitch.SwitchFlagsByTransparencyKey(true);
                }
                //_transparentFormSwitch.SwitchFlagsByTransparencyKey(toOn);
                //_transparentFormSwitch.SwitchFlagsByTransparencyKey(
                //    (this.TransparencyKey == this.BackColor));
            }
            //else if (e.KeyCode == Keys.S)
            //{
            //    _formOpacity += 10;
            //    this.Opacity = _formOpacity;
            //    _logger.PrintInfo(String.Format("_formOpacity = {0}", _formOpacity));
            //    this.Refresh();
            //}else if (e.KeyCode == Keys.A)
            //{
            //    _formOpacity -= 10;
            //    this.Opacity = _formOpacity;
            //    _logger.PrintInfo(String.Format("_formOpacity = {0}", _formOpacity));
            //    this.Refresh();
            //}
            else if (e.KeyCode == Keys.D)
            {
            }
            else if (e.KeyCode == Keys.F)
            {
                //if (this.FormBorderStyle == FormBorderStyle.None)
                //{
                //    //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                //    //this.FormBorderStyle = FormBorderStyle.FixedDialog;
                //    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                //    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "FixedSingle"));
                //}
                //else if(this.FormBorderStyle == FormBorderStyle.FixedSingle)
                //{
                //    this.FormBorderStyle = FormBorderStyle.Sizable;
                //    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "Sizable"));
                //}
                //else if (this.FormBorderStyle == FormBorderStyle.Sizable)
                //{
                //    this.FormBorderStyle = FormBorderStyle.None;
                //    _logger.PrintInfo(String.Format("FormBorderStyle = {0}", "None"));
                //}
            }
        }

        private void FormTransportTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            _logger.Dispose();
            _draggerInnerToInner.Dispose();
            _draggerInnerToFrame.Dispose();
            _draggerFrameToFrame.Dispose();
            _draggerFrameToForm.Dispose();
            _draggerInnerToForm.Dispose();
            _draggerFormToForm.Dispose();
        }
    }
}
