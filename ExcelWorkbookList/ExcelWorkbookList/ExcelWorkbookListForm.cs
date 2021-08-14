using ExcelUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelWorkbookList
{
    public partial class ExcelWorkbookListForm : Form
    {
        protected ErrorManager.ErrorManager _err;
        protected ExcelManager _excelManager;
        protected IWorkbookListControl _workbookListControl;
        protected ExcelWorkbookList _excelWorkbookList;
        public ExcelWorkbookListForm()
        {
            InitializeComponent();
            _err = new ErrorManager.ErrorManager(1);
            _excelManager = new ExcelManager(_err);
            _workbookListControl = new WorkbookListCheckedListBox(_err,this.checkedListBox1);
            _excelWorkbookList = new ExcelWorkbookList(_err,_excelManager,_workbookListControl);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _err.ClearError();
            _workbookListControl.ClearItems();
            if (_err.hasAlert) { _err.AddLogAlert("_workbookListControl.ClearItems Failed"); }
            _excelManager.UpdateOpendExcelApplication();
            if (_err.hasAlert) { 
                _err.AddLogAlert("_excelManager.UpdateOpendExcelApplication Failed");
                Console.WriteLine(_err.GetMesseges());
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _err.ClearError();
            _excelWorkbookList.CloseWorkbookSelectedItems();
            if (_err.hasAlert) { 
                _err.AddLogAlert("_workbookListControl.CloseWorkbookSelectedItems Failed");
                Console.WriteLine(_err.GetMesseges());
            }
        }
    }
}
