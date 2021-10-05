using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MousePointCapture
{
    public class MousePointCaptureOnScreenEdgeManager
    {
        protected ErrorManager.ErrorManager _error;
        public MousePointCaptureOnScreenEdgeSettings Settings;
        protected Form _form;
        protected Size _screenSize;
        protected Size _minFormSize;
        
        protected Form[] _thinFormsOnEnge;
        public event EventHandler LeftForm_MouseEnterEvent;
        public event EventHandler TopForm_MouseEnterEvent;
        public event EventHandler RightForm_MouseEnterEvent;
        public event EventHandler BottomForm_MouseEnterEvent;

        public event MouseEventHandler LeftForm_MouseMoveEvent;
        public event MouseEventHandler TopForm_MouseMoveEvent;
        public event MouseEventHandler RightForm_MouseMoveEvent;
        public event MouseEventHandler BottomForm_MouseMoveEvent;

        public MousePointCaptureOnScreenEdgeFormEvents EdgeFormEvents;
        //public EventHandler SaveFormActivateEvent;
        public Form LastActivateForm;
        public MousePointCaptureOnScreenEdgeManager(ErrorManager.ErrorManager error)
        {
            ConstractaProcess(error);
            // EdgeFormEvents 設定なし
        }

        public MousePointCaptureOnScreenEdgeManager(ErrorManager.ErrorManager error,Form form)
        {
            ConstractaProcess(error);
            if (EdgeFormEvents == null)
            {
                if (form == null) { _error.AddLog(this.ToString() + ".MouseCaptureInScreenEdgeManager form is null.");  return; }
                _error.AddLog("MouseCaptureInScreenEdgeManager Constracta: new MouseCaptureInScreenEdgeFormEvents Form.Text="+form.Text);
                this.EdgeFormEvents = new MousePointCaptureOnScreenEdgeFormEvents(_error,this, form);
            }
        }

        public void SaveFormActivateEvent(object sender, EventArgs e)
        {
            try
            {
                //if (sender.GetType().Equals(typeof(Form)))
                //{
                //}
                LastActivateForm = Form.ActiveForm;
                if(LastActivateForm != null)
                {
                    _error.AddLog(this, "SaveFormActivateEvent LastActivateForm=" + LastActivateForm.Name);
                }
                else
                {
                    _error.AddLog(this, "SaveFormActivateEvent LastActivateForm= NULL");
                }

            }
            catch (Exception ex)
            {
                _error.AddLogAlert(this, "SaveFormActivateEvent Failed", "SaveFormActivateEvent Failed", ex);
            }
        }

        private void ConstractaProcess(ErrorManager.ErrorManager error)
        {
            _error = error;
            Settings = new MousePointCaptureOnScreenEdgeSettings();
            Settings.IsCaputureLeft = true;
            Settings.IsCaptureTop = true;
            Settings.IsCaptureRight = false;
            Settings.IsCaptureBottom = false;
            //Settings.LeftValidRange = new int[] { 200, 400 };
            InitializeValidRange();
        }

        public void InitializeValidRange()
        {
            try
            {
                int[,] validRange = new int[4, 2];
                int screenW = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                int screenH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                int basicValW = (int)(screenW / 2);
                int basicValH = (int)(screenH / 2);
                int scope = 100;
                //left
                int[] buf = new int[2];
                buf[0] = basicValH - scope;
                buf[1] = basicValH + scope;
                Settings.LeftValidRange = buf;
                //top
                buf = new int[2];
                buf[0] = basicValW - scope;
                buf[1] = basicValW + scope;
                Settings.TopValidRange = buf;
                //right
                buf = new int[2];
                buf[0] = basicValH - scope;
                buf[1] = basicValH + scope;
                Settings.RightValidRange = buf;
                //bottom
                buf = new int[2];
                buf[0]= basicValW - scope;
                buf[1] = basicValW + scope;
                Settings.BottomValidrange = buf;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".InitializeValidRange");
            }
        }

        private int[] CheckRangeValue(ref int[] value,int max)
        {
            try
            {
                if (value.Length < 1)
                {
                    _error.AddLog("CheckRangeValue value.Length < 1");
                    return new int[] { 0, 0 };
                }
                if (value.Length == 1)
                {
                    _error.AddLog("CheckRangeValue value.Length == 1");
                    value = new int[] { 0,value[0]};
                }
                if (value.Length >= 2)
                {
                    if(value[0] > value[1])
                    {
                        int buf = value[0];
                        value[0] = value[1];
                        value[1] = buf;
                    }
                    if (value[0] > max) { value[0] = max; }
                    if (value[1] > max) { value[1] = max; }
                }
                return value;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".CheckRangeValue");
                return new int[] { 0, 0 };
            }
        }

        /// <summary>
        /// 設定値をセット・リセットする
        /// </summary>
        /// <param name="ScreenNumbers"></param>
        /// <param name="leftVal"></param>
        /// <param name="topVal"></param>
        /// <param name="rightVal"></param>
        /// <param name="bottomVal"></param>
        public void ResetSettings(int[] ScreenNumbers,int[] leftVal,int[] topVal,int[] rightVal,int[] bottomVal)
        {
            try
            {
                // ScreenNumber で マルチディスプレイ時の Screen を指定する
                Screen[] screens = GetScreensBySettinsValue(ScreenNumbers);
                Settings.ScreenNumbers = ScreenNumbers;
                // 配列要素数が奇数の場合は最後に追加する
                // Max (Screen 座標)以上の時は Max 値にする
                // 1つ目と2つ目、2つ目と3つ目、･･･比較して小さいほうを左にする xx
                // 配列の1つ目と2つ目だけ判定する
                // left
                int e = 0; // element Num
                int screenW = screens[e].Bounds.Width;  int screenH = screens[e].Bounds.Height;
                CheckRangeValue(ref leftVal, screenH);
                Settings.LeftValidRange = leftVal;
                // top
                e = 1; // element Num
                screenW = screens[e].Bounds.Width;  screenH = screens[e].Bounds.Height;
                CheckRangeValue(ref topVal, screenW);
                Settings.TopValidRange = topVal;
                // right
                e = 2; // element Num
                screenW = screens[e].Bounds.Width; screenH = screens[e].Bounds.Height;
                CheckRangeValue(ref rightVal, screenH);
                Settings.RightValidRange = rightVal;
                // bottom
                e = 3; // element Num
                screenW = screens[e].Bounds.Width; screenH = screens[e].Bounds.Height;
                CheckRangeValue(ref bottomVal, screenW);
                Settings.BottomValidrange = bottomVal;

                if (EdgeFormEvents == null) { throw new Exception("EdgeFormEvents ClassObject == Null"); }
                // EdgeForms に格納しておく 別途
                EdgeFormEvents.SetValidRangeValue (
                new int[,]
                {
                     { leftVal[0] , leftVal[1] } ,
                     { topVal[0] , topVal[1] },
                     { rightVal[0] , rightVal[1] },
                     { bottomVal[0] , bottomVal[0] }
                });
                // あとはイベント発生時にそれぞれ判定する
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ResetSettings");
            }
        }

        
        public void SetFlagIsValidCapture(bool flag)
        {
            Settings.IsValidCapture = flag;
            if (!flag)
            {
                SetFormVisible(false, false, false, false);
            } else
            {
                SetFormVisible(Settings.IsCaputureLeft, Settings.IsCaptureTop, Settings.IsCaptureRight, Settings.IsCaptureBottom);
            }
        }
        public void SetFormVisible(bool left,bool top,bool right,bool bottom)
        {
            try
            {
                if(_thinFormsOnEnge == null) { _error.AddLog(this.ToString() + ".SetFormVisible Forms is null. all"); }
                if (_thinFormsOnEnge[0] == null) { _error.AddLog("SetFormVisible Forms is null. 0"); }
                else
                {
                    _thinFormsOnEnge[0].Visible = left;
                }
                if (_thinFormsOnEnge[1] == null) { _error.AddLog(this.ToString() + ".SetFormVisible Forms is null. 1"); }
                else
                {
                    _thinFormsOnEnge[1].Visible = top;
                }
                if (_thinFormsOnEnge[2] == null) { _error.AddLog(this.ToString() + ".SetFormVisible Forms is null. 2"); }
                else
                {
                    _thinFormsOnEnge[2].Visible = right;
                }
                if (_thinFormsOnEnge[3] == null) { _error.AddLog(this.ToString() + ".SetFormVisible Forms is null. 3"); }
                else
                {
                    _thinFormsOnEnge[3].Visible = bottom;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetFormVisible");
            }
        }

        public void ShowForms()
        {
            try
            {
                foreach(Form form in _thinFormsOnEnge)
                {
                    form.Show();
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ShowForms");
            }
        }

        public Screen[] GetScreensBySettinsValue(int[] ScreenNumbers)
        {
            try
            {
                Screen[] screens = new Screen[4];
                for (int i = 0; i < ScreenNumbers.Length; i++)
                {
                    screens[i] = GetScreen(ScreenNumbers[i]);
                }
                return screens;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetScreensBySettinsValue");
                return new Screen[] {
                    Screen.PrimaryScreen,
                    Screen.PrimaryScreen,
                    Screen.PrimaryScreen,
                    Screen.PrimaryScreen
                };
            }
        }

        public Screen GetScreen(int ScreenNumber)
        {
            try
            {
                _error.AddLog(this.ToString() + ".GetScreen");
                if ((ScreenNumber > Screen.AllScreens.Length) || (ScreenNumber < 1))
                {
                    _error.AddLog(" ScreenNumber Is Invalid. num="+ScreenNumber); ScreenNumber = 1;
                }
                int count = 0;
                Screen nowScreen = null;
                foreach (Screen scr in Screen.AllScreens)
                {
                    if (ScreenNumber - 1 == count)
                    {
                        nowScreen = scr;
                    }
                }
                if (nowScreen == null) { nowScreen = Screen.PrimaryScreen; }
                return nowScreen;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetScreen");
                return Screen.PrimaryScreen;
            }
        }

        public void InitializeForms()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".InitializeForms");
                // SettingsValue から Screen を取得しておく
                Screen[] screens = GetScreensBySettinsValue(Settings.ScreenNumbers);

                Size minFomsSize = new Size(136, 39);
                _minFormSize = minFomsSize;
                double opacity = 0.01;
                _thinFormsOnEnge = new Form[4];
                // LeftForm
                int nowElement = 0;
                _screenSize = new Size(screens[nowElement].Bounds.Width, screens[nowElement].Bounds.Height);
                int width = minFomsSize.Width;
                int height = _screenSize.Height;
                Initialize(ref _thinFormsOnEnge[nowElement], width, height,
                   1 - minFomsSize.Width, 0, opacity, LeftForm_MouseEnter);
                _thinFormsOnEnge[nowElement].ShowInTaskbar = false;
                // TopForm
                nowElement = 1;
                _screenSize = new Size(screens[nowElement].Bounds.Width, screens[nowElement].Bounds.Height);
                width = _screenSize.Width;
                height = minFomsSize.Height;
                Initialize(ref _thinFormsOnEnge[nowElement], width, height,
                    0, 1 - minFomsSize.Height, opacity, TopForm_MouseEnter);
                _thinFormsOnEnge[1].ShowInTaskbar = false;
                // RightForm
                nowElement = 2;
                width = minFomsSize.Width;
                height = _screenSize.Height;
                int locationX = _screenSize.Width - 1;
                int locationY = 0;
                Initialize(ref _thinFormsOnEnge[nowElement], width, height,
                    locationX, locationY, opacity, RightForm_MouseEnter);
                _thinFormsOnEnge[nowElement].ShowInTaskbar = false;
                // BottomForm
                nowElement = 3;
                width = _screenSize.Width;
                height = minFomsSize.Height;
                locationX = 0;
                locationY = _screenSize.Height - 1;
                Initialize(ref _thinFormsOnEnge[nowElement], width, height,
                    locationX, locationY, opacity, BottomForm_MouseEnter);
                _thinFormsOnEnge[nowElement].ShowInTaskbar = false;
                // MouseMove
                _thinFormsOnEnge[0].MouseMove += LeftForm_MouseMove;
                _thinFormsOnEnge[0].MouseMove += LeftForm_MouseMoveEvent;
                _thinFormsOnEnge[1].MouseMove += TopForm_MouseMove;
                _thinFormsOnEnge[1].MouseMove += TopForm_MouseMoveEvent;
                _thinFormsOnEnge[2].MouseMove += RightForm_MouseMove;
                _thinFormsOnEnge[2].MouseMove += RightForm_MouseMoveEvent;
                _thinFormsOnEnge[3].MouseMove += BottomForm_MouseMove;
                _thinFormsOnEnge[3].MouseMove += BottomForm_MouseMoveEvent;

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".InitializeForms");
            }
        }

        private void LeftForm_MouseEnter(object sender,EventArgs e)
        {
            //_error.AddLog("LeftForm_MouseEnter");
            if (LeftForm_MouseEnterEvent != null) { LeftForm_MouseEnterEvent.Invoke(sender, e); }
        }
        private void TopForm_MouseEnter(object sender,EventArgs e)
        {
            //_error.AddLog("TopForm_MouseEnter");
            if (TopForm_MouseEnterEvent != null) { TopForm_MouseEnterEvent.Invoke(sender, e); }
        }
        private void RightForm_MouseEnter(object sender,EventArgs e)
        {
            //_error.AddLog("RightForm_MouseEnter");
            if (RightForm_MouseEnterEvent != null) { RightForm_MouseEnterEvent.Invoke(sender, e); }
        }
        private void BottomForm_MouseEnter(object sender,EventArgs e)
        {
            //_error.AddLog("BottomForm_MouseEnter");
            if (BottomForm_MouseEnterEvent != null) { BottomForm_MouseEnterEvent.Invoke(sender, e); }
        }
        private void LeftForm_MouseMove(object sender,MouseEventArgs e)
        {
            try
            {
                if (Settings.IsValidCapture)
                {
                    if (e.X != (_minFormSize.Width - 1)) { _error.AddLog("LeftForm_MouseMove [e.X != (_minFormSize.Width -1)] x=" + e.X); }
                    else
                    {
                        if (LeftForm_MouseMoveEvent != null) { LeftForm_MouseMoveEvent.Invoke(sender, e); }
                    }
                }
            } catch (Exception ex) { _error.AddException(ex, this.ToString() + ".LeftForm_MouseMove"); }
        }
        private void TopForm_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (Settings.IsValidCapture)
                {
                    if (e.Y != (_minFormSize.Height - 1)) { _error.AddLog("TopForm_MouseMove [e.Y != (_minFormSize.Height -1)] y= " + e.Y); }
                    else
                    {
                        if (TopForm_MouseMoveEvent != null) { TopForm_MouseMoveEvent.Invoke(sender, e); }
                    }
                }
            } catch (Exception ex) { _error.AddException(ex, this.ToString() + ".TopForm_MouseMove"); }
        }
        private void RightForm_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (Settings.IsValidCapture)
                {
                    if (e.X != 0) { _error.AddLog("RightForm_MouseMove [e.X != 0] x= " + e.X); }
                    else
                    {
                        if (RightForm_MouseMoveEvent != null) { RightForm_MouseMoveEvent.Invoke(sender, e); }
                    }
                }
            }
            catch (Exception ex) { _error.AddException(ex, this.ToString() + ".RightForm_MouseMove"); }
        }
        private void BottomForm_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (Settings.IsValidCapture)
                {
                    if (e.Y != 0) { _error.AddLog("BottomForm_MouseMove [e.Y != 0] y= " + e.Y); }
                    else
                    {
                        if (BottomForm_MouseMoveEvent != null) { BottomForm_MouseMoveEvent.Invoke(sender, e); }
                    }
                }
            }
            catch (Exception ex) { _error.AddException(ex, this.ToString() + ".BottomForm_MouseMove"); }
        }

        public void Initialize(ref Form form,
            int width, int height, int locationX, int locationY, double opacity, EventHandler handler)
        {
            try
            {
                form = new Form();
                form.FormBorderStyle = FormBorderStyle.None;
                form.Text = "";
                form.Width = width;
                form.Height = height;
                form.StartPosition = FormStartPosition.Manual;
                form.DesktopLocation = new System.Drawing.Point(locationX, locationY);
                form.Location = new System.Drawing.Point(locationX, locationY);
                //_form.TransparencyKey = Form.DefaultBackColor; // MouseEventは補足されなくなる
                form.Opacity = opacity;
                form.TopMost = true;
                //_form.MouseMove += _form_MouseMove;
                form.MouseEnter += handler;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
            }
        }


        //private void Initialize()
        //{
        //    try
        //    {
        //        int FormLocationX = -135;

        //        _form = new Form();
        //        _form.FormBorderStyle = FormBorderStyle.None;
        //        _form.Text = "";
        //        _form.Width = 136;
        //        _form.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        //        _form.StartPosition = FormStartPosition.Manual;
        //        _form.DesktopLocation = new System.Drawing.Point(FormLocationX, 0);
        //        _form.Location = new System.Drawing.Point(FormLocationX, 0);
        //        //_form.TransparencyKey = Form.DefaultBackColor; // MouseEventは補足されなくなる
        //        _form.Opacity = 0.01;
        //        _form.TopMost = true;
        //        //_form.MouseMove += _form_MouseMove;
        //        _form.MouseEnter += _form_MouseEnter;

        //        _form.Show();
        //        Console.WriteLine("form width = " + _form.Width); // 136
        //    } catch (Exception ex)
        //    {
        //        _error.AddException(ex,this.ToString()+ ".Initialize");
        //    }
        //}


        private void _form_MouseEnter(object sender,EventArgs e)
        {

        }

        private void _form_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Console.WriteLine("Sub form_MouseMove : x="+ e.X + " , y=" + e.Y );
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ "._form_MouseMove");
            }
        }
    }
}
