
using System;
using System.Collections.Generic;
using CommonUtility;

namespace ExcelCellsManager.CellsValuesConrol.CellsValuesList
{
    public class DataGridViewValueConverter : IDataGridViewValueConverter
    {
        protected ErrorManager.ErrorManager _error;
        protected int _fieldsLength = 0;
        public DataGridViewValueConverter(ErrorManager.ErrorManager error)
        {
            _error = error;

            ClassInfo info = new ClassInfo(_error);
            string[] fieldNames = info.GetMemberFeildsNameList(typeof(ExcelCellsInfo2));
            _fieldsLength = fieldNames.Length;
        }

        public List<string[]> ConvertArrayStringListFromStringList(List<string> list,char delimiter)
        {
            List<string[]> retList = new List<string[]>();
            try
            {
                if (list == null) { _error.AddLogWarning("ConvertArrayStringListFromStringList : list is null"); return retList; }
                if (list.Count < 1) { _error.AddLogWarning("ConvertArrayStringListFromStringList : list.Count < 1"); return retList; }

                foreach (string value in list)
                {
                    string[] buf = SplitData(value, delimiter);
                    if (buf.Length == _fieldsLength)
                    {
                        retList.Add(buf);
                    }
                    else
                    {
                        _error.AddLogAlert("ConvertArrayStringListFromStringList. value.Split.Length Not Match ExcelCellsInfo2");
                        throw new Exception("  value.Split.Length Not Match ExcelCellsInfo2");
                    }
                }
                return retList;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertArrayStringListFromStringList");
                return retList;
            }
        }
        public List<object> ConvertObjectListFromStringList(List<string> list,char delimiter)
        {
            List<object> retList = new List<object>();
            try
            {
                if(list == null) { _error.AddLog("list is null"); return retList; }
                if (list.Count < 1) { _error.AddLog("list.Count < 1"); return retList; }

                foreach (string value in list)
                {
                    string[] buf = SplitData(value, delimiter);
                    if (buf.Length == _fieldsLength)
                    {
                        retList.Add((object)buf);
                    }
                    else
                    {
                        _error.AddLog("SplitData.value.Length Not Match ExcelCellsInfo2");
                    }
                }
                return retList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertObjectListFromStringList");
                return retList;
            }
        }

        private string[] SplitData(string value,char delimiter)
        {
            string[] ret;
            try
            {
                if(value == null) { _error.AddLog("SplitData.value is null"); return null; }
                ret = value.Split(delimiter);
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SplitData");
                return null;
            }
        }

        //-----------------------------
        //private List<ExcelCellsInfo> ConvertListTypeAsExcelCellsInfo(List<object> list)
        //{
        //    List<ExcelCellsInfo> retList = new List<ExcelCellsInfo>();
        //    try
        //    {
        //        if (list == null)
        //        {
        //            _error.AddLog(this.ToString() + ".ConvertListTypeAsExcelCellsInfo", "List Is Null");
        //            return retList;
        //        }
        //        if (list.Count < 1)
        //        {
        //            _error.AddLog(this.ToString() + ".ConvertListTypeAsExcelCellsInfo", "List.Count < 1");
        //            return retList;
        //        }
        //        foreach (var val in list)
        //        {
        //            retList.Add((ExcelCellsInfo)val);
        //        }
        //        return retList;
        //    }
        //    catch (Exception ex)
        //    {
        //        _error.AddException(ex, this.ToString() + ".ConvertListTypeAsExcelCellsInfo");
        //        return retList;
        //    }
        //}
    }
}
