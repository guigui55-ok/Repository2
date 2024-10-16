﻿
using System;
using System.Drawing;

namespace ErrorUtility
{
    public interface IErrorMessenger
    {
        // Suppress error display エラー表示を抑制する
        bool IsSuppressErrorShow { get; set; }
        EventHandler ShowErrorMessageEvent { get; set; }
        void ShowLastErrorByMessageBox(string title = "");
        void ShowMessage(string msg, string title = "");
        void ShowMessage(string msg, FontStyle style, Color color, string title = "");
        void ShowResultSuccessMessage(string msg, string title = "");
        void ShowNormalMessage(string msg, string title = "");
        void ShowWarningMessage(string msg, string title = "");
        void ShowAlertMessage(string msg, string title = "");
        void ShowAlertLastErrorWhenHasException(string title = "");
        void ShowAlertMessages(string title = "");
        void ShowUserMessageOnly(string title = "", bool OrderIsRev = true,bool isAddExceptionMessage=false);
        void ShowMessageAddToExistingStringToBehind(FontStyle style, Color color, string msg, string delimiter = "\n", string title = "");
        //Add to an existing string 既存の文字列に追加
        void ShowMessageAddToExistingString(FontStyle style, Color color, string msg, string title = "");
        void ShowResultSuccessMessageAddToExisting(string msg, bool isBehind = true, string title = "");
        void ShowWarningMessageMessageAddToExisting(string msg, bool isBeHind = true, string title = "");
        void ShowAlertMessageMessageAddToExisting(string msg, bool isBehind = true, string title = "");
        void ShowUserMessageOnlyAddToExisting(string msg,string title = "", bool OrderIsRev = true);
        void ShowErrorMessageseAddToExisting(bool isBehind = true);
        void ShowMessageAddToExistingString(
            FontStyle style, Color color, string msg, bool isBehind = true, string delimiter = "\n", string title = "");
        void ChangeFont(FontStyle style, Color color);
        void test();
    }
}
