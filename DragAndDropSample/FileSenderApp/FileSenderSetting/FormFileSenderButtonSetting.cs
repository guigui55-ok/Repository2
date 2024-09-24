using FileSenderApp.FileSenderSetting;
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

namespace FileSenderApp
{
    public partial class FormFileSenderButtonSetting : Form
    {
        public Point _Margin = new Point();
        public EventHandler ClickButtonApply;
        public FormFileSenderButtonSetting()
        {
            InitializeComponent();
        }

        private void FormFileSenderButtonSetting_Load(object sender, EventArgs e)
        {

        }

        public void ClearSettingParts()
        {
            //this.Controls.Clear();
            foreach(Control con in this.Controls)
            {
                if (con.GetType().ToString().IndexOf("ButtonSettingGroup") > 0)
                {
                    this.Controls.Remove(con);
                }
            }
        }

        public Dictionary<string ,object> GetButtonSettingDict(SendButton sendButton)
        {
            Dictionary<string, object> retDict = new Dictionary<string, object> { };
            foreach(Control con in this.Controls)
            {
                if (con.GetType().ToString().IndexOf("ButtonSettingGroup") > 0)
                {
                    ButtonSettingGroup settingGroup = (ButtonSettingGroup)con;
                    //(settingGroup.GetGroupText() == sendButton.Text)
                    if (settingGroup._buttonName == sendButton.Name)
                    {
                        retDict = settingGroup.GetSettingDict();
                        break;
                    }
                }
            }
            return retDict;
        }


        public void ShowDialogForList(AppLogger logger, List<SendButton> sendButtonList)
        {
            logger.PrintInfo("FormFileSenderButtonSetting > ShowDialogForList");
            // ボタン分の設定のUserControlを表示して、Windowサイズも調整
            string baseName = "ButtonSettingGroup";
            int index = 1;
            Point location = new Point(0, 0);
            //Size size = buttonSetting
            foreach (SendButton button in sendButtonList)
            {
                ButtonSettingGroup buttonSetting = new ButtonSettingGroup();
                this.Controls.Add(buttonSetting);
                buttonSetting.Initialize(logger, button);

                //retSendButton.Location = new System.Drawing.Point(15, 8);
                location.Y = (index - 1) * buttonSetting.Size.Height;
                buttonSetting.Location = location;
                buttonSetting.Name = baseName + index;
                //retSendButton.Size = new System.Drawing.Size(150, 36);
                //retSendButton.Size = size;
                //retSendButton.TabIndex = tabIndex;
                //buttonSetting.Text = text;
                buttonSetting.SetGroupText(button.Text);
                //retSendButton.UseVisualStyleBackColor = true;
                PrintSettingGroupInfo(logger, buttonSetting);
                index++;
            }
        }

        private void PrintSettingGroupInfo(AppLogger logger, ButtonSettingGroup buttonSettingGroup)
        {
            string buf = "";
            buf += string.Format("Name={0}", buttonSettingGroup.Name);
            buf += string.Format(", Text={0}", buttonSettingGroup.Text);
            buf += string.Format(", Size={0}", buttonSettingGroup.Size);
            buf += string.Format(", Loc={0}", buttonSettingGroup.Location);
            //buf += string.Format(", TabIndex={0}", buttonSettingGroup.TabIndex);
            //buf += string.Format(", BColor={0}", buttonSettingGroup.BackColor.ToString());
            logger.PrintInfo(buf);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            ClickButtonApply(this, EventArgs.Empty);
            this.Visible = false;
        }
    }
}
