
using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelWorkbookList
{
    public class CheckedListBoxEvent
    {
        protected ErrorManager.ErrorManager _error;
        protected CheckedListBox _checkedListBox;
        protected IWorkbookListEvents _workbookListEvents;
        protected event EventHandler _itemMouseRightDoubleClickedAfterEvent;
        protected event KeyEventHandler _itemKeyDownEAfterEvent;
        

        public CheckedListBoxEvent(ErrorManager.ErrorManager error,CheckedListBox checkedListBox)
        {
            try
            {
                _error = error;
                _checkedListBox = checkedListBox;
                _checkedListBox.MouseDoubleClick += CheckedListBox_MouseClick;

                
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".CheckedListBoxEvent Constracta");
            }
        }
        public CheckedListBoxEvent(
            ErrorManager.ErrorManager error, CheckedListBox checkedListBox,IWorkbookListEvents workbookListEvent)
        {
            try
            {
                _error = error;
                _checkedListBox = checkedListBox;
                _checkedListBox.MouseClick += CheckedListBox_MouseClick;
                _checkedListBox.KeyDown += CheckedListBox_KeyDown;
                _workbookListEvents = workbookListEvent;
                _itemMouseRightDoubleClickedAfterEvent = _workbookListEvents.CheckedListBoxItemMouseRightClickAfterEvent;
                _itemKeyDownEAfterEvent = _workbookListEvents.CheckedListBoxKeyDownAfterEvent;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".CheckedListBoxEvent Constracta");
            }
        }

        private void CheckedListBox_KeyDown(object sender,KeyEventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString() + ".CheckedListBox_KeyDown");

                _itemKeyDownEAfterEvent?.Invoke(sender, e);

            } catch (Exception ex)
            {
                _error.AddException(ex, ToString()+ ".CheckedListBox_PreviewKeyDown");
            }
        }

        private void CheckedListBox_MouseClick(object sender,MouseEventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".CheckedListBox_MouseClick");
                if (e.Button == MouseButtons.Right)
                {
                    _error.AddLog("MouseButtons.Right");
                    string value = _checkedListBox.SelectedItem.ToString();

                    _itemMouseRightDoubleClickedAfterEvent?.Invoke(value, EventArgs.Empty);
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "CheckedListBox_MouseClick");
            }
        }

    }
}
