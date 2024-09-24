
namespace FileSenderApp
{
    partial class ButtonsGroup
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelRename = new System.Windows.Forms.Panel();
            this.buttonRenameOK = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelRename = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.sendButton4 = new FileSenderApp.SendButton();
            this.sendButton3 = new FileSenderApp.SendButton();
            this.sendButton2 = new FileSenderApp.SendButton();
            this.sendButton1 = new FileSenderApp.SendButton();
            this.panelRename.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRename
            // 
            this.panelRename.Controls.Add(this.buttonRenameOK);
            this.panelRename.Controls.Add(this.textBox1);
            this.panelRename.Controls.Add(this.labelRename);
            this.panelRename.Location = new System.Drawing.Point(0, 0);
            this.panelRename.Name = "panelRename";
            this.panelRename.Size = new System.Drawing.Size(342, 25);
            this.panelRename.TabIndex = 1;
            // 
            // buttonRenameOK
            // 
            this.buttonRenameOK.Location = new System.Drawing.Point(275, 3);
            this.buttonRenameOK.Name = "buttonRenameOK";
            this.buttonRenameOK.Size = new System.Drawing.Size(55, 19);
            this.buttonRenameOK.TabIndex = 2;
            this.buttonRenameOK.Text = "OK";
            this.buttonRenameOK.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(88, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(177, 19);
            this.textBox1.TabIndex = 2;
            // 
            // labelRename
            // 
            this.labelRename.AutoSize = true;
            this.labelRename.Location = new System.Drawing.Point(3, 6);
            this.labelRename.Name = "labelRename";
            this.labelRename.Size = new System.Drawing.Size(80, 12);
            this.labelRename.TabIndex = 2;
            this.labelRename.Text = "RenameButton";
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.sendButton4);
            this.panelButtons.Controls.Add(this.sendButton3);
            this.panelButtons.Controls.Add(this.sendButton2);
            this.panelButtons.Controls.Add(this.sendButton1);
            this.panelButtons.Location = new System.Drawing.Point(0, 25);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(342, 238);
            this.panelButtons.TabIndex = 3;
            // 
            // sendButton4
            // 
            this.sendButton4.Location = new System.Drawing.Point(175, 50);
            this.sendButton4.Name = "sendButton4";
            this.sendButton4.Size = new System.Drawing.Size(150, 36);
            this.sendButton4.TabIndex = 4;
            this.sendButton4.Text = "破棄される";
            this.sendButton4.UseVisualStyleBackColor = true;
            // 
            // sendButton3
            // 
            this.sendButton3.Location = new System.Drawing.Point(15, 50);
            this.sendButton3.Name = "sendButton3";
            this.sendButton3.Size = new System.Drawing.Size(150, 36);
            this.sendButton3.TabIndex = 3;
            this.sendButton3.Text = "このボタンはすべて";
            this.sendButton3.UseVisualStyleBackColor = true;
            // 
            // sendButton2
            // 
            this.sendButton2.Location = new System.Drawing.Point(175, 8);
            this.sendButton2.Name = "sendButton2";
            this.sendButton2.Size = new System.Drawing.Size(150, 36);
            this.sendButton2.TabIndex = 2;
            this.sendButton2.Text = "再作成する";
            this.sendButton2.UseVisualStyleBackColor = true;
            // 
            // sendButton1
            // 
            this.sendButton1.Location = new System.Drawing.Point(15, 8);
            this.sendButton1.Name = "sendButton1";
            this.sendButton1.Size = new System.Drawing.Size(150, 36);
            this.sendButton1.TabIndex = 1;
            this.sendButton1.Text = "ボタンはソース内で";
            this.sendButton1.UseVisualStyleBackColor = true;
            // 
            // ButtonsGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelRename);
            this.Name = "ButtonsGroup";
            this.Size = new System.Drawing.Size(342, 266);
            this.Load += new System.EventHandler(this.ButtonsGroup_Load);
            this.panelRename.ResumeLayout(false);
            this.panelRename.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelRename;
        private System.Windows.Forms.Button buttonRenameOK;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label labelRename;
        private System.Windows.Forms.Panel panelButtons;
        private SendButton sendButton2;
        private SendButton sendButton1;
        private SendButton sendButton4;
        private SendButton sendButton3;
    }
}
