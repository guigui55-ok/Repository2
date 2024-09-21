using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AppLoggerModule;
using System.Collections.Generic;

namespace TransportForm
{
    // InnerContrl ControlMove用　ChangeLocation
    // ImageViewer.CommonModules のファイル・クラスと名前が競合するので 末尾にBを付与
    // このクラスは動作テスト用
    // ControlのAnchr、Dock設定はOFFにしておくこと
    public class ControlDraggerB
    {
        protected AppLogger _logger;
        protected Control _control;
        protected Control _recieveControl;
        //public Events.ViewImageMouseEventHandler MouseEventHandler;
        protected bool IsDown = false;
        protected bool IsMove = false;
        private Point mpBegin;
        protected bool ControlAncherIsNone = true;
        protected bool ControlDockIsNone = true;

        public bool isSendToForm = true;
        public Form _sendControl;
        public IsDragEnable _isDragEnable;
        public List<Point> _historyList = new List<Point> {  };
        //このキーが押された＋MouseDrag、AND、_isDragEnable=True で control移動をする
        //Keys.Noneの時は、_isDragEnable=Trueのみで移動する
        public SwitchKeys _switchKeys = new SwitchKeys(Keys.None); // 未設定でもエラーにならないよう初期化する
        protected bool _isDownTrigerKey = false;
        //
        // コンストラクタで設定する。デバッグの条件を満たすかどうかを格納する
        // レシーバーと対象のコントロール名などが、特定の場合に動作させたいときに使用する 240920
        public bool _isDebugMatch = false;
        public string _DebugMemo = "";
        
        public ControlDraggerB(AppLogger logger, Control control,Control recieveEventControl, SwitchKeys switchKeys)
        {
            //_enableKeys = enableKeys;
            _logger = logger;
            _control = control;
            _recieveControl = recieveEventControl;
            _isDragEnable = new IsDragEnable(true);
            if (switchKeys != null)
            {
                _switchKeys = switchKeys;
            }
            _switchKeys.SetEventForControl(recieveEventControl);
            // イベントのインスタンスを生成
            //MouseEventHandler = new ViewImageMouseEventHandler();
            // このクラス内のメソッドをイベントへ紐づけ
            _recieveControl.MouseMove += Control_MouseMove;
            _recieveControl.MouseDown += Control_MouseDown;
            _recieveControl.MouseUp += Control_MouseUp;

            string controlTypeStr = _control.GetType().ToString();
            _DebugMemo = _recieveControl.GetType() + "_>_" + controlTypeStr;
            if ((_recieveControl.GetType()==typeof(PictureBox)) && (controlTypeStr.IndexOf("ImageMainFrame")>0))
            {
                //_isDebugMatch = true;
                PrintInfo("ControlDraggerB > _isDebugMatch = true");
                _logger.PrintInfo("ControlDraggerB > _isDebugMatch = true");
                _logger.PrintInfo(_DebugMemo);
            }
        }

        public void SetTrigerKey(Keys key)
        {

        }

        public void PrintInfo(string value)
        {
            string msg = this.ToString() + " > ";
            msg += _recieveControl.Name + " -> " + _control.Name + " > " + value;
            _logger.PrintInfo(msg);
        }

        private void ChangeLocationByMouse(MouseEventArgs e,Point mp)
        {
            try
            {
                // ドラッグする間延々とログが出続けてしまうのでフラグで抑制する
                if ((!ControlAncherIsNone) || (!ControlDockIsNone)) { return; }
                // Ancher が設定されているときは実行しない
                if ((_control.Anchor != AnchorStyles.None))
                {
                    string buf = CommonModules.CommonUtility.DisplayEnumValues(typeof(AnchorStyles), (int)_control.Anchor);
                    if (buf == "") { buf = _control.Anchor.ToString(); }
                    _logger.AddLogAlert(this, "ChangeLocationByMouse:_control.Anchor != AnchorStyles.None > " + buf);
                    ControlAncherIsNone = false;
                    return;
                }
                // Dock が設定されているときは実行しない
                if (_control.Dock != DockStyle.None)
                {
                    string buf = CommonModules.CommonUtility.DisplayEnumValues(typeof(AnchorStyles), (int)_control.Dock);
                    _logger.AddLogAlert(this, "ChangeLocationByMouse:_control.Dock != DockStyle.None > " + buf);
                    ControlDockIsNone = false;
                    return;
                }
                if (_isDebugMatch)
                {
                    PrintInfo(_DebugMemo);
                    _logger.PrintInfo(_DebugMemo);
                    _logger.PrintInfo("_isDebugMatch");
                }
                if (_isDragEnable._value)
                {
                    // 左ボタンの時に実行する
                    if (e.Button == MouseButtons.Left)
                    {
                        _historyList.Add(new Point(mp.X, mp.Y));
                        _control.Left += e.X - mp.X;
                        _control.Top += e.Y - mp.Y;
                        //Console.Write("#");
                    }
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex, this.ToString(), "changeLocationByMouse");
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
            } catch (Exception ex)
            {
                _logger.AddException(ex, this.ToString(), "Control_MouseMove");
            } finally
            {
                IsMove = false;
            }
        }
        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (_isDragEnable._value)
            {
                PrintInfo("Control_MouseDown");
            }
            IsDown = true;
            try
            {
                mpBegin = e.Location;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this.ToString(), "Control_MouseDown");
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
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this.ToString(), "Control_MouseUp");
            }
            finally
            {
                //IsDown = false;
            }
        }
    }


    // ################################################################################
    /// <summary>
    /// draggerクラスで値を保持して、外部から変更したものを反映させるためのクラス
    /// </summary>
    public class IsEnableFlag
    {
        public bool _value;
        public IsEnableFlag(bool flag = false)
        {
            _value = flag;
        }
    }

    /// <summary>
    /// draggerクラスで、移動を有効するときのKey設定
    /// このクラスでKeyが押されたかどうかを判定して、そのbool値を保持する
    /// イベントの設定はSetEventForControlで行う
    /// </summary>
    public class SwitchKeys
    {
        public Keys _key;
        public Keys _controlKey;
        public IsEnableFlag _isEnableFlag;
        public bool _isControlOnly;

        public SwitchKeys(Keys key, Keys controlKey = Keys.None, bool isControlOnly = false)
        {
            _key = key;
            _controlKey = controlKey;
            _isControlOnly = isControlOnly;
            _isEnableFlag = new IsEnableFlag();
        }

        public void SetEventForControl(Control control)
        {
            control.KeyDown += SwitchKeys_KeyDown;
            control.KeyUp += SwitchKeys_KeyUp;
        }

        public void SwitchKeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsMatch(e))
            {
                _isEnableFlag._value = true;
            }
        }

        public void SwitchKeys_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsMatch(e))
            {
                _isEnableFlag._value = false;
            }
        }

        public bool IsMatch(KeyEventArgs e)
        {
            bool retCon;
            bool ret;
            //Control以外は未対応
            //(_controlKey == Keys.Control)
            //Console.WriteLine(string.Format("## {0} , {1}", _controlKey, e.KeyData)); //## Control , ControlKey, Contro
            //Console.WriteLine(string.Format("## {0} , {1}", _controlKey, e.KeyData));
            //if (_controlKey  Keys.ControlKey == e.KeyCode)
            if (e.KeyCode == _controlKey)
            {
                if (e.Control)
                {
                    retCon = true;
                }
                else
                {
                    retCon = false;
                }
                //Controlキーのみで判定する
                if (_isControlOnly)
                {
                    return retCon;
                }
            }
            else
            {
                retCon = true;
            }
            //
            if (_key != Keys.None)
            {
                if (e.KeyCode == _key)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }
            else
            {
                ret = true;
            }
            //Console.WriteLine("IsMatch = " + (ret && retCon));
            return ret && retCon;
        }
    }

}
