using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace FileSenderApp.FileSenderSetting
{
    public partial class ButtonSettingGroup : UserControl
    {
        AppLogger _logger;
        ColorPicker _colorPicker;
        Color _buttonColor;
        Color _resetColor = SystemColors.Control;
        public string _buttonName = "";
        SendButton _sendButton = null;
        public ButtonSettingGroup()
        {
            InitializeComponent();
        }

        private void ButtonSettingGroup_Load(object sender, EventArgs e)
        {

        }

        public void Initialize(AppLogger logger, SendButton sendButton)
        {
            _logger = logger;
            _colorPicker = new ColorPicker();
            // Name、DirectoryPath、Colorを取得する
            // TextBoxなどに反映
            textBoxButtonName.Text = sendButton.Text;
            textBoxDirPath.Text = sendButton._directoryPath;
            labelButtonColor.Text = sendButton.BackColor.ToString();
            _buttonColor = sendButton.BackColor;
            //#
            _buttonName = sendButton.Name;
            _sendButton = sendButton;
        }

        public void SetGroupText(string Text)
        {
            groupBoxSetting.Text = Text;
        }

        public string GetGroupText()
        {
            return groupBoxSetting.Text;
        }

        public Dictionary<string, object> GetSettingDict()
        {
            Dictionary<string, object> retDict = new Dictionary<string, object> { };
            retDict.Add(ConstFileSender.KEY_BUTTON_NAME, _buttonName);
            retDict.Add(ConstFileSender.KEY_BUTTON_TEXT, textBoxButtonName.Text);
            retDict.Add(ConstFileSender.KEY_DIRECTORY_PATH, textBoxDirPath.Text);
            retDict.Add(ConstFileSender.KEY_BUTTON_COLOR_TEXT, labelButtonColor.Text);
            retDict.Add(ConstFileSender.KEY_BUTTON_COLOR, _buttonColor);
            return retDict;
        }

        private void buttonColorPicker_Click(object sender, EventArgs e)
        {
            Color ret = _colorPicker.ShowDialogAndGetSelectedColor(_buttonColor);
            _buttonColor = ret;
            this.labelButtonColor.Text = ret.ToString();
            _logger.PrintInfo(string.Format("labelButtonColor = {0}", labelButtonColor.Text));
        }

        private void buttonColorReset_Click(object sender, EventArgs e)
        {
            _buttonColor = _resetColor;
            this.labelButtonColor.Text = _buttonColor.ToString();
            _logger.PrintInfo(string.Format("resetColor = {0}", labelButtonColor.Text));
        }
    }
}
