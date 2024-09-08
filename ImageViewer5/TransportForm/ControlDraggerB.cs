using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AppLoggerModule;

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
        
        public ControlDraggerB(AppLogger logger, Control control,Control recieveEventControl)
        {
            _logger= logger;
            _control = control;
            _recieveControl = recieveEventControl;
            // イベントのインスタンスを生成
            //MouseEventHandler = new ViewImageMouseEventHandler();
            // このクラス内のメソッドをイベントへ紐づけ
            _recieveControl.MouseMove += Control_MouseMove;
            _recieveControl.MouseDown += Control_MouseDown;
            _recieveControl.MouseUp += Control_MouseUp;
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
                // 左ボタンの時に実行する
                if (e.Button == MouseButtons.Left)
                {
                    _control.Left += e.X - mp.X;
                    _control.Top += e.Y - mp.Y;
                    //Console.Write("#");
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
            Debug.WriteLine(this.ToString()+".Control_MouseDown"); ;
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
}
