using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelWorkbookList
{
    public interface IWorkbookListControl
    {
        void SetWorkbookList(List<string> list);

        List<int> GetIndexListSelectedItem();
        void ClearItems();
    }
}
