
namespace ProgressDialog.FormWindow
{
    partial class ProgressDialogForm
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.ProcessDialogLabel1 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(321, 28);
            this.progressBar1.TabIndex = 0;
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.Location = new System.Drawing.Point(258, 71);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.Button_Cancel.TabIndex = 1;
            this.Button_Cancel.Text = "Cancel";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            //this.Button_Cancel.Click += new System.EventHandler(this.Button_Cancel_Click);
            // 
            // ProcessDialogLabel1
            // 
            this.ProcessDialogLabel1.AutoSize = true;
            this.ProcessDialogLabel1.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ProcessDialogLabel1.Location = new System.Drawing.Point(12, 53);
            this.ProcessDialogLabel1.Name = "ProcessDialogLabel1";
            this.ProcessDialogLabel1.Size = new System.Drawing.Size(138, 19);
            this.ProcessDialogLabel1.TabIndex = 2;
            this.ProcessDialogLabel1.Text = "Now Processing...";
            // 
            // ProcessingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 104);
            this.Controls.Add(this.ProcessDialogLabel1);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.progressBar1);
            this.Name = "ProcessingDialog";
            this.Text = "ProcessingDialog";
            this.Load += new System.EventHandler(this.ProcessingDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button Button_Cancel;
        private System.Windows.Forms.Label ProcessDialogLabel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}