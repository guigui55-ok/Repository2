using System;
using System.Collections.Generic;

namespace ExcelCellsManager.ExcelWorkbookList
{
    public interface IWorkbookListControl
    {
        ErrorCode ErrorCode { get; }
        ErrorMessage ErrorMessage { get; }
        void UpdateItemListAfterClearList(List<string> addList);
        List<string> GetListAll();
        List<string> GetValueListOfAll();
        List<int> GetIndexListChecked();
        List<string[]> GetAppsInfoListFromCheckdItemList();
        string[] GetSelectedItem();
        int GetIndexForExcelAppsFromCheckedListBox(string bookName);

    }
}
