using ImageViewer.Function;
using System;
using System.Windows.Forms;

namespace ImageViewer.ParentForms
{
    public class MenuStripEvents
    {
        //
        //public CommonFunctions Functions;
        public ViewImageManager ViewImageManager;
        public MainFormManager.Constants Constants;
        public MainFormState MainFormState;
        //
        protected ErrorLog.IErrorLog _errorLog;
        protected MenuStrip _menuStrip;
        protected MenuStripRegister _register;

        public MenuStripEvents(ErrorLog.IErrorLog errorlog,ViewImageManager maager, MenuStrip menuStrip)
        {
            _errorLog = errorlog;
            ViewImageManager = maager;
            _menuStrip = menuStrip;
            _register = new MenuStripRegister(_menuStrip);
            _menuStrip.VisibleChanged += MenuStrip_VisibleChanged;
        }

        public MenuStrip GetMenuStrip() { return _menuStrip; }
        // 実行したい機能のクラス
        //public TestFunction TestFunction;

        private void MenuStrip_VisibleChanged(object sender,EventArgs e)
        {
            try
            {
                //if (ViewImageManager.MainControls.MainFormManger.State.Initialize) { return; }
                //ViewImageManager.MainControls.MainFormFunction.
                //    ChangeChecked_MenuStrip_MenuVisible_FromVisibleFlag(_menuStrip.Visible);
                // 
                if (ViewImageManager.Settings.IsMenuBarVisibleAlways)
                {
                    if(ViewImageManager.MainControls.MainFormManger.State.Initialize) { return; }
                    // 常に表示するなのに、表示されていない場合は表示する
                    if (GetMenuStrip().Visible == false) { GetMenuStrip().Visible = true; }
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "MenuStrip_VisibleChanged Failed.");
            }
        }

        public int RegistFunctionForMenuStripEvents()
        {
            try
            {
                ToolStripMenuItem item;
                // 機能クラスを読み込んでおく　->外部で設定する
                //TestFunction = new TestFunction();
                // 定数クラスを読み込んでおく ->外部で設定する
                //Constants = new MenuStripSampleForm.Constants();

                // イベントをセット
                _menuStrip.Paint += MenuStrip_Paint;

                // メニューイベントをセット
                // OpenFile
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_FILE, Constants.MENU_FILE_OPEN);
                item.Click += MenuStrip_OpenFile;
                // SaveAsName
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_FILE, Constants.MENU_FILE_SAVE_AS_NAME);
                item.Click += MenuStrip_SaveAsName;
                // Print
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_FILE, Constants.MENU_FILE_PRINT);
                item.Click += MenuStrip_Print;
                // Exit
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_FILE, Constants.MENU_FILE_EXIT);
                item.Click += MenuStrip_Exit;
                // DisplayFoward
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_DISPLAY, Constants.MENU_DISPLAY_FOWARD);
                item.Click += MenuStrip_DisplayFoward;
                // DisplayReverse
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_DISPLAY, Constants.MENU_DISPLAY_REVERSE);
                item.Click += MenuStrip_DisplayReverse;
                // DisplayMenuVisible
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_DISPLAY, Constants.MENU_DISPLAY_MENUBAR_VISIBLE);
                item.Click += MenuStrip_MenuVisible;
                // SettingsOption
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_SETTINGS, Constants.MENU_SETTINGS_OPTION);
                item.Click += MenuStrip_SettingsOption;
                // ShowHelp
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_HELP, Constants.MENU_HELP_HELP);
                item.Click += MenuStrip_ShowHelp;
                // ShowVersion
                item = _register.GetToolStripMenuItemFromText(Constants.MENU_HELP, Constants.MENU_HELP_VERSION);
                item.Click += MenuStrip_ShowVersion;
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "RegistFunctionForMenuStripEvents");
                return 0;
            }
        }

        private void MenuStrip_Paint(object sender,EventArgs e)
        {
            if (!MainFormState.IsVisibleMenuStrip)
            {
                //Debug.WriteLine("MenuStrip_Paint State Visible  false -> true");
                // false -> true
                // コントロールを変化させるタイミング
                MainFormState.IsVisibleMenuStrip = true;
                //Debug.WriteLine("Height = " + _menuStrip.Height);
            } else
            {
                //Debug.WriteLine("MenuStrip_Paint State Visible true ");
                //Debug.WriteLine("Height = " + _menuStrip.Height);
            }
        }
        private void MenuStrip_OpenFile(object sender,EventArgs e)
        {
            ViewImageManager.TestFunction("MenuStrip_OpenFile");
        }
        private void MenuStrip_SaveAsName(object sender, EventArgs e)
        {
            ViewImageManager.TestFunction("MenuStrip_SaveAsName");
        }
        private void MenuStrip_Print(object sender, EventArgs e)
        {
            ViewImageManager.TestFunction("MenuStrip_Print");
        }
        private void MenuStrip_Exit(object sender, EventArgs e)
        {
            ViewImageManager.MainControls.MainForm.Close();
        }
        private void MenuStrip_DisplayFoward(object sender, EventArgs e)
        {
            ViewImageManager.BasicFunction.ViewNext();
        }
        private void MenuStrip_DisplayReverse(object sender, EventArgs e)
        {
            ViewImageManager.BasicFunction.ViewPrevious();
        }
        private void MenuStrip_MenuVisible(object sender, EventArgs e)
        {
            // MenuStrip.Visible を変更する、これに伴うフラグ等も変更する
            ViewImageManager.MainControls.MainFormFunction.ChangeChecked_MenuStrip_MenuVisible_Switch();
        }



        private void MenuStrip_SettingsOption(object sender, EventArgs e)
        {
            ViewImageManager.TestFunction("MenuStrip_SettingsOption");
        }
        private void MenuStrip_ShowHelp(object sender, EventArgs e)
        {
            ViewImageManager.TestFunction("MenuStrip_ShowHelp");
        }
        private void MenuStrip_ShowVersion(object sender, EventArgs e)
        {
            ViewImageManager.TestFunction("MenuStrip_ShowVersion");
        }
    }
}
