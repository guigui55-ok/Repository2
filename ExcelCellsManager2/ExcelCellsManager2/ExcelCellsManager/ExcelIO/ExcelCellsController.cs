using Microsoft.Office.Interop.Excel;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ExcelIO
{
    public class ExcelCellsController
    {
        protected ErrorManager.ErrorManager _error;

        public ExcelCellsController(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public void Copy(in Microsoft.Office.Interop.Excel.Application application,string bookName,string sheetName,string address)
        {
            try
            {
                _error.AddLog(this, "Copy");
                _error.AddLog("  ThreadId = " + Thread.CurrentThread.ManagedThreadId);
                // application,bookName,sheetName,address チェック済み
                ((Worksheet)application.Workbooks[bookName].Sheets[sheetName]).Range[address].Copy();
                ExcelCells cells = new ExcelCells(_error);
                //string buf = cells.GetValue(application, bookName, sheetName, address);
                //if (buf != "")
                //{
                //    //Clipboard.SetDataObject(((Worksheet)application.Workbooks[bookName].Sheets[sheetName]).Range[address]);
                //}
                //Clipboard.SetText(buf);
                //_error.AddLog("ClipBoad.GetText = " + Clipboard.GetText());
            }
            catch (System.Threading.ThreadStateException ex)
            {
                _error.AddLogWarning(this.ToString(), "Excute  ThreadStateException", ex);
            }
            catch (Exception ex)
            {
                _error.AddException(ex,this, "Copy");
            }
        }
    }
}
