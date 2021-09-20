using System;
using System.Collections.Generic;

namespace ExcelCellsManager.DataGridStruct
{
    public class DataGridViewItems : CellsManagerControl.Utility.IDataGridViewItems
    {
        protected ErrorManager.ErrorManager _error;
        protected object _dataSourceOriginal;
        protected ExcelCellsInfo2[] _dataSource;
        protected List<ExcelCellsInfo2> _ListForDataSource;

        public DataGridViewItems(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public int DataSourceListCount { get => _ListForDataSource.Count;  }

        public object GetItem(int index)
        {
            try
            {
                return _ListForDataSource[index];
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetItem");
                return null;
            }
        }

        public void Insert(int index,object value)
        {
            try
            {
                _ListForDataSource.Insert(index, (ExcelCellsInfo2)value);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Insert");
            }
        }
        public void Add(object value)
        {
            try
            {
                _ListForDataSource.Add((ExcelCellsInfo2)value);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Add");
            }
        }
        public void Remove(int index)
        {
            try
            {
                _ListForDataSource.RemoveAt(index);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Remove");
            }
        }

        public object GetDataSourceAsArray()
        {
            try
            {
                if(_ListForDataSource != null)
                {
                    return _ListForDataSource.ToArray();
                } else
                {
                    return new List<ExcelCellsInfo2>().ToArray();
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetDataSourceAsArray");
                return new List<ExcelCellsInfo2>().ToArray();
            }
        }

        /// <summary>
        /// object BindingSource.DataSource を扱うときにセットする
        /// </summary>
        /// <param name="values"></param>
        public void SetDataSource(in object values)
        {
            try
            {
                if (values != null)
                {
                    _dataSourceOriginal = values;
                    ExcelCellsInfo2[] _dataSource = ConvertFromObjectAsExcelCellsInfo2(_dataSourceOriginal);
                } else
                {
                    _dataSourceOriginal = new string[] { };
                    ExcelCellsInfo2[] _dataSource = new ExcelCellsInfo2[]{ };
                }
                // 以前のデータが影響しないよう List を null にしておく
                //_ListForDataSource = null;

                //if (_dataSource != null)
                //{
                //    _ListForDataSource = _dataSource.ToList();
                //}
                //else
                //{
                //    _ListForDataSource = new List<ExcelCellsInfo2>();
                //}

            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".SetDataSource");
            }
        }

        public void RemoveAllDataSource()
        {
            try
            {
                if (_ListForDataSource is null) { return; }
                if (_ListForDataSource.Count < 1) { return; }

                while(_ListForDataSource.Count >= 1)
                {
                    _ListForDataSource.RemoveAt(0);
                }
                _dataSourceOriginal = _ListForDataSource.ToArray();

                if (_dataSource is null) { return; }
                if (_dataSource.Length < 1) { return; }
                while (_dataSource.Length >= 1)
                {
                    _dataSource[_dataSource.Length] = null;
                    Array.Resize(ref _dataSource, _dataSource.Length - 1);
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".RemoveAllDataSource");
            }
        }
        private ExcelCellsInfo2[] ConvertFromObjectAsExcelCellsInfo2(object values)
        {
            ExcelCellsInfo2[] ret = { };
            _ListForDataSource = new List<ExcelCellsInfo2>();
            try
            {
                if (values == null)
                {
                    return ret;
                }
                int count = 0;
                foreach(var val in (ExcelCellsInfo2[])values)
                {
                    Array.Resize(ref ret,  count+1);
                    ret[count] = (ExcelCellsInfo2)val;
                    _ListForDataSource.Add((ExcelCellsInfo2)val);
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertFromObjectAsExcelCellsInfo2");
                return null;
            }
        }

        public void ConvertDataSourceToListFromObjectType()
        {
            try
            {
                if (_ListForDataSource == null)
                {
                    _ListForDataSource = new List<ExcelCellsInfo2>();
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertDataSourceToListFromObjectType");
            }
        }

        public void AddData(object value)
        {
            try
            {
                _ListForDataSource.Add((ExcelCellsInfo2)value);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".AddData");
            }
        }

        public void AddDataList(List<object> values)
        {
            try
            {
                if(values == null) { _error.AddLogWarning("AddDataList.values is Null"); return; }
                if(values.Count < 1) { _error.AddLogWarning("AddDataList.values.Count < 1"); return; }
                foreach (var val in values)
                {
                    //AddData((ExcelCellsInfo2)((object)val));
                    AddData(ConvertValueFromArray((string[])val));
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".AddDataList");
            }
        }

        public void GetDataSouceAsObjectList(out List<object> dataList)
        {
            dataList = new List<object>();
            try
            {
                if(_ListForDataSource.Count < 1)
                {
                    return;
                }
                foreach(var val in _ListForDataSource)
                {
                    dataList.Add((object)val);
                }
                return;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetDataSouceAsObjectList");
                return;
            }
        }

        public string[] GetFieldNames()
        {
            try
            {
                Utility.ClassInfo info = new Utility.ClassInfo(_error);
                string[] fieldNames = info.GetMemberFeildsNameList(typeof(ExcelCellsInfo2));
                return fieldNames;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetFieldNames");
                return new string[] { };
            }
        }

        public object ConvertValueFromArray(string[] values)
        {
            ExcelCellsInfo2 info = new ExcelCellsInfo2();
            try
            {
                info.Value = values[0];
                info.Address = values[1];
                info.SheetName = values[2];
                info.BookName = values[3];
                info.Path = values[4];
                info.Memo = values[5];

                return info;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertValueFromArray");
                return info;
            }
        }
    }
}
