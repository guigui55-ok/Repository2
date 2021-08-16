using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    public class DataGridViewFormEventCellsManager
    {
        protected ErrorManager.ErrorManager _error;
        protected DataGridView _dataGridView;
        protected IDataGridViewUtility _dataGridViewUtility;
        protected ExcelCellsManagerMain _excelCellsManagerMain;

        //public event EventHandler ColumnWidthChanged;
        public DataGridViewFormEventCellsManager(ErrorManager.ErrorManager error, 
            ExcelCellsManagerMain excelCellsManagerMain, IDataGridViewUtility dataGridViewUtility, DataGridView dataGridview)
        {
            _error = error;
            _dataGridView = dataGridview;
            _dataGridViewUtility = dataGridViewUtility;
            _dataGridView.ColumnWidthChanged += _dataGridView_ColumnWidthChanged;
            _dataGridView.KeyDown += _dataGridView_KeyDown;
            _dataGridView.RowHeaderMouseClick += _dataGridView_RowHeaderMouseClick;
            _excelCellsManagerMain = excelCellsManagerMain;
        }

        //private int GetClickedRow(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    try
        //    {
        //        // *** クリックしたセルを取得する ***

        //        DataGridView.HitTestInfo info = ((DataGridView)sender).HitTest(e.X, e.Y);

        //        if (info != null)
        //        {
        //            _error.AddLog("  info.RowIndex = " + info.RowIndex);
        //            _error.AddLog("  info.ColumnIndex = " + info.ColumnIndex);
        //            return info.RowIndex;
        //        }
        //        else
        //        {
        //            _error.AddLog("  Clicked Place is Not Cell");
        //        }

        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        _error.AddException(ex, this, "GetClickedRow");
        //        return 0;
        //    }
        //}

        private void _dataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    // Select して Copy する
                    _error.AddLog(this, "_dataGridView_RowHeaderMouseClick Right Clicked");
                    _dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                    //_dataGridView.Rows[e.RowIndex].Selected = true;
                    int row = e.RowIndex;
                    _error.AddLog(" Selected row="+row);
                    _dataGridViewUtility.SelectRow(row);
                    if (_error.hasAlert) { 
                        //_error.AddLogAlert("SelectRow");
                        throw new Exception("SelectRow Failed");
                    }
                    else
                    {
                        _dataGridView.Rows[row].Selected = true;
                        _excelCellsManagerMain.CopyCellsValue(false);
                        if (_error.hasAlert) { _error.AddLogAlert("CopyCellsValue"); }
                        if (_error.hasAboveWarning) { _error.Messenger.ShowAlertMessages(); }
                    }
                }
            } catch (Exception ex)
            {
                _error.AddLogAlert(this, "_dataGridView_RowHeaderMouseClick", "", ex);
            }
        }

        private void _dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                _error.AddLog(this, "_dataGridView_KeyDown");
                switch (e.KeyData)
                {
                    //case Keys.Control | Keys.C:
                    case Keys.F1:
                        _excelCellsManagerMain.CopyCellsValue(false);
                        if (_error.hasAlert) { _error.AddLogAlert("CopyCellsValue"); }
                        break;
                    case Keys.F2:
                        Console.WriteLine("Clipboad.GetText = "+ Clipboard.GetText());
                        Console.WriteLine("Clipboad.Get = " + Clipboard.GetDataObject().ToString());
                        break;
                    default:
                        break;
                }
                if (_error.hasAboveWarning) { _error.Messenger.ShowAlertMessages(); }
            } catch (Exception ex)
            {
                _error.AddLogAlert(this, "_dataGridView_KeyDown", "",ex);
            } finally
            {
                _error.AddLog("ClipBoad.GetText = " + Clipboard.GetText());
            }
        }

        private void _dataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                string methodName = this.ToString() + "._dataGridView_ColumnWidthChanged ";
                _error.AddLog(methodName);
                if (_excelCellsManagerMain.AppsSettings.IsSaveColumnWidthOfDataList)
                {
                    if (_dataGridView == null) { _error.AddLog(methodName + " datagridview is null"); }
                    if (_dataGridView.Columns.Count < 1) { _error.AddLog(methodName + " dataGridView.Columns.Count < 1"); }
                    List<int> columnsWidth = new List<int>();
                    foreach (DataGridViewColumn val in _dataGridView.Columns)
                    {
                        columnsWidth.Add(val.Width);
                    }
                    _excelCellsManagerMain.AppsSettings.ColumnWidthOfDataList = columnsWidth.ToArray();
                }

            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString() + "._dataGridView_ColumnWidthChanged");
                _error.ClearError();
            }
        }
    }
}
