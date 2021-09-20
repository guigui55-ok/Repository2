using CellsManagerControl.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    public class DataGridViewUtility : IDataGridViewUtility
    {
        protected ErrorManager.ErrorManager _error;
        protected DataGridView _dataGridView;
        protected BindingSource _wrapper;
        protected IDataGridViewItems _dataGridViewItems;
        protected IDataGridViewValueConverter _dataGridViewValueConverter;
        //protected List<object> _list;
        protected bool _isInitialize  = false;
        protected event EventHandler _dataGridView_InsertRow;
        protected event EventHandler _dataGridView_DeleteRow;
        protected event EventHandler _dataGridView_AddRow;
        protected event EventHandler _dataGridView_ChangeValue;

        public bool IsInitialize => _isInitialize;

        public IDataGridViewValueConverter DataGridViewValueConverter => _dataGridViewValueConverter;

        EventHandler IDataGridViewUtility.DataGridView_InsertRow
        {
            get { return _dataGridView_InsertRow; }
            set { _dataGridView_InsertRow = value; }
        }
        EventHandler IDataGridViewUtility.DataGridView_DeleteRow
        {
            get { return _dataGridView_DeleteRow; }
            set { _dataGridView_DeleteRow = value; }
        }
        EventHandler IDataGridViewUtility.DataGridView_AddRow
        {
            get { return _dataGridView_AddRow; }
            set { _dataGridView_AddRow = value; }
        }
        EventHandler IDataGridViewUtility.DataGridView_ChangeValue
        {
            get { return _dataGridView_ChangeValue; }
            set { _dataGridView_ChangeValue = value; }
        }

        public DataGridViewUtility(ErrorManager.ErrorManager error,DataGridView dataGridView,IDataGridViewItems dataGridViewItems)
        {
            _error = error;
            _dataGridView = dataGridView;
            _dataGridViewItems = dataGridViewItems;
            _dataGridViewValueConverter = new DataGridViewValueConverter(_error);
        }



        public void Initialize(string[] columnNameList){
            try
            {
                _error.AddLog(this.ToString()+ ".Initialize");
                if (_dataGridView.Columns.Count < 1)
                {
                    //列が自動的に作成されないようにする
                    _dataGridView.AutoGenerateColumns = false;
                    // Column がないときは作成する
                    foreach (string colname in columnNameList)
                    {
                        AddColumn(colname);
                        if (_error.hasError) { _error.AddException(new Exception("AddColumn : colname="+colname)); return; }
                    }
                    // Initialize 終了
                    _isInitialize = true;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initilaize");
            }
        }

        public void SelectRow(int row)
        {
            try
            {
                _dataGridView.ClearSelection();
                _dataGridView.Rows[row].Selected = true;
                //_dataGridView.CurrentRow.Index = row;
                _dataGridView.CurrentCell = _dataGridView.Rows[row].Cells[0];
            } catch (Exception ex)
            {
                _error.AddException(ex, this, "SelectRow");
            }
        }

        public void SetColumnWidth(int[] columnsWidthList)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetColumnWidth");
                if (columnsWidthList == null) { _error.AddLog("  columnsWidthList is null"); return; }
                if (columnsWidthList.Length < 1) { _error.AddLog("  columnsWidthList.Length < 1"); return; }
                if (_dataGridView == null) { _error.AddLog("  _dataGridView is null"); return; }
                if (_dataGridView.Columns.Count < 1) { _error.AddLog("  _dataGridView.Columns.Count < 1"); return; }

                for(int i=0; i< columnsWidthList.Length; i++)
                {
                    if((columnsWidthList[i] > 0)||(columnsWidthList[i] < _dataGridView.Width)){
                        // 値が 0 以上 DataGridView.Width 以下の時
                        _dataGridView.Columns[i].Width = columnsWidthList[i];
                    } else
                    {
                        // columnsWidthList 値が無効の時
                        _error.AddLogAlert(new Exception("  columns value is invalid. value=" + columnsWidthList[i] + " , index=" + i));  
                    }
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetColumnWidth");
            }
        }

        public void Initialize(string[] columnNameList, object value)
        {
            try
            {
                Initialize(columnNameList);
                if (_error.hasAlert) { throw new Exception("Initialize Failed"); }

                // DataSource がないときは new する
                if (_dataGridView.DataSource == null) { _wrapper = new BindingSource(); }
                // DataSouce を取得する
                _dataGridViewItems.SetDataSource(_wrapper.DataSource);
                if (_error.hasAlert) { throw new Exception("SetDataSource Failed"); }

                // 追加しやすいように List に変換する
                _dataGridViewItems.ConvertDataSourceToListFromObjectType();
                if (_error.hasAlert) { throw new Exception("ConvertDataSourceToListFromObjectType Failed"); }

                // 追加する
                _dataGridViewItems.AddData(value);
                if (_error.hasAlert) { throw new Exception("AddData Failed"); }

                // DataSource にセットする
                _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                if (_error.hasAlert) { throw new Exception("GetDataSourceAsArray Failed"); }

                // _wrapper を DataGridView にセットする
                _dataGridView.DataSource = _wrapper;
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString() + ".Initialize");
            }
        }

        public void AddValue(object value)
        {
            try
            { 
                if (!_isInitialize) { throw new Exception("DataGridView Not Initialize"); }
                if (_dataGridView.DataSource == null)
                {
                    _wrapper = new BindingSource();
                }

                // DataSouce を取得する
                _dataGridViewItems.SetDataSource(_wrapper.DataSource);
                if (_error.hasAlert) { throw new Exception("SetDataSource Failed"); }

                // 追加しやすいように List に変換する
                _dataGridViewItems.ConvertDataSourceToListFromObjectType();
                if (_error.hasAlert) { throw new Exception("ConvertDataSourceToListFromObjectType Failed"); }

                // 追加する
                _dataGridViewItems.AddData(value);
                if (_error.hasAlert) { throw new Exception("AddData Failed"); }

                // DataSource にセットする
                _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                if (_error.hasAlert) { throw new Exception("GetDataSourceAsArray Failed"); }

                // View を更新する
                _dataGridView.DataSource = _wrapper;
                _dataGridView.Update();
                _dataGridView.Refresh();

                int row = _dataGridView.Rows.Count-1;
                // Selectする
                _dataGridView.CurrentCell = _dataGridView.Rows[row].Cells[0];
                _dataGridView.Rows[row].Selected = true;
                // Event
                _dataGridView_AddRow?.Invoke(null, EventArgs.Empty);

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".AddValue");
            }
        }

        public void RemoveAll()
        {
            try
            {
                if (!_isInitialize)
                {
                    throw new Exception("DataGridView Not Initialize");
                }
                if (_dataGridView.DataSource == null) { _wrapper = new BindingSource(); }

                // DataSouce をすべて削除する
                _dataGridViewItems.RemoveAllDataSource();
                // 一度削除したものを _wapper へ
                _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                // DataSource にセットする
                _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                // View を更新する
                _dataGridView.DataSource = _wrapper;
                _dataGridView.Update();
                _dataGridView.Refresh();
                // Selectする
                if (_dataGridView.Rows.Count < 1) { _error.AddLog("AddValues.NotSelected", "_dataGridView.Rows.Count<1"); return; }
                int row = _dataGridView.Rows.Count - 1;
                _dataGridView.CurrentCell = _dataGridView.Rows[row].Cells[0];
                _dataGridView.Rows[row].Selected = true;
                // Event // 現在の行を渡す
                _dataGridView_DeleteRow?.Invoke(null, EventArgs.Empty);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".RemoveAll");
            }
        }

        public void ResetValue(List<string[]> values)
        {
            try
            {
                if (!_isInitialize) { throw new Exception("DataGridView Not Initialize"); }
                if (_dataGridView.DataSource == null) { _wrapper = new BindingSource(); }

                // DataSouce をすべて削除する
                _dataGridViewItems.RemoveAllDataSource();
                if (_error.hasAlert) 
                { _error.AddLogWarning("ResetValue:_dataGridViewItems.RemoveAllDataSource"); _error.ReleaseErrorState(); }
                // 一度削除したものを _wapper へ
                _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                if (_error.hasAlert)
                { _error.AddLogWarning("ResetValue:_dataGridViewItems.GetDataSourceAsArray"); _error.ReleaseErrorState(); }
                // ここからは通常通り
                // DataSouce を取得する
                _dataGridViewItems.SetDataSource(_wrapper.DataSource);
                if (_error.hasAlert) { throw new Exception("DataGridViewItems.SetDataSource Failed"); }
                // 追加しやすいように List に変換する
                _dataGridViewItems.ConvertDataSourceToListFromObjectType();
                if (_error.hasAlert) { throw new Exception("DataGridViewItems.ConvertDataSourceToListFromObjectType Failed"); }
                // 追加する (List<string> から List<object> に変換する)
                _dataGridViewItems.AddDataList(ConvertObjectList(values));
                if (_error.hasAlert) { throw new Exception("DataGridViewItems.AddDataList Failed"); }
                // DataSource にセットする
                _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                if (_error.hasAlert) { throw new Exception("DataGridViewItems.GetDataSourceAsArray Failed"); }
                // View を更新する
                _dataGridView.DataSource = _wrapper;
                _dataGridView.Update();
                _dataGridView.Refresh();
                // Selectする
                if (_dataGridView.Rows.Count < 1) { _error.AddLogWarning("AddValues.NotSelected", "_dataGridView.Rows.Count<1"); return; }
                int row = _dataGridView.Rows.Count - 1;
                _dataGridView.CurrentCell = _dataGridView.Rows[row].Cells[0];
                _dataGridView.Rows[row].Selected = true;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ResetValue");
            }
        }

        private List<object> ConvertObjectList(List<string[]> list)
        {
            List<object> retList = new List<object>();
            try
            {
                if(list == null) { return retList; }
                if(list.Count < 1) { return retList; }
                foreach(var value in list)
                {
                    retList.Add((object)value);
                }
                return retList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertObjectList");
                return retList;
            }
        }

        //public void ResetValue(List<string[]> values)
        //{
        //    try
        //    {
        //        if (values == null) { _error.AddLog("values is null");  return; }
        //        if (values.Count < 1) { _error.AddLog("values.Count < 1"); return; }
        //        // DataSouce が null のとき
        //        if (_dataGridView.DataSource == null) { _wrapper = new BindingSource(); }
        //        // DataSouce を取得する
        //        _dataGridViewItems.SetDataSource(_wrapper.DataSource);
        //        // DataSouce をすべて削除する
        //        _dataGridViewItems.RemoveAllDataSource();
        //        if (_error.HasException()) { return; }
        //        // DataSouce に List<string> values を追加する
        //        this.AddValues(values);
        //    }
        //    catch (Exception ex)
        //    {
        //        _error.AddException(ex, this.ToString() + ".ResetValue");
        //    }
        //}

        private void AddColumn(string ColumnName)
        {
            try
            {
                //DataGridViewTextBoxColumn列を作成する
                DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn
                {
                    //データソースの"Column1"をバインドする
                    DataPropertyName = ColumnName,
                    //名前とヘッダーを設定する
                    Name = ColumnName,
                    HeaderText = ColumnName
                };
                //列を追加する
                _dataGridView.Columns.Add(textColumn);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".AddColumn");
            }
        }

        /// <summary>
        /// 現在選択している行の出たを取得する
        /// </summary>
        /// <returns></returns>
        public string[] GetCurrentRowData()
        {
            string errmsg = "";
            string[] retAry = { };
            try
            {
                if (_dataGridView.Rows.Count < 1) {
                    errmsg = "Data.Rows.Count Is Zero";
                    throw new Exception("Data.Rows.Count Is Zero"); 
                }
                if (_dataGridView.CurrentCell == null)
                {
                    errmsg = "DataGridView.CurrentCell == Null";
                    throw new Exception("DataGridView.CurrentCell == Null"); 
                }
                int row = _dataGridView.CurrentCell.RowIndex;
                retAry = GetRowData(row);
                return retAry;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetSelectionData",errmsg);
                return retAry;
            }
        }

        public string[] GetRowData(int row)
        {
            string errmsg = "";
            string[] retAry = { };
            try
            {
                if (_dataGridView.Rows.Count < 1) { throw new Exception("Data.Rows.Count Is Zero"); }
                if (!((0 <= row)&&(row < _dataGridView.Rows.Count))) {
                    errmsg = "Row Value is Invalid";
                    throw new Exception("Row Value is Invalid"); 
                }

                for (int i = 0; i < _dataGridView.ColumnCount; i++)
                {
                    Array.Resize(ref retAry, i + 1);
                    retAry[i] = _dataGridView[i, row].Value.ToString();
                }
                return retAry;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetRowData", errmsg);
                return retAry;
            }
        }
        
        public void MoveItemUpInList()
        {
            try
            {
                if (_dataGridView.Rows.Count < 1) { throw new Exception("Data.Rows.Count Is Zero"); }
                if (_dataGridView.CurrentCell == null) { throw new Exception("_dataGridView.CurrentCell is Null"); }
                int row = _dataGridView.CurrentCell.RowIndex;
                InsertCutLine(_dataGridView, row, row - 1);
                if (_error.HasException()) { return; }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MoveItemUpInList");
            }
        }

        public void MoveItemDownInList()
        {
            try
            {
                if (_dataGridView.Rows.Count < 1) { throw new Exception("Data.Rows.Count Is Zero"); }
                if (_dataGridView.CurrentCell == null) { throw new Exception("_dataGridView.CurrentCell is Null"); }
                int row = _dataGridView.CurrentCell.RowIndex;
                InsertCutLine(_dataGridView, row, row + 1);
                if (_error.HasException()) { return; }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MoveItemDownInList");
            }
        }

        public void InsertCutLine(in DataGridView dataGrid,int cutRow,int insertRow)
        {
            try
            {
                //if (dataGrid.Rows.Count < 1) { throw new Exception("Data.Rows.Count Is Zero"); }
                if (dataGrid.Rows.Count < cutRow) { throw new Exception("Data.Rows.Count Under cutRow"); }
                if (dataGrid.Rows.Count <= insertRow-1) { throw new Exception("Data.Rows.Count Under insertRow"); }
                if ((insertRow < 0) || (insertRow  >= dataGrid.Rows.Count)) {
                    //throw new Exception("insertRow is Invalid"); 
                    return;
                }
                if ((cutRow < 0) || (cutRow > dataGrid.Rows.Count)) {
                    //throw new Exception("cutRow is Invalid"); 
                    return;
                }

                // cutRow が最大値で、cutRow < insertRow の時は無効にする
                if ((dataGrid.Rows.Count <= cutRow)&&(cutRow >= insertRow)) { return; }
                // cutRow が最小値で、cutRow >= insertRow の時は無効にする
                if ((cutRow <= 0)&&(insertRow <= cutRow)) { return; }

                int row = dataGrid.CurrentCell.RowIndex;
                if (row >= 0)
                {
                    // DataSouce を取得する
                    //ExcelCellsInfo[] list = (ExcelCellsInfo[])_wrapper.DataSource;
                    _dataGridViewItems.SetDataSource(_wrapper.DataSource);
                    if (_error.HasException()) { return; }
                    // 追加しやすいように List に変換する
                    _dataGridViewItems.ConvertDataSourceToListFromObjectType();
                    // ここで編集するように DataSource を List<object> で取得する
                    _dataGridViewItems.GetDataSouceAsObjectList(out List<object> EditList);
                    //_list = list.ToList();
                    // Cut する
                    //object tmpInfo = EditList[cutRow];
                    object tmpInfo = _dataGridViewItems.GetItem(cutRow);
                    //EditList.RemoveAt(cutRow);
                    _dataGridViewItems.Remove(cutRow);
                    // Insert する
                    if (_dataGridViewItems.DataSourceListCount < 1)
                    {
                        _dataGridViewItems.Add(tmpInfo);
                    } else
                    {
                        _dataGridViewItems.Insert(insertRow, tmpInfo);
                    }

                    // View を更新する
                    //_wrapper.DataSource = EditList.ToArray();
                    _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                    dataGrid.DataSource = _wrapper;
                    dataGrid.Update();
                    dataGrid.Refresh();
                    // Selectする
                    dataGrid.CurrentCell = dataGrid.Rows[insertRow].Cells[0];
                    dataGrid.Rows[insertRow].Selected = true;

                    // Event
                    _dataGridView_InsertRow(null,EventArgs.Empty);
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".InsertCutLine");
            }
        }

        public void DeleteLine(DataGridView dataGrid,int targetRow)
        {
            try
            {
                _error.AddLog(this, "DeleteLine");
                if (dataGrid.Rows.Count < 1)
                {
                    _error.AddLogCaution("dataGridView.CurrentCell is Null");
                    return;
                    //throw new Exception("Data.Rows.Count Is Zero");
                }
                if ((targetRow >= 0)&&(targetRow <= dataGrid.Rows.Count))
                {
                    // DataSouce を取得する
                    _dataGridViewItems.SetDataSource(_wrapper.DataSource);
                    if (_error.hasAlert)
                    { _error.AddLogWarning("ResetValue:_dataGridViewItems.RemoveAllDataSource"); _error.ReleaseErrorState(); }

                    // 追加しやすいように List に変換する
                    _dataGridViewItems.ConvertDataSourceToListFromObjectType();
                    if (_error.hasAlert) { throw new Exception("DataGridViewItems.ConvertDataSourceToListFromObjectType Failed"); }

                    // ここで編集するように DataSource を List<object> で取得する
                    _dataGridViewItems.GetDataSouceAsObjectList(out List<object> EditList);
                    if (_error.hasAlert) { throw new Exception("DataGridViewItems.GetDataSouceAsObjectList Failed"); }

                    // Delete する
                    //EditList.RemoveAt(targetRow);
                    _dataGridViewItems.Remove(targetRow);
                    if (_error.hasAlert) { throw new Exception("DataGridViewItems.Remove Failed"); }
                    // View を更新する
                    //_wrapper.DataSource = EditList.ToArray();
                    _wrapper.DataSource = _dataGridViewItems.GetDataSourceAsArray();
                    dataGrid.DataSource = _wrapper;
                    dataGrid.Update();
                    dataGrid.Refresh();
                    // 一つ削除したので一つ減らす
                    targetRow--;
                    // Selectする
                    if (dataGrid.Rows.Count < 1) { _error.AddLog("dataGrid.Rows.Count < 1"); return; }
                    if (targetRow > (dataGrid.Rows.Count-1)) { targetRow = dataGrid.Rows.Count; }
                    dataGrid.CurrentCell = dataGrid.Rows[targetRow].Cells[0];
                    dataGrid.Rows[targetRow].Selected = true;
                    // Event
                    _dataGridView_DeleteRow?.Invoke(null, EventArgs.Empty);
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".DeleteLine");
            }
        }

        public int GetCurrentRow()
        {
            try
            {
                if (_dataGridView.Rows.Count < 1) {
                    _error.AddLogCaution("_dataGridView.CurrentCell is Null");
                    throw new Exception("Data.Rows.Count Is Zero"); 
                }
                if (_dataGridView.CurrentCell == null) {
                    _error.AddLogCaution("_dataGridView.CurrentCell is Null");
                    throw new Exception("_dataGridView.CurrentCell is Null"); 
                }
                return _dataGridView.CurrentCell.RowIndex;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetCurrentRow");
                return 0;
            }
        }
        public bool LineIsBlank(int row)
        {
            try
            {
                if (_dataGridView.Rows.Count < 1)
                {
                    _error.AddLogCaution("LineIsBlank:_dataGridView.Rows.Count < 1");
                    return false;
                }
                if (_dataGridView.Rows.Count -1 > row)
                {
                    _error.AddLogCaution("LineIsBlank:check row invlid. row=" + row);
                    return false;
                }
                if (_dataGridView.Columns.Count < 1)
                {
                    _error.AddLogCaution("LineIsBlank:_dataGridView.Columns.Count < 1");
                    return false;
                }
                object value;
                bool isValid = true;
                for (int col = 0; col < _dataGridView.Columns.Count; col++)
                {
                    value = _dataGridView[col, row].Value;
                    if (value != null)
                    {
                        isValid = true;
                        return false;
                    }
                }
                return isValid;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".LineIsBlank");
                return false;
            }
        }
        private string GetDataInDataGridView(object value)
        {
            try
            {
                if (value != null) { return value.ToString(); }
                else { return ""; }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetDataInDataGridView");
                return "";
            }
        }

        public string GetAllData(string delimiter)
        {
            try
            {
                if (_dataGridView == null)
                {
                    _error.AddLogCaution("GetAllData:_dataGridView is Null");
                    return "";
                }
                if (_dataGridView.Rows.Count < 1)
                {
                    _error.AddLogCaution("GetAllData:_dataGridView.Rows.Count < 1");
                    return "";
                }
                if (_dataGridView.Columns.Count < 1)
                {
                    _error.AddLogCaution("GetAllData:_dataGridView.Columns.Count < 1");
                    return "";
                }
                if (_dataGridView.Rows.Count == 1)
                {
                    if (LineIsBlank(0))
                    {
                        _error.AddLogCaution("GetAllData:_dataGridView Data Is Nothing");
                        return "";
                    }
                }

                string ret="";
                for (int row = 0; row < _dataGridView.Rows.Count; row++)
                {
                    for (int col = 0; col < _dataGridView.Columns.Count; col++)
                    {
                        // 最後の行がすべてからの場合は追加しない
                        if (row == _dataGridView.Rows.Count-1)
                        {
                            if (LineIsBlank(row))
                            {
                                break;
                            }
                        }
                        ret += GetDataInDataGridView(_dataGridView[col,row].Value);
                        if (col < (_dataGridView.Columns.Count - 1))
                        {
                            // 区切り文字
                            ret += delimiter;
                        }
                    }
                    if (row < (_dataGridView.Rows.Count - 1))
                    {
                        ret += "\n";
                    }
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetAllData");
                return "";
            }
        }

    }
}
