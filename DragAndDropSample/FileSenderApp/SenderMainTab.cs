using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using CommonModule;

namespace FileSenderApp
{
    public class SenderMainTab
    {
        AppLogger _logger;
        public TabControl _tabControl;
        TabPage _rightTabPage;
        Control _renameControl;
        Control _checkBoxControl;
        Form _parentForm;
        string NEW_TAB_NAME = "[+]Add Tab";
        int _tmpHeight;
        string _ObjectName = "SenderMainTab1";
        public SenderMainTab(AppLogger logger, TabControl tabControl, Control renameControl,Control checkboxControl, Form parentForm)
        {
            _logger = logger;
            _tabControl = tabControl;
            //_rightTabPage = _tabControl.TabPages[1];
            _renameControl = renameControl;
            _checkBoxControl = checkboxControl;
            _parentForm = parentForm;
            _tabControl.KeyDown += TabControl_KeyDown;
        }

        public void Initialize()
        {
            _logger.PrintInfo(this.ToString() + " > Initialize") ;
            // タブが2つある状態、1つ目が"Tab1"、2つ目が"＋"
            _tabControl.SizeMode = TabSizeMode.Fixed;
            _tabControl.Selecting += TabControl_Selecting;
            //
            //#
            // Menu
            ContextMenuStrip menu = new ContextMenuStrip();
            //
            ToolStripMenuItem item_AddButton = new ToolStripMenuItem("AddButton");
            menu.Items.Add(item_AddButton);
            item_AddButton.Click += AddButton_Click;
            //
            ToolStripMenuItem item_Delete = new ToolStripMenuItem("DeleteTab");
            menu.Items.Add(item_Delete);
            item_Delete.Click += DeleteTab_Click;
            //
            _tabControl.ContextMenuStrip = menu;
            //#
            _tmpHeight = _renameControl.Height;
            UnvisibleRenameControl();
            _logger.PrintInfo(string.Format("tabPages[0].ClientSize = {0}", _tabControl.TabPages[0].ClientSize));

            //#
            // デザイナで配置済みのControl位置調整
            ButtonsGroup buttonGroup = GetButtonGroupInTabPage(0);
            if (buttonGroup != null)
            {
                buttonGroup.Location = new Point(0, 0);
                buttonGroup.Size = _tabControl.TabPages[0].ClientSize;
            }
            InitializeButton();
            InitializeButtonGroup();
        }


        public void InitializeButtonGroup()
        {
            _logger.PrintInfo(this.ToString() + " > InitializeButtonGroup");
            foreach (TabPage page in _tabControl.TabPages)
            {
                List<Control> buttonGroupList = CommonGeneral.GetControlListIsMatchType(page, typeof(ButtonsGroup));
                if (0 < buttonGroupList.Count)
                {
                    ButtonsGroup buttonGroup = (ButtonsGroup)buttonGroupList[0];
                    InitializeButtonGroupSingle(buttonGroup);
                    FormFileSenderApp formMain = (FormFileSenderApp)_parentForm;
                    buttonGroup.SendButtonClickEvent -= formMain.AnyButtonClickedRecieveEvent;
                    buttonGroup.SendButtonClickEvent += formMain.AnyButtonClickedRecieveEvent;
                }
                else
                {
                    // ButtonGroupがない
                }
            }
        }

        /// <summary>
        /// ButtonGroup=ボタンの親Panel、の初期化
        /// 、ボタン名変更用フォームの非表示
        /// </summary>
        /// <param name="buttonGroup"></param>
        private void InitializeButtonGroupSingle(ButtonsGroup buttonGroup)
        {
            buttonGroup.Initialize(_logger);
            FormFileSenderApp formMain = (FormFileSenderApp)this._parentForm;
            buttonGroup._fileSenderSettingValues = formMain._fileSenderSettingValues;
            Panel panelButtons = buttonGroup.GetPanelButtons();
            buttonGroup.UnvisibleRenamePanel();
        }


        public void InitializeButton()
        {
            _logger.PrintInfo(this.ToString() + " > InitializeButton");
            //int count = 0;
            //int i = 0;
            foreach(TabPage page in _tabControl.TabPages)
            {
                List<Control> buttonGroupList = CommonGeneral.GetControlListIsMatchType(page, typeof(ButtonsGroup));
                if (0 < buttonGroupList.Count)
                {
                    ButtonsGroup buttonGroup = (ButtonsGroup)buttonGroupList[0];
                    InitializeButtonInButtonGroupSingle(buttonGroup);
                }
                else
                {
                    // buttonGroupがない
                }
            }
        }

        public ButtonsGroup GetButtonsGroupMatchName(TabPage page)
        {
            List<Control> conList = CommonModule.CommonGeneral.GetControlListIsMatchType(page, typeof(ButtonsGroup));
            return (ButtonsGroup)conList[0];
        }

        public TabPage GetTabPageMatchName(string tabPageName)
        {
            TabPage ret = null;
            foreach(TabPage page in _tabControl.TabPages)
            {
                if(page.Name == tabPageName)
                {
                    return page;
                }
            }
            return ret;
        }


        /// <summary>
        /// ボタンの初期化
        /// 、ボタンの親Panelの持つボタンをすべて消去して、ボタンを1つ作成する
        /// <para></para>
        /// buttonGroup.Initialize も行う（注意）
        /// </summary>
        /// <param name="buttonGroup"></param>
        private ButtonsGroup InitializeButtonInButtonGroupSingle(ButtonsGroup buttonGroup)
        {
            buttonGroup.Initialize(_logger);
            //既存のボタンをクリア
            buttonGroup.GetPanelButtons().Controls.Clear();
            //#
            Panel panelButtons = buttonGroup.GetPanelButtons();
            for (int j = 0; j < 1; j++)
            {
                SendButton newButton = new SendButton();
                panelButtons.Controls.Add(newButton);
                newButton = buttonGroup.InitializeButton(
                    newButton, _logger, "SendButton", 1, "SendButton1", buttonGroup.BUTTON_MARGIN, buttonGroup.BUTTON_SIZE, 1);
                buttonGroup.AttachButtonClickHandler(newButton);
            }
            return buttonGroup;
        }


        /// <summary>
        /// TabPageのもつ ButtonsGroup（UserControl）を取得する（Index指定）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ButtonsGroup GetButtonGroupInTabPage(int pageIndex)
        {
            return GetButtonGroupInTabPage(_tabControl.TabPages[pageIndex]);
        }

        /// <summary>
        /// TabPageのもつ ButtonsGroup（UserControl）を取得する
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
            public ButtonsGroup GetButtonGroupInTabPage(TabPage page)
        {
            List<Control> buttonGroupList = CommonGeneral.GetControlListIsMatchType(page, typeof(ButtonsGroup));
            if (0 < buttonGroupList.Count)
            {
                if (1 < buttonGroupList.Count){
                    _logger.PrintInfo("1 < buttonGroupList.Count");
                    _logger.PrintInfo(string.Format("page.Name = {0}", page.Name));
                }
                ButtonsGroup buttonGroup = (ButtonsGroup)buttonGroupList[0];
                return buttonGroup;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// タブ名を変更する
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public int RenameTab(string newName)
        {
            try
            {
                foreach (TabPage page in _tabControl.TabPages)
                {
                    if (page.Text == newName)
                    {
                        string msg = String.Format("Exists Already [{0}]", newName);
                        _logger.PrintInfo(msg);
                        MessageBox.Show(msg, "Exists Already", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return -1;
                    }
                }
                string oldName = _tabControl.TabPages[_tabControl.SelectedIndex].Text;
                _tabControl.TabPages[_tabControl.SelectedIndex].Text = newName;
                _logger.PrintInfo(String.Format("Rename Tab [{0}] => [{1}]", oldName, newName));
                return 1;
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "RenameTab");
            }
            return 0;
        }

        /// <summary>
        /// 上部の入力部を非表示に
        /// </summary>
        public void UnvisibleRenameControl()
        {
            _renameControl.Visible = false;
            _checkBoxControl.Location = new Point(0, 0);
            _tabControl.Location = new Point(0, _checkBoxControl.Height);
            _tabControl.Size = _parentForm.ClientSize;
            //サイズ変更したときに、イベントをとらえなくなる？
            // タブを切り替えないと、TabControl_KeyDownが実行されない
            //_tabControl.KeyDown -= TabControl_KeyDown;
            //_tabControl.KeyDown += TabControl_KeyDown;
        }

        private void TabControl_KeyDown(object sender, KeyEventArgs e)
        {
            _logger.PrintInfo("TabControl_KeyDown");
            if (e.KeyCode == Keys.F2)
            {
                _logger.PrintInfo("TabControl_KeyDown  F2");
                if (_renameControl.Visible)
                {
                    UnvisibleRenameControl();
                }
                else
                {
                    _checkBoxControl.Location = new Point(0, _tmpHeight);
                    _tabControl.Location = new Point(0, _tmpHeight + _checkBoxControl.Height);
                    Size newSize = _parentForm.ClientSize;
                    newSize.Height -= (_tmpHeight + _checkBoxControl.Height);
                    _tabControl.Size = newSize;
                    _renameControl.Visible = true;
                    foreach (Control con in _renameControl.Controls)
                    {
                        if (con.GetType().ToString().IndexOf("TextBox") > 0)
                        {
                            TextBox buf = (TextBox)con;
                            buf.Focus();
                        }
                    }
                }
                //_renameControl.Focus();
            }
            else
            {
                _logger.PrintInfo("TabControl_KeyDown");
            }
        }

        /// <summary>
        /// 削除メニュークリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTab_Click(object sender, EventArgs e)
        {
            int nowIndex = _tabControl.SelectedIndex;
            if (_tabControl.TabPages[nowIndex].Text== NEW_TAB_NAME)
            {
                return;
            }
            else
            {
                string nowTab = _tabControl.TabPages[nowIndex].Text;
                DialogResult ret = MessageBox.Show(String.Format("Delete? [{0}]", nowTab), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ret == DialogResult.Yes)
                {
                    //_tabControl.TabPages.RemoveAt(nowIndex);
                    //_tabControl.TabPages.RemoveByKey(nowTab);
                    _tabControl.TabPages.Remove(_tabControl.TabPages[nowIndex]);
                    _logger.PrintInfo(string.Format("Deleted Tab [{0}]({1})", nowTab, nowIndex));
                }
            }
        }

        /// <summary>
        /// メニュー　ボタン追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("SenderMainTab > AddButton_Click");
            ButtonsGroup buttonsGroup = GetButtonGroupInTabPage(_tabControl.SelectedIndex);
            if (buttonsGroup == null)
            {
                buttonsGroup = AddButtonGroupInTab(_tabControl.SelectedIndex);
            }
            buttonsGroup.AddButton();
        }

        private ButtonsGroup AddButtonGroupInTab(int tabIndex)
        {
            TabPage page = _tabControl.TabPages[tabIndex];
            return AddButtonGroupInTab(page);
        }

        private ButtonsGroup AddButtonGroupInTab(TabPage page)
        {
            _logger.PrintInfo("SenderMainTab > AddButtonGroupInTab");
            ButtonsGroup buttonsGroup = new ButtonsGroup();
            page.Controls.Add(buttonsGroup);
            //
            buttonsGroup.Location = new Point(0, 0);
            buttonsGroup.Size = page.ClientSize;
            buttonsGroup.Initialize(_logger);
            FormFileSenderApp formMain = (FormFileSenderApp)this._parentForm;
            buttonsGroup._fileSenderSettingValues = formMain._fileSenderSettingValues;
            InitializeButtonInButtonGroupSingle(buttonsGroup);
            InitializeButtonGroupSingle(buttonsGroup);
            buttonsGroup.SendButtonClickEvent -= formMain.AnyButtonClickedRecieveEvent;
            buttonsGroup.SendButtonClickEvent += formMain.AnyButtonClickedRecieveEvent;
            return buttonsGroup;
        }


        /// <summary>
        /// タブ選択 （AddTabクリックは、タブを追加）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_Selecting(object sender, EventArgs e)
        {
            _logger.PrintInfo("SenderMainTab > TabControl_Selecting");
            int nowIndex = _tabControl.SelectedIndex;
            int tabMax = _tabControl.TabPages.Count - 1;
            if (nowIndex == tabMax)
            {
                AddTabPage();
            }
            else
            {
                // そのまま選択する
                //
                // 中身のコントロールがない場合は、作成する
                ButtonsGroup buttonsGroup = GetButtonGroupInTabPage(_tabControl.SelectedIndex);
                if (buttonsGroup == null)
                {
                    buttonsGroup = AddButtonGroupInTab(_tabControl.SelectedIndex);
                }
            }
        }

        /// <summary>
        /// タブを追加する
        /// </summary>
        public TabPage AddTabPage()
        {
            _logger.PrintInfo("AddTabPage");
            TabPage newPage = new TabPage();
            newPage.Text = GetNewTabName("NewTab", 1);
            //newPage.BorderStyle = BorderStyle.FixedSingle;//テスト用
            _tabControl.TabPages.Add(newPage);
            newPage.Padding = new System.Windows.Forms.Padding(3);
            newPage.Size = _tabControl.ClientSize;
            _logger.PrintInfo(string.Format("Added TabPage [{0}]", newPage.Text));
            int selectedIndex = OrderMoveAddTabToLast();
            _tabControl.TabPages[selectedIndex].Select();
            _tabControl.TabPages[selectedIndex].Focus();
            // 上記でSelectingイベントが発生しないので、以下で設定する
            //ButtonsGroup buttonsGroup = GetButtonGroupInTabPage(selectedIndex);
            ButtonsGroup buttonsGroup = GetButtonGroupInTabPage(newPage);
            if (buttonsGroup == null)
            {
                //buttonsGroup = AddButtonGroupInTab(_tabControl.SelectedIndex);
                buttonsGroup = AddButtonGroupInTab(newPage);
            }
            //_logger.PrintInfo(string.Format("newPage.size = {0}", newPage.Size));
            //_logger.PrintInfo(string.Format("_tabControl.ClientSize = {0}", _tabControl.ClientSize));
            return newPage;
        }

        /// <summary>
        /// AddTabを最後に持っていく（タブを追加した後に実行する）
        /// </summary>
        private int OrderMoveAddTabToLast()
        {
            int retIndex = _tabControl.SelectedIndex;
            string title = NEW_TAB_NAME;
            for (int i = 0; i<_tabControl.TabPages.Count; i++)
            {
                if (_tabControl.TabPages[i].Text == title)
                {
                    TabPage buf = _tabControl.TabPages[_tabControl.TabPages.Count - 1];
                    _tabControl.TabPages[_tabControl.TabPages.Count - 1] = _tabControl.TabPages[i];
                    _tabControl.TabPages[i] = buf;
                    break;
                }
            }
            return retIndex;
        }

        /// <summary>
        /// 新しいタブ名を取得（既存のタブ名と重複していないか）
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetNewTabName(string baseName, int index=1)
        {
            foreach(TabPage page in _tabControl.TabPages)
            {
                if ( page.Text == baseName + index)
                {
                    return GetNewTabName(baseName, index+1);
                }
            }
            return baseName + index;
        }

        /// <summary>
        /// Handles TabControl1.DrawItem
        /// 、 タブのタイトル部の描画を行う（フォントや表示、背景色のｶｽﾀﾑを行う）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="System"></param>
        /// <param name=""></param>
        /// <param name="e"></param>
        /// <param name="System"></param>
        /// <param name=""></param>
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                int fontSize = 10;
                SolidBrush backBrush;
                SolidBrush foreBrush;
                StringFormat format = new StringFormat();
                RectangleF rect;
                Font font;
                int tabMax = _tabControl.TabPages.Count-1;
                if (e.Index == tabMax)
                {
                    backBrush = new SolidBrush(SystemColors.Control);
                    foreBrush = new SolidBrush(Color.Black);
                    font = new Font("Arial Black", fontSize, FontStyle.Regular);
                    //rect = new RectangleF(e.Bounds.X, e.Bounds.Y + 0, e.Bounds.Width, e.Bounds.Height);
                    rect = new RectangleF(e.Bounds.X, e.Bounds.Y + 0, e.Bounds.Width, e.Bounds.Height);
                    Console.WriteLine(String.Format("rect = {0}", rect));
                    format.Alignment = StringAlignment.Center;
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                    e.Graphics.DrawString(_tabControl.TabPages[e.Index].Text, font, foreBrush, rect, format);
                }
                else
                {
                    //return;
                }

                //'背景や文字色、フォントは自由に設定してください
                if (_tabControl.SelectedIndex == e.Index){
                    backBrush = new SolidBrush(Color.Navy);
                    foreBrush = new SolidBrush(Color.White);
                    font = new Font("Arial Black", fontSize, FontStyle.Bold);
                }
                else
                {
                    backBrush = new SolidBrush(SystemColors.Control);
                    foreBrush = new SolidBrush(Color.Black);
                    font = new Font("Arial Black", fontSize, FontStyle.Regular);
                }

                rect = new RectangleF(e.Bounds.X, e.Bounds.Y + 0, e.Bounds.Width, e.Bounds.Height);
                format.Alignment = StringAlignment.Center;

                e.Graphics.FillRectangle(backBrush, e.Bounds);
                e.Graphics.DrawString(_tabControl.TabPages[e.Index].Text, font, foreBrush, rect, format);
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "TabControl_DrawItem");
            }
        }

        /////////////////
        public Dictionary<string, object> GetValueAll()
        {
            Dictionary<string, object> retDict = new Dictionary<string, object> { };
            retDict.Add("TabAmount", _tabControl.TabPages.Count);
            int count = 0;
            foreach(TabPage page in _tabControl.TabPages)
            {
                //string keyName = _ObjectName;
                string keyName = "TabPage" + count;
                ButtonsGroup buttonsGroup = GetButtonGroupInTabPage(count);
                if (buttonsGroup == null)
                {
                    count++;
                    continue;
                }
                Dictionary<string, object> pageInfo = buttonsGroup.GetButtonsInfoAll();
                count++;
                if (retDict.ContainsKey(keyName))
                {
                    _logger.PrintInfo("[!]");
                    _logger.PrintInfo("Duplication : " + keyName);
                }
                else
                {
                    pageInfo.Add(ConstFileSender.KEY_TAB_PAGE_TEXT, page.Text);
                    retDict.Add(keyName, pageInfo);
                }
            }
            return retDict;
        }

        public void PrintInfoDictAll(Dictionary<string, object> infoDict, string num_base="")
        {
            //var dictStr = String.Join(",", infoDict.Select(kvp => kvp.Key + " : " + kvp.Value));
            //_logger.PrintInfo(string.Format("dictStr = {0}", dictStr));
            List<string> keys = infoDict.Keys.ToList();
            for(int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                if (infoDict[key].GetType().ToString().IndexOf("Dictionary")>0)
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)infoDict[key];
                    //var dictStr = String.Join(",", dict.Select(kvp => kvp.Key + " : " + kvp.Value));
                    //_logger.PrintInfo(string.Format("i={0}, {1} : {2}", i + num_base, key, dictStr));
                    _logger.PrintInfo(string.Format("i={0}-{1}, {2} : {3}", num_base, i, key, "Dictionary"));
                    if (num_base != "") { num_base += "-"; }
                    num_base += i.ToString();
                    PrintInfoDictAll(dict, num_base);
                }
                else
                {
                    _logger.PrintInfo(string.Format("i={0}-{1}, {2} : {3}",num_base,i, key, infoDict[key]));
                }
            }
        }
    }
}
