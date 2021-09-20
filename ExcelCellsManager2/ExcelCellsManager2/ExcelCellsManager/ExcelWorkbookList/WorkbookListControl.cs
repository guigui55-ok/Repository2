using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelWorkbookList
{
    public class WorkbookListCheckedListBoxControl : IWorkbookListControl
    {
        protected ErrorManager.ErrorManager _error;
        protected CheckedListBox _checkedListBox;
        public WorkbookListCheckedListBoxControl(ErrorManager.ErrorManager error,CheckedListBox checkedListBox)
        {
            _error = error;
            _checkedListBox = checkedListBox;
        }

        public ErrorCode ErrorCode => throw new NotImplementedException();

        public ErrorMessage ErrorMessage => throw new NotImplementedException();

        public List<string[]> GetAppsInfoListFromCheckdItemList()
        {
            throw new NotImplementedException();
        }

        public int GetIndexForExcelAppsFromCheckedListBox(string bookName)
        {
            throw new NotImplementedException();
        }

        public List<int> GetIndexListChecked()
        {
            throw new NotImplementedException();
        }

        public List<string> GetListAll()
        {
            throw new NotImplementedException();
        }

        public string[] GetSelectedItem()
        {
            throw new NotImplementedException();
        }

        public List<string> GetValueListOfAll()
        {
            throw new NotImplementedException();
        }

        public void UpdateItemListAfterClearList(List<string> addList)
        {
            throw new NotImplementedException();
        }
    }
}
