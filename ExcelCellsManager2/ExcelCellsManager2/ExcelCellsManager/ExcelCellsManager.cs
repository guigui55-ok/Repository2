using ExcelUtility;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;

namespace ExcelCellsManager.ExcelCellsManager
{
    public class ExcelCellsManager
    {
        protected ErrorManager.ErrorManager _error;
        protected int _debugMode;
        public List<ExcelCellsInfo> CellsInfoList;
        public ExcelApps.ActiveCellsInfo ActiveCellsInfo;
        public ExcelCellsManagerConstants Constants;
        public ExcelCellsManager(ErrorManager.ErrorManager error,int DebugMode,ExcelCellsManagerConstants constants)
        {
            _error = error;
            _debugMode = DebugMode;
            Constants = constants;
            CellsInfoList = new List<ExcelCellsInfo>();
        }

        public List<object> GetCellsInfoListAsObjectList()
        {
            return ConvertListToObjectType(this.CellsInfoList);
        }

        public List<object> ConvertListToObjectType(List<ExcelCellsInfo> list)
        {
            List<object> retList = new List<object>();
            try
            {
                if (list == null)
                {
                    _error.AddLog(this.ToString() + ".ConvertListToObjectType", "List Is Null");
                    throw new Exception("List Is Null");
                    //return retList;
                }
                if (list.Count < 1)
                {
                    _error.AddLog(this.ToString() + ".ConvertListToObjectType", "List.Count < 1");
                    throw new Exception("List.Count < 1");
                    //return retList;
                }
                foreach (var val in list)
                {
                    retList.Add((object)val);
                }
                return retList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertListToObjectType");
                return retList;
            }
        }


        public string GetNewNameForAdd(string name)
        {
            try
            {
                if (name == "")
                {
                    name = "Item #" + CellsInfoList.Count + 1;
                }
                return name;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".AddList");
                return "Item ";
            }
        }

        // MakeAddValue
        public ExcelCellsInfo2 MakeAddValue(in Application application, string bookName, int Pid, string labelName)
        {
            try
            {
                ExcelCellsInfo2 info = null;
                string address = application.Selection.Address;
                if (address != "")
                {
                    info = new ExcelCellsInfo2();
                    if (labelName == "")
                    {
                        labelName = "Item #" + CellsInfoList.Count + 1;
                    }

                    string buf = GetValue2((Worksheet)application.ActiveSheet, (Range)application.ActiveSheet.Range[address]);
                    if (_error.hasAlert) { _error.AddLogWarning("MakeAddValue Failed"); return info; }
                    info.Value = buf;
                    info.Address = address;
                    info.SheetName = application.ActiveSheet.Name;
                    info.BookName = bookName;
                    info.Path = application.Workbooks[bookName].Path;
                    info.Memo = "";

                    //this.CellsInfoList.Add(info);
                }
                return info;
            } catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.Message.IndexOf(ExcelCellsManagerErrorCodes.RPC_E_CALL_REJECTED.ToString()) > 0)
                {
                    string msg = Constants.GetErrorMessage(ExcelCellsManagerErrorCodes.RPC_E_CALL_REJECTED);
                    _error.AddException(ex, this, "MakeAddValue Failed(System.Runtime.InteropServices.COMException)",msg);
                } else
                {
                    _error.AddException(ex, this.ToString() + ".MakeAddValue");
                }
                return null;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MakeAddValue");
                return null;
            }
        }

        public ExcelCellsInfo AddList(in Application application,string bookName,int Pid,string labelName)
        {　
            try
            {
                string address = application.Selection.Address;
                if (address != "")
                {
                    ExcelCellsInfo info = new ExcelCellsInfo();
                    if (labelName == "")
                    {
                        labelName = "Item #" + CellsInfoList.Count + 1;
                    }
                    info.Name = labelName;
                    info.Address = address;
                    info.SheetName = application.ActiveSheet.Name;
                    //info.Row = application.ActiveSheet.Range[address].Row;
                    //info.Column = application.ActiveSheet.Range[address].Column;
                    info.BookName = bookName;
                    info.Path = application.Workbooks[bookName].Path;
                    string buf = GetValue2((Worksheet)application.ActiveSheet, (Range)application.ActiveSheet.Range[address]);
                    if (_error.HasException()) { return info; }
                    info.Value = buf;
                    info.Pid = Pid;

                    this.CellsInfoList.Add(info);
                    return info;
                }
                return null;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".AddList");
                return null;
            }
        }

        private string GetValue2(in Worksheet worksheet, in Range range)
        {
            try
            {
                string buf = "";
                if (range.Cells.Count > 1)
                {
                    ExcelValueFinder util = new ExcelValueFinder(_error);
                    string address = util.GetCornerAddressFromRangeAddress(worksheet, range.Address,
                        ExcelValueFinder.ExcelValueFinderConstants.DIRECTION_UP |
                        ExcelValueFinder.ExcelValueFinderConstants.DIRECTION_LEFT);
                    if (_error.HasException()) { return ""; }
                    buf = ConvertValueToString(worksheet.Range[address]);
                    return buf;
                }
                else
                {
                    buf = ConvertValueToString(range);
                    return buf;
                }                
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetValue2");
                return "";
            }
        }

        private string ConvertValueToString(in Range range)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".ConvertValueToString");
                //string ret="";
                object obj = range.Value;
                if (range.Value == null)
                {
                    // Range.Value の値が空の時は Null になる
                    _error.AddLog(this.ToString() + ".ConvertValueToString : " + "Range.Value Is Null");
                    return "";
                }
                _error.AddLog(this.ToString() + ".ConvertValueToString : " + "range.Value.Type = " + obj.GetType());

                if (obj.GetType() == typeof(string))
                {
                    return (string)obj;
                }
                else if (obj.GetType() == typeof(double))
                {
                    //Double d;
                    return obj.ToString();
                }
                else if (obj.GetType() == typeof(System.Double))
                {
                    //Double d;
                    return obj.ToString();
                }
                else if (obj.GetType() == typeof(int))
                {
                    return obj.ToString();
                }
                else if (obj.GetType() == typeof(System.DateTime))
                {
                    return obj.ToString();
                }
                else if ((range.Columns.Count > 1)||(range.Rows.Count > 1))
                {
                    // 複数選択時は実行前に Range.Cells.Count > 1 で処理するようにする
                    _error.AddLog("(range.Columns.Count > 1)||(range.Rows.Count > 1)");
                    throw new Exception("(range.Columns.Count > 1)||(range.Rows.Count > 1)");
                } else
                {

                    throw new Exception("Value2 Type Is Not Supported : " + obj.GetType());
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertValue2ToString");
                return "";
            }
        }
    }
}
