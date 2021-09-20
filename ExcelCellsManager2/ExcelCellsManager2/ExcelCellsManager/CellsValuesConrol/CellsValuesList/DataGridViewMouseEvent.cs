using System;
using System.Windows.Forms;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    class DataGridViewMouseEvent
    {
        protected ErrorManager.ErrorManager _error;
        protected DataGridView _dataGridView;
        protected IDataGridViewUtility _dataGridViewUtility;
        public DataGridViewMouseEvent(ErrorManager.ErrorManager error, IDataGridViewUtility dataGridViewUtility, DataGridView dataGrid)
        {
            _error = error;
            _dataGridView = dataGrid;
            _dataGridViewUtility = dataGridViewUtility;
            _dataGridView.RowHeaderMouseDoubleClick += DataGridView_RowHeaderMouseDoubleClick;
        }

        private void DataGridView_RowHeaderMouseDoubleClick(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString()+ "._dataGridView_RowHeaderMouseDoubleClick");
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._dataGridView_RowHeaderMouseDoubleClick");
                _error.ClearError();
            }
        }
    }
}
