using ErrorUtility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ErrorUtility
{
    public class ErrorMessengerMessageBox : IErrorMessenger
    {
        protected ErrorManager _err;
        // Suppress error display エラー表示を抑制する
        protected bool _isSuppressErrorShow = false;
        protected EventHandler _showErrorMessageEvent;
        protected string _initializeValue;

        public ErrorMessengerMessageBox(ErrorManager err)
        {
            _err = err;
        }
        public bool IsSuppressErrorShow { get { return _isSuppressErrorShow; } set { _isSuppressErrorShow = value; } }
        public EventHandler ShowErrorMessageEvent { get { return _showErrorMessageEvent; } set { _showErrorMessageEvent = value; } }

        public void ChangeFont(FontStyle style, Color color)
        {
            _err.AddLog("FontStyle And FontColor Is Not Supported This Class");
        }

        public void ShowAlertLastErrorWhenHasException(string title = "")
        {
            if (_err.HasException())
            {
                if ((title == "") || (title == null))
                {
                    title = ErrorUtility.Constants.DEFAULT_WINDOW_TITLE;
                }
                string msg = _err.LastErrorMessageToUser;
                if ((msg == null) || (msg == ""))
                {
                    msg = _err.LastExceptionMessage;
                }
                msg += "\n ------- \n";
                msg += _err.LastException.Message;
                msg += "\n" + _err.LastException.StackTrace;
                // userMesssage を表示する
                ShowAlertMessage(msg,title);
            }
        }

        public void ShowAlertMessage(string msg, string title = "")
        {
            ShowMessage(msg, FontStyle.Bold, Color.Red);
        }

        public void ShowAlertMessages(string title = "")
        {
            string msg = _err.GetLastAlertMessages(true);
            ShowAlertMessage(msg, title);
        }

        public void ShowAlertMessageMessageAddToExisting(string msg, bool isBehind = true, string title = "")
        {
            ChangeFont(FontStyle.Regular | FontStyle.Bold, Color.Red);
            ShowMessageAddToExistingString(FontStyle.Regular | FontStyle.Bold, Color.Red, msg, isBehind, "\n", title);
        }


        /// <summary>
        /// Control が保持しているメッセージに、保持しているエラーメッセージを付加して表示する
        /// </summary>
        /// <param name="isBehind"></param>
        public void ShowErrorMessageseAddToExisting(bool isBehind = true)
        {
            try
            {
                //string msg = "";
                //if (_statusLabel.Text != "")
                //{
                //    msg = _statusLabel.Text;
                //}
                //msg = "ERROR! : " + msg;
                //string msg2 = _err.GetLastErrorMessagesAsString();
                //if (msg2 != "")
                //{
                //    msg += "\n" + msg2;
                //}
                _err.AddLog(this, "ShowErrorMessageseAddToExisting(bool) : msg is nothing");
                ShowMessageAddToExistingString(FontStyle.Regular | FontStyle.Bold, Color.Red, "", isBehind);

            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ShowErrorMessageseAddToExisting(bool)");
            }
        }

        /// <summary>
        /// Control が保持しているメッセージに、保持しているエラーメッセージを付加して表示する
        /// ShowMessageAddToExistingString Main Method
        /// </summary>
        /// <param name="style"></param>
        /// <param name="color"></param>
        /// <param name="msg"></param>
        /// <param name="isBehind"></param>
        /// <param name="delimiter"></param>
        /// <param name="title"></param>
        public void ShowMessageAddToExistingString(
            FontStyle style, Color color, string msg, bool isBehind = true, string delimiter = "\n", string title = "")
        {
            try
            {
                if (IsSuppressErrorShow) { _err.AddLog(this, "*IsSuppressErrorShow = true , KEEP_ERROR_STATE"); return; }
                //string buf = _statusLabel.Text;
                //if (IsInitializeValue)
                //{
                //    buf = "";
                //    IsInitializeValue = false;
                //}
                string buf = _err.GetLastErrorMessagesAsString();
                if (buf.Length >= 1)
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
                else
                {
                    buf = msg;
                }
                ShowMessage(buf, style, color);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ShowMessageAddToExistingString(FontStyle, Color, string, bool, string, string");
            }
        }

        public void ShowLastErrorByMessageBox(string title = "")
        {
            try
            {
                if (IsSuppressErrorShow){ _err.AddLog(this,"*IsSuppressErrorShow = true , KEEP_ERROR_STATE"); return; }
                if (_err.HasException())
                {
                    if ((title == "") || (title == null)) { title = Constants.DEFAULT_WINDOW_TITLE; }
                    string msg = _err.LastErrorMessageToUser;
                    if ((msg == null) || (msg == "")) { msg = _err.LastExceptionMessage; }
                    msg += "\n ------- \n";
                    msg += _err.LastException.Message;
                    msg += "\n" + _err.LastException.StackTrace;
                    // userMesssage を表示して
                    MessageBox.Show(msg, title);
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ShowLastErrorByMessageBox");
            }
        }

        /// <summary>
        /// ShowMessage Main Method
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        public void ShowMessage(string msg, string title = "")
        {
            try
            {
                if(title == "") { title = "ERROR!"; }
                MessageBox.Show(msg,title);
                //if (!_statusStrip.Visible)
                //{

                //    _statusLabel.Text = msg;
                //    _statusStrip.Visible = true;
                //    msgTimer.Start();
                //}
                //else
                //{
                //    _statusLabel.Text = msg;
                //}
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ShowMessage");
            }
        }

        public void ShowMessage(string msg, FontStyle style, Color color, string title = "")
        {
            try
            {
                if(style != FontStyle.Regular) { _err.AddLog(this,"Style Not Apply"); }
                if (color != null) { _err.AddLog(this, "color Not Apply"); }
                ShowMessage(msg, title);
            }
            catch (Exception ex) 
            {
                _err.AddException(ex,this, "ShowMessage(string, FontStyle, Color, string)");
            }
        }

        public void ShowMessageAddToExistingString(FontStyle style, Color color, string msg, string title = "")
        {
            ShowMessageAddToExistingString(style, color, msg, true, "\n", "");
        }

        public void ShowNormalMessage(string msg, string title = "")
        {
            ShowMessage(msg, FontStyle.Regular, Color.Black);
        }

        public void ShowResultSuccessMessage(string msg, string title = "")
        {
            ShowMessage(msg, FontStyle.Bold, Color.Green);
        }

        // ShowMessageAddToExistingStringToBehind(FontStyle,Color,string,string,string)
        public void ShowMessageAddToExistingStringToBehind(
            FontStyle style, Color color, string msg, string delimiter = "\n", string title = "")
        {
            ShowMessageAddToExistingString(style, color, msg, false, delimiter, title);
        }

        // ShowResultSuccessMessageAddToExisting(string,bool,string)
        public void ShowResultSuccessMessageAddToExisting(string msg, bool isBehind = true, string title = "")
        {
            try
            {
                ChangeFont(FontStyle.Regular, Color.Green);
                ShowMessageAddToExistingStringToBehind(FontStyle.Regular, Color.Green, msg, " < ", title);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ShowResultSuccessMessageAddToExisting(string,bool,string)");
            }
        }

        /// <summary>
        /// ShowUserMessageOnly(string,bool,bool)
        /// </summary>
        /// <param name="title"></param>
        /// <param name="OrderIsRev"></param>
        /// <param name="isAddExceptionMessage"></param>
        public void ShowUserMessageOnly(string title = "", bool OrderIsRev = true, bool isAddExceptionMessage = false)
        {
            string msg = _err.GetUserMessageOnlyAsString(OrderIsRev, isAddExceptionMessage);
            if (msg == "")
            {
                msg = _err.GetLastErrorMessagesAsString(3, isAddExceptionMessage);
            }
            ShowAlertMessage(msg, title);
        }

        // ShowUserMessageOnlyAddToExisting(string,string,bool)
        public void ShowUserMessageOnlyAddToExisting(string msg, string title = "", bool OrderIsRev = true)
        {
            ChangeFont(FontStyle.Regular, Color.Black);
            ShowMessageAddToExistingString(FontStyle.Regular, Color.Black, msg, title);
        }

        public void ShowWarningMessage(string msg, string title = "")
        {
            ShowMessage(msg, FontStyle.Bold, Color.OrangeRed);
        }

        // ShowWarningMessageMessageAddToExisting(string,bool,string)
        public void ShowWarningMessageMessageAddToExisting(string msg, bool isBeHind = true, string title = "")
        {
            ChangeFont(FontStyle.Regular, Color.Yellow);
            ShowMessageAddToExistingString(FontStyle.Regular, Color.Yellow, msg, isBeHind, "\n", title);
        }

        public void test()
        {
            _err.AddLog(this, "test");
        }
    }
}
