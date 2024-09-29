
namespace ImageViewer5.ImageControl
{
    partial class ImageMainFrame
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox_ImageMain = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowFileList_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SlideShowOnOff_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseApp_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseFrame_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowFileSenderForm_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ImageMain)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox_ImageMain
            // 
            this.pictureBox_ImageMain.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox_ImageMain.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox_ImageMain.Location = new System.Drawing.Point(3, 3);
            this.pictureBox_ImageMain.Name = "pictureBox_ImageMain";
            this.pictureBox_ImageMain.Size = new System.Drawing.Size(415, 421);
            this.pictureBox_ImageMain.TabIndex = 0;
            this.pictureBox_ImageMain.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowFileList_ToolStripMenuItem,
            this.SlideShowOnOff_ToolStripMenuItem,
            this.CloseFrame_ToolStripMenuItem,
            this.ShowFileSenderForm_ToolStripMenuItem,
            this.CloseApp_ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(186, 136);
            // 
            // ShowFileList_ToolStripMenuItem
            // 
            this.ShowFileList_ToolStripMenuItem.Name = "ShowFileList_ToolStripMenuItem";
            this.ShowFileList_ToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.ShowFileList_ToolStripMenuItem.Text = "FileList表示";
            this.ShowFileList_ToolStripMenuItem.Click += new System.EventHandler(this.ShowFileList_ToolStripMenuItem_Click);
            // 
            // SlideShowOnOff_ToolStripMenuItem
            // 
            this.SlideShowOnOff_ToolStripMenuItem.Name = "SlideShowOnOff_ToolStripMenuItem";
            this.SlideShowOnOff_ToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.SlideShowOnOff_ToolStripMenuItem.Text = "スライドショー開始";
            this.SlideShowOnOff_ToolStripMenuItem.Click += new System.EventHandler(this.SlideShowOnOff_ToolStripMenuItem_Click);
            // 
            // CloseApp_ToolStripMenuItem
            // 
            this.CloseApp_ToolStripMenuItem.Name = "CloseApp_ToolStripMenuItem";
            this.CloseApp_ToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.CloseApp_ToolStripMenuItem.Text = "終了";
            this.CloseApp_ToolStripMenuItem.Click += new System.EventHandler(this.CloseApp_ToolStripMenuItem_Click);
            // 
            // CloseFrame_ToolStripMenuItem
            // 
            this.CloseFrame_ToolStripMenuItem.Name = "CloseFrame_ToolStripMenuItem";
            this.CloseFrame_ToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.CloseFrame_ToolStripMenuItem.Text = "フレームを閉じる";
            this.CloseFrame_ToolStripMenuItem.Click += new System.EventHandler(this.CloseFrame_ToolStripMenuItem_Click);
            // 
            // ShowFileSenderForm_ToolStripMenuItem
            // 
            this.ShowFileSenderForm_ToolStripMenuItem.Name = "ShowFileSenderForm_ToolStripMenuItem";
            this.ShowFileSenderForm_ToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.ShowFileSenderForm_ToolStripMenuItem.Text = "振分けダイアログを表示";
            this.ShowFileSenderForm_ToolStripMenuItem.Click += new System.EventHandler(this.ShowFileSenderForm_ToolStripMenuItem_Click);
            // 
            // ImageMainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.pictureBox_ImageMain);
            this.Name = "ImageMainFrame";
            this.Size = new System.Drawing.Size(415, 421);
            this.Load += new System.EventHandler(this.ImageMainFrame_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageMainFrame_KeyDown);
            this.Resize += new System.EventHandler(this.ImageMainFrame_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ImageMain)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_ImageMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem CloseApp_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowFileList_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SlideShowOnOff_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseFrame_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowFileSenderForm_ToolStripMenuItem;
    }
}
