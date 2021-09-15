using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Office.Interop.Excel;

namespace ExcelUtility
{
    public class ExcelCells
    {
        protected ErrorManager.ErrorManager _error;

        public BackgroundWorker BackgroundWorker;
        protected object _result;
        public ExcelCells(ErrorManager.ErrorManager error)
        {
            _error = error;
        }
        public enum ExcelValueFinderConstants
        {
            FIRST = 0x0001,
            LAST = 0x0002,
            DIRECTION_UP = 0x0004,
            DIRECTION_DOWN = 0x0008,
            DIRECTION_LEFT = 0x00010,
            DIRECTION_RIGHT = 0x00020
        }
        // BackGroundWorker クラスに進捗を報告する
        public void SetBackGroundWorker_ReportProgress(int percentProgress, object userState)
        {
            try
            {
                if (this.BackgroundWorker == null)
                {
                    _error.AddLog(this, ".SetBackGroundWorker_ReportProgress:BackgroundWorker == null");
                }
                else
                {
                    //結果を設定する
                    _result = percentProgress;
                    this.BackgroundWorker.ReportProgress(percentProgress, userState);
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this, "SetBackGroundWorker_ReportProgress");
            }
        }

        public string GetValueCornerAddress(Application application, string bookname, string sheetname, string address)
        {
            try
            {
                _error.AddLog(this, "GetValueCornerAddress");
                string ret = "";
                int rows = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).Range[address].Rows.Count;
                int cols = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).Range[address].Columns.Count;

                string beginAddress = "";
                if ((rows == 1) && (cols == 1))
                {
                    beginAddress = address;
                }
                else
                {
                    beginAddress = GetCornerAddressForAddress(
                        (Worksheet)(application.Workbooks[bookname].Worksheets[sheetname]),
                        address,
                        XlSearchOrder.xlByRows,
                        ExcelValueFinderConstants.DIRECTION_UP | ExcelValueFinderConstants.DIRECTION_LEFT);
                    if (_error.hasAlert) { throw new Exception("GetCornerAddressForAddress Failed"); }
                }
                object val = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).Range[beginAddress].Value;

                if (val == null)
                {
                    _error.AddLog(this, "GetValue : " + bookname + " > " + sheetname + " > " + beginAddress + " > " + "null");
                }
                else
                {
                    _error.AddLog(this, "GetValue : " + bookname + " > " + sheetname + " > " + beginAddress + " > " + val);
                    _error.AddLog(" GetValue.Type : " + val.GetType());
                    ret = val.ToString();
                }
                return ret;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetValueCornerAddress");
                return "";
            }
        }
        public string GetValue(Application application,string bookname ,string sheetname,string address)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".GetValue");
                string ret = "";
                int rows = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).Range[address].Rows.Count;
                int cols = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).Range[address].Columns.Count;

                if ((rows == 1) && (cols == 1))
                {
                    object val = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).Range[address].Value;

                    if (val == null)
                    {
                        _error.AddLog(this.ToString() + ".GetValue", bookname + " > " + sheetname + " > " + address + " > " + "null");
                    }
                    else
                    {
                        _error.AddLog(this.ToString() + ".GetValue", bookname + " > " + sheetname + " > " + address + " > " + val);
                        _error.AddLog("GetValue.Type : " + val.GetType());
                        ret = val.ToString();
                    }
                } else
                {
                    string beginAd = GetCornerAddressForAddress(
                        (Worksheet)(application.Workbooks[bookname].Worksheets[sheetname]),
                        address, 
                        XlSearchOrder.xlByRows, 
                        ExcelValueFinderConstants.DIRECTION_UP | ExcelValueFinderConstants.DIRECTION_LEFT);
                    if (_error.hasAlert) { throw new Exception("GetCornerAddressForAddress Failed"); }
                    object val=null;
                    string str = "";
                    //string dot = "";
                    for (int row=0; row < rows; row++)
                    {
                        for(int col=0; col <cols; col++)
                        {
                            val = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).
                                Range[beginAd].Offset[row, col].Value;
                            if (val == null) { str = ""; }
                            else { str = val.ToString(); }
                            ret += str;
                            //ret += GetCellsValueToString(application,bookname,sheetname,
                            //    ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).
                            //    Range[beginAd].Offset[row, col].Address);
                            if (col < cols - 1)
                            {
                                ret += "\t";
                            }
                        }
                        if( row < rows - 1)
                        {
                            ret += "\n";
                        }
                        int n = (int)row / rows;
                        //if(dot.Length >= 8) { dot = ""; } else { dot += "."; }
                        // コントロールの表示を変更する
                        // ※進捗を表示する処理を追加すること
                        //SetBackGroundWorker_ReportProgress(n, "Processing" + dot);
                    }
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetValue");
                return "";
            }
        }

        public string GetCellsValueToString(Application application, string bookname, string sheetname, string address)
        {
            try
            {

                object val = ((Worksheet)(application.Workbooks[bookname].Worksheets[sheetname])).
                    Range[address].Value;
                if (val == null)
                {
                    return "";
                } else
                {
                    return val.ToString();
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetCellsValueToString");
                return "";
            }
        }

        
        /// <summary>
        ///  address が Rows.Count=1 && Columns.Count=1 か判定する
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool AddressIsSingleCell(in Worksheet worksheet, string address)
        {
            try
            {
                if ((worksheet.Range[address].Rows.Count == 1) &&
                    (worksheet.Range[address].Columns.Count == 1))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetIncludeCellsCountInRangeAddress");
                return false;
            }
        }

        // コンマを含むアドレス(複数アドレス)形式のアドレスから、最初 (左上) のアドレスを取得する
        // Row,Column の優先は order で決定する
        public string GetFirstAddressFromRange(in Worksheet worksheet, string address, XlSearchOrder order,bool IsCheckAddress)
        {
            try
            {
                string ret = "";
                // 処理が重くなるので flag で処理するかしないか判定する
                if (IsCheckAddress)
                {
                    //アドレスが有効か判定する
                    if (!ValueIsAddressAndExceptionThrowWhenValueIsInvalid(worksheet, address))
                    {
                        throw new Exception("Address Is Invalid");
                    }
                }
                // コンマを含むアドレス(複数アドレス)形式の場合があるので分割する
                string[] AddressArray = address.Split(',');
                // アドレスの左上を取得する
                ret = GetCornerAddressForAddress(worksheet, address, order, 
                    ExcelValueFinderConstants.DIRECTION_UP & ExcelValueFinderConstants.DIRECTION_LEFT);
                return ret;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetFirstAddressFromRange");
                return "";
            }
        }
        //-----------------------------------------------------------------------
        /// <summary>
        /// アドレスの左上を返す
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="address"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public string GetCornerAddressForAddress(
            in Worksheet worksheet, string address,XlSearchOrder order, ExcelValueFinderConstants direction)
        {
            try
            {
                _error.AddLog(this.ToString() + ".GetCornerAddressForAddress");
                if (address == null) { _error.AddLog("Address is Null"); return ""; }

                // コンマを含むアドレス(複数アドレス)形式の場合があるので分割する
                string[] AddressArray = address.Split(',');
                if (AddressArray.Length < 1) { _error.AddLog("address.Split.Count < 1"); return ""; }

                // AddressArray すべてを含むアドレスの左上を返す
                string retAddress = GetCornerAddressFromRangeAddress(worksheet, AddressArray, order, direction);

                return retAddress;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetCornerAddressForAddress");
                return "";
            }
        }


        //---------------------------------------------------------------------------------------------------------------
        // 新
        // 引数 address の左上などの角のアドレスを取得する
        // 対象形式は $A$1 , $A$1:$B$2 , $A$1,$B$2,$C$3 ... 
        // Direction 指定Ver
        public string GetCornerAddressFromRangeAddress(
            in Worksheet worksheet, string address, ExcelValueFinderConstants direction)
        {
            try
            {
                // address は IsAddress で判定済み
                // コロンを含むアドレス(複数アドレス)形式の場合があるので分割する
                string[] addressArray = address.Split(':');

                // 判定結果用変数の最大値取得用に最小値を、最小値取得用に最大値を格納しておく
                int maxCol = 1, maxRow = 1;
                int minCol = worksheet.Columns.Count, minRow = worksheet.Rows.Count;

                for (int i = 0; i < addressArray.Length; i++)
                {
                    // コロンが含まれる場合は除外する
                    if (addressArray[i].Contains(',')) { continue; }
                    if (i == 0)
                    {
                        minCol = worksheet.Range[addressArray[i]].Column;
                        minRow = worksheet.Range[addressArray[i]].Row;
                        maxCol = worksheet.Range[addressArray[i]].Column;
                        maxRow = worksheet.Range[addressArray[i]].Row;
                    }
                    else
                    {
                        if (worksheet.Range[addressArray[i]].Column < minCol)
                        {
                            minCol = worksheet.Range[addressArray[i]].Column;
                        }
                        if (worksheet.Range[addressArray[i]].Row < minRow)
                        {
                            minRow = worksheet.Range[addressArray[i]].Row;
                        }
                        if (worksheet.Range[addressArray[i]].Column > maxCol)
                        {
                            maxCol = worksheet.Range[addressArray[i]].Column;
                        }
                        if (worksheet.Range[addressArray[i]].Row > maxRow)
                        {
                            maxRow = worksheet.Range[addressArray[i]].Row;
                        }
                    }
                } // for Loop End

                // 左下と右上だけ取得しておいて、Direction によって戻り値を変更する
                int retRow = 1, retCol = 1;

                if ((direction & ExcelValueFinderConstants.DIRECTION_DOWN) != 0) { retRow = maxRow; }
                if ((direction & ExcelValueFinderConstants.DIRECTION_UP) != 0) { retRow = minRow; }
                if ((direction & ExcelValueFinderConstants.DIRECTION_RIGHT) != 0) { retCol = maxCol; }
                if ((direction & ExcelValueFinderConstants.DIRECTION_LEFT) != 0) { retCol = minCol; }

                return worksheet.Cells[retRow, retCol].Address;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetLeftAndTopAddressFromRangeAddress");
                return "";
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        // 新
        // 引数 address の左上を取得する、複数アドレス
        // 以下形式の「配列」が対象
        // 対象形式は $A$1 , $A$1:$B$2 , $A$1,$B$2,$C$3 ... 
        // コンマで Split した Address の配列が対象
        // それぞれの左上を取得し、さらにそれを比較して、左上を取得する
        private string GetCornerAddressFromRangeAddress(
            in Worksheet worksheet, string[] addressArray,
            XlSearchOrder prioritySearchOrder, ExcelValueFinderConstants direction)
        {
            try
            {
                string tempadd = "";
                
                int maxrow = worksheet.Rows.Count;
                int maxcol = worksheet.Columns.Count;
                //maxrow = worksheet.UsedRange.Row;
                //maxcol = worksheet.UsedRange.Column;
                Console.WriteLine("** "+ worksheet.Name);
                Console.WriteLine("   row=" + maxrow + "  col="+maxcol);
                string maxAddress="";
                maxAddress = worksheet.Cells[maxrow, maxcol].Address;
                //maxAddress = worksheet.Range["XFD1048576"].Address;
                string minRowAddress = "$A$1", maxRowAddress = "$A$1";
                string minColAddress = maxAddress, maxColAddress = maxAddress;
                // address は IsAddress で判定済み

                // 方針：Row、Column それぞれの最小値および最大値を抜き出した後
                // 双方を比べて、優先度によって決める
                for (int i = 0; i < addressArray.Length; i++)
                {
                    string buf = addressArray[i];
                    // 要素の corner のアドレスを取得する
                    tempadd = GetCornerAddressFromRangeAddress(worksheet, buf, direction);
                    if (_error.HasException()) { return ""; }
                    if (i == 0)
                    {
                        minRowAddress = tempadd;
                        maxRowAddress = tempadd;
                        minColAddress = tempadd;
                        maxColAddress = tempadd;
                    }
                    else
                    {
                        if (worksheet.Range[tempadd].Row < worksheet.Range[minRowAddress].Row) { minRowAddress = tempadd; }
                        if (worksheet.Range[tempadd].Row > worksheet.Range[maxRowAddress].Row) { maxRowAddress = tempadd; }
                        if (worksheet.Range[tempadd].Column < worksheet.Range[minColAddress].Column) { minColAddress = tempadd; }
                        if (worksheet.Range[tempadd].Column < worksheet.Range[maxColAddress].Column) { maxColAddress = tempadd; }
                    }
                } // For Loop End

                // direction が二つ以上ある場合は比較する
                // direction のあり得ない組み合わせはエラー 未実装
                // 3つ以上の組み合わせはエラー
                // 有効な組み合わせは、Rigt+Up,RIght+Down,Lef+Up,Left+Down の4つのみ

                string retAddress = "";
                bool DirectionIsOnlyOne = true;
                // direction が一つのみ
                if (direction == ExcelValueFinderConstants.DIRECTION_DOWN) { retAddress = maxRowAddress; }
                else if (direction == ExcelValueFinderConstants.DIRECTION_UP) { retAddress = minRowAddress; }
                else if (direction == ExcelValueFinderConstants.DIRECTION_LEFT) { retAddress = minColAddress; }
                else if (direction == ExcelValueFinderConstants.DIRECTION_RIGHT) { retAddress = maxColAddress; }
                else
                {
                    // Direction が2つ以上ある
                    DirectionIsOnlyOne = false;
                }
                // Direction が一つだけの場合は、ここで終了する
                if (DirectionIsOnlyOne) { return retAddress; }

                string retRowAddress = "", retColAddress = "";
                if (direction.HasFlag(ExcelValueFinderConstants.DIRECTION_UP)) { retRowAddress = minRowAddress; }
                else if (direction.HasFlag(ExcelValueFinderConstants.DIRECTION_DOWN)) { retRowAddress = maxRowAddress; }

                if (direction.HasFlag(ExcelValueFinderConstants.DIRECTION_LEFT)) { retColAddress = minColAddress; }
                else if (direction.HasFlag(ExcelValueFinderConstants.DIRECTION_RIGHT)) { retColAddress = maxColAddress; }

                if (!(direction.HasFlag(ExcelValueFinderConstants.DIRECTION_UP) ||
                    direction.HasFlag(ExcelValueFinderConstants.DIRECTION_DOWN) ||
                    direction.HasFlag(ExcelValueFinderConstants.DIRECTION_LEFT) ||
                    direction.HasFlag(ExcelValueFinderConstants.DIRECTION_RIGHT)
                    ))
                {
                    // Direction が規定内の1つも指定されていない時はエラーとする
                    throw new Exception("Direction Value Is Invalid");
                }

                // direction がふたつ
                if (prioritySearchOrder == XlSearchOrder.xlByRows)
                {
                    // Row が小さいほう、大きいほうを優先する
                    retAddress = retRowAddress;
                }
                else if (prioritySearchOrder == XlSearchOrder.xlByColumns)
                {
                    // Column が小さいほう、大きいほうを優先する
                    retAddress = retColAddress;
                }
                return retAddress;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetLeftAndTopAddressFromRangeAddress string.Array");
                return "";
            }

        }
        // これはアドレスで値が無効時は例外が発生する
        // This is an address and an exception is thrown when the value is invalid
        public bool ValueIsAddressAndExceptionThrowWhenValueIsInvalid(
            in Worksheet worksheet, string address, int errMode = 1)
        {
            try
            {
                string buf = worksheet.Range[address].Value2;
                return true;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ValueIsAddressAndExceptionThrowWhenValueIsInvalid", errMode);
                return false;
            }
        }
    }
}
