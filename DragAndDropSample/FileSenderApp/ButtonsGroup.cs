using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using CommonModule;
using CommonUtility.FileListUtility;
using DragAndDropSample;
using JsonStreamModule;

namespace FileSenderApp
{
    public partial class ButtonsGroup : UserControl
    {
        AppLogger _logger;
        public Size BUTTON_SIZE = new Size(150, 36);
        public Point BUTTON_MARGIN = new Point(15, 8);
        public int ButtonAmount = 1;
        int _tmpHeight; //上部テキストボックス表示/非表示を切り替えるときの、その下のコントロールの高さ調整用。
        int _rowMax = -1;
        int _colMax = 2;
        int _orderPriority_RowCol = ConstFileSender.TO_RIGHT; //ボタンを配置するときに、右方向を優先するか、下方向を優先するか。0=右,1=下
        public FileSenderSettingValues _fileSenderSettingValues;
        //ボタンが押されたときのイベント（すべてのボタン）
        // 第1引数は SendButtonオブジェクト
        // FormFileSenderApp.AnyButtonClickedRecieveEvent に紐づける
        public EventHandler SendButtonClickEvent;

        public ButtonsGroup()
        {
            InitializeComponent();
        }

        public void Initialize(AppLogger logger)
        {
            _logger = logger;
        }

        private void ButtonsGroup_Load(object sender, EventArgs e)
        {

        }

        private void Button_Click(object sender ,EventArgs e)
        {
            // このイベントはボタン生成時 AttachButtonClickHandler で SendButon.Clickと このメソッドが紐づけられる
            SendButton clickedButton = sender as SendButton;
            if (clickedButton != null)
            {
                string buttonName = clickedButton.Name;
                _logger.PrintInfo($"Button Name: {buttonName}");
                if (SendButtonClickEvent == null) { _logger.PrintInfo("SendButtonClickEvent==null"); }
                SendButtonClickEvent?.Invoke(clickedButton, e);
            }
        }

        public int GetButtonCount()
        {
            //List<Control> conList = CommonModule.CommonGeneral.GetControlListIsMatchType(this, typeof(SendButton));
            //return conList.Count;
            return GetButtonAmount();
        }

        public void DeleteAllButton()
        {
            _logger.PrintInfo("ButtonsGroup > DeleteAllButton");
            List<Control> conList = CommonModule.CommonGeneral.GetControlListIsMatchType(panelButtons, typeof(SendButton));
            //List<Control> conList = CommonModule.CommonGeneral.GetControlListIsMatchType(this, typeof(SendButton));
            List<SendButton> btnList = this.ConvertControlList(conList);
            foreach(SendButton btn in btnList)
            {
                btn.Enabled = false;
                this.panelButtons.Controls.Remove(btn);
                btn.Visible = false;
                btn.Dispose();
                //btn = null;
            }
        }

        public SendButton AddButton()
        {
            _logger.PrintInfo("ButtonsGroup > AddButton");
            //現在のボタン数を取得
            int buttonAmount = GetButtonAmount();
            //縦の行、横の行の設定取得
            int rowMax = _rowMax;
            int colMax = _colMax;
            if (rowMax < 0) { rowMax = buttonAmount; }
            if (colMax < 0) { colMax = buttonAmount; }
            //カウント処理（新しいボタンの位置取得）
            //追加する位置を取得
            Point newPoint = GetPointNewButton(buttonAmount, rowMax, colMax);
            _logger.PrintInfo(string.Format("newPoint = {0}", newPoint));
            //追加するLocationを計算
            Point newLoc = CalcNewButtonLocation(newPoint);
            _logger.PrintInfo(string.Format("newLoc = {0}", newLoc));
            //ボタン追加

            SendButton newButton = new SendButton();
            panelButtons.Controls.Add(newButton);
            int nextIndex = buttonAmount + 1;
            string buttonName = "SendButton" + nextIndex;
            newButton = this.InitializeButton(
                    newButton, _logger, "SendButton", nextIndex, buttonName, newLoc, this.BUTTON_SIZE, nextIndex);
            this.AttachButtonClickHandler(newButton);
            return newButton;
        }


        /// <summary>
        /// 設定を反映させる（初期化時に実行する）(すべてのボタンを設定値から初期化する）
        /// 　、ボタンは1つもない想定
        /// </summary>
        /// <param name="settingDict"></param>
        public void ApplySettingButton(Dictionary<string, object> settingDict)
        {
            _logger.PrintInfo("ButtonsGroup > ApplySettingButton");

            //List<Dictionary<string, object>> settingList = _fileSenderSettingValues.GetListMatchValues(settingDict, "SendButton");
            //foreach (Dictionary<string, object> bufDict in settingList)
            foreach (string key in settingDict.Keys)
            {
                //Dictionary<string, object> bufDict = (Dictionary<string, object>)settingDict[key];
                //string buttonName = bufDict.Keys.ToArray()[0];
                string buttonName = key;
                if (!key.StartsWith("SendButton")) { continue; }
                //Dictionary<string, object> buttonSettingDict = (Dictionary<string, object>)settingDict[key];
                // 型 'Newtonsoft.Json.Linq.JObject' のオブジェクトを型 'System.Collections.Generic.Dictionary`2[System.String,System.Object]' にキャストできません。
                Dictionary<string, object> buttonSettingDict = JsonStream.ConvertJObjectToDict((object)settingDict[key]);

                _logger.PrintInfo("ButtonsGroup > ApplySettingButton > settingDict  > " + buttonName);
                CommonGeneral.PrintDict(buttonSettingDict);
                SendButton sendButton = this.AddButton();
                sendButton = this.ApplySettingButtonSingle(sendButton, buttonName, buttonSettingDict);
                _logger.PrintInfo(string.Format("ButtonsGroup > ApplySettingButton > [{0} : {1}]", buttonName, sendButton.Text));
            }
            /*
             * ボタン設定は以下のようなDictを受け取る
    {
    "SendButton1": {
      "ButtonName": "SendButton1",
      "ButtonText": "SendButton1",
      "DirectoryPath": "",
      "ButtonColorText": "Color [Control]",
      "ButtonColor": "Control"
    },
    "SendButton2": {
      ～～～～～
    },
    "TabPageText": "NewTab1"
    }
             * 
             */
        }

        /// <summary>
        /// 設定を反映させる（初期化時に実行する）
        /// </summary>
        /// <param name="settingDict"></param>
        public SendButton ApplySettingButtonSingle(SendButton sendButton, string ButtonName, Dictionary<string, object> settingDict)
        {
            /*
             * 以下のような辞書を扱う
    {
      "ButtonName": "SendButton1",
      "ButtonText": "SendButton1",
      "DirectoryPath": "",
      "ButtonColorText": "Color [Control]",
      "ButtonColor": "Control"
    },
             * 
             */
            sendButton.Name = ButtonName;
            sendButton.Text = (string)settingDict[ConstFileSender.KEY_BUTTON_TEXT];
            sendButton._directoryPath = (string)settingDict[ConstFileSender.KEY_DIRECTORY_PATH];
            string colorName = (string)settingDict[ConstFileSender.KEY_BUTTON_COLOR];
            Color bufColor = GetColorFromName(colorName);
            sendButton.BackColor = bufColor;
            return sendButton;
        }

        /// <summary>
        /// 指定された色名からColorオブジェクトを取得します。
        /// </summary>
        /// <param name="colorName">色名（例："Control", "Red", "Blue"など）</param>
        /// <returns>指定された名前に対応するColorオブジェクト</returns>
        public static Color GetColorFromName(string colorName)
        {
            // 色名からColorオブジェクトを取得
            Color color = Color.FromName(colorName);

            // 未知の色名が指定された場合、Color.Emptyを返す
            if (color.IsKnownColor || color.IsSystemColor || color.IsNamedColor)
            {
                return color;
            }
            else
            {
                throw new ArgumentException($"指定された色名 '{colorName}' は無効です。");
            }
        }

        /// <summary>
        /// 新しいボタンのLocationを計算する
        ///  , GetPointNewButtonで得られた値をもとに計算する
        /// </summary>
        /// <param name="newPoint"></param>
        /// <returns></returns>
        private Point CalcNewButtonLocation(Point newPoint)
        {
            int otherButtonSpaceX;
            int buttonMarginX;
            int otherButtonSpaceY;
            int buttonMarginY;
            Point retPoint = new Point(0,0);
            //一番左は、ボタンの幅が必要ない
            if (newPoint.X == 1)
            {
                otherButtonSpaceX = (newPoint.X - 1) * this.BUTTON_SIZE.Width;
                buttonMarginX = newPoint.X * this.BUTTON_MARGIN.X;
            }
            else
            {
                //2列目以降はボタンの幅を個数分追加する
                otherButtonSpaceX = (newPoint.X - 1) * this.BUTTON_SIZE.Width;
                buttonMarginX = newPoint.X * this.BUTTON_MARGIN.X;
            }
            retPoint.X = otherButtonSpaceX + buttonMarginX;
            //#
            otherButtonSpaceY = (newPoint.Y - 1) * this.BUTTON_SIZE.Height;
            buttonMarginY = newPoint.Y * this.BUTTON_MARGIN.Y;
            retPoint.Y = otherButtonSpaceY + buttonMarginY;
            return retPoint;
        }


        /// <summary>
        /// ボタン追加時用の、新しいボタンの位置取得
        /// </summary>
        /// <param name="buttonAmount"></param>
        /// <returns></returns>
        private Point GetPointNewButton(int buttonAmount, int rowMax, int colMax)
        {
            int count = 0;
            bool isCountOver = false;
            int retRow=0, retCol=0;
            if (_orderPriority_RowCol == 0)
            {
                //横優先
                for (int row = 1; row <= rowMax; row++)
                {
                    for (int col = 1; col <= colMax; col++)
                    {
                        count++;
                        if (buttonAmount < count)
                        {
                            isCountOver = true;
                            retRow = row;
                            retCol = col;
                            break;
                        }
                    }
                    if (isCountOver) { break; }
                }
            }
            else
            {
                //縦優先
                for (int col = 1; col <= colMax; col++)
                {
                    for (int row = 1; row <= rowMax; row++)
                    {
                        count++;
                        if (buttonAmount < count)
                        {
                            isCountOver = true;
                            retRow = row;
                            retCol = col;
                            break;
                        }
                    }
                    if (isCountOver) { break; }
                }
            }
            if ((retCol == 0) && (retRow == 0))
            {
                retCol = 1; retRow = 1;
            }
            return new Point(retCol, retRow);
        }

        public int GetButtonAmount()
        {
            List<Control> list = CommonModule.CommonGeneral.GetControlListIsMatchType(panelButtons, typeof(SendButton));
            return list.Count;
        }


        public void AttachButtonClickHandler(SendButton button)
        {
            // 既にハンドラーが登録されていれば削除
            button.Click -= Button_Click;
            // ハンドラーを追加
            button.Click += Button_Click;
        }

        public void UnvisibleRenamePanel()
        {
            _tmpHeight = panelRename.Size.Height;
            panelRename.Visible = false;
            panelButtons.Location = new Point(0, 0);
            panelButtons.Size = this.ClientSize;
        }

        /// <summary>
        /// List Controlを List SendButton に変換する
        /// </summary>
        /// <param name="controlList"></param>
        /// <returns></returns>
        public List<SendButton> ConvertControlList(List<Control> controlList)
        {
            List<SendButton> retList = new List<SendButton> { };
            foreach (Control con in controlList)
            {
                //retList.Add(Convert.ChangeType(con, typeof(SendButton));
                retList.Add((SendButton)con);
            }
            return retList;
        }

        public Panel GetPanelButtons()
        {
            return this.panelButtons;
        }

        

        public SendButton InitializeButton(
            SendButton newButton,
            AppLogger logger, string baseName, int index, string text, Point location, Size size, int tabIndex)
        {
            //SendButton retSendButton = new SendButton();
            SendButton retSendButton = newButton;
            //retSendButton.Location = new System.Drawing.Point(15, 8);
            retSendButton.Location = location;
            retSendButton.Name = baseName + index;
            //retSendButton.Size = new System.Drawing.Size(150, 36);
            retSendButton.Size = size;
            retSendButton.TabIndex = tabIndex;
            retSendButton.Text = text;
            retSendButton.UseVisualStyleBackColor = true;
            retSendButton.InitializeB(logger, index);
            PrintSendButtonInfo(retSendButton);
            return retSendButton;
        }

        public void PrintSendButtonInfo(SendButton button)
        {
            string buf = "";
            buf += string.Format("Name={0}", button.Name);
            buf += string.Format(", Text={0}", button.Text);
            buf += string.Format(", Size={0}", button.Size);
            buf += string.Format(", Loc={0}", button.Location);
            buf += string.Format(", TabIndex={0}", button.TabIndex);
            buf += string.Format(", BColor={0}", button.BackColor.ToString());
            _logger.PrintInfo(buf);
        }

        /// <summary>
        /// Panel ButtonsGroup が持つすべての SendButton の設定値を取得する
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetButtonsInfoAll()
        {
            _logger.PrintInfo("ButtonsGroup > GetButtonsInfoAll");

            List<Control> controlList = CommonGeneral.GetControlListIsMatchType(panelButtons, typeof(SendButton));
            List<SendButton> sendButtonList = ConvertControlList(controlList);
            //
            Dictionary<string, object> retDict = new Dictionary<string, object>{ };
            int count = 0;
            foreach(SendButton sendButton in sendButtonList)
            {
                string key = sendButton.Name;
                Dictionary<string, object> infoDict = sendButton.GetSettingDict();
                count++;
                retDict.Add(key, infoDict);
            }
            return retDict;
        }

    }

    // ################################################################################
    // ################################################################################
    // ################################################################################
    /// <summary>
    /// 送信ボタン　オブジェクト
    /// </summary>
    public class SendButton : Button
    {
        AppLogger _logger;
        int _index = 0;
        public string _directoryPath = "";
        public Color _buttonColor = SystemColors.Control;
        public DragAndDropOnControl _dragAndDropOnControl;
        public SendButton()
        {

        }

        public void InitializeB(AppLogger logger, int index)
        {
            logger.PrintInfo("SendButton > Initialize");
            _logger = logger;
            _index = index;
            //#

            _dragAndDropOnControl = new DragAndDropOnControl(_logger, this); //RecieveするControl
            //_dragAndDropOnControlB.AddRecieveControls(new Control[] { button1 }); //リストでなくてもよい
            _dragAndDropOnControl.AddRecieveControl(this);
            DragAndDropForFile _dragAndDropForFileB = new DragAndDropForFile(_logger, _dragAndDropOnControl);
            _dragAndDropForFileB.DragAndDropEventAfterEventForFile += DragAndDropFileEvent;
            _dragAndDropOnControl._dragAndDropForFile = _dragAndDropForFileB;
            //#
            // Menu
            ContextMenuStrip menu = new ContextMenuStrip();
            //#
            ToolStripMenuItem item_Setting = new ToolStripMenuItem("Setting");
            item_Setting.Click += Menu_Setting_Click;
            menu.Items.Add(item_Setting);
            //#
            ToolStripMenuItem item_OpenFolder = new ToolStripMenuItem("OpenFolder");
            item_OpenFolder.Click += Menu_OpenFolder_Click;
            menu.Items.Add(item_OpenFolder);
            //#
            //ToolStripMenuItem item_SetDirPath = new ToolStripMenuItem("SetDirPath");
            //item_SetDirPath.Click += Menu_SetDirPath_Click;
            //menu.Items.Add(item_SetDirPath);
            ////#
            //ToolStripMenuItem item_Rename = new ToolStripMenuItem("Rename");
            //item_Rename.Click += Menu_Rename_Click;
            //menu.Items.Add(item_Rename);
            ////#
            //ToolStripMenuItem item_ChangeColor = new ToolStripMenuItem("ChangeColor");
            //item_ChangeColor.Click += Menu_ChangeColor_Click;
            //menu.Items.Add(item_ChangeColor);
            //#
            this.ContextMenuStrip = menu;
        }

        public void DragAndDropFileEvent(object sender, EventArgs e)
        {
            try
            {
                DragAndDropForFile dragAndDropForFile = _dragAndDropOnControl._dragAndDropForFile;
                _logger.AddLog(this, this.Name + " > DragAndDropFileEvent");
                if (dragAndDropForFile.Files == null) { _logger.PrintInfo("Files == null"); return; }
                if (dragAndDropForFile.Files.Length < 1) { _logger.PrintInfo("Files.Length < 1"); return; }
                string targetPath = dragAndDropForFile.Files[0];
                //ショートカットならそのフォルダを取得
                FileListRegister fileListRegister = new FileListRegister();
                string targetPathB = fileListRegister.GetFilePathFromShortcut(targetPath);
                if (!Directory.Exists(targetPathB)){ _logger.PrintInfo("Not Directory");  return; }
                _logger.AddLog("  GetPath=" + targetPathB);
                //ファイル名をボタンTextに出力
                string buf = Path.GetFileName(targetPathB);
                _directoryPath = targetPathB;
                this.Text = buf;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "DragAndDropFileEvent");
                _logger.ClearError();
            }
        }

        public void Menu_Setting_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("Menu_Setting_Click");
            FormFileSenderButtonSetting _formSetting = new FormFileSenderButtonSetting();
            List<SendButton> list = new List<SendButton> { this };
            _formSetting.ClickButtonApply += RecieveInfoFromSettingForm;
            _formSetting.ClearSettingParts();
            _formSetting.ShowDialogForList(_logger, list);
            _formSetting.Show();
        }
        public void Menu_OpenFolder_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("Menu_OpenFolder_Click");
            CommonModules.CommonGeneral.OpenFolder(this._directoryPath);
        }

        public void RecieveInfoFromSettingForm(object sender, EventArgs e)
        {
            _logger.PrintInfo("RecieveInfoFromSettingForm");
            FormFileSenderButtonSetting _formSetting = (FormFileSenderButtonSetting)sender;
            Dictionary<string, object> infoDict = _formSetting.GetButtonSettingDict(this);
            _logger.PrintInfo(String.Format("GetButtonSettingDict = {0}", infoDict));
            _logger.PrintInfo(String.Format("KEY_BUTTON_NAME = {0}", infoDict[ConstFileSender.KEY_BUTTON_NAME]));
            _logger.PrintInfo(String.Format("KEY_BUTTON_TEXT = {0}", infoDict[ConstFileSender.KEY_BUTTON_TEXT]));
            _logger.PrintInfo(String.Format("KEY_DIRECTORY_PATH = {0}", infoDict[ConstFileSender.KEY_DIRECTORY_PATH]));
            _logger.PrintInfo(String.Format("KEY_BUTTON_COLOR_TEXT = {0}", infoDict[ConstFileSender.KEY_BUTTON_COLOR_TEXT]));
            _logger.PrintInfo(String.Format("KEY_BUTTON_COLOR = {0}", infoDict[ConstFileSender.KEY_BUTTON_COLOR]));
            //#
            ApplySetting(infoDict);
        }

        public void ApplySetting(Dictionary<string, object> infoDict)
        {
            this.Text = (string)infoDict[ConstFileSender.KEY_BUTTON_TEXT];
            this.BackColor = (Color)infoDict[ConstFileSender.KEY_BUTTON_COLOR];
            //this.ForeColor = (Color)infoDict[ConstFileSender.KEY_BUTTON_COLOR];
            this._directoryPath = (string)infoDict[ConstFileSender.KEY_DIRECTORY_PATH];
        }

        public Dictionary<string, object> GetSettingDict()
        {
            Dictionary<string, object> retDict = new Dictionary<string, object> { };
            retDict.Add(ConstFileSender.KEY_BUTTON_NAME, this.Name);
            retDict.Add(ConstFileSender.KEY_BUTTON_TEXT, this.Text);
            retDict.Add(ConstFileSender.KEY_DIRECTORY_PATH, this._directoryPath);
            retDict.Add(ConstFileSender.KEY_BUTTON_COLOR_TEXT, this.BackColor.ToString());
            retDict.Add(ConstFileSender.KEY_BUTTON_COLOR, this.BackColor);
            return retDict;
        }

        public void Menu_SetDirPath_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("Menu_SetDirPath_Click");
        }
        public void Menu_Rename_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("Menu_Rename_Click");

        }
        public void Menu_ChangeColor_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("Menu_ChangeColor_Click");

        }


    }

    public static class ConstFileSender
    {
        public static readonly int TO_RIGHT = 0;
        public static readonly int TO_BOTTOM = 1;
        public static readonly string KEY_BUTTON_NAME = "ButtonName"; // オブジェクト名
        public static readonly string KEY_BUTTON_TEXT = "ButtonText"; // オブジェクトのテキスト
        public static readonly string KEY_DIRECTORY_PATH = "DirectoryPath";
        public static readonly string KEY_BUTTON_COLOR_TEXT = "ButtonColorText";
        public static readonly string KEY_BUTTON_COLOR = "ButtonColor";
        public static readonly string KEY_TAB_PAGE_TEXT = "TabPageText";
        //#
        // DefaultJson
        public static readonly string DEFAULT_JSON = @"
{
  ""TabAmount"": 2,
  ""TabPage0"": {
    ""SendButton1"": {
      ""ButtonName"": ""SendButton1"",
      ""ButtonText"": ""Button"",
      ""DirectoryPath"": """",
      ""ButtonColorText"": ""Color [Control]"",
      ""ButtonColor"": ""Control""
    },
    ""TabPageText"": ""Tab1""
  }
}";
        // DefaultJson END
        //#
        //public static readonly string SETTING_FILE_NAME = "setting.json";
        public static readonly string SETTING_FILE_NAME = "settingFileSender.json";
        //#
        //設定
        public static readonly string KEY_SETTING_ITEM_KIND = "ItemKind";
        public static readonly int ITEM_KIND_TAB_CONTROL = 0;
        public static readonly int ITEM_KIND_TAB_PAGE = 1; //TabPage+UserControl
        public static readonly int ITEM_KIND_BUTTON = 2;
        public static readonly string KEY_SETTING_TAB_PAGE_TEXT = "TabPageText";
        public static readonly string KEY_SETTING_TAB_PAGE_NAME = "TabPageName";
        public static readonly string KEY_SETTING_TAB_CONTROL_TEXT = "TabControlText";
        public static readonly string KEY_SETTING_TAB_CONTROL_NAME = "TabControlName";


    }
}
