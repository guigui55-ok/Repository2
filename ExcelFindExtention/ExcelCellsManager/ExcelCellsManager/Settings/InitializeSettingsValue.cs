using SettingsManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Utility;

namespace ExcelCellsManager.ExcelCellsManager.Settings
{
    public class InitializeSettingsValue
    {
        protected ErrorManager.ErrorManager _error;
        public ExcelCellsManagerMain ExcelCellsManagerMain;
        protected ExcelCellsManagerConstants Constants;
        public int SettingsFormWidth = 150;
        public int TabLocationY = 50;

        public InitializeSettingsValue(ErrorManager.ErrorManager error,ExcelCellsManagerMain excelCellsManagerMain)
        {
            _error = error;
            ExcelCellsManagerMain = excelCellsManagerMain;
            Constants = excelCellsManagerMain.Constants;
        }

        public void Excute()
        {
            try
            {
                List<SettingsManager.ISettingsObject> SettingsList = new List<SettingsManager.ISettingsObject>();

                int padding = 3;
                int lineHeight = 16;
                int count = 1;
                int SettingsPanelWidth = this.SettingsFormWidth;

                int nowLocationY = TabLocationY + padding;

                FormUtility formUtility = new FormUtility(_error);
                TypeUtility typeUtility = new TypeUtility(_error);

                //
                ISettingsObject temp = new SettingsObject(_error, formUtility, typeUtility);
                // アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする
                count = 1;
                temp.SettingsTypeName = "application";
                temp.Name = "IsActivateWorkbookWindowAfterRegistAddress";
                temp.Description = "アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする";
                temp.ValueType = typeof(bool);
                temp.InitialValue = true;
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                CheckBox checkbox = new CheckBox
                {
                    Location = new Point(padding, nowLocationY),
                    Size = new Size(SettingsPanelWidth, lineHeight)
                };
                nowLocationY += lineHeight + padding;
                checkbox.TabIndex = count;
                checkbox.Text = temp.Description;
                checkbox.Name = temp.Name;
                temp.Control = checkbox;
                SettingsList.Add(temp);


                // 起動時に前回に開いていたファイルをひらく
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility)
                {
                    SettingsTypeName = "application"
                };
                temp.Name = "IsOpenLastFileWhenRunApplication";
                temp.Description = "起動時に最後に開いていたファイルを開く";
                temp.ValueType = typeof(bool);
                temp.InitialValue = true;
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                checkbox = new CheckBox
                {
                    Location = new Point(padding, nowLocationY),
                    Size = new Size(SettingsPanelWidth, lineHeight)
                };
                nowLocationY += lineHeight + padding;
                checkbox.TabIndex = count;
                checkbox.Text = temp.Description;
                checkbox.Name = temp.Name;
                temp.Control = checkbox;
                SettingsList.Add(temp);


                // 前回最後に開いていたファイル
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "application";
                temp.Name = "LastOpendFilePath";
                temp.Description = "前回最後に開いていたファイルパス";
                temp.ValueType = typeof(string);
                temp.InitialValue = (string)Constants.DefalutNewFileName + Constants.DefaultFileType;
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                Panel panel = new Panel();
                Label label = new Label();
                label.Text = temp.Description;
                TextBox textbox = new TextBox();
                panel.Controls.Add(label);
                panel.Controls.Add(textbox);

                label.Location = new Point(0, 0);
                label.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Location = new Point(0, lineHeight);
                textbox.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Name = temp.Name;

                panel.Location = new Point(padding, nowLocationY);
                panel.Size = new Size(SettingsPanelWidth, lineHeight*2);
                nowLocationY += lineHeight * 2 + padding;
                textbox.TabIndex = count;
                temp.Control = panel;
                SettingsList.Add(temp);


                // 画面端にマウスポインタを移動したときにフォームをアクティブにする
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "application";
                temp.Name = "IsActivateFormWhenMoveMouseScreenEdge";
                temp.Description = "画面端にマウスポインタを移動したときにフォームをアクティブにする";
                temp.ValueType = typeof(bool);
                temp.InitialValue = false;
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                checkbox = new CheckBox();
                checkbox.Location = new Point(padding, nowLocationY);
                checkbox.Size = new Size(SettingsPanelWidth, lineHeight);
                nowLocationY += lineHeight + padding;
                checkbox.TabIndex = count;
                checkbox.Text = temp.Description;
                checkbox.Name = temp.Name;
                temp.Control = checkbox;
                SettingsList.Add(temp);


                // DataList のカラム幅を記憶する
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "dataList";
                temp.Name = "IsSaveColumnWidthOfDataList";
                temp.Description = "DataList のカラム幅を記憶する";
                temp.ValueType = typeof(bool);
                temp.InitialValue = true;
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                checkbox = new CheckBox();
                checkbox.Location = new Point(padding, nowLocationY);
                checkbox.Size = new Size(SettingsPanelWidth, lineHeight);
                nowLocationY += lineHeight + padding;
                checkbox.TabIndex = count;
                checkbox.Text = temp.Description;
                checkbox.Name = temp.Name;
                temp.Control = checkbox;
                SettingsList.Add(temp);


                // ColumnWidth [10-10-10-...]
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "dataList";
                temp.Name = "ColumnWidthOfDataList";
                temp.Description = "DataList のカラム幅";
                temp.ValueType = typeof(int[]);
                temp.InitialValue = new int[] { 100, 100, 100, 100, 100, 100 };
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                panel = new Panel();
                label = new Label();
                label.Text = temp.Description;
                textbox = new TextBox();
                panel.Controls.Add(label);
                panel.Controls.Add(textbox);

                label.Location = new Point(0, 0);
                label.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Location = new Point(0, lineHeight);
                textbox.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Name = temp.Name;

                panel.Location = new Point(padding, nowLocationY);
                panel.Size = new Size(SettingsPanelWidth, lineHeight * 2);
                nowLocationY += lineHeight * 2 + padding;
                textbox.TabIndex = count;
                temp.Control = panel;
                SettingsList.Add(temp);


                // MouseCaptureSettingLeft;
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "application";
                temp.Name = "MouseCaptureSettingLeft";
                temp.Description = "フォームをアクティブする設定詳細 (左)";
                temp.ValueType = typeof(int[]);
                temp.InitialValue = new int[] { 1,200 , 300, 1 };
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                 panel = new Panel();
                 label = new Label();
                label.Text = temp.Description;
                 textbox = new TextBox();
                panel.Controls.Add(label);
                panel.Controls.Add(textbox);

                label.Location = new Point(0, 0);
                label.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Location = new Point(0, lineHeight);
                textbox.Size = new Size(SettingsPanelWidth, lineHeight);

                panel.Location = new Point(padding, nowLocationY);
                panel.Size = new Size(SettingsPanelWidth, lineHeight * 2);
                nowLocationY += lineHeight * 2 + padding;
                textbox.TabIndex = count;
                temp.Control = panel;
                SettingsList.Add(temp);


                // MouseCaptureSettingTop;
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "application";
                temp.Name = "MouseCaptureSettingTop";
                temp.Description = "フォームをアクティブする設定詳細 (上)";
                temp.ValueType = typeof(int[]);
                temp.InitialValue = new int[] { 1, 200, 300, 1 };
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                panel = new Panel();
                label = new Label();
                label.Text = temp.Description;
                textbox = new TextBox();
                panel.Controls.Add(label);
                panel.Controls.Add(textbox);

                label.Location = new Point(0, 0);
                label.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Location = new Point(0, lineHeight);
                textbox.Size = new Size(SettingsPanelWidth, lineHeight);

                panel.Location = new Point(padding, nowLocationY);
                panel.Size = new Size(SettingsPanelWidth, lineHeight * 2);
                nowLocationY += lineHeight * 2 + padding;
                textbox.TabIndex = count;
                temp.Control = panel;
                SettingsList.Add(temp);


                // MouseCaptureSettingRight
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility)
                {
                    SettingsTypeName = "application"
                };
                temp.Name = "MouseCaptureSettingRight";
                temp.Description = "フォームをアクティブする設定詳細 (右)";
                temp.ValueType = typeof(int[]);
                temp.InitialValue = new int[] { 0, 200, 300, 1 };
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                panel = new Panel();
                label = new Label();
                label.Text = temp.Description;
                textbox = new TextBox();
                panel.Controls.Add(label);
                panel.Controls.Add(textbox);

                label.Location = new Point(0, 0);
                label.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Location = new Point(0, lineHeight);
                textbox.Size = new Size(SettingsPanelWidth, lineHeight);

                panel.Location = new Point(padding, nowLocationY);
                panel.Size = new Size(SettingsPanelWidth, lineHeight * 2);
                nowLocationY += lineHeight * 2 + padding;
                textbox.TabIndex = count;
                temp.Control = panel;
                SettingsList.Add(temp);


                // MouseCaptureSettingBottom
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "application";
                temp.Name = "MouseCaptureSettingBottom";
                temp.Description = "フォームをアクティブする設定詳細 (下)";
                temp.ValueType = typeof(int[]);
                temp.InitialValue = new int[] { 0, 200, 300, 1 };
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                // control
                panel = new Panel();
                label = new Label();
                label.Text = temp.Description;
                textbox = new TextBox();
                panel.Controls.Add(label);
                panel.Controls.Add(textbox);

                label.Location = new Point(0, 0);
                label.Size = new Size(SettingsPanelWidth, lineHeight);
                textbox.Location = new Point(0, lineHeight);
                textbox.Size = new Size(SettingsPanelWidth, lineHeight);

                panel.Location = new Point(padding, nowLocationY);
                panel.Size = new Size(SettingsPanelWidth, lineHeight * 2);
                nowLocationY += lineHeight * 2 + padding;
                textbox.TabIndex = count;
                temp.Control = panel;
                SettingsList.Add(temp);


                // エクセルを開いた時、閉じた時に WorkbookList を更新する
                count++;
                temp = new SettingsObject(_error, formUtility, typeUtility);
                temp.SettingsTypeName = "application";
                temp.Name = "IsUpdateListWhenWorkbookOpendAndClosed";
                temp.Description = "エクセルを開いた時、閉じた時に WorkbookList を更新する";
                temp.ValueType = typeof(bool);
                temp.InitialValue = true;
                temp.RegKey = "";
                temp.ShortCutKeys = Keys.None;
                checkbox = new CheckBox();
                checkbox.Location = new Point(padding, nowLocationY);
                checkbox.Size = new Size(SettingsPanelWidth, lineHeight);
                nowLocationY += lineHeight + padding;
                checkbox.Text = temp.Description;
                checkbox.TabIndex = count;
                temp.Control = checkbox;
                SettingsList.Add(temp);


                ExcelCellsManagerMain.AppsSettings.SettingsManager.SettingsList = SettingsList;
            }
            catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".Excute");
            }
        } 
    }
}
