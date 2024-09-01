using AppLoggerModule;
using ControlUtility.SelectFiles;
using CotnrolUtility.SelectFiles;
using ErrorManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectSingleFileSample
{
    public partial class SelectSingleFileSample : Form
    {
        public AppLogger _logger;
        ErrorManager.ErrorManager _err = null;
        IFiles files;
        FileListManager fileListManager;
        SelectFileByDragDrop fileDragDrop;
        List<string> list;
        public SelectSingleFileSample()
        {
            this._logger = new AppLogger();
            this._logger.SetDefaultValues();
            _err = new ErrorManager.ErrorManager(1);
            InitializeComponent();
        }

        private void SelectSingleFileSample_Load(object sender, EventArgs e)
        {

            _err = new ErrorManager.ErrorManager(1);
            // DragDrop をするためのクラス
            this.AllowDrop = true;
            fileDragDrop = new SelectFileByDragDrop(this._logger, _err, this);
            // Control を追加する
            this.richTextBox1.AllowDrop = true;
            fileDragDrop.AddRecieveControl(this.richTextBox1);
            // ファイルリストを扱うクラス
            list = new List<string>();
            files = new SingleFile(this._logger, _err, list);
            // ファイルリストを登録するクラス
            fileListManager = new FileListManager(this._logger, _err, files);
            // FileDragDrop イベントを紐づけする
            this.DragDrop += fileListManager.RegistFileByDragDrop;
            this.richTextBox1.DragDrop += fileListManager.RegistFileByDragDrop;
            // ファイルリストを取得した後にコントロールなどに表示する場合、以下に紐づける
            fileListManager.UpdateFileListAfterEvent += AddValueToControlFromUpdateList;

            // FileListManager settings
            fileListManager.IsReadMatchFirstOnly = true;
            fileListManager.IsReadSourceOfShotcut = false;
            fileListManager.ReadFolderHierarchy = 0;
        }
        public void AddValueToControlFromUpdateList(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "AddValueToControlFromUpdateList_Main");
                // ファイルリストを更新した後に実行する
                // コントロールにファイルリストを表示する
                if (sender.GetType().Equals(typeof(string[])))
                {
                    string buf = "";
                    //buf += "" + "----------------\n";
                    string[] ary = (string[])sender;
                    if (ary.Length < 1) { buf += "\n" + "ary.Length < 1"; }
                    foreach (string val in ary)
                    {
                        buf += val + "\n";
                        //richTextBox1.AppendText(val);
                    }
                    buf = buf.Substring(1, buf.Length - 1); // 最後の1文字\nを消す
                    richTextBox1.AppendText(buf);
                }
                else
                {
                    _logger.AddLogAlert(this, "  sender is Invalid Type");
                }

            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "AddValueToControlFromUpdateList_Main Failed");
            }
        }
        public void ForJumpSource() { }
    }
}
