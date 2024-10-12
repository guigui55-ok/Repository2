
namespace FileSenderApp
{
    partial class FormFileSenderApp
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonsGroup1 = new FileSenderApp.ButtonsGroup();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonRenameTabOK = new System.Windows.Forms.Button();
            this.textBoxRenameTabPage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelCheckBox = new System.Windows.Forms.Panel();
            this.buttonRedo = new System.Windows.Forms.Button();
            this.buttonUndo = new System.Windows.Forms.Button();
            this.checkBoxFileMove = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelCheckBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(-3, 54);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(350, 267);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.buttonsGroup1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(342, 241);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tab1";
            // 
            // buttonsGroup1
            // 
            this.buttonsGroup1.Location = new System.Drawing.Point(0, 3);
            this.buttonsGroup1.Name = "buttonsGroup1";
            this.buttonsGroup1.Size = new System.Drawing.Size(342, 266);
            this.buttonsGroup1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(342, 241);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "[+]Add Tab";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonRenameTabOK);
            this.panel1.Controls.Add(this.textBoxRenameTabPage);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(-3, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 26);
            this.panel1.TabIndex = 1;
            // 
            // buttonRenameTabOK
            // 
            this.buttonRenameTabOK.Location = new System.Drawing.Point(280, 5);
            this.buttonRenameTabOK.Name = "buttonRenameTabOK";
            this.buttonRenameTabOK.Size = new System.Drawing.Size(61, 19);
            this.buttonRenameTabOK.TabIndex = 2;
            this.buttonRenameTabOK.Text = "OK";
            this.buttonRenameTabOK.UseVisualStyleBackColor = true;
            this.buttonRenameTabOK.Click += new System.EventHandler(this.ButtonRenameTabOK_Click);
            // 
            // textBoxRenameTabPage
            // 
            this.textBoxRenameTabPage.Location = new System.Drawing.Point(60, 5);
            this.textBoxRenameTabPage.Name = "textBoxRenameTabPage";
            this.textBoxRenameTabPage.Size = new System.Drawing.Size(214, 19);
            this.textBoxRenameTabPage.TabIndex = 1;
            this.textBoxRenameTabPage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxRenameTab_KeyDown);
            this.textBoxRenameTabPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxRenameTab_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rename:";
            // 
            // panelCheckBox
            // 
            this.panelCheckBox.Controls.Add(this.buttonRedo);
            this.panelCheckBox.Controls.Add(this.buttonUndo);
            this.panelCheckBox.Controls.Add(this.checkBoxFileMove);
            this.panelCheckBox.Location = new System.Drawing.Point(0, 28);
            this.panelCheckBox.Name = "panelCheckBox";
            this.panelCheckBox.Size = new System.Drawing.Size(346, 22);
            this.panelCheckBox.TabIndex = 1;
            // 
            // buttonRedo
            // 
            this.buttonRedo.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonRedo.Location = new System.Drawing.Point(302, 0);
            this.buttonRedo.Name = "buttonRedo";
            this.buttonRedo.Size = new System.Drawing.Size(30, 22);
            this.buttonRedo.TabIndex = 2;
            this.buttonRedo.Text = "→";
            this.buttonRedo.UseVisualStyleBackColor = false;
            this.buttonRedo.Click += new System.EventHandler(this.buttonRedo_Click);
            // 
            // buttonUndo
            // 
            this.buttonUndo.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonUndo.Location = new System.Drawing.Point(264, 0);
            this.buttonUndo.Name = "buttonUndo";
            this.buttonUndo.Size = new System.Drawing.Size(30, 22);
            this.buttonUndo.TabIndex = 1;
            this.buttonUndo.Text = "←";
            this.buttonUndo.UseVisualStyleBackColor = true;
            this.buttonUndo.Click += new System.EventHandler(this.buttonUndo_Click);
            // 
            // checkBoxFileMove
            // 
            this.checkBoxFileMove.AutoSize = true;
            this.checkBoxFileMove.Location = new System.Drawing.Point(4, 3);
            this.checkBoxFileMove.Name = "checkBoxFileMove";
            this.checkBoxFileMove.Size = new System.Drawing.Size(193, 16);
            this.checkBoxFileMove.TabIndex = 0;
            this.checkBoxFileMove.Text = "FileMove ( CheckOff = FileCopy)";
            this.checkBoxFileMove.UseVisualStyleBackColor = true;
            // 
            // FormFileSenderApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 321);
            this.Controls.Add(this.panelCheckBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormFileSenderApp";
            this.Text = "File Sender";
            this.Activated += new System.EventHandler(this.FormFileSenderApp_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFileSenderApp_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormFileSenderApp_FormClosed);
            this.Load += new System.EventHandler(this.FormFileSenderApp_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormFileSenderApp_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormFileSenderApp_KeyDown);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelCheckBox.ResumeLayout(false);
            this.panelCheckBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxRenameTabPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRenameTabOK;
        private ButtonsGroup buttonsGroup1;
        private System.Windows.Forms.Panel panelCheckBox;
        private System.Windows.Forms.CheckBox checkBoxFileMove;
        private System.Windows.Forms.Button buttonRedo;
        private System.Windows.Forms.Button buttonUndo;
    }
}

