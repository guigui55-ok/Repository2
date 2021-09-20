
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelCellsManager.Conrolx
{
    public interface IDataGridViewItem
    {
        void SetDataToMemberFromDataGridView(DataGridView dataGrid, int row);
        void CopyAndPasteRowData(DataGridView dataGrid, int copyRow, int pasteRow);
        void SetDataToDataGridView(DataGridView dataGrid, object Data, int setRow);
        object GetData();
    }
}
