using ExcelCellsManager.ExcelCellsManager;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExcelCellsManager.ErrorMessage
{
    public class ErrorMessengerStatusBar : ErrorManager.IErrorMessenger
    {
        protected ErrorManager.ErrorManager _error;
        protected StatusStrip _statusStrip;
        protected Form _parentForm;
        protected ToolStripStatusLabel _statusLabel;
        protected Font _worningFont;
        protected Font _alertFont;
        event EventHandler _showErrorEvent;
        private int _statusStripHeight;
        private ToolStripDropDownButton _button;
        private bool isVisibleTrueAfter = false;
        protected bool isSuppressErrorShow = false;
        protected Timer msgTimer;
        public int MessageUnvisibleInterval = 5000;
        public EventHandler ShowErrorMessageEvent { get => _showErrorEvent; set => _showErrorEvent = value; }
        public bool IsSuppressErrorShow { get => isSuppressErrorShow; set => isSuppressErrorShow = value; }

        public ErrorMessengerStatusBar(ErrorManager.ErrorManager error, Form parentForm, StatusStrip statusStrip,int MsgInterval)
        {
            try
            {
                _error = error;
                if (statusStrip == null)
                {
                    _statusStrip = new StatusStrip();
                } else
                {
                    _statusStrip = statusStrip;
                }                
                _parentForm = parentForm;
                Initialize();
                msgTimer = new Timer();
                MessageUnvisibleInterval = MsgInterval;
                if (MessageUnvisibleInterval > 0)
                {
                    _error.AddLog(" MessageUnvisibleInterval > 0 : MsgTimer_Tick Function Not Set");
                    msgTimer.Tick += MsgTimer_Tick;
                    msgTimer.Interval = MessageUnvisibleInterval;
                } else
                {
                    _statusStrip.Visible = true;
                    _statusLabel.Visible = true;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Constracta");
            }

        }

        private void MsgTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                msgTimer.Stop();
                _error.AddLog(this, "MsgTimer_Tick Excute");
                if (!_statusStrip.Visible) { return; }
                msgTimer.Stop();
                int height = _statusStrip.Height;
                _statusStripHeight = _statusLabel.Height;
                _statusLabel.Text = "";
                _statusStrip.Visible = false;
                _showErrorEvent?.Invoke(-height, EventArgs.Empty);
            } catch (Exception ex)
            {
                _error.AddException(ex, this, "MsgTimer_Tick");
            }
        }

        public void ShowMessageAddToExistingString(FontStyle style, Color color, string msg, string title = "")
        {
            ShowMessageAddToExistingString(style, color, msg,true, '\n', "");
        }

        public void ShowMessageAddToExistingStringToBehind(FontStyle style, Color color, string msg, char delimiter = '\n', string title = "")
        {
            ShowMessageAddToExistingString(style, color, msg, false, delimiter, title);
        }


        public void ShowMessageAddToExistingString(FontStyle style,Color color, string msg, bool isBehind = true,char delimiter = '\n',string title = "")
        {
            try
            {
                string buf = _statusLabel.Text;
                if(buf.Length >= 1)
                {
                    if (isBehind)
                    {
                        buf += delimiter + msg;
                    }
                    else
                    {
                        buf = msg + delimiter + buf;
                    }
                }
                ShowMessage(buf,  style,color); 
            } catch (Exception ex)
            {
                _error.AddException(ex, this, "ShowMessageAddToExistingString");
            }
        }

        /// <summary>
        /// ErrorManager.HasException=true のときに、その内容を表示する
        /// </summary>
        /// <param name="msg"></param>
        public void ShowLastErrorByMessageBox(string MsgBoxTitle = "")
        {
            try
            {
                if (IsSuppressErrorShow)
                { _error.AddLog("*IsSuppressErrorShow = true , KEEP_ERROR_STATE"); return; }
                if (_error.HasException())
                {
                    if ((MsgBoxTitle == "") || (MsgBoxTitle == null))
                    {
                        MsgBoxTitle = "Error";
                    }
                    string msg = _error.LastErrorMessageToUser;
                    if ((msg == null) || (msg == ""))
                    {
                        msg = _error.LastExceptionMessage;
                    }
                    msg += "\n ------- \n";
                    msg  += _error.LastException.Message;
                    msg += "\n" + _error.LastException.StackTrace;
                    // userMesssage を表示して
                    MessageBox.Show(msg, MsgBoxTitle);
                } 
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ShowException");
            }
        }

        public void ChangeFont(FontStyle style, Color color)
        {
            Font font = new Font(_statusLabel.Font, style);
            _statusLabel.ForeColor = color;
            _statusLabel.Font = font;
        }
        public void test()
        {
            _statusLabel.ForeColor = Color.Red;
            _statusLabel.Text = "StatusLabel View Test ForeColor Red";
        }

        public void ShowAlertLastErrorWhenHasException(string title = "")
        {
            try
            {
                if (IsSuppressErrorShow)
                { _error.AddLog("*IsSuppressErrorShow = true , KEEP_ERROR_STATE"); return; }
                if (_error.HasException())
                {
                    if ((title == "") || (title == null))
                    {
                        title = "Error";
                    }
                    string msg = _error.LastErrorMessageToUser;
                    if ((msg == null) || (msg == ""))
                    {
                        msg = _error.LastExceptionMessage;
                    }
                    msg += "\n ------- \n";
                    msg += _error.LastException.Message;
                    msg += "\n" + _error.LastException.StackTrace;
                    // userMesssage を表示する
                    ShowAlertMessage(msg);
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ShowException");
            }
        }



        public void ShowUserMessageOnly(string title = "",bool OrderIsRev = true,bool isAddExceptionMessage = false)
        {
            if (IsSuppressErrorShow)
            { _error.AddLog("*IsSuppressErrorShow = true ,GetUserMessage Not Excute, KEEP_ERROR_STATE");return; }
            string msg = _error.GetUserMessageOnlyAsString(OrderIsRev, isAddExceptionMessage);
            if (msg == "")
            {
                msg = _error.GetLastErrorMessagesAsString(3, isAddExceptionMessage);
            }
            ShowAlertMessage(msg, title);
        }



        public void ShowErrorMessageseAddToExisting()
        {
            try
            {
                string msg = "";
                if(_statusLabel.Text != "")
                {
                    msg = _statusLabel.Text;
                }
                string msg2 = _error.GetLastErrorMessagesAsString();
                if (msg2 != "")
                {
                    msg += "\n" + msg2;
                }
                ShowAlertMessage(msg);

            } catch (Exception ex)
            {
                _error.AddException(ex, this, "ShowErrorMessageseAddToExisting");
            }
        }

        public void ShowAlertMessages(string title = "")
        {
            try
            {
                if (IsSuppressErrorShow)
                { _error.AddLog("*IsSuppressErrorShow = true ,GetLastAlertMessages Not Excute, KEEP_ERROR_STATE"); return; }
                string msg = _error.GetLastAlertMessages(true);
                ShowAlertMessage(msg, title);
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".ShowAlertMessages");
            }
        }

       
        public void ShowResultSuccessMessage(string msg,string title = "")
        {
            ShowMessage(msg, FontStyle.Bold, Color.Green);
        }
        public void ShowAlertMessage(string msg,string title = "")
        {
            ShowMessage(msg, FontStyle.Bold, Color.Red);
        }
        public void ShowWarningMessage(string msg, string title = "")
        {
            ShowMessage(msg, FontStyle.Bold, Color.OrangeRed);
        }
        public void ShowNormalMessage(string msg, string title = "")
        {
            ShowMessage(msg, FontStyle.Regular, Color.Black);
        }

        public void ShowMessage(string msg,FontStyle style,Color color, string title = "")
        {
            try
            {
                if (IsSuppressErrorShow)
                { _error.AddLog("*IsSuppressErrorShow = true , KEEP_ERROR_STATE"); return; }
                Font font = new Font(_statusLabel.Font,style);
                _statusLabel.ForeColor = color;
                _statusLabel.Font = font;
                ShowMessage(msg);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ShowMessage");
            }
        }

        public void ShowMessage(string msg, string title = "")
        {
            try
            {
                if (!_statusStrip.Visible)
                {

                    _statusLabel.Text = msg;
                    _statusStrip.Visible = true;
                    msgTimer.Start();
                }
                else
                {
                    _statusLabel.Text = msg;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ShowMessage");
            }
        }

        public void Initialize()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".Initialize");
                _parentForm.Controls.Add(_statusStrip);
                _statusStrip.Parent = _parentForm;
                _statusLabel = new ToolStripStatusLabel
                {
                    Text = "StatusBarLabel1.Text 2",
                    TextAlign = ContentAlignment.MiddleLeft,
                    Spring = true
                };
                _button = new ToolStripDropDownButton
                {
                    Text = "×",
                    Name = "Button_StatusStripClose",
                    Size = new Size(30, 20),
                    ShowDropDownArrow = false
                };
                _button.Click += _button_Click;
                //_button.Location = new Point(3,3);
                //_statusStrip.Controls(_button);
                _statusStrip.Items.Add(_button);
                _statusStrip.Items.Add(_statusLabel);
                _statusStrip.Visible = false;
                _statusStrip.DoubleClick += _statusStrip_DoubleClick;
                _statusStrip.VisibleChanged += _statusStrip_VisibleChanged;
                _statusStrip.Paint += _statusStrip_Paint;


            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
            }
        }

        private void _statusStrip_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                _error.AddLog("_statusStrip_Paint");
                if (isVisibleTrueAfter)
                {
                    isVisibleTrueAfter = false;
                    _error.AddLog("_statusStrip.Height = " + _statusStrip.Height);
                    _statusStripHeight = _statusStrip.Height;
                } else
                {
                    _statusStripHeight = -_statusStrip.Height;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._statusStrip_Paint");
            }
        }

        private void _button_Click(object sender, EventArgs e)
        {
            _statusStrip.Visible = false;
            //_showErrorEvent?.Invoke(-_statusStripHeight, EventArgs.Empty);
        }

        private void _statusStrip_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString()+"._statusLabel_VisibleChanged");
                if (_statusLabel.Visible)
                {
                    //_parentForm.Height += _statusLabel.Height;
                    _statusStripHeight = _statusStrip.Height;
                    isVisibleTrueAfter = true;
                } else
                {
                    msgTimer.Stop();
                    _statusStripHeight = -_statusStrip.Height;
                    //_parentForm.Height -= _statusLabel.Height;
                }
                _showErrorEvent?.Invoke(_statusStripHeight, EventArgs.Empty);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._statusLabel_VisibleChanged");
            }
        }

        private void _statusStrip_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString()+ "._statusStrip_DoubleClick : _statusStrip.Visible = false");
                _statusStrip.Visible = false;
                //_showErrorEvent?.Invoke(-_statusStripHeight, EventArgs.Empty);

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._statusStrip_DoubleClick");
            }
        }

        public void ShowResultSuccessMessageAddToExisting(string msg, string title = "")
        {
            try
            {
                ChangeFont(_statusLabel.Font.Style, Color.Green);
                ShowMessageAddToExistingString(_statusLabel.Font.Style, Color.Green,msg, title);
            }catch  (Exception ex)
            {
                _error.AddException(ex, this, "ShowResultSuccessMessageAddToExisting");
            }
        }

        public void ShowWarningMessageMessageAddToExisting(string msg, string title = "")
        {
            try
            {
                ChangeFont(_statusLabel.Font.Style, Color.Yellow);
                ShowMessageAddToExistingString(_statusLabel.Font.Style, Color.Yellow,msg, title);
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this, "ShowResultSuccessMessageAddToExisting");
            }
        }

        public void ShowAlertMessageMessageAddToExisting(string msg, string title = "")
        {
            try
            {
                ChangeFont(_statusLabel.Font.Style | FontStyle.Bold, Color.Red);
                ShowMessageAddToExistingString(_statusLabel.Font.Style | FontStyle.Bold, Color.Red,msg, title);
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this, "ShowResultSuccessMessageAddToExisting");
            }
        }

        public void ShowUserMessageOnlyAddToExisting(string msg,string title = "", bool OrderIsRev = true)
        {
            try
            {
                ChangeFont(_statusLabel.Font.Style, Color.Black);
                ShowMessageAddToExistingString(_statusLabel.Font.Style, Color.Black,msg, title);
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this, "ShowResultSuccessMessageAddToExisting");
            }
        }
    }
}
