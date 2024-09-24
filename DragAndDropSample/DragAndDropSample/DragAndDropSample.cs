using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace DragAndDropSample
{
    public partial class DragAndDropSample : Form
    {
        public AppLogger _logger;
        protected DragAndDropOnControl _dragAndDropOnControl;
        protected DragAndDropForFile _dragAndDropForFile;
        //
        public DragAndDropOnControl _dragAndDropOnControlB;
        public DragAndDropSample()
        {
            InitializeComponent();
            _logger = new AppLogger();
            _dragAndDropOnControl = new DragAndDropOnControl(_logger, this);
            _dragAndDropOnControl.AddRecieveControls(new Control[] { richTextBox1});
            _dragAndDropForFile = new DragAndDropForFile(_logger, _dragAndDropOnControl);
            _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
            //
            //240921 追加（テスト用）
            //button1.AllowDrop = true;//これはクラス内部で処理される
            _dragAndDropOnControlB = new DragAndDropOnControl(_logger, button1); //RecieveするControl
            //_dragAndDropOnControlB.AddRecieveControls(new Control[] { button1 }); //リストでなくてもよい
            _dragAndDropOnControlB.AddRecieveControl( button1);
            DragAndDropForFile _dragAndDropForFileB = new DragAndDropForFile(_logger, _dragAndDropOnControlB);
            _dragAndDropForFileB.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForButton;
            _dragAndDropOnControlB._dragAndDropForFile = _dragAndDropForFileB;

        }

        private void DragAndDropEventAfterEventForButton(object sender, EventArgs e)
        {
            try
            {
                DragAndDropForFile dragAndDropForFileB = _dragAndDropOnControlB._dragAndDropForFile;
                _logger.AddLog(this, "DragAndDropEventAfterEventForButton");
                if (dragAndDropForFileB.Files == null) { _logger.AddLogWarning("Files == null"); return; }
                if (dragAndDropForFileB.Files.Length < 1) { _logger.AddLogWarning("Files.Length < 1"); return; }

                _logger.AddLog("  GetPath=" + dragAndDropForFileB.Files[0]);
                //ファイル名をボタンTextに出力
                string buf = Path.GetFileName(dragAndDropForFileB.Files[0]);
                button1.Text = buf;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "DragAndDropEventAfterEventForFile");
                _logger.ClearError();
            }
        }

        private void DragAndDropEventAfterEventForFile(object sender, EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "DragAndDropEventAfterEventForFile");
                if(_dragAndDropForFile.Files == null) { _logger.AddLogWarning("Files == null"); return; }
                if (_dragAndDropForFile.Files.Length < 1) { _logger.AddLogWarning("Files.Length < 1"); return; }

                _logger.AddLog("  GetPath="+ _dragAndDropForFile.Files[0]);
                this.richTextBox1.AppendText(_dragAndDropForFile.Files[0] + "\n");
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "DragAndDropEventAfterEventForFile");
                _logger.ClearError();
            }
        }

        private void DragAndDropSample_Load(object sender, EventArgs e)
        {

        }
    }
}
