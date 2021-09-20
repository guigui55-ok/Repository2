using ImageViewer.Function;
using ImageViewer.ParentForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.Functions
{
    // CommonFunctions > MainFormFunction 
    // ViewImageManager > MainControls > MainFormFunction
    public class MainFormFunction
    {
        protected ErrorLog.IErrorLog _errorLog;
        //public CommonFunctions Functions;
        readonly Form _mainForm;
        protected ContentsControl _contentsControl;
        protected ViewImageManager _viewImageManager;
        public MainFormFunction(ErrorLog.IErrorLog errorlog, 
            Form mainForm, ContentsControl contentscontrol, ViewImageManager viewImageManager)
        {
            _errorLog = errorlog;
            _mainForm = mainForm;
            _contentsControl = contentscontrol;
            _viewImageManager = viewImageManager;
        }

        public void ChangeVisibleMenuWithProcess()
        {
            try
            {
                int beforeWidth = _contentsControl.GetSize().Width;
                int beforeHeight = _contentsControl.GetSize().Height;

                // MenuVisible
                int afterWidth = _mainForm.Size.Width;
                int afterHeight = _mainForm.Size.Height;

                afterWidth = beforeWidth;
                afterHeight = beforeHeight;

                MenuStrip menu = GetMenuStrip();
                if (menu.Visible)
                {
                    afterHeight -= menu.Height;
                }

                // ContentsControl SizeChange
                _contentsControl.ChangeSize(new Size(afterWidth, afterHeight));

                // 倍率
                //double raito = afterHeight / beforeHeight;

                // FrameControl 縮小 位置変更
                //Functions.ControlFunction.ChangeSizeViewControl(raito);
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeVisibleMenuWithProcess Failed");
                return;
            }
        }

        public void ChangeContentSize(Size size)
        {
            try
            {
                _contentsControl.ChangeSize(size);
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "changeContentSize Failed");
                return;
            }
        }

        public void ChangeVisibleToTrueMenuStripIfFalse()
        {
            try
            {
                MenuStrip menu = GetMenuStrip();
                if (!menu.Visible) { 
                    menu.Visible = true; 
                }

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ChangeVisibleToTrueMenuStripIfFalse Failed");
            }
        }

        public void ChangeVisibleMenuStrip()
        {
            try
            {
                MenuStrip menu = GetMenuStrip();
                if (menu.Visible)
                {
                    menu.Visible = false;
                } else { 
                    menu.Visible = true; 
                }
                ChangeVisibleMenuWithProcess();
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "chcangeVisibleMenuStrip Failed");
                return;
            }
        }

        private MenuStrip GetMenuStrip()
        {
            try
            {
                foreach(Control item in _mainForm.Controls)
                {
                    if (item.GetType().Equals(typeof( MenuStrip )))
                    {
                        return (MenuStrip)item;
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "chcangeVisibleMenuStrip Failed");
                return null;
            }
        }
        // ===============================================
        // Method がたまってきたら後でクラスを分ける
        // ===============================================
        // 設定値を読み込み MenuStrip 内項目 表示(&D) > メニューバーを常に表示するのチェックを変更する
        public void ChangeChecked_MenuStrip_MenuVisible_FromSettings()
        {
            try
            {
                // 設定値を取得
                string value = _viewImageManager.MainControls.SettingsFileFunction.GetParameterValue(
                    "settings", ImageViewerConstants.SettingsName.IsMenuBarVisibleAlways);
                // string に変換
                bool flag = new Common.Convert().StrintToBool(value);
                // 値を変更
                if (flag)
                { flag = true; value = "1"; /*　ON 常に表示する*/ }
                else { flag = false; value = "0"; /* OFF 常に表示しない */ }
                // 設定値を上書き
                _viewImageManager.MainControls.SettingsFileFunction.SetParameterValue(
                    "settings", ImageViewerConstants.SettingsName.IsMenuBarVisibleAlways, value);
                // MenuStrip チェックを変更
                MenuStripRegister register = _viewImageManager.MainControls.MainFormManger.GetMenuStripRegister();
                ToolStripMenuItem item = register.GetToolStripMenuItemFromText(
                    _viewImageManager.MainControls.MainFormManger.MenuConstants.MENU_DISPLAY,
                    _viewImageManager.MainControls.MainFormManger.MenuConstants.MENU_DISPLAY_MENUBAR_VISIBLE);
                item.Checked = flag;
                // Visible
                _viewImageManager.MainControls.MainFormFunction.GetMenuStrip().Visible = flag;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ChangeChecked_MenuStrip_MenuVisible Failed");
            }
        }
        // MENU_DISPLAY_MENUBAR_VISIBLE の値を変更し、チェックを変更 switch する
        public void ChangeChecked_MenuStrip_MenuVisible_Switch()
        {
            try
            {
                // checkを変更
                // MenuStrip チェックを変更
                MenuStripRegister register = _viewImageManager.MainControls.MainFormManger.GetMenuStripRegister();
                ToolStripMenuItem item = register.GetToolStripMenuItemFromText(
                    _viewImageManager.MainControls.MainFormManger.MenuConstants.MENU_DISPLAY,
                    _viewImageManager.MainControls.MainFormManger.MenuConstants.MENU_DISPLAY_MENUBAR_VISIBLE);
                item.Checked = !item.Checked;

                // 設定値を変更
                bool flag = false; string value;
                // 値を変更
                if (!flag)
                { flag = true; value = "1"; /*　ON 常に表示する*/ }
                else { flag = false; value = "0"; /* OFF 常に表示しない */ }
                _viewImageManager.MainControls.SettingsFileFunction.SetParameterValue(
                    "settings", ImageViewerConstants.SettingsName.IsMenuBarVisibleAlways, value);
                // Settings クラス値変更
                _viewImageManager.Settings.IsMenuBarVisibleAlways = item.Checked;
                // false なら非表示に
                if (!item.Checked) { GetMenuStrip().Visible = false; }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ChangeChecked_MenuStrip_MenuVisible_FromVisibleFlag Failed");
            }
        }


        // MenuStrip.Visible のフラグから、MenuStrip の項目のチェックを変更する
        public void ChangeChecked_MenuStrip_MenuVisible_FromVisibleFlag(bool flag)
        {
            try
            {
                // checkを変更
                // MenuStrip チェックを変更
                MenuStripRegister register = _viewImageManager.MainControls.MainFormManger.GetMenuStripRegister();
                ToolStripMenuItem item = register.GetToolStripMenuItemFromText(
                    _viewImageManager.MainControls.MainFormManger.MenuConstants.MENU_DISPLAY,
                    _viewImageManager.MainControls.MainFormManger.MenuConstants.MENU_DISPLAY_MENUBAR_VISIBLE);
                item.Checked = flag;

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ChangeChecked_MenuStrip_MenuVisible_FromVisibleFlag Failed");
            }
        }
    }
}
