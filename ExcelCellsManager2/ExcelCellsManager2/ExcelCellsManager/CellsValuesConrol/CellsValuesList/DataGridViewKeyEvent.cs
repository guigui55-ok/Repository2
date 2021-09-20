using System;
using System.Windows.Forms;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    public class DataGridViewKeyEvent
    {
        protected ErrorManager.ErrorManager _error;
        protected DataGridView _dataGridView;
        protected IDataGridViewUtility _dataGridViewUtility;
        public DataGridViewKeyEvent(ErrorManager.ErrorManager error,IDataGridViewUtility dataGridViewUtility,DataGridView dataGrid)
        {
            _error = error;
            _dataGridView = dataGrid;
            _dataGridViewUtility = dataGridViewUtility;
            _dataGridView.KeyDown += DataGrid_KeyDown;
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int row = 0;
                switch (e.KeyData)
                {
                    case Keys.Delete:
                        row = _dataGridViewUtility.GetCurrentRow();
                        if (_error.hasAlert) {
                            _error.AddLogAlert("DataGrid_KeyDown.Delete", "GetCurrentRow Failed");return; 
                        }
                        _dataGridViewUtility.DeleteLine(_dataGridView, row);
                        if (_error.hasAlert) { 
                            _error.AddLogAlert("DataGrid_KeyDown.Delete", "DeleteLine Failed");return; 
                        }
                        break;
                    case Keys.Control | Keys.C:
                        //row = _dataGridViewUtility.GetCurrentRow();
                        //if (_error.hasAlert) { _error.AddLogAlert("DataGrid_KeyDown.Delete", "GetCurrentRow Failed"); return; }

                        break;
                    default:
                        break;
                }
                if (_error.hasAlert) { _error.Messenger.ShowUserMessageOnly(); }
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString() + ".DataGrid_KeyDown");
                _error.ClearError();
            }
            finally
            {
            }
        }
    }
}
