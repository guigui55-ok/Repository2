using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using CommonModules;

namespace TransportForm
{

    /// <summary>
    /// Draggerの設定の組み合わせ（パターン）ごとのMode/名前を扱う
    /// </summary>
    class ConstMoveControlSetMode
    {
        public const int DEFAULT = 0;
        public const int DEFAULT_ONLY_FREE_INNER = 1;
        public const int DEFAULT_ONLY_FREE_FRAME = 2;
        public const int ONLY_INNER_TO_FRAME = 3;
        //const int DEFAULT_FREE_INNER_FRAME = 11;
        public const int WINDOW_MOVE_MAIN = 11;
        public const int FIX_WINDOW = 12;
    }

    /*
     * ControlDraggerB と FormDragger に依存するクラス
     * Formの透明度と、フォームをドラッグで移動できるかを管理、切替するクラス
     * 
     * _form.TransparencyKeyを使用してFormを透明にして、タイトルバーをなくすと、
     * Formの背景で移動できるようにしても、全くFormをつかめなくなりマウスで移動できなくなる。
     * この時、Form上の子コントロールのマウスイベントに紐づけてFormを移動するが、その時の動作を制御するクラス
     * 
     * SetDraggerFlagsを使用して制御する
     * ドラッグする機能は～Draggerクラスで行う。このクラスが持つフラグ IsDragEnable クラスで制御する
     * 
     * _formDragFlagを持つDraggerクラスでは、_Formの背景をドラッグするとFormが動かせる
     * _controlDragFlagを持つDraggerクラスでは、Frameコントロールの背景をドラッグするとFormが動かせる
     * 
     * ついでにForm_KeyDown Ctrl+Tで透明と非透明を切り替え
     * Ctrl+Yでウィンドウタイトル表示、非表示を切り替え
     * Ctrl+Uでウィンドウ枠のスタイルを FixedSingle、Sizable、Noneの3つを切り替える
     * 
     * ほぼ常に、FrameはFromをほぼ覆う想定
     * なので、ほぼ常に、Frmae＞Fromを移動として、
     * タイトルがないときに、PictureBox＞Form移動とする、
     * この状態で何かのアクションでモードを切り替え、Picturebox＞PictureBox移動とする
     */
    public class TransparentFormSwitch
    {
        AppLogger _logger;
        Form _form;
        Control _control;
        IsDragEnable _formToFormDragFlag;
        //
        IsDragEnable _frameToFormDragFlag;
        IsDragEnable _frameToFrameDragFlag;
        //
        IsDragEnable _innerToinnerDragFlag;
        IsDragEnable _innerToFormDragFlag;
        IsDragEnable _innerToFrameDragFlag;
        // CtrlのKeyDown時にFrameを動かすときに、元に戻す用のフラグ格納リスト
        //List<bool> _tempFrameFlagList = new List<bool> { false, false, false };
        //240910
        // ControlKeyはKeyDown,Up判定処理が異なるが一旦以下のままとする
        //public Keys _moveInnerKey = Keys.Space;
        //public Keys _moveFrameKey = Keys.Control;
        //public SwitchKeys _moveInnerKey);
        //public SwitchKeys _moveFrameKeyl;
        //Load時に参照エラーとなるのでインスタンスを生成（初期化時に外部からインスタンスが生成されるので注意）
        public SwitchKeys _moveInnerKey = new SwitchKeys(Keys.None); 
        public SwitchKeys _moveFrameKey = new SwitchKeys(Keys.None);
        //
        private ExecuteDelayTimer _innerKeyTimer = new ExecuteDelayTimer();
        private ExecuteDelayTimer _frameKeyTimer = new ExecuteDelayTimer();
        int _mode;
        string msg = "";
        public TransparentFormSwitch(AppLogger logger, Form form, Control control)
        {
            _logger = logger;
            _logger.PrintInfo("TransparentFormSwitch Constracta");
            _form = form;
            _control = control;
            _form.KeyDown += FormTransport_KeyDown;
            _form.KeyUp += FormTransport_KeyUp;
        }

        /// <summary>
        /// 各フラグをセットする（渡し間違え注意）
        /// </summary>
        /// <param name="formToFormDragFlag"></param>
        /// <param name="frameToFormDragFlag"></param>
        /// <param name="frameToFramelDragFlag"></param>
        /// <param name="innerToInnerDragFlag"></param>
        /// <param name="innerToFormDragFlag"></param>
        public void SetDraggerFlags(
            ref IsDragEnable formToFormDragFlag,
            ref IsDragEnable frameToFormDragFlag,
            ref IsDragEnable frameToFramelDragFlag,
            ref IsDragEnable innerToInnerDragFlag,
            ref IsDragEnable innerToFormDragFlag,
            ref IsDragEnable innerToFrameDragFlag)
        {
            _formToFormDragFlag = formToFormDragFlag;
            _frameToFormDragFlag = frameToFormDragFlag;
            _frameToFrameDragFlag = frameToFramelDragFlag;
            _innerToinnerDragFlag = innerToInnerDragFlag;
            _innerToFormDragFlag = innerToFormDragFlag;
            _innerToFrameDragFlag = innerToFrameDragFlag;
        }


        public void SwitchNowMode()
        {
            SwitchDefaultNotFormTitle_A_MoveWindowMain();
            SwitchFlagsByTransparency();
        }

        /// <summary>
        /// InnerからFrameを移動する
        /// FormToForm=On
        /// </summary>
        /// <param name="toOn"></param>
        public void SwitchInnerToFormOnly(bool toOn)
        {
            if (toOn)
            {
                _mode = ConstMoveControlSetMode.ONLY_INNER_TO_FRAME;
                _logger.PrintInfo("SwitchInnerToFormOnly");
                //#
                //InnerToInner_OFF , InnerToForm_ON
                _innerToinnerDragFlag._value = false;
                _innerToFormDragFlag._value = !_innerToFormDragFlag._value;
                //
                //FrameToForm_ON, FrameToFrame_OFF
                _frameToFormDragFlag._value = true;
                _frameToFrameDragFlag._value = !_frameToFormDragFlag._value;
                //
                //FormToForm_OFF
                _formToFormDragFlag._value = true;
            }
            {
                SwitchDefault();
            }
        }

        /// <summary>
        /// タイトルバーがあって、それぞれ個別に動かせるのをデフォルトとする
        /// FormToForm=ON
        /// </summary>
        public void SwitchDefault()
        {
            _logger.PrintInfo("SwitchDefault");
            _mode = ConstMoveControlSetMode.DEFAULT;
            ////#
            ////InnerToInner on , InnerToForm off
            //_innerToinnerDragFlag._value = true;
            //_innerToFormDragFlag._value = !_innerToFormDragFlag._value;
            ////
            ////FrameToForm off, FrameToFrame on
            //_frameToFrameDragFlag._value = true;
            //_frameToFormDragFlag._value = !_frameToFrameDragFlag._value;
            ////
            ////FormToForm on
            //_formToFormDragFlag._value = true;
            ////
            ////InnerToFrame off
            //_innerToFrameDragFlag._value = true;
            //#
            //InnerToInner ON, FrameToFrame ONにしていたが、変更 Form移動をメインに
            //#
            SwitchDefaultNotFormTitle_A_MoveWindowMain();

            SwitchFlagsByTransparency();
        }

        /// <summary>
        /// FormタイトルがないときのデフォルトA（Window移動メイン）
        /// innerToInner OFF(InnerToForm ON), FrameToFrame OFF (FrameToForm ON)
        /// FormToForm ON
        /// </summary>
        public void SwitchDefaultNotFormTitle_A_MoveWindowMain()
        {
            _logger.PrintInfo("SwitchDefaultNotFormTitle_A_MoveWindowMain");
            _mode = ConstMoveControlSetMode.WINDOW_MOVE_MAIN;
            //#
            //InnerToInner off , InnerToForm on
            _innerToinnerDragFlag._value = false;
            _innerToFormDragFlag._value = true;
            //InnerToFrame
            _innerToFrameDragFlag._value = false;
            //
            //FrameToForm off, FrameToFrame on
            _frameToFrameDragFlag._value = false;
            _frameToFormDragFlag._value = !_frameToFrameDragFlag._value;
            //
            //FormToForm
            _formToFormDragFlag._value = true;
        }

        /// <summary>
        /// FormタイトルがないときのデフォルトB（Innnerのみフリー）
        /// innerToInner ON(InnerToForm OFF), FrameToFrame OFF (FrameToForm ON)
        /// FormToForm ON
        /// InnerFree,
        /// </summary>
        public void SwitchDefaultNotFormTitle_B_FreeInner()
        {
            _mode = ConstMoveControlSetMode.DEFAULT_ONLY_FREE_FRAME;
            //#
            //InnerToInner on , InnerToForm off
            _innerToinnerDragFlag._value = true;
            _innerToFormDragFlag._value = !_innerToinnerDragFlag._value;
            //
            //FrameToForm off, FrameToFrame on
            _frameToFrameDragFlag._value = !_innerToinnerDragFlag._value;
            _frameToFormDragFlag._value = true;
            //
            //FormToForm on
            _formToFormDragFlag._value = true;
            //InnerToFrame
            _innerToFrameDragFlag._value = true;
        }


        /// <summary>
        /// FormタイトルがないときC（Frameのみフリー）
        /// innerToInner OFF(InnerToForm ON), FrameToFrame ON (FrameToForm OFF)
        /// FormToForm ON
        /// </summary>
        public void SwitchDefaultNotFormTitle_C_FreeFrame()
        {
            _mode = ConstMoveControlSetMode.DEFAULT_ONLY_FREE_FRAME;
            //#
            //InnerToInner on , InnerToForm off
            _innerToinnerDragFlag._value = false;
            _innerToFormDragFlag._value = false;
            //
            //FrameToForm off, FrameToFrame on
            _frameToFrameDragFlag._value = true;
            _frameToFormDragFlag._value = !_frameToFrameDragFlag._value;
            //
            //FormToForm on
            _formToFormDragFlag._value = true;
            //InnerToFrame
            _innerToFrameDragFlag._value = true;
        }


        /// <summary>
        /// 透明なときも InnerToInner OFF, FrameToFrame OFF ＝ Form移動メインにする
        /// 
        /// </summary>
        public void SwitchFlagsByTransparency()
        {
            if(_form.TransparencyKey != _form.BackColor)
            {
                //SwitchDefault(); //SwitchDefaultから呼ばれるようにする
                _innerToinnerDragFlag._value = false;
                _innerToFormDragFlag._value = true;
            }
            else
            {
                //_frameToFormDragFlag._value = true;
                //_formToFormDragFlag._value = false;
                _innerToinnerDragFlag._value = false;
                _innerToFormDragFlag._value = true;
                _frameToFormDragFlag._value = true;
            }
        }

        public void SwitchFlagsByTransparencyKey(bool toOn)
        {
            _logger.PrintInfo("SwitchFlagsByTransparencyKey");
            //if (this.TransparencyKey == this.BackColor)
            if (!toOn)
            {
                //
                _form.TransparencyKey = Color.Empty;
                _form.FormBorderStyle = FormBorderStyle.Sizable;
                PrintInfo(String.Format("TransparencyKey = {0}", "Empty, False"));
            }
            else
            {
                //透明
                _form.TransparencyKey = _form.BackColor;
                _form.FormBorderStyle = FormBorderStyle.None;
                PrintInfo(String.Format("TransparencyKey = {0}", "BackColor, True"));
            }
            SwitchFlagsByTransparency();
        }

        private void PrintInfo(string value)
        {
            msg = this.ToString() + ".";
            _logger.PrintInfo(msg + value);
        }


        public void SwitchFormTitleBarVisible(bool toOn)
        {
            if (toOn)
            {
                _form.FormBorderStyle = FormBorderStyle.Sizable;
                _form.ControlBox = true;
                _form.Text = "ImageViewer5";
                _logger.PrintInfo(this.ToString() + String.Format(".FormTitle Visible = {0}", "True"));
                //FrameToForm_OFF, FormToForm_ON
                _frameToFormDragFlag._value = false;
                _formToFormDragFlag._value = !_frameToFormDragFlag._value;
                //FormON
                //#
                SwitchDefault();
            }
            else
            {
                //タイトルバーを消す
                _form.FormBorderStyle = FormBorderStyle.None;
                _form.ControlBox = false;
                _form.Text = "";
                _logger.PrintInfo(this.ToString() + String.Format(".FormTitle Visible = {0}", "False"));
                //FrameのUserControlはほぼ常にFormを追っているので、
                //ウィンドウがないときはUserControlでFormを動かす
                //#
                //InnerToInner_OFF , InnerToForm_ON
                _innerToinnerDragFlag._value = false;
                _innerToFormDragFlag._value = !_innerToFormDragFlag._value;
                //
                //FrameToForm_ON, FormToForm_OFF
                _frameToFormDragFlag._value = true;
                _formToFormDragFlag._value = !_frameToFormDragFlag._value;
                //
                //FrameToFrame_OFF
                _frameToFrameDragFlag._value = false;
            }
        }

        private void FormTransport_KeyUp(object sender, KeyEventArgs e)
        {
            //    if (e.KeyCode == _moveInnerKey)
            //    {
            //        //_innerToinnerDragFlag._value = false;
            //        //_innerToFormDragFlag._value = !_innerToinnerDragFlag._value;
            //        //_logger.PrintInfo("KeyUp , " + _moveFrameKey.ToString("G"));
            //        ////SwitchNowMode();
            //        //SwitchDefault();
            //    }
            //    else if (IsControlKeyPressed(e, _moveFrameKey))
            //    {
            //        //_frameToFrameDragFlag._value = false;
            //        //_frameToFormDragFlag._value = !_frameToFrameDragFlag._value;
            //        ////Frameの時はInnerでもFrameを動かせ多のを元に戻す
            //        ////_innerToinnerDragFlag._value = _tempFrameFlagList[0];
            //        ////_innerToFrameDragFlag._value = _tempFrameFlagList[1];
            //        //_innerToinnerDragFlag._value = _tempFrameFlagList[0];
            //        //_innerToFrameDragFlag._value = _tempFrameFlagList[1];
            //        //_innerToFormDragFlag._value = _tempFrameFlagList[2];
            //        //_logger.PrintInfo(String.Join(",", _tempFrameFlagList));
            //        ////SwitchNowMode();
            //        //SwitchDefault();
            //        _logger.PrintInfo("KeyUp , " + _moveFrameKey.ToString("G"));
            //    }
            if (_moveFrameKey.IsMatch(e))
            {
                //_logger.PrintInfo("*Key_UP , " + e.KeyCode.ToString("G"));
                bool FrameIsOn = false;
                _frameToFrameDragFlag._value = FrameIsOn;
                _frameToFormDragFlag._value = !FrameIsOn;
                _innerToFrameDragFlag._value = FrameIsOn;
                _innerToFormDragFlag._value = !FrameIsOn;
                SwitchDefault();
            }
            else if (_moveInnerKey.IsMatch(e))
            {
                //_logger.PrintInfo("*Key_UP , " + e.KeyCode.ToString("G"));
                bool InnerIsOn = false;
                _innerToinnerDragFlag._value = InnerIsOn;
                _innerToFormDragFlag._value = !InnerIsOn;
                _frameToFormDragFlag._value = !InnerIsOn;
                _frameToFrameDragFlag._value = InnerIsOn;
                SwitchDefault();
            }
        }

        //private bool IsControlKeyPressed(KeyEventArgs e, Keys controlKeyCode)
        //{
        //    // Controlキーが押されたかどうかを判定
        //    return e.KeyCode == Keys.ControlKey || e.KeyCode == controlKeyCode;
        //}

        private void FormTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (_moveFrameKey.IsMatch(e))
                {
                    //_logger.PrintInfo("*KeyDown , " + e.KeyCode.ToString("G"));
                    bool FrameIsOn = true;
                    _frameToFrameDragFlag._value = FrameIsOn;
                    _frameToFormDragFlag._value = !FrameIsOn;
                    _innerToFrameDragFlag._value = FrameIsOn;
                    _innerToFormDragFlag._value = !FrameIsOn;
                }
                else if (_moveInnerKey.IsMatch(e))
                {
                    //_logger.PrintInfo("*KeyDown , " + e.KeyCode.ToString("G"));
                    bool InnerIsOn = true;
                    _innerToinnerDragFlag._value = InnerIsOn;
                    _innerToFormDragFlag._value = !InnerIsOn;
                    _frameToFormDragFlag._value = !InnerIsOn;
                    _frameToFrameDragFlag._value = InnerIsOn;
                }

                //if (e.KeyCode == _moveInnerKey)
                //{
                //    //_innerToinnerDragFlag._value = true;
                //    //_innerToFormDragFlag._value = !_innerToinnerDragFlag._value;
                //    ////_logger.PrintInfo("KeyDown , " + e.KeyCode.ToString("G"));
                //    ////_innerKeyTimer.Execute(_logger.PrintInfo, "KeyDown , " + e.KeyCode.ToString("G"));
                //    _innerKeyTimer.Execute(value => _logger.PrintInfo(value.ToString()), "KeyDown , " + e.KeyCode.ToString("G"));
                //}
                //else if (IsControlKeyPressed(e, _moveFrameKey))
                //{
                //    //_frameToFrameDragFlag._value = true;
                //    //_frameToFormDragFlag._value = !_frameToFrameDragFlag._value;
                //    ////Frameの時はInnerでもFrameを動かせるように
                //    //_tempFrameFlagList[0] = _innerToinnerDragFlag._value;
                //    //_innerToinnerDragFlag._value = false;
                //    //_tempFrameFlagList[1] = _innerToFrameDragFlag._value;
                //    //_innerToFrameDragFlag._value = true;
                //    //_tempFrameFlagList[2] = _innerToFrameDragFlag._value;
                //    //_innerToFormDragFlag._value = false;
                //    ////_logger.PrintInfo("KeyDown , " + e.KeyCode.ToString("G"));
                //    _frameKeyTimer.Execute(value => _logger.PrintInfo(value.ToString()), "KeyDown , " + e.KeyCode.ToString("G"));
                //}
                if (e.KeyCode == Keys.Y && e.Control)
                {
                    if (_form.Text == "")
                    {
                        SwitchFormTitleBarVisible(true);
                    }
                    else
                    {
                        //タイトルバーを消す
                        SwitchFormTitleBarVisible(false);
                    }
                }
                else if (e.KeyCode == Keys.T && e.Control)
                {
                    if (_form.TransparencyKey == _form.BackColor)
                    {
                        SwitchFlagsByTransparencyKey(false);
                    }
                    else
                    {
                        SwitchFlagsByTransparencyKey(true);
                    }

                    //SwitchFlagsByTransparencyKey(
                    //    (_form.TransparencyKey != _form.BackColor));
                }
                else if (e.KeyCode == Keys.U)
                {
                    if (_form.FormBorderStyle == FormBorderStyle.None)
                    {
                        //this.FormBorderStyle = FormBorderStyle.Fixed3D;
                        //this.FormBorderStyle = FormBorderStyle.FixedDialog;
                        _form.FormBorderStyle = FormBorderStyle.FixedSingle;
                        _logger.PrintInfo(this.ToString() + String.Format("FormBorderStyle = {0}", "FixedSingle"));
                    }
                    else if (_form.FormBorderStyle == FormBorderStyle.FixedSingle)
                    {
                        _form.FormBorderStyle = FormBorderStyle.Sizable;
                        _logger.PrintInfo(this.ToString() + String.Format("FormBorderStyle = {0}", "Sizable"));
                    }
                    else if (_form.FormBorderStyle == FormBorderStyle.Sizable)
                    {
                        _form.FormBorderStyle = FormBorderStyle.None;
                        _logger.PrintInfo(this.ToString() + String.Format("FormBorderStyle = {0}", "None"));
                    }
                }
            } catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + " > FormTransport_KeyDown");
            }
        }
    }
}
