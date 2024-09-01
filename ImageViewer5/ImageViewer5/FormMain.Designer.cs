
namespace ImageViewer5
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.imageMainFrame1 = new ImageViewer5.ImageControl.ImageMainFrame();
            this.SuspendLayout();
            // 
            // imageMainFrame1
            // 
            this.imageMainFrame1.Location = new System.Drawing.Point(0, 1);
            this.imageMainFrame1.Name = "imageMainFrame1";
            this.imageMainFrame1.Size = new System.Drawing.Size(414, 419);
            this.imageMainFrame1.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 763);
            this.Controls.Add(this.imageMainFrame1);
            this.Name = "FormMain";
            this.Text = "ImageViewer5";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_Closed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormMain_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageControl.ImageMainFrame imageMainFrame1;
    }
}

