using System;
using System.Windows.Forms;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    public class DataGridViewMouseEventForCellsManager
    {
        protected ErrorManager.ErrorManager _error;
        protected DataGridView _dataGridView;
        protected IDataGridViewUtility _dataGridViewUtility;
        protected ExcelCellsManagerMain _excelCellsManagerMain;
        public DataGridViewMouseEventForCellsManager(ErrorManager.ErrorManager error, 
            ExcelCellsManagerMain excelCellsManagerMain, IDataGridViewUtility dataGridViewUtility, DataGridView dataGrid)
        {
            _error = error;
            _excelCellsManagerMain = excelCellsManagerMain;
            _dataGridView = dataGrid;
            _dataGridViewUtility = dataGridViewUtility;
            _dataGridView.RowHeaderMouseDoubleClick += DataGridView_RowHeaderMouseDoubleClick;
        }

        // RowHeader ダブルクリック
        private void DataGridView_RowHeaderMouseDoubleClick(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString()+ "._dataGridView_RowHeaderMouseDoubleClick");
                _excelCellsManagerMain.SelectCells();
                _excelCellsManagerMain.ActivateWorkbookWindowActivate();
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._dataGridView_RowHeaderMouseDoubleClick");
                _error.ClearError();
            }
        }
    }
}
