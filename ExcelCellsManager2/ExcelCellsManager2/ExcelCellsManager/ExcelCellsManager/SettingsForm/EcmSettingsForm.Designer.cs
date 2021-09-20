
namespace ExcelCellsManager.ExcelCellsManager.SettingsForm
{
    partial class EcmSettingsForm
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
            this.tabPage_General = new System.Windows.Forms.TabPage();
            this.Panel_General = new System.Windows.Forms.Panel();
            this.Ecm_Settings_tabControl1 = new System.Windows.Forms.TabControl();
            this.Button_SettingsApply = new System.Windows.Forms.Button();
            this.Button_SettingsClose = new System.Windows.Forms.Button();
            this.tabPage_General.SuspendLayout();
            this.Ecm_Settings_tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage_General
            // 
            this.tabPage_General.AutoScroll = true;
            this.tabPage_General.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_General.Controls.Add(this.Panel_General);
            this.tabPage_General.Location = new System.Drawing.Point(4, 22);
            this.tabPage_General.Name = "tabPage_General";
            this.tabPage_General.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_General.Size = new System.Drawing.Size(564, 322);
            this.tabPage_General.TabIndex = 0;
            this.tabPage_General.Text = "tabPage1";
            // 
            // Panel_General
            // 
            this.Panel_General.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_General.Location = new System.Drawing.Point(3, 3);
            this.Panel_General.Name = "Panel_General";
            this.Panel_General.Size = new System.Drawing.Size(558, 316);
            this.Panel_General.TabIndex = 0;
            // 
            // Ecm_Settings_tabControl1
            // 
            this.Ecm_Settings_tabControl1.Controls.Add(this.tabPage_General);
            this.Ecm_Settings_tabControl1.Location = new System.Drawing.Point(14, 12);
            this.Ecm_Settings_tabControl1.Name = "Ecm_Settings_tabControl1";
            this.Ecm_Settings_tabControl1.SelectedIndex = 0;
            this.Ecm_Settings_tabControl1.Size = new System.Drawing.Size(572, 348);
            this.Ecm_Settings_tabControl1.TabIndex = 0;
            // 
            // Button_SettingsApply
            // 
            this.Button_SettingsApply.Location = new System.Drawing.Point(416, 366);
            this.Button_SettingsApply.Name = "Button_SettingsApply";
            this.Button_SettingsApply.Size = new System.Drawing.Size(75, 23);
            this.Button_SettingsApply.TabIndex = 0;
            this.Button_SettingsApply.Text = "適用";
            this.Button_SettingsApply.UseVisualStyleBackColor = true;
            this.Button_SettingsApply.Click += new System.EventHandler(this.Button_SettingsApply_Click);
            // 
            // Button_SettingsClose
            // 
            this.Button_SettingsClose.Location = new System.Drawing.Point(497, 366);
            this.Button_SettingsClose.Name = "Button_SettingsClose";
            this.Button_SettingsClose.Size = new System.Drawing.Size(75, 23);
            this.Button_SettingsClose.TabIndex = 1;
            this.Button_SettingsClose.TabStop = false;
            this.Button_SettingsClose.Text = "閉じる";
            this.Button_SettingsClose.UseVisualStyleBackColor = true;
            this.Button_SettingsClose.Click += new System.EventHandler(this.Button_SettingsClose_Click);
            // 
            // EcmSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 401);
            this.Controls.Add(this.Button_SettingsClose);
            this.Controls.Add(this.Button_SettingsApply);
            this.Controls.Add(this.Ecm_Settings_tabControl1);
            this.Name = "EcmSettingsForm";
            this.Text = "ExcelCellsManagerSettingsForm";
            this.Load += new System.EventHandler(this.Ecm_SettingsForm_Load);
            this.tabPage_General.ResumeLayout(false);
            this.Ecm_Settings_tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage tabPage_General;
        private System.Windows.Forms.TabControl Ecm_Settings_tabControl1;
        private System.Windows.Forms.Button Button_SettingsApply;
        private System.Windows.Forms.Button Button_SettingsClose;
        private System.Windows.Forms.Panel Panel_General;
    }
}