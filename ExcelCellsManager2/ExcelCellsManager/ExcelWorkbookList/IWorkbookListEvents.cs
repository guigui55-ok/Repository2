using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelWorkbookList
{
    public interface IWorkbookListEvents
    {

        EventHandler CheckedListBoxItemMouseRightClickAfterEvent { get; set; }
        KeyEventHandler CheckedListBoxKeyDownAfterEvent { get; set; }
    }
}
