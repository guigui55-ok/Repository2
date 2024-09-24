
namespace FileSenderApp.FileSenderSetting
{
    partial class ButtonSettingGroup
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
            this.groupBoxSetting = new System.Windows.Forms.GroupBox();
            this.labelButtonColor = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonColorPicker = new System.Windows.Forms.Button();
            this.textBoxDirPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxButtonName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonColorReset = new System.Windows.Forms.Button();
            this.groupBoxSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSetting
            // 
            this.groupBoxSetting.Controls.Add(this.buttonColorReset);
            this.groupBoxSetting.Controls.Add(this.labelButtonColor);
            this.groupBoxSetting.Controls.Add(this.label3);
            this.groupBoxSetting.Controls.Add(this.buttonColorPicker);
            this.groupBoxSetting.Controls.Add(this.textBoxDirPath);
            this.groupBoxSetting.Controls.Add(this.label2);
            this.groupBoxSetting.Controls.Add(this.textBoxButtonName);
            this.groupBoxSetting.Controls.Add(this.label1);
            this.groupBoxSetting.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSetting.Name = "groupBoxSetting";
            this.groupBoxSetting.Size = new System.Drawing.Size(499, 102);
            this.groupBoxSetting.TabIndex = 1;
            this.groupBoxSetting.TabStop = false;
            this.groupBoxSetting.Text = "Setting";
            // 
            // labelButtonColor
            // 
            this.labelButtonColor.AutoSize = true;
            this.labelButtonColor.Font = new System.Drawing.Font("MS UI Gothic", 10F);
            this.labelButtonColor.Location = new System.Drawing.Point(112, 76);
            this.labelButtonColor.Name = "labelButtonColor";
            this.labelButtonColor.Size = new System.Drawing.Size(82, 14);
            this.labelButtonColor.TabIndex = 6;
            this.labelButtonColor.Text = "Button Color";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Button Color : ";
            // 
            // buttonColorPicker
            // 
            this.buttonColorPicker.Location = new System.Drawing.Point(400, 71);
            this.buttonColorPicker.Name = "buttonColorPicker";
            this.buttonColorPicker.Size = new System.Drawing.Size(75, 23);
            this.buttonColorPicker.TabIndex = 4;
            this.buttonColorPicker.Text = "ColorPicker";
            this.buttonColorPicker.UseVisualStyleBackColor = true;
            this.buttonColorPicker.Click += new System.EventHandler(this.buttonColorPicker_Click);
            // 
            // textBoxDirPath
            // 
            this.textBoxDirPath.Location = new System.Drawing.Point(114, 46);
            this.textBoxDirPath.Name = "textBoxDirPath";
            this.textBoxDirPath.Size = new System.Drawing.Size(361, 19);
            this.textBoxDirPath.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Directory Path : ";
            // 
            // textBoxButtonName
            // 
            this.textBoxButtonName.Location = new System.Drawing.Point(114, 21);
            this.textBoxButtonName.Name = "textBoxButtonName";
            this.textBoxButtonName.Size = new System.Drawing.Size(361, 19);
            this.textBoxButtonName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Button Name : ";
            // 
            // buttonColorReset
            // 
            this.buttonColorReset.Location = new System.Drawing.Point(308, 71);
            this.buttonColorReset.Name = "buttonColorReset";
            this.buttonColorReset.Size = new System.Drawing.Size(75, 23);
            this.buttonColorReset.TabIndex = 7;
            this.buttonColorReset.Text = "Reset";
            this.buttonColorReset.UseVisualStyleBackColor = true;
            this.buttonColorReset.Click += new System.EventHandler(this.buttonColorReset_Click);
            // 
            // ButtonSettingGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxSetting);
            this.Name = "ButtonSettingGroup";
            this.Size = new System.Drawing.Size(501, 105);
            this.Load += new System.EventHandler(this.ButtonSettingGroup_Load);
            this.groupBoxSetting.ResumeLayout(false);
            this.groupBoxSetting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSetting;
        private System.Windows.Forms.Label labelButtonColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonColorPicker;
        private System.Windows.Forms.TextBox textBoxDirPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxButtonName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonColorReset;
    }
}
