using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelFinderAddress
{

}
namespace ExcelIO
{
    public class ExcelFinderAddress
    {
        protected ErrorManager.ErrorManager _Error;
        //protected Range _Range = null;
        public ExcelFinderAddressConstants EnumConst = new ExcelFinderAddressConstants();
        public ExcelFinderAddress(ErrorManager.ErrorManager error)
        {
            _Error = error;
        }

        public enum ExcelFinderAddressConstants
        {
            FIRST = 0x0001,
            LAST = 0x0002,
            DIRECTION_UP = 0x0004,
            DIRECTION_DOWN = 0x0008,
            DIRECTION_LEFT = 0x00010,
            DIRECTION_RIGHT = 0x00020
        }

        // rangeAddress の中に checkAddress が含まれているか
        // すべて含まれている true それ以外は false
        public bool IsIncludeAddressInRangeAddress(in Worksheet worksheet,string checkAddress,string rangeAddress)
        {
            try
            {
                long includeCount = GetIncludeCellsCountInRangeAddress(worksheet, checkAddress, rangeAddress);
                long checkCellsCount = worksheet.Range[checkAddress].Rows.Count * worksheet.Range[checkAddress].Columns.Count;

                if (includeCount == checkCellsCount) { return true; }
                else { return false; }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsIncludeAddressInRangeAddress");
                return false;
            }
        }

        // rangeAddress と checkAddress のアドレスが一致している数を返す
        public long GetIncludeCellsCountInRangeAddress(in Worksheet worksheet,string checkAddress,string rangeAddress)
        {
            try
            {
                ExcelAddressForLoop checkAddressClass = new ExcelAddressForLoop(_Error, checkAddress);
                ExcelAddressForLoop rangeAddressClass = new ExcelAddressForLoop(_Error, rangeAddress);

                checkAddressClass.AnalyzeAddress(worksheet, XlSearchOrder.xlByColumns);
                if (_Error.HasException()) { return -1; }
                rangeAddressClass.AnalyzeAddress(worksheet, XlSearchOrder.xlByColumns);
                if (_Error.HasException()) { return -1; }

                string checknow = "";
                string rangenow = "";
                int retCount = 0;
                while (!checkAddressClass.IsEOA())
                {
                    checknow = checkAddressClass.GetAddress(worksheet);
                    while (!rangeAddressClass.IsEOA())
                    {
                        rangenow = rangeAddressClass.GetAddress(worksheet);

                        if (checknow == rangenow)
                        {
                            retCount++;
                            rangeAddressClass.MoveNext();
                            continue;
                        }
                        if (checkAddressClass.GetMaxCount() <= retCount)
                        {
                            break;
                        }
                        rangeAddressClass.MoveNext();
                    }
                    checkAddressClass.MoveNext();
                }
                return retCount;

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetIncludeCellsCountInRangeAddress");
                return -1;
            }
        }

        // address が Rows.Count=1 && Columns.Count=1 か判定する
        public bool AddressIsSingleCell(in Worksheet worksheet, string address)
        {
            try
            {
                if((worksheet.Range[address].Rows.Count == 1)&&
                    (worksheet.Range[address].Columns.Count == 1))
                {
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetIncludeCellsCountInRangeAddress");
                return false;
            }
        }

        // コンマを含むアドレス(複数アドレス)形式のアドレスから、最初 (左上) のアドレスを取得する
        // Row,Column の優先は order で決定する
        public string GetFirstAddressFromRange(in Worksheet worksheet,string address,XlSearchOrder order)
        {
            try
            {
                string ret = "";
                // アドレスが有効か判定する
                if (!ValueIsAddressAndExceptionThrowWhenValueIsInvalid(worksheet, address))
                {
                    throw new Exception("Address Is Invalid");
                }
                // コンマを含むアドレス(複数アドレス)形式の場合があるので分割する
                string[] AddressArray = address.Split(',');
                // アドレスの左上を取得する
                ret = GetLeftAndTopAddressFromRangeAddress(worksheet, AddressArray,order);
                return ret;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetFirstAddressFromRange");
                return "";
            }
        }

        // 旧
        // 引数 address の左上を取得する、複数アドレス
        // 以下形式の配列が対象
        // 対象形式は $A$1 , $A$1:$B$2 , $A$1,$B$2,$C$3 ... 
        // コンマで Split した配列に対して、それぞれの左上を取得し、それをさらに比較して左上を取得する
        private string GetLeftAndTopAddressFromRangeAddress(
            in Worksheet worksheet,string[] addressArray,XlSearchOrder order)
        {
            try
            {
                string tempadd = "";
                int minCol = worksheet.Columns.Count, minRow = worksheet.Rows.Count;
                // address は IsAddress で判定済み
                for(int i = 0; i<addressArray.Length; i++)
                {
                    // 要素の左上のアドレスを取得する
                    tempadd = GetLeftAndTopAddressFromRangeAddress(worksheet, addressArray[i]);
                    if (_Error.HasException()) { return ""; }
                    if (i == 0)
                    {
                        minRow = worksheet.Range[tempadd].Row;
                        minCol = worksheet.Range[tempadd].Column;
                    }
                    // 比較する
                    if (order == XlSearchOrder.xlByRows)
                    {
                        // Row が小さいほうを優先する
                        if (worksheet.Range[tempadd].Row < minRow)
                        {
                            minRow = worksheet.Range[tempadd].Row;
                            minCol = worksheet.Range[tempadd].Column;
                        }
                    } else if (order == XlSearchOrder.xlByRows) { 
                        // Column が小さいほうを優先する
                        if (worksheet.Range[tempadd].Column < minCol)
                        {
                            minRow = worksheet.Range[tempadd].Row;
                            minCol = worksheet.Range[tempadd].Column;
                        }                    
                    } else
                    {
                        throw new Exception("Search Order Value Is Invalid");
                    }
                }
                return worksheet.Cells[minRow, minCol].Address;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetLeftAndTopAddressFromRangeAddress string.Array");
                return "";
            }
        }

        // 旧
        // 引数 address の左上を取得する
        // 対象形式は $A$1 , $A$1:$B$2 , $A$1,$B$2,$C$3 ... 
        // 長方形型の Range の左上を取得する
        private string GetLeftAndTopAddressFromRangeAddress(in Worksheet worksheet,string address)
        {
            try
            {
                // address は IsAddress で判定済み
                // コロンを含むアドレス(複数アドレス)形式の場合があるので分割する
                string[] addressArray = address.Split(':');
                int minCol = worksheet.Columns.Count, minRow = worksheet.Rows.Count;
                for(int i = 0; i<addressArray.Length; i++)
                {
                    // コロンが含まれる場合は除外する
                    if (addressArray[i].Contains(',')){ continue; }
                    if (i == 0)
                    {
                        minCol = worksheet.Range[addressArray[i]].Column;
                        minRow = worksheet.Range[addressArray[i]].Row;
                    }
                    else
                    {
                        if (worksheet.Range[addressArray[i]].Column < minCol)
                        {
                            minCol = worksheet.Range[addressArray[i]].Column;
                        }
                        if (worksheet.Range[addressArray[i]].Column < minCol)
                        {
                            minRow = worksheet.Range[addressArray[i]].Row;
                        }
                    }
                }
                return worksheet.Cells[minRow, minCol].Address;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetLeftAndTopAddressFromRangeAddress");
                return "";
            }
        }
        //-----------------------------------------------------------------------
        //-----------------------------------------------------------------------
        // 新
        // 引数 address の左上などの角のアドレスを取得する
        // 対象形式は $A$1 , $A$1:$B$2 , $A$1,$B$2,$C$3 ... 
        // Direction 指定Ver
        public string GetCornerAddressFromRangeAddress(
            in Worksheet worksheet, string address, ExcelFinderAddressConstants direction)
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

                if ((direction & ExcelFinderAddressConstants.DIRECTION_DOWN ) != 0) { retRow = maxRow; }
                if ((direction & ExcelFinderAddressConstants.DIRECTION_UP) != 0) { retRow = minRow; }
                if ((direction & ExcelFinderAddressConstants.DIRECTION_RIGHT) != 0) { retCol = maxCol; }
                if ((direction & ExcelFinderAddressConstants.DIRECTION_LEFT) != 0) { retCol = minCol; }

                return worksheet.Cells[retRow, retCol].Address;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetLeftAndTopAddressFromRangeAddress");
                return "";
            }
        }

        // 新
        // 引数 address の左上を取得する、複数アドレス
        // 以下形式の「配列」が対象
        // 対象形式は $A$1 , $A$1:$B$2 , $A$1,$B$2,$C$3 ... 
        // コンマで Split した Address の配列が対象
        // それぞれの左上を取得し、さらにそれを比較して、左上を取得する
        private string GetCornerAddressFromRangeAddress(
            in Worksheet worksheet, string[] addressArray,
            XlSearchOrder prioritySearchOrder, ExcelFinderAddressConstants direction)
        {
            try
            {
                string tempadd = "";
                string maxAddress = worksheet.Range[worksheet.Rows.Count, worksheet.Columns.Count].Address;
                string minRowAddress = "$A$1", maxRowAddress = "$A$1";
                string minColAddress = maxAddress, maxColAddress = maxAddress;
                // address は IsAddress で判定済み

                // 方針：Row、Column それぞれの最小値および最大値を抜き出した後
                // 双方を比べて、優先度によって決める
                for (int i = 0; i < addressArray.Length; i++)
                {
                    // 要素の corner のアドレスを取得する
                    tempadd = GetCornerAddressFromRangeAddress(worksheet, addressArray[i],direction);
                    if (_Error.HasException()) { return ""; }
                    if (i == 0)
                    {
                        minRowAddress = tempadd;
                        maxRowAddress = tempadd;
                        minColAddress = tempadd;
                        maxColAddress = tempadd;
                    } else
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
                if (direction == ExcelFinderAddressConstants.DIRECTION_DOWN) { retAddress = maxRowAddress; }
                else if(direction == ExcelFinderAddressConstants.DIRECTION_UP) { retAddress = minRowAddress; }
                else  if(direction == ExcelFinderAddressConstants.DIRECTION_LEFT) { retAddress = minColAddress; }
                else if(direction == ExcelFinderAddressConstants.DIRECTION_RIGHT) { retAddress = maxColAddress; }
                else
                {
                    // Direction が2つ以上ある
                    DirectionIsOnlyOne = false;
                }
                // Direction が一つだけの場合は、ここで終了する
                if (DirectionIsOnlyOne) { return retAddress; }

                string retRowAddress = "", retColAddress = "";
                if (direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_UP)) { retRowAddress = minRowAddress; }
                else if (direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_DOWN)) { retRowAddress = maxRowAddress; }

                if (direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_LEFT)) { retColAddress = minColAddress; }
                else if (direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_RIGHT)) { retColAddress = maxColAddress; }
                
                if (direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_UP) ||
                    direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_DOWN) ||
                    direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_LEFT) ||
                    direction.HasFlag(ExcelFinderAddressConstants.DIRECTION_RIGHT)
                    )
                {
                    // Direction が規定内の1つも指定されていない時はエラーとする
                    throw new Exception("Direction Value Is Invalid");
                }

                // direction がふたつ
                if (prioritySearchOrder == XlSearchOrder.xlByRows)
                {
                    // Row が小さいほう、大きいほうを優先する
                    retAddress = retRowAddress;
                } else if (prioritySearchOrder == XlSearchOrder.xlByColumns)
                {
                    // Column が小さいほう、大きいほうを優先する
                    retAddress = retColAddress;
                }
                return retAddress;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetLeftAndTopAddressFromRangeAddress string.Array");
                return "";
            }
        }
        //-----------------------------------------------------------------------
        // 検索用、検索時のアドレス分割メソッド
        // FindAddres を縦または横に切り出す
        public List<string> GetAddressListSepalatedForFind(in Worksheet sheet, in string address,XlSearchOrder order)
        {
            List<string> retList = new List<string>();
            try
            {
                string buf = address;
                // $A$2
                // $A$2:$AE$137
                // $A$2:$B$3:$C$4
                // 分割する
                string[] ary = buf.Split(':');
                int minRow = 1, maxRow = 1, minCol = 1, maxCol = 1;
                // Range の形式が単一セルの場合 $A$1
                if (ary.Length == 1)
                {
                    retList.Add(ary[0]);
                    return retList;
                }
                else
                {
                    // Range の形式が複数セルの場合 $A$1:$B$2、$A$1:$B$2:$C$3....
                    // length >= 2
                    minRow = sheet.Range[ary[0]].Row;
                    maxRow = sheet.Range[ary[0]].Row;
                    for (int i = 1; i < ary.Length; i++)
                    {
                        if (sheet.Range[ary[i]].Row < minRow)
                        {
                            minRow = sheet.Range[ary[i]].Row;
                        }
                        if (maxRow < sheet.Range[ary[i]].Row)
                        {
                            maxRow = sheet.Range[ary[i]].Row;
                        }
                        if (sheet.Range[ary[i]].Column < minCol)
                        {
                            minCol = sheet.Range[ary[i]].Column;
                        }
                    }
                }
                string beginAddress, endAddress;
                if (order == XlSearchOrder.xlByRows)
                {
                    for (int i = minCol; i < maxCol; i++)
                    {
                        beginAddress = sheet.Cells[minRow, i].Address;
                        endAddress = sheet.Cells[maxRow, i].Address;
                        retList.Add(beginAddress + ":" + endAddress);
                    }
                }
                else if (order == XlSearchOrder.xlByColumns)
                {
                    for (int i = minRow; i < maxRow; i++)
                    {
                        beginAddress = sheet.Cells[i, minCol].Address;
                        endAddress = sheet.Cells[i, maxCol].Address;
                        retList.Add(beginAddress + ":" + endAddress);
                    }
                }
                return retList;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, ToString() + ".GetAddressListSepalatedForFind");
                return retList;
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
                _Error.AddException(ex, this.ToString() + ".ValueIsAddressAndExceptionThrowWhenValueIsInvalid", errMode);
                return false;
            }
        }
    }
}
