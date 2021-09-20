using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelWorkbookList
{
    public class WorkbookList
    {
        protected ErrorManager.ErrorManager _error;
        public IWorkbookListControl Control;
        protected CheckedListBoxEvent _checkedListBoxEvent;
        protected IWorkbookListEvents _workbookListEvents;
        public WorkbookList(ErrorManager.ErrorManager error,
            Control contorl,IWorkbookListControl workbookListControl,ExcelCellsManagerMain excelCellsManagerMain)
        {
            try
            {
                _error = error;
                if (contorl.GetType() == typeof(CheckedListBox))
                {
                    if (workbookListControl == null)
                    {
                        Control = new WorkbookListCheckedListBoxControl(_error, (CheckedListBox)contorl);
                    } else
                    {
                        Control = workbookListControl;
                    }
                    _workbookListEvents = new WorkbookListEvents(_error,workbookListControl, excelCellsManagerMain);
                    _checkedListBoxEvent = new CheckedListBoxEvent(_error, (CheckedListBox)contorl, _workbookListEvents);
                } else
                {
                    throw new Exception("Control Type Is Not CheckedListBox");
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".WorkbookList Constracta Failed");
            }            
        }
    }
}
