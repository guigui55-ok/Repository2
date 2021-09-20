
using System.Collections.Generic;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    public interface IDataGridViewValueConverter
    {
        List<object> ConvertObjectListFromStringList(List<string> list, char delimiter);
        List<string[]> ConvertArrayStringListFromStringList(List<string> list, char delimiter);
    }
}
