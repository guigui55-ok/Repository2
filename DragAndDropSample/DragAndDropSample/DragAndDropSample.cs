using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragAndDropSample
{
    public partial class DragAndDropSample : Form
    {
        protected ErrorManager.ErrorManager _err;
        protected DragAndDropOnControl _dragAndDropOnControl;
        protected DragAndDropForFile _dragAndDropForFile;
        public DragAndDropSample()
        {
            InitializeComponent();
            _err = new ErrorManager.ErrorManager(1);
            _dragAndDropOnControl = new DragAndDropOnControl(_err, this);
            _dragAndDropOnControl.AddRecieveControls(new Control[] { richTextBox1});
            _dragAndDropForFile = new DragAndDropForFile(_err, _dragAndDropOnControl);
            _dragAndDropForFile.DragAndDropEventAfterEventForFile += DragAndDropEventAfterEventForFile;
        }

        private void DragAndDropEventAfterEventForFile(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this, "DragAndDropEventAfterEventForFile");
                if(_dragAndDropForFile.Files == null) { _err.AddLogWarning("Files == null"); return; }
                if (_dragAndDropForFile.Files.Length < 1) { _err.AddLogWarning("Files.Length < 1"); return; }

                _err.AddLog("  GetPath="+ _dragAndDropForFile.Files[0]);
                this.richTextBox1.AppendText(_dragAndDropForFile.Files[0] + "\n");
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "DragAndDropEventAfterEventForFile");
                _err.ClearError();
            }
        }

        private void DragAndDropSample_Load(object sender, EventArgs e)
        {

        }
    }
}
