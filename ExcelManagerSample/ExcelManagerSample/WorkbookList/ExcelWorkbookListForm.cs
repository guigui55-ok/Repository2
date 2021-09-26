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
            _excelManager.IsAlwaysOpenExcelApplication = false;
            _workbookListControl = new WorkbookListCheckedListBox(_err,this,checkedListBox1);
            _excelWorkbookList = new ExcelWorkbookList(_err,_excelManager,_workbookListControl);
            _excelManager.IsSyncExcelWorkbook = true;
            _excelManager.IsAlwaysOpenExcelApplication = true;
            // IsSyncExcelWorkbook=true で以下例外が発生する
            // 有効ではないスレッド間の操作: コントロールが作成されたスレッド以外のスレッドからコントロール 'checkedListBox1' がアクセスされました。
            _excelManager.ExcelApplicationRunWhenNothing(true);
            UpdateExcelAppsList();
            this.Paint += ExcelWorkbookListForm_Paint;
        }

        private void ExcelWorkbookListForm_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                //_err.AddLog(this,"Paint");
                //if ((_excelManager.IsWorkbookOpened) || (_excelManager.IsWorkbookClosed))
                //{
                //    _err.AddLog(this, "ExcelWorkbookListForm_Paint");
                //    _err.AddLog("  IsWorkbookOpened || IsWorkbookClosed = true");
                //    _excelManager.UpdateOpendExcelApplication();
                //}
            } catch(Exception ex)
            {
                _err.AddLogAlert(this, "Event ExcelWorkbookListForm_Paint", "ExcelWorkbookListForm_Paint Failed",ex);
            }
        }

        private void UpdateExcelAppsList()
        {
            _excelManager.UpdateOpendExcelApplication();
            if (_err.hasAlert)
            {
                _err.AddLogAlert("_excelManager.UpdateOpendExcelApplication Failed");
                Console.WriteLine(_err.GetMesseges());
                _err.ClearError();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _err.ClearError();
            _workbookListControl.ClearItems();
            if (_err.hasAlert) { _err.AddLogAlert("_workbookListControl.ClearItems Failed"); }
            UpdateExcelAppsList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _err.ClearError();
            _excelWorkbookList.CloseWorkbookSelectedItems();
            if (_err.hasAlert) { 
                _err.AddLogAlert("_workbookListControl.CloseWorkbookSelectedItems Failed");
                Console.WriteLine(_err.GetMesseges());
            }
            UpdateExcelAppsList();
        }

        private void ExcelWorkbookListForm_Load(object sender, EventArgs e)
        {

        }
    }
}
