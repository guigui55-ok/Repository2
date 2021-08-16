
using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelWorkbookList
{
    public class WorkbookListEvents : IWorkbookListEvents
    {
        protected ErrorManager.ErrorManager _error;
        private event EventHandler _checkedListBoxItemMouseRightClickAfterEvent;
        private event KeyEventHandler _checkedListBoxKeyDownAfterEvent;
        public ExcelCellsManagerMain ExcelCellsManagerMain;
        public IWorkbookListControl _workbookListControl;

        public EventHandler CheckedListBoxItemMouseRightClickAfterEvent {
            get => _checkedListBoxItemMouseRightClickAfterEvent; set => _checkedListBoxItemMouseRightClickAfterEvent = value; }
        public KeyEventHandler CheckedListBoxKeyDownAfterEvent {
            get => _checkedListBoxKeyDownAfterEvent; set => _checkedListBoxKeyDownAfterEvent = value; }

        public WorkbookListEvents(ErrorManager.ErrorManager error, IWorkbookListControl workbookListControl, ExcelCellsManagerMain excelCellsManagerMain)
        {
            _error = error;
            this.ExcelCellsManagerMain = excelCellsManagerMain;
            _workbookListControl = workbookListControl;

            _checkedListBoxItemMouseRightClickAfterEvent += WorkbookList_CheckedListBoxItemMouseRightClickAfterEvent;
            _checkedListBoxKeyDownAfterEvent += WorkbookList_CheckedListBoxKeyDownAfterEvent;
        }

        private void WorkbookList_CheckedListBoxKeyDownAfterEvent(object sender,KeyEventArgs e)
        {
            try
            {
                switch (e.KeyData)
                {
                    case Keys.Control | Keys.S:
                        _error.AddLog("_form_KeyDown :  Keys.Control | Keys.S");
                        break;
                    case Keys.Enter:
                        _error.AddLog("_form_KeyDown :  Enter");
                        // get selected checkedListBox
                        this.ExcelCellsManagerMain.ActivateWorkbookSelectedCheckedListBox();
                        break;
                    default:
                        _error.AddLog("_form_KeyDown : " + e.KeyData.ToString());
                        break;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".WorkbookList_CheckedListBoxKeyDownAfterEvent");
            }
        }

        private void WorkbookList_CheckedListBoxItemMouseRightClickAfterEvent(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".WorkbookList_CheckedListBoxItemMouseRightClickAfterEvent");

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".WorkbookList_CheckedListBoxItemMouseRightClickAfterEvent");
            }
        }
    }
}
