using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ErrorLog;
using ImageViewer.Function;

namespace ImageViewer.ParentForms
{
    public class MainFormManager
    {
        protected ErrorLog.IErrorLog _errorLog;
        readonly Form _mainForm;
        MenuStripRegister _menuRegister; // initialize で new する
        MenuStripEvents _menuEvents;  // initialize で new する
        protected ImageViewerSettings Settings;
        public MainFormState State;
        public ViewImageManager ViewImageManager;
        public Constants MenuConstants = new MainFormManager.Constants();

        readonly public Function.CommonFunctions Functions;
        public MainFormManager(ErrorLog.IErrorLog errorlog, Form form,ImageViewerSettings settings,ViewImageManager viewImageManager)
        {
            _errorLog = errorlog;
            _mainForm = form;
            Settings = settings;
            //Functions = functions;
            this.ViewImageManager = viewImageManager;
            State = new MainFormState();
        }

        public int Initialize()
        {
            try
            {
                int ret = 0;
                // MenuStrip
                ret = InitMenuStrip();
                if (ret < 1)
                { _errorLog.AddErrorNotException(this.ToString(),
                          "SetAppendMenuStripToForm failed"); }

                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "registMenuToMenuStripFromToolStripLiistForRegistList");
                return 0;
            }
        }

        private int InitMenuStrip()
        {
            try
            {
                // SetErrorLog
                //GlobalErrloLog.ErrorLog = new ErrorLog.ErrorLog();
                //_errorLog = new ErrorLog.ErrorLog();
                // MenuStrip を生成
                _menuRegister = new MenuStripRegister(new MenuStrip());
                _menuRegister.SetErrorLog(_errorLog);
                // リストの元を作る
                List<ToolStripLiistForRegist> listsRegist = MakeMenuNameList();

                // MenuStripに追加
                int ret = _menuRegister.RegistMenuToMenuStripFromToolStripLiistForRegistList(listsRegist);
                if (ret < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString(),
                        "registMenuToMenuStripFromToolStripLiistForRegistList failed");
                }

                // イベントハンドラを追加する
                _menuEvents = new MenuStripEvents(_errorLog,ViewImageManager,_menuRegister.GetMenuStrip())
                {
                    Constants = new MainFormManager.Constants(),
                    //Functions = Functions,
                    MainFormState = State
                };
                ret = _menuEvents.RegistFunctionForMenuStripEvents();
                if (ret < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString(),
                        "RegistFunctionForMenuStripEvents failed");
                }

                // Form に MenuStrip を追加
                //this.Controls.Add(_menustripRegister.getMenuStrip());
                //_mainForm.menustrip = _menustripRegister.getMenuStrip();
                ret = _menuRegister.SetAppendMenuStripToForm(_mainForm);
                if (ret < 1)
                { _errorLog.AddErrorNotException(this.ToString(),
                        "SetAppendMenuStripToForm failed"); }
                // Debug用
                //_menuRegister.IsExistsMenuStripInForm(_mainForm);

                // 設定値からVisibleを設定
                if (Settings.IsMenuBarVisibleOnStartUp)
                { 
                    _menuEvents.GetMenuStrip().Visible = true;
                    State.IsVisibleMenuStrip = true;
                }
                else { 
                    _menuEvents.GetMenuStrip().Visible = false;
                    State.IsVisibleMenuStrip = false;
                }

                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "initMenuStrip");
                return 0;
            }
        }

        private List<ToolStripLiistForRegist> MakeMenuNameList()
        {
            try
            {
                Constants constants = new Constants();
                Keys keys;
                // メニューリスト 2次元リスト
                List<ToolStripLiistForRegist> listsRegist = new List<ToolStripLiistForRegist>();
                // ファイルメニュー
                ToolStripLiistForRegist listRegist = new ToolStripLiistForRegist
                {

                    // ファイルメニュー
                    constants.MENU_FILE
                };
                keys = Keys.Control | Keys.O;
                listRegist.Add(constants.MENU_FILE_OPEN, keys);
                keys = Keys.Control | Keys.A;
                listRegist.Add(constants.MENU_FILE_SAVE_AS_NAME, keys);
                //listRegist.Add(constants.MENU_SEPALATOR);
                keys = Keys.Control | Keys.P;
                listRegist.Add(constants.MENU_FILE_PRINT, keys);
                keys = Keys.Control | Keys.X;
                listRegist.Add(constants.MENU_FILE_EXIT, keys);
                // Add_Clear
                listsRegist.Add(listRegist.GetListCopied());
                listRegist.Clear();

                // 表示メニュー
                listRegist.Add(constants.MENU_DISPLAY);
                keys = Keys.Right;
                listRegist.Add(constants.MENU_DISPLAY_FOWARD, keys);
                keys = Keys.Left;
                listRegist.Add(constants.MENU_DISPLAY_REVERSE, keys);
                keys = Keys.V;
                listRegist.Add(constants.MENU_DISPLAY_MENUBAR_VISIBLE, keys);
                // Add_Clear
                listsRegist.Add(listRegist.GetListCopied());
                listRegist.Clear();

                // 設定メニュー
                listRegist.Add(constants.MENU_SETTINGS);
                keys = Keys.Control | Keys.O;
                listRegist.Add(constants.MENU_SETTINGS_OPTION, keys);
                // Add_Clear
                listsRegist.Add(listRegist.GetListCopied());
                listRegist.Clear();

                // ヘルプメニュー
                listRegist.Add(constants.MENU_HELP);
                keys = Keys.Control | Keys.H;
                listRegist.Add(constants.MENU_HELP_HELP, keys);
                keys = Keys.Control | Keys.V;
                listRegist.Add(constants.MENU_HELP_VERSION, keys);
                listsRegist.Add(listRegist.GetListCopied());

                return listsRegist;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "MakeMenuNameList");
                return null;
            }
        }

        public MenuStripRegister GetMenuStripRegister() { return _menuRegister; }
        public class Constants
        {
            public readonly string MENU_FILE = "ファイル(&F)";
            public readonly string MENU_FILE_OPEN = "開く(&O)...";
            public readonly string MENU_FILE_SAVE_AS_NAME = "名前を付けて保存(&A)...";
            public readonly string MENU_SEPALATOR = "_MenuSepalator";
            public readonly string MENU_FILE_PRINT = "印刷(&P)";
            public readonly string MENU_FILE_EXIT = "終了(&X)";
            public readonly string MENU_DISPLAY = "表示(&D)";
            public readonly string MENU_DISPLAY_FOWARD = "次へ(&P)";
            public readonly string MENU_DISPLAY_REVERSE = "前へ(&R)";
            public readonly string MENU_DISPLAY_MENUBAR_VISIBLE = "メニューバーを常に表示する";
            public readonly string MENU_SETTINGS = "設定(&S)";
            public readonly string MENU_SETTINGS_OPTION = "オプション(&O)";
            public readonly string MENU_HELP = "ヘルプ(&H)";
            public readonly string MENU_HELP_HELP = "ヘルプ(&H)";
            public readonly string MENU_HELP_VERSION = "バージョン情報(&V)";
        }
    }
}
