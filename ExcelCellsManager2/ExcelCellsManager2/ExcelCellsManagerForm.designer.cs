
namespace ExcelCellsManager
{
    partial class ExcelCellsManagerForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.Button_CopyRangeValue = new System.Windows.Forms.Button();
            this.Button_Down_CheckedListBoxItem = new System.Windows.Forms.Button();
            this.Button_GetActiveCell = new System.Windows.Forms.Button();
            this.Button_Up_CheckedListBoxItem = new System.Windows.Forms.Button();
            this.Button_CloseWorkbook = new System.Windows.Forms.Button();
            this.Button_UpdateList = new System.Windows.Forms.Button();
            this.Button_Exit = new System.Windows.Forms.Button();
            this.Button_SelectCells = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Button_RegistAddress = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Button_CopyRangeValue);
            this.panel1.Controls.Add(this.Button_Down_CheckedListBoxItem);
            this.panel1.Controls.Add(this.Button_GetActiveCell);
            this.panel1.Controls.Add(this.Button_Up_CheckedListBoxItem);
            this.panel1.Controls.Add(this.Button_CloseWorkbook);
            this.panel1.Controls.Add(this.Button_UpdateList);
            this.panel1.Controls.Add(this.Button_Exit);
            this.panel1.Controls.Add(this.Button_SelectCells);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.Button_RegistAddress);
            this.panel1.Controls.Add(this.checkedListBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(787, 398);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            // 
            // Button_CopyRangeValue
            // 
            this.Button_CopyRangeValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button_CopyRangeValue.Location = new System.Drawing.Point(401, 367);
            this.Button_CopyRangeValue.Name = "Button_CopyRangeValue";
            this.Button_CopyRangeValue.Size = new System.Drawing.Size(125, 23);
            this.Button_CopyRangeValue.TabIndex = 6;
            this.Button_CopyRangeValue.Text = "コピー";
            this.Button_CopyRangeValue.UseVisualStyleBackColor = true;
            this.Button_CopyRangeValue.Click += new System.EventHandler(this.Button_CopyRangeValue_Click);
            // 
            // Button_Down_CheckedListBoxItem
            // 
            this.Button_Down_CheckedListBoxItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Down_CheckedListBoxItem.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Button_Down_CheckedListBoxItem.Location = new System.Drawing.Point(752, 234);
            this.Button_Down_CheckedListBoxItem.Name = "Button_Down_CheckedListBoxItem";
            this.Button_Down_CheckedListBoxItem.Size = new System.Drawing.Size(27, 74);
            this.Button_Down_CheckedListBoxItem.TabIndex = 3;
            this.Button_Down_CheckedListBoxItem.Text = "↓";
            this.Button_Down_CheckedListBoxItem.UseVisualStyleBackColor = true;
            this.Button_Down_CheckedListBoxItem.Click += new System.EventHandler(this.Button_Down_CheckedListBoxItem_Click);
            // 
            // Button_GetActiveCell
            // 
            this.Button_GetActiveCell.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button_GetActiveCell.Location = new System.Drawing.Point(270, 367);
            this.Button_GetActiveCell.Name = "Button_GetActiveCell";
            this.Button_GetActiveCell.Size = new System.Drawing.Size(125, 23);
            this.Button_GetActiveCell.TabIndex = 2;
            this.Button_GetActiveCell.Text = "ActiveCell表示";
            this.Button_GetActiveCell.UseVisualStyleBackColor = true;
            this.Button_GetActiveCell.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Button_Up_CheckedListBoxItem
            // 
            this.Button_Up_CheckedListBoxItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Up_CheckedListBoxItem.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Button_Up_CheckedListBoxItem.Location = new System.Drawing.Point(752, 151);
            this.Button_Up_CheckedListBoxItem.Name = "Button_Up_CheckedListBoxItem";
            this.Button_Up_CheckedListBoxItem.Size = new System.Drawing.Size(27, 77);
            this.Button_Up_CheckedListBoxItem.TabIndex = 2;
            this.Button_Up_CheckedListBoxItem.Text = "↑";
            this.Button_Up_CheckedListBoxItem.UseVisualStyleBackColor = true;
            this.Button_Up_CheckedListBoxItem.Click += new System.EventHandler(this.Button_Up_CheckedListBoxItem_Click);
            // 
            // Button_CloseWorkbook
            // 
            this.Button_CloseWorkbook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_CloseWorkbook.Location = new System.Drawing.Point(613, 41);
            this.Button_CloseWorkbook.Name = "Button_CloseWorkbook";
            this.Button_CloseWorkbook.Size = new System.Drawing.Size(125, 23);
            this.Button_CloseWorkbook.TabIndex = 1;
            this.Button_CloseWorkbook.Text = "ブックを閉じる";
            this.Button_CloseWorkbook.UseVisualStyleBackColor = true;
            this.Button_CloseWorkbook.Click += new System.EventHandler(this.Button_CloseWorkbook_Click);
            // 
            // Button_UpdateList
            // 
            this.Button_UpdateList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_UpdateList.Location = new System.Drawing.Point(613, 12);
            this.Button_UpdateList.Name = "Button_UpdateList";
            this.Button_UpdateList.Size = new System.Drawing.Size(125, 23);
            this.Button_UpdateList.TabIndex = 5;
            this.Button_UpdateList.Text = "リスト更新";
            this.Button_UpdateList.UseVisualStyleBackColor = true;
            this.Button_UpdateList.Click += new System.EventHandler(this.Button_UpdateList_Click);
            // 
            // Button_Exit
            // 
            this.Button_Exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button_Exit.Location = new System.Drawing.Point(621, 367);
            this.Button_Exit.Name = "Button_Exit";
            this.Button_Exit.Size = new System.Drawing.Size(125, 23);
            this.Button_Exit.TabIndex = 4;
            this.Button_Exit.Text = "終了";
            this.Button_Exit.UseVisualStyleBackColor = true;
            this.Button_Exit.Click += new System.EventHandler(this.Button_Exit_Click);
            // 
            // Button_SelectCells
            // 
            this.Button_SelectCells.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button_SelectCells.Location = new System.Drawing.Point(139, 367);
            this.Button_SelectCells.Name = "Button_SelectCells";
            this.Button_SelectCells.Size = new System.Drawing.Size(125, 23);
            this.Button_SelectCells.TabIndex = 3;
            this.Button_SelectCells.Text = "選択する";
            this.Button_SelectCells.UseVisualStyleBackColor = true;
            this.Button_SelectCells.Click += new System.EventHandler(this.Button_SelectCells_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 120);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(734, 241);
            this.dataGridView1.TabIndex = 2;
            // 
            // Button_RegistAddress
            // 
            this.Button_RegistAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Button_RegistAddress.Location = new System.Drawing.Point(8, 367);
            this.Button_RegistAddress.Name = "Button_RegistAddress";
            this.Button_RegistAddress.Size = new System.Drawing.Size(125, 23);
            this.Button_RegistAddress.TabIndex = 1;
            this.Button_RegistAddress.Text = "アドレスを登録";
            this.Button_RegistAddress.UseVisualStyleBackColor = true;
            this.Button_RegistAddress.Click += new System.EventHandler(this.Button_RegistAddress_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 12);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(595, 102);
            this.checkedListBox1.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 398);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(787, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.StatusStrip1_ItemClicked);
            // 
            // statusStrip2
            // 
            this.statusStrip2.Location = new System.Drawing.Point(0, 398);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(787, 22);
            this.statusStrip2.TabIndex = 2;
            this.statusStrip2.Text = "statusStrip2";
            this.statusStrip2.Visible = false;
            this.statusStrip2.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.StatusStrip2_ItemClicked);
            // 
            // ExcelCellsManagerForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 420);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip2);
            this.Controls.Add(this.statusStrip1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(799, 459);
            this.Name = "ExcelCellsManagerForm";
            this.Text = "Excel Cells Manager2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExcellCellsManagerForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExcelCellsManagerForm_FormClosed);
            this.Load += new System.EventHandler(this.ExcelCellsManagerForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ExcelCellsManagerForm_KeyDown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Button_UpdateList;
        private System.Windows.Forms.Button Button_Exit;
        private System.Windows.Forms.Button Button_SelectCells;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button Button_RegistAddress;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button Button_CloseWorkbook;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button Button_GetActiveCell;
        private System.Windows.Forms.Button Button_Up_CheckedListBoxItem;
        private System.Windows.Forms.Button Button_Down_CheckedListBoxItem;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.Button Button_CopyRangeValue;
    }
}

