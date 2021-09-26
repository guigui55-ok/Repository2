using System.Collections.Generic;

namespace ExcelWorkbookList
{
    public interface IWorkbookListControl
    {
        void SetWorkbookList(List<string> list);

        List<int> GetIndexListSelectedItem();
        void ClearItems();
        string GetItemValue(int index);
        ExcelUtility.AppsInfo ConvertToAppsInfoFromItemValue(string value);
        List<string> GetValueList();
        string[] GetSelectedItems();
    }
}
