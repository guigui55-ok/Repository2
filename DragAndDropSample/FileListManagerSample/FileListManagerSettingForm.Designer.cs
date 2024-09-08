
namespace FileListManagerSample
{
    partial class FileListManagerSettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBox_EnableRandomList = new System.Windows.Forms.CheckBox();
            this.checkBox_FixedFileList = new System.Windows.Forms.CheckBox();
            this.checkBox_ReadSubDirFiles = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_FixedDirectory = new System.Windows.Forms.CheckBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBoxSetting = new System.Windows.Forms.GroupBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.checkBox_EnableRandomList, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_FixedFileList, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_ReadSubDirFiles, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_FixedDirectory, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 18);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(566, 99);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // checkBox_EnableRandomList
            // 
            this.checkBox_EnableRandomList.AutoSize = true;
            this.checkBox_EnableRandomList.Location = new System.Drawing.Point(3, 81);
            this.checkBox_EnableRandomList.Name = "checkBox_EnableRandomList";
            this.checkBox_EnableRandomList.Size = new System.Drawing.Size(155, 15);
            this.checkBox_EnableRandomList.TabIndex = 4;
            this.checkBox_EnableRandomList.Text = "ファイルリストをランダムにする";
            this.checkBox_EnableRandomList.UseVisualStyleBackColor = true;
            // 
            // checkBox_FixedFileList
            // 
            this.checkBox_FixedFileList.AutoSize = true;
            this.checkBox_FixedFileList.Location = new System.Drawing.Point(3, 59);
            this.checkBox_FixedFileList.Name = "checkBox_FixedFileList";
            this.checkBox_FixedFileList.Size = new System.Drawing.Size(354, 16);
            this.checkBox_FixedFileList.TabIndex = 0;
            this.checkBox_FixedFileList.Text = "ファイルリストを固定する（テキストボックスのみ。ボタンでの変更をしない）";
            this.checkBox_FixedFileList.UseVisualStyleBackColor = true;
            // 
            // checkBox_ReadSubDirFiles
            // 
            this.checkBox_ReadSubDirFiles.AutoSize = true;
            this.checkBox_ReadSubDirFiles.Location = new System.Drawing.Point(3, 3);
            this.checkBox_ReadSubDirFiles.Name = "checkBox_ReadSubDirFiles";
            this.checkBox_ReadSubDirFiles.Size = new System.Drawing.Size(229, 16);
            this.checkBox_ReadSubDirFiles.TabIndex = 2;
            this.checkBox_ReadSubDirFiles.Text = "サブフォルダ以下のファイルをすべて読み込む";
            this.checkBox_ReadSubDirFiles.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(544, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "※サブフォルダ以下のファイルをすべて読み込むとき、ディレクトリを変更すると処理に時間がかかる可能性があるため。";
            // 
            // checkBox_FixedDirectory
            // 
            this.checkBox_FixedDirectory.AutoSize = true;
            this.checkBox_FixedDirectory.Location = new System.Drawing.Point(3, 37);
            this.checkBox_FixedDirectory.Name = "checkBox_FixedDirectory";
            this.checkBox_FixedDirectory.Size = new System.Drawing.Size(355, 16);
            this.checkBox_FixedDirectory.TabIndex = 1;
            this.checkBox_FixedDirectory.Text = "フォルダを固定する（テキストボックス入力のみ。ボタンでの変更をしない）";
            this.checkBox_FixedDirectory.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(521, 143);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // groupBoxSetting
            // 
            this.groupBoxSetting.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxSetting.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSetting.Name = "groupBoxSetting";
            this.groupBoxSetting.Size = new System.Drawing.Size(584, 125);
            this.groupBoxSetting.TabIndex = 2;
            this.groupBoxSetting.TabStop = false;
            this.groupBoxSetting.Text = "設定";
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(431, 143);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 3;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // FileListManagerSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 177);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.groupBoxSetting);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FileListManagerSettingForm";
            this.Text = "FileListManagerSetting";
            this.Activated += new System.EventHandler(this.FileListManagerSetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileManagerSettingForm_FormClosing);
            this.Load += new System.EventHandler(this.FileListManagerSettingForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxSetting.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBox_FixedFileList;
        private System.Windows.Forms.CheckBox checkBox_ReadSubDirFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_FixedDirectory;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupBoxSetting;
        private System.Windows.Forms.CheckBox checkBox_EnableRandomList;
        private System.Windows.Forms.Button buttonApply;
    }
}