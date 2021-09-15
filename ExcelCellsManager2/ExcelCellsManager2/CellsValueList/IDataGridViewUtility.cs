using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    public interface IDataGridViewUtility
    {
        EventHandler DataGridView_InsertRow { get; set; }
        EventHandler DataGridView_DeleteRow { get; set; }
        EventHandler DataGridView_AddRow { get; set; }
        EventHandler DataGridView_ChangeValue { get; set; }
        bool IsInitialize { get; }
        IDataGridViewValueConverter DataGridViewValueConverter { get; }

        void Initialize(string[] ColumnNameList, object value);
        void Initialize(string[] ColumnNameList);
        void SelectRow(int row);
        void SetColumnWidth(int[] columnsWidthList);
        void DeleteLine(DataGridView dataGridView, int targetRow);
        void AddValue(object value);
        void ResetValue(List<string[]> values);
        int GetCurrentRow();
        string[] GetCurrentRowData();
        string[] GetRowData(int row);
        void MoveItemUpInList();
        void MoveItemDownInList();
        string GetAllData(string delimiter);
    }
}
