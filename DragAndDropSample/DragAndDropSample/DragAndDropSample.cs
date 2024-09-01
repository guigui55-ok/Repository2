using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public DragAndDropSample()
        {
            InitializeComponent();
            _logger = new AppLogger();
            _dragAndDropOnControl = new DragAndDropOnControl(_logger, this);
            _dragAndDropOnControl.AddRecieveControls(new Control[] { richTextBox1});
            _dragAndDropForFile = new DragAndDropForFile(_logger, _dragAndDropOnControl);
            _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
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
