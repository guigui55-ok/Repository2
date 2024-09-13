
namespace FileListManagerSample
{
    partial class FileListManagerSampleForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
            this.label_DirctoryPath = new System.Windows.Forms.Label();
            this.textBox_DirectoryPath = new System.Windows.Forms.TextBox();
            this.listBox_FileList = new System.Windows.Forms.ListBox();
            this.label_FileList = new System.Windows.Forms.Label();
            this.label_Log = new System.Windows.Forms.Label();
            this.button_NextDirectory = new System.Windows.Forms.Button();
            this.button_PreviousDirectory = new System.Windows.Forms.Button();
            this.button_MovePreviousFile = new System.Windows.Forms.Button();
            this.button_MoveNextFile = new System.Windows.Forms.Button();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox_Log
            // 
            this.richTextBox_Log.Location = new System.Drawing.Point(12, 284);
            this.richTextBox_Log.Name = "richTextBox_Log";
            this.richTextBox_Log.Size = new System.Drawing.Size(683, 57);
            this.richTextBox_Log.TabIndex = 0;
            this.richTextBox_Log.Text = "";
            // 
            // label_DirctoryPath
            // 
            this.label_DirctoryPath.AutoSize = true;
            this.label_DirctoryPath.Location = new System.Drawing.Point(12, 9);
            this.label_DirctoryPath.Name = "label_DirctoryPath";
            this.label_DirctoryPath.Size = new System.Drawing.Size(75, 12);
            this.label_DirctoryPath.TabIndex = 1;
            this.label_DirctoryPath.Text = "DirectoryPath";
            // 
            // textBox_DirectoryPath
            // 
            this.textBox_DirectoryPath.Location = new System.Drawing.Point(93, 6);
            this.textBox_DirectoryPath.Name = "textBox_DirectoryPath";
            this.textBox_DirectoryPath.Size = new System.Drawing.Size(602, 19);
            this.textBox_DirectoryPath.TabIndex = 2;
            // 
            // listBox_FileList
            // 
            this.listBox_FileList.FormattingEnabled = true;
            this.listBox_FileList.ItemHeight = 12;
            this.listBox_FileList.Location = new System.Drawing.Point(12, 51);
            this.listBox_FileList.Name = "listBox_FileList";
            this.listBox_FileList.Size = new System.Drawing.Size(683, 196);
            this.listBox_FileList.TabIndex = 3;
            this.listBox_FileList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_FileList_KeyDown);
            // 
            // label_FileList
            // 
            this.label_FileList.AutoSize = true;
            this.label_FileList.Location = new System.Drawing.Point(12, 32);
            this.label_FileList.Name = "label_FileList";
            this.label_FileList.Size = new System.Drawing.Size(43, 12);
            this.label_FileList.TabIndex = 4;
            this.label_FileList.Text = "FileList";
            // 
            // label_Log
            // 
            this.label_Log.AutoSize = true;
            this.label_Log.Location = new System.Drawing.Point(10, 269);
            this.label_Log.Name = "label_Log";
            this.label_Log.Size = new System.Drawing.Size(23, 12);
            this.label_Log.TabIndex = 5;
            this.label_Log.Text = "Log";
            // 
            // button_NextDirectory
            // 
            this.button_NextDirectory.Location = new System.Drawing.Point(472, 347);
            this.button_NextDirectory.Name = "button_NextDirectory";
            this.button_NextDirectory.Size = new System.Drawing.Size(108, 23);
            this.button_NextDirectory.TabIndex = 6;
            this.button_NextDirectory.Text = "Next Directory";
            this.button_NextDirectory.UseVisualStyleBackColor = true;
            this.button_NextDirectory.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_PreviousDirectory
            // 
            this.button_PreviousDirectory.Location = new System.Drawing.Point(358, 347);
            this.button_PreviousDirectory.Name = "button_PreviousDirectory";
            this.button_PreviousDirectory.Size = new System.Drawing.Size(108, 23);
            this.button_PreviousDirectory.TabIndex = 7;
            this.button_PreviousDirectory.Text = "PreviousDirectory";
            this.button_PreviousDirectory.UseVisualStyleBackColor = true;
            this.button_PreviousDirectory.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_MovePreviousFile
            // 
            this.button_MovePreviousFile.Location = new System.Drawing.Point(93, 347);
            this.button_MovePreviousFile.Name = "button_MovePreviousFile";
            this.button_MovePreviousFile.Size = new System.Drawing.Size(112, 23);
            this.button_MovePreviousFile.TabIndex = 8;
            this.button_MovePreviousFile.Text = "MovePreviousFile";
            this.button_MovePreviousFile.UseVisualStyleBackColor = true;
            this.button_MovePreviousFile.Click += new System.EventHandler(this.button_MovePreviousFile_Click);
            // 
            // button_MoveNextFile
            // 
            this.button_MoveNextFile.Location = new System.Drawing.Point(211, 347);
            this.button_MoveNextFile.Name = "button_MoveNextFile";
            this.button_MoveNextFile.Size = new System.Drawing.Size(112, 23);
            this.button_MoveNextFile.TabIndex = 9;
            this.button_MoveNextFile.Text = "MoveNextFile";
            this.button_MoveNextFile.UseVisualStyleBackColor = true;
            this.button_MoveNextFile.Click += new System.EventHandler(this.button4_Click);
            // 
            // buttonSetting
            // 
            this.buttonSetting.Location = new System.Drawing.Point(12, 347);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(60, 23);
            this.buttonSetting.TabIndex = 10;
            this.buttonSetting.Text = "Setting";
            this.buttonSetting.UseVisualStyleBackColor = true;
            this.buttonSetting.Click += new System.EventHandler(this.buttonSetting_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(634, 347);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(60, 23);
            this.buttonClose.TabIndex = 11;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // FileListManagerSampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 385);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonSetting);
            this.Controls.Add(this.button_MoveNextFile);
            this.Controls.Add(this.button_MovePreviousFile);
            this.Controls.Add(this.button_PreviousDirectory);
            this.Controls.Add(this.button_NextDirectory);
            this.Controls.Add(this.label_Log);
            this.Controls.Add(this.label_FileList);
            this.Controls.Add(this.listBox_FileList);
            this.Controls.Add(this.textBox_DirectoryPath);
            this.Controls.Add(this.label_DirctoryPath);
            this.Controls.Add(this.richTextBox_Log);
            this.Name = "FileListManagerSampleForm";
            this.Text = "FileListManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileListManagerForm_Closing);
            this.Load += new System.EventHandler(this.FileListManagerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_Log;
        private System.Windows.Forms.Label label_DirctoryPath;
        private System.Windows.Forms.TextBox textBox_DirectoryPath;
        private System.Windows.Forms.ListBox listBox_FileList;
        private System.Windows.Forms.Label label_FileList;
        private System.Windows.Forms.Label label_Log;
        private System.Windows.Forms.Button button_NextDirectory;
        private System.Windows.Forms.Button button_PreviousDirectory;
        private System.Windows.Forms.Button button_MovePreviousFile;
        private System.Windows.Forms.Button button_MoveNextFile;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.Button buttonClose;
    }
}

