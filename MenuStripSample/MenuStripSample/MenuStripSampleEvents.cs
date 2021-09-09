using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MenuStripSample
{
    public class MenuStripSampleEvents
    {
        ErrorManager.ErrorManager _err;
        MenuStripManager _menuManager;
        public MenuStripSampleEvents(ErrorManager.ErrorManager err,MenuStripManager menuStripManager)
        {
            _err = err;
            _menuManager = menuStripManager;
            Initialize();
        }
        public void Initialize()
        {
            try
            {
                ToolStripMenuItem item;
                string[] tempary;
                // 開く メニュー Click イベントを設定する
                tempary = new string[] { _menuManager.Constants.FILE_MENU, _menuManager.Constants.FILE_OPEN };
                item = _menuManager.GetToolStripMenuItemFromMenuStrip(tempary);
                if (item != null) { item.Click += FileOpen_Clicked; }
                // 上書き保存 メニュー Click イベントを設定する
                tempary = new string[] { _menuManager.Constants.FILE_MENU, _menuManager.Constants.FILE_APPEND };
                item = _menuManager.GetToolStripMenuItemFromMenuStrip(tempary);
                if (item != null) { item.Click += FileAppend_Clicked; }
                // 名前を付けて保存 メニュー Click イベントを設定する
                tempary = new string[] { _menuManager.Constants.FILE_MENU, _menuManager.Constants.FILE_SAVEAS };
                item = _menuManager.GetToolStripMenuItemFromMenuStrip(tempary);
                if (item != null) { item.Click += FileSaveAs_Clicked; }

                tempary = new string[] { _menuManager.Constants.FILE_MENU, _menuManager.Constants.FILE_EXIT };
                _menuManager.AddEventToMenu(tempary, FileExit_Clicked);

            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "Initialize");
            }
        }

        public void FileOpen_Clicked(object sender,EventArgs e)
        {
            try
            {
                _err.AddLog(this, "FileOpen_Clicked");
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "FileOpenClicked");
            }
        }
        public void FileAppend_Clicked(object sender, EventArgs e)
        {
            _err.AddLog(this, "FileAppend_Clicked");
        }
        public void FileSaveAs_Clicked(object sender, EventArgs e)
        {
            _err.AddLog(this, "FileSaveAs_Clicked");
        }
        public void FileExit_Clicked(object sender, EventArgs e)
        {
            _err.AddLog(this, "FileExit_Clicked");
        }
    }
}
