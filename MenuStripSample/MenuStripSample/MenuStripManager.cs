using CommonUtility.ControlUtility;
using System;
using System.Windows.Forms;

namespace MenuStripSample
{
    public class MenuStripConstants
    {
        public readonly string FILE_MENU = "ファイル(&F)";
        public readonly string FILE_OPEN = "開く(&)...";
        public readonly string FILE_APPEND = "上書き保存(&S)";
        public readonly string FILE_SAVEAS = "名前を付けて保存(&A)...";
        public readonly string FILE_EXIT = "終了(&X)";
    }
    public class MenuStripManager
    {
        protected ErrorManager.ErrorManager _err;
        protected Form _parentForm;
        protected MenuStrip _menuStrip;
        public MenuStripConstants Constants = new MenuStripConstants();
        public MenuStripManager(ErrorManager.ErrorManager err,Form form)
        {
            _err = err;
            _parentForm = form;
            Initialize();
        }
        /// <summary>
        /// MenuStrip および MenuStrip.Itmes から Text に合致した ToolStripMenuItem を取得する
        /// </summary>
        /// <param name="textArray"></param>
        /// <returns></returns>
        public ToolStripMenuItem GetToolStripMenuItemFromMenuStrip(string[] textArray)
        {
            try
            {
                MenuStripUtility menuUtil = new MenuStripUtility(_err);
                ToolStripMenuItem ret = menuUtil.GetToolStripMenuItemMatchTextFromMenuStripWithMultipleHierarchies(
                    _menuStrip, textArray);
                return ret;

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetToolStripMenuItemFromMenuStrip");
                return null;
            }
        }


        public void AddEventToMenu(string[] textArray,EventHandler menuClickEventHandler)
        {
            try
            {
                _err.AddLog(this, "AddEventToMenu");
                MenuStripUtility menuUtil = new MenuStripUtility(_err);
                ToolStripMenuItem item = menuUtil.GetToolStripMenuItemMatchTextFromMenuStripWithMultipleHierarchies(
                    _menuStrip, textArray);
                if(item != null)
                {
                    item.Click += menuClickEventHandler;
                    _err.AddLog("  AddEvent Click:"+ string.Join(", ", textArray));
                } else
                {
                    _err.AddLogAlert("  AddEvent Click Failed:" + string.Join(", ", textArray));
                }
            } catch(Exception ex)
            {
                _err.AddException(ex, this, "AddEventToMenu");
            }
        }

        public void Initialize()
        {
            try
            {
                _menuStrip = new MenuStrip();
                // ファイルメニューを作成する
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = this.Constants.FILE_MENU;
                // 開く メニューを追加する
                AddMenu(menuItem, this.Constants.FILE_OPEN, true, Keys.Control | Keys.O);
                // 上書き保存 メニューを追加する
                AddMenu(menuItem, this.Constants.FILE_APPEND, true, Keys.Control | Keys.S);
                // 名前を付けて保存 メニューを追加する
                AddMenu(menuItem, this.Constants.FILE_SAVEAS, true, Keys.Control | Keys.Shift | Keys.A);
                // セパレータを追加する
                menuItem.DropDownItems.Add(new ToolStripSeparator());
                // 終了 メニューを追加する
                AddMenu(menuItem, this.Constants.FILE_EXIT, true, Keys.Control | Keys.X);

                // MenuStrip に追加する
                _menuStrip.Items.Add(menuItem);
                // 親フォームに追加する
                _parentForm.Controls.Add(_menuStrip);


            } catch (Exception ex)
            {
                _err.AddException(ex, this, "Initialize");
            }
        }


        public void AddMenu(
            ToolStripMenuItem parentMenuItem,
            string text,bool showShortCutKeys, Keys shortcutKeys)
        {
            try
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = text;
                item.ShortcutKeys = shortcutKeys;
                item.ShowShortcutKeys = showShortCutKeys;
                // parentMenuItem に追加する
                parentMenuItem.DropDownItems.Add(item);
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "AddMenu");
            }
        }

    }
}
