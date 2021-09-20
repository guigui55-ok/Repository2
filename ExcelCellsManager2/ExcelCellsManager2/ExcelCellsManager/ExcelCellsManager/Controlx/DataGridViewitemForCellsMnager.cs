using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelCellsManager.Conrolx
{
    public class DataGridViewitemForCellsMnager : IDataGridViewItem
    {
        protected ErrorManager.ErrorManager _error;
        protected ExcelCellsInfo _excelCellsInfo;
        public DataGridViewitemForCellsMnager(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public object GetData()
        {
            return _excelCellsInfo;
        }

        public void SetDataToMemberFromDataGridView(DataGridView dataGrid, int row)
        {
            try
            {
                if(dataGrid == null) { throw new Exception("DataGridView is Null"); }
                Utility.ClassInfo info = new Utility.ClassInfo(_error);
                string[] fieldNames = info.GetMemberFeildsNameList(typeof(ExcelCellsInfo));
                // DataGridView から値を取得する
                string[] values = { };
                for (int i = 0; i < dataGrid.ColumnCount; i++)
                {
                    Array.Resize(ref values, i + 1);
                    values[i] = dataGrid[i, row].Value.ToString();
                }

                _excelCellsInfo = new ExcelCellsInfo
                {
                    Name = values[0],
                    Address = values[1],
                    SheetName = values[2],
                    BookName = values[3],
                    Path = values[4],
                    Value = values[5]
                };
                if (int.TryParse(values[6],out int pid)){
                    _excelCellsInfo.Pid = pid;
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetDataFromDataGridView");
            }
        }

        //public List<object> SplitValueInList(List<string> list)
        //{
        //    List<object> retList = new List<object>();
        //    try
        //    {
        //        if (list == null) { _error.AddLog("SplitValueInList.List Is null"); return retList; }
        //        if (list.Count < 1) { _error.AddLog("SplitValueInList.List.Count < 1"); return retList; }
        //        foreach(string value in list)
        //        {

        //        }

        //    } catch (Exception ex)
        //    {
        //        _error.AddException(ex, this.ToString() + ".SplitValueInList");
        //    }
        //}

        public void CopyAndPasteRowData(DataGridView dataGrid, int copyRow, int pasteRow)
        { 
            try
            {
                if (dataGrid == null) { throw new Exception("DataGridView is Null"); }
                if (dataGrid.RowCount < copyRow) { throw new Exception("dataGrid.RowCount < copyRow"); }
                if (dataGrid.RowCount < pasteRow) { throw new Exception("dataGrid.RowCount < pasteRow"); }

                for (int i = 0; i < dataGrid.ColumnCount; i++)
                {
                    //dataGrid.DataBindings[i,pasteRow].se
                    dataGrid[i, pasteRow].Value = dataGrid[i, copyRow].Value.ToString();
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".CopyAndPasteRowData");
            }
        }

        public void SetDataToDataGridView(DataGridView dataGrid,object Data,int setRow)
        {
            try
            {
                if (dataGrid == null) { throw new Exception("DataGridView is Null"); }
                if (dataGrid.RowCount < setRow) { throw new Exception("dataGrid.RowCount < setRow"); }

                dataGrid[0, setRow].Value = _excelCellsInfo.Name;
                dataGrid[1, setRow].Value = _excelCellsInfo.Address;
                dataGrid[2, setRow].Value = _excelCellsInfo.SheetName;
                dataGrid[3, setRow].Value = _excelCellsInfo.BookName;
                dataGrid[4, setRow].Value = _excelCellsInfo.Path;
                dataGrid[5, setRow].Value = _excelCellsInfo.Value;
                dataGrid[6, setRow].Value = _excelCellsInfo.Pid;

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetDataToDataGridView");
            }
        }

    }
}
