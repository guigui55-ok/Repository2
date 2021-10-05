using ErrorManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MousePointCapture
{
    public class MousePointCaptureOnScreenEdgeFormEvents
    {
        protected ErrorManager.ErrorManager _error;
        protected MousePointCaptureOnScreenEdgeManager _captureFormManager;
        protected int[,] _validRangeMinMax; // 2次元配列
        protected Form _form;
        public MousePointCaptureOnScreenEdgeFormEvents(ErrorManager.ErrorManager error,MousePointCaptureOnScreenEdgeManager mouseCaptureInScreenEdgeManager, Form form)
        {
            _error = error;
            _captureFormManager = mouseCaptureInScreenEdgeManager;
            _captureFormManager.LeftForm_MouseMoveEvent += _captureFormManager_LeftForm_MouseMoveEvent;
            _captureFormManager.TopForm_MouseMoveEvent += _captureFormManager_TopForm_MouseMoveEvent;
            _captureFormManager.RightForm_MouseMoveEvent += _captureFormManager_RightForm_MouseMoveEvent;
            _captureFormManager.BottomForm_MouseMoveEvent += _captureFormManager_BottomForm_MouseMoveEvent;
            _validRangeMinMax = new int[4,2];
            InitializeValidRange(ref _validRangeMinMax);
            _form = form;
        }


        public void SetValidRangeValue(int[,] value)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetValidRangeValue");
                int n = 0;
                string buf = "[" + value[n, 0].ToString() + " , " + value[n, 1].ToString() + "] ";
                n++; buf += "[" + value[n, 0].ToString() + " , " + value[n, 1].ToString() + "] ";
                n++; buf += "[" + value[n, 0].ToString() + " , " + value[n, 1].ToString() + "] ";
                n++; buf += "[" + value[n, 0].ToString() + " , " + value[n, 1].ToString() + "] ";
                _error.AddLog( "  " +buf );
                _validRangeMinMax = value;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValidRangeValue");
            }
        }

        private void InitializeValidRange(ref int[,] validRange)
        {
            try
            {
                int screenW = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                int screenH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                int basicValW = (int)(screenW / 2);
                int basicValH = (int)(screenH / 2);
                int scope = 100;
                //left
                validRange[0, 0] = basicValH - scope;
                validRange[0, 1] = basicValH + scope;
                //top
                validRange[1, 0] = basicValW - scope;
                validRange[1, 1] = basicValW + scope;
                //right
                validRange[2, 0] = basicValH - scope;
                validRange[2, 1] = basicValH + scope;
                //bottom
                validRange[3, 0] = basicValW - scope;
                validRange[3, 1] = basicValW + scope;

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString()+ ".InitializeValidRange");
            }
        }

        private void _captureFormManager_LeftForm_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            try
            {
                if (_captureFormManager.Settings.IsCaputureLeft)
                {
                    // アクティブ時は何もしない
                    if ((Form.ActiveForm != _form))
                    {
                        if ((_validRangeMinMax[0, 0] <= e.Y) && (e.Y <= _validRangeMinMax[0, 1]))
                        {
                            string buf = _validRangeMinMax[0, 0].ToString() + "," + _validRangeMinMax[0, 1].ToString();
                            buf += " / e = " + e.X + "," + e.Y;
                            _error.AddLog("LeftForm_MouseMoveEvent FormActivate : " + buf);
                            ActivateForm();
                        }
                    }
                    else if (_form.WindowState == FormWindowState.Minimized)
                    {
                        _error.AddLog("LeftForm_MouseMoveEvent ChangeStateToNormal");
                        _form.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        ActivateForm();
                    }
                }
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ "._captureFormManager_LeftForm_MouseMoveEvent");
            }
        }

        private void _captureFormManager_TopForm_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            try
            {
                if (_captureFormManager.Settings.IsCaptureTop)
                {
                    // アクティブ時は何もしない
                    if (Form.ActiveForm != _form)
                    {
                        if ((_validRangeMinMax[1, 0] <= e.X) && (e.X <= _validRangeMinMax[1, 1]))
                        {
                            string buf = _validRangeMinMax[1, 0].ToString() + "," + _validRangeMinMax[1, 1].ToString();
                            buf += " / e = " + e.X + "," + e.Y;
                            _error.AddLog("TopForm_MouseMoveEvent FormActivate : " + buf);
                            ActivateForm();
                        }
                    }
                    else if (_form.WindowState == FormWindowState.Minimized)
                    {
                        _error.AddLog("TopForm_MouseMoveEvent ChangeStateToNormal");
                        _form.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        ActivateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._captureFormManager_TopForm_MouseMoveEvent");
            }
        }

        private void _captureFormManager_RightForm_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            try
            {
                if (_captureFormManager.Settings.IsCaptureRight)
                {
                    // アクティブ時は何もしない
                    if (Form.ActiveForm != _form)
                    {
                        if ((_validRangeMinMax[2, 0] <= e.Y) && (e.Y <= _validRangeMinMax[2, 1]))
                        {
                            string buf = _validRangeMinMax[2, 0].ToString() + "," + _validRangeMinMax[2, 1].ToString();
                            buf += " / e = " + e.X + "," + e.Y;
                            _error.AddLog("RightForm_MouseMoveEvent FormActivate : " + buf);
                            ActivateForm();
                        }
                    }
                    else if (_form.WindowState == FormWindowState.Minimized)
                    {
                        _error.AddLog("RightForm_MouseMoveEvent ChangeStateToNormal");
                        _form.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        ActivateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._captureFormManager_RightForm_MouseMoveEvent");
            }
        }

        private void _captureFormManager_BottomForm_MouseMoveEvent(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (_captureFormManager.Settings.IsCaptureBottom)
                {
                    // アクティブ時は何もしない
                    if (Form.ActiveForm != _form)
                    {
                        if ((_validRangeMinMax[3, 0] <= e.X) && (e.X <= _validRangeMinMax[3, 1]))
                        {
                            string buf = _validRangeMinMax[3, 0].ToString() + "," + _validRangeMinMax[3, 1].ToString();
                            buf += " / e = " + e.X + "," + e.Y;
                            _error.AddLog("BottomForm_MouseMoveEvent FormActivate : " + buf);
                            ActivateForm();
                        }
                    }
                    else if (_form.WindowState == FormWindowState.Minimized)
                    {
                        _error.AddLog("BottomForm_MouseMoveEvent ChangeStateToNormal");
                        _form.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        ActivateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._captureFormManager_BottomForm_MouseMoveEvent");
            }
        }

        private void ActivateForm()
        {
            try
            {
                //_error.AddLog(this, "ActivateForm");


                if (_form != null)
                {
                    if(Form.ActiveForm != null)
                    {
                        if ((Form.ActiveForm.Name == _form.Name) || (Form.ActiveForm.Name == _captureFormManager.LastActivateForm.Name))
                        {
                            //_error.AddLog(this,"Activated Already");
                            return;
                        }
                    }

                    if (_captureFormManager.LastActivateForm != null)
                    {
                        _captureFormManager.LastActivateForm.Activate();
                        _error.AddLog(this, "ActivateForm LastActivateForm");
                        return;
                    }
                    if (!_form.Modal)
                    {
                        _form.Activate();
                    } else
                    {
                        _error.AddLog("  Form.Modal=true");
                    }
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ActivateForm");
            }
        }
    }
}
