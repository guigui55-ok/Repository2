using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExcelIO
{
    public class ExcelFinder
    {
        protected ErrorManager.ErrorManager _Error;
        public Workbook Workbook = null;
        protected Range _currentFindRange;
        public List<ExcelFinderResult> ResultList = new List<ExcelFinderResult>();
        protected ExcelFinderAddress finderAddressUtil;

        protected Range _targetFindRange;
        public ExcelFinder(ErrorManager.ErrorManager error)
        {
            _Error = error;
            finderAddressUtil = new ExcelFinderAddress(_Error);
        }

        // class 検索条件、除外条件

        public void SelectRangeByFindNext(in Application application, string sheetName, ExcelFindSearchConditions conditions)
        {
            try
            {
                // 検索関連の値 (SheetName など) が正しいか判定する
                if (!IsSearchRelatedValuesCorrect(sheetName)) { return; }

                // Condition 内の値が有効か判定する
                if (!IsValidConditionsValue(conditions)) { return; }
                //this.Worksheet = Workbook.Worksheets[sheetName];

                // conditions.FindRange 検索対象の Range が正しいか判定する
                // _targetFindRange へ保存する
                SetFindAddressFromConditions(sheetName, conditions);
                if (_Error.HasException()) { return; }

                // _targetFindRange から検索用に Range から
                // AddressList(conditions.TargetAddressListFromTargetRange) に変換し保存する
                // List.Count<1 はエラーとする
                //SetFindAddressListInConditionsFromFindRange(sheetName, conditions);
                //if (_Error.HasException()) { return; }

                // conditions.BeginAddress が有効か判定・設定する
                // BeginAddress がもともと設定されていない場合は FindRange の左上を始点とする
                //conditions.BeginAddress = finderAddressUtil.GetFirstAddressFromRange(
                //    Workbook.Sheets[sheetName],conditions.BeginAddress,conditions.SearchOrder);
                //if (_Error.HasException()) { return; }

                // 検索開始アドレスを設定する
                conditions.AfterAddress = GetAfterFromSelectionCell(application,sheetName, conditions);
                if (_Error.HasException()) { return; }

                // xxxx
                // 検索用に SearchOrder の方向に分割されたアドレスリストを conditions にセットする
                //conditions.sepalatedAddressListBySearchOrder = GetToConditionsAddressSepalatedBySearchOrderForFind(
                //    sheetName, conditions.TargetAddressListFromTargetRange, conditions.SearchOrder);
                //if (_Error.HasException()) { return; }

                // 次を検索
                string resultAddress = FindNextByConditions(sheetName, conditions);
                if (_Error.HasException()) { return; }

                //if (resultAddress.Contains(conditions.AfterAddress))
                //{
                //    resultAddress = FindNextByConditions(sheetName, conditions);
                //    if (_Error.HasException()) { return; }
                //}

                // Selectする
                if (resultAddress != "") {
                    //this.Workbook.Worksheets[sheetName].Range[resultAddress].Select();
                    this.Workbook.Worksheets[sheetName].Activate();
                    this.Workbook.Worksheets[sheetName].Range(resultAddress).Select();
                } else
                {
                    Console.WriteLine("resultAddress : Nothing");
                }
            }
            catch ( Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SelectRangeByFindNext");
                return;
            }
        }

        public bool SetAfterIfIncludeSearchValue(string sheetName, ExcelFindSearchConditions conditions)
        {
            try
            {
                string buf = Workbook.Sheets[sheetName].Range[conditions.AfterAddress].Value2();
                if (buf.Contains(conditions.KeyWord))
                {
                    if (conditions.SearchOrder == XlSearchOrder.xlByColumns)
                    {
                        conditions.AfterAddress = Workbook.Sheets[sheetName].Range[conditions.AfterAddress].Offset(0, 1).Address;
                    }
                    if (conditions.SearchOrder == XlSearchOrder.xlByRows)
                    {
                        conditions.AfterAddress = Workbook.Sheets[sheetName].Range[conditions.AfterAddress].Offset(1, 0).Address;
                    }
                }
                return true;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetAfterIfIncludeSearchValue");
                return false;
            }
        }

        // Selection.Address から検索開始アドレスを取得する
        // Selection.Address=="" なら FindRange の左上を取得する
        private string GetAfterFromSelectionCell(
            in Application application, string sheetName, ExcelFindSearchConditions conditions)
        {
            string ret;
            try
            {
                // FindRange 設定済み

                // Selection.Address を取得する
                //ret = Workbook.Sheets[sheetName].Selection.Address;
                ret = application.Selection.Address;
                if (ret == "")
                {
                    // Seleciton.Address が空の場合は ActiveCell もチェックする
                }

                if (ret == "")
                {
                    // Seleciton.Address が空の場合は FindRange の左上を設定する
                    ret = finderAddressUtil.GetCornerAddressFromRangeAddress(
                        Workbook.Sheets[sheetName], conditions.FindRange,
                        ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_UP |
                        ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_LEFT);
                    if (_Error.HasException()) { return ret; }
                }
                return ret;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetAfterFromSelectionCell");
                return "";
            }
        }

        // 
        private string FindNextByConditions(
            string sheetName,ExcelFindSearchConditions conditions)
        {
            try
            {
                // 検索関連値のチェック済
                // FindRange 内に BeginRange(After) があるか判定する
                //if (!finderAddressUtil.IsIncludeAddressInRangeAddress(
                //    (Worksheet)Workbook.Sheets[sheetName], conditions.AfterAddress , conditions.FindRange))
                //{
                //    // 含まれない場合はスキップする
                //} else
                {
                    // 検索する
                    _currentFindRange = Workbook.Sheets[sheetName].Range[conditions.FindRange].Find(
                        conditions.KeyWord, Workbook.Sheets[sheetName].Range[conditions.AfterAddress],
                        conditions.LookIn, conditions.LookAt,
                        conditions.SearchOrder, conditions.SearchDirection, conditions.MatchCase,
                        conditions.MatchByete, conditions.SearchFormat);

                    if (_currentFindRange == null)
                    {
                        // ない場合はスキップする
                    }
                    else
                    {
                        // あったら終了する
                        return _currentFindRange.Address;
                    }
                }
                // すべてなし
                return "";
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".FindNextByConditions");
                return "";
            }
        }

        // xxxx
        // 検索用に SearchOrder の方向に分割されたアドレスリストを conditions にセットする
        private List<string> GetToConditionsAddressSepalatedBySearchOrderForFind(
            string sheetName,List<string> TargetAddressListFromTargetRange,XlSearchOrder order)
        {
            List<string> retList = new List<string>();
            try
            {
                if (TargetAddressListFromTargetRange == null) { throw new Exception("conditions.TargetAddressList Is Null"); }
                if (TargetAddressListFromTargetRange.Count < 1) { throw new Exception("conditions.TargetAddressList.Count Is Zero"); }

                // コンマで分割された AddressList(conditions.TargetAddressListFromTargetRange) を順次検索する
                foreach (string sepalatedAddressByComma in TargetAddressListFromTargetRange)
                {
                    // 検索範囲を分割する
                    // SearchOrder が XlRows か XlColumns しか選べないので
                    // 検索用に一行ずつ分割したアドレスリストを取得する
                    retList.AddRange( 
                        finderAddressUtil.GetAddressListSepalatedForFind(
                        Workbook.Sheets[sheetName], sepalatedAddressByComma, order));
                    if (_Error.HasException()) { return retList; }
                }
                return retList;

                // conditions.FindBegin が含まれている場合は検索する
                // 含まれず FindBegin より FindRange の範囲が大きい場合はスキップする
                // 含まれず FindBegin  FindRange の範囲が小さい場合はスキップする
                // FindNext
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetToConditionsAddressSepalatedBySearchOrderForFind");
                return retList;
            }
        }

        // FindBeginAddress を conditions.FindBeginAddress が有効であるか判定・セットする
        public void SetFindRangeAddress(string sheetName, ExcelFindSearchConditions conditions)
        {
            try
            {
                if ((conditions.FindRange == "") || (conditions.FindRange == null))
                {
                    // 値がない場合は _targetFindRange の左上を取得・設定する
                    //_targetFindRange = Workbook.Sheets[sheetName].UsedRange;
                    _targetFindRange = finderAddressUtil.GetFirstAddressFromRange(
                        Workbook.Sheets[sheetName], conditions.FindRange, conditions.SearchOrder);
                }
                else
                {
                    // FindRange 値がある場合アドレス形式かどうかを判定する
                    if (ValueIsAddressAndExceptionThrowWhenValueIsInvalid(Workbook.Sheets[sheetName], conditions.FindRange)){
                        // 有効
                        _targetFindRange = Workbook.Sheets[sheetName].Range[conditions.FindRange];
                    }
                    else
                    {
                        // 無効
                        // ここでは、無効の時はエラーとする
                        if (_Error.HasException()) { return; }
                        throw new Exception("BeginAddres Value Is Invalid.");
                    }
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetFindRangeAddress");
                return;
            }
        }

        // xxxxx
        // AddressList(string) を FindRange(Range) から
        // condiions.TargetAddressListFromTargetRange(List<string>) にセットする
        private void SetFindAddressListInConditionsFromFindRange(
            string sheetName, ExcelFindSearchConditions condiions)
        {
            try
            {
                // coniditons.FindRange 判定済み
                // _targetFindRange 設定済み

                string[] findAddressArray = _targetFindRange.Address.Split(',');
                condiions.TargetAddressListFromTargetRange = new List<string>();
                foreach (string value in findAddressArray)
                {
                    // SearchOrder が XlRows か XlColumns しか選べないので
                    // 検索用に一行ずつ分割したアドレスリストを取得する
                    condiions.TargetAddressListFromTargetRange.AddRange(GetRowAddresListInRangeForFind(
                        Workbook.Sheets[sheetName], value));
                    if (_Error.HasException()) { return; }
                }
                if (condiions.TargetAddressListFromTargetRange.Count < 1)
                {
                    throw new Exception("Find Target Address List Count Is Zero");
                }

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetFindAddressListInConditionsFromFindRange");
                return;
            }
        }

        public void SetAfterRange(in Application application,string sheetName,ExcelFindSearchConditions conditions)
        {
            try
            {
                string selectionActiveCellAddress = application.Selection.Address;
                
                //if (!finderAddressUtil.ValueIsAddressAndExceptionThrowWhenValueIsInvalid(
                //    (Worksheet)Workbook.Sheets[sheetName], selectionActiveCellAddress))
                //{
                    selectionActiveCellAddress = application.ActiveCell.Address;
                    if (!finderAddressUtil.AddressIsSingleCell(
                        (Worksheet)Workbook.Sheets[sheetName], selectionActiveCellAddress))
                    {
                        // Singleではない
                        // SelectionAddress Rows.Count=1,Columns.Count=1 ではないときは、左上を取得する
                        selectionActiveCellAddress = finderAddressUtil.GetCornerAddressFromRangeAddress(
                            (Worksheet)Workbook.Sheets[sheetName], selectionActiveCellAddress, 
                            ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_LEFT |
                            ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_UP);
                    }
                //}

                SetAfterRangeWhenEndAddressInFindRange(application,sheetName, conditions, selectionActiveCellAddress);
                if (_Error.HasException()) { return; }

                // 一度検索済みで、同じキーワードが含まれている場合は次から検索する
                //string buf = Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Value;
                //if (buf.Contains(conditions.KeyWord))
                //{
                //    // 検索方向によって Offset 値を切り替える
                //    int col = 0, row = 0;
                //    if (conditions.SearchDirection == XlSearchDirection.xlNext)
                //    {
                //        // **
                //        if (conditions.SearchOrder == XlSearchOrder.xlByColumns) { col = 1; }
                //        if (conditions.SearchOrder == XlSearchOrder.xlByRows) { row = 1; }
                //        selectionActiveCellAddress = Workbook.Sheets[sheetName]
                //            .Range[selectionActiveCellAddress].Offset(row, col).Address;
                //    }
                //    else if (conditions.SearchDirection == XlSearchDirection.xlPrevious)
                //    {
                //        // **
                //        if (conditions.SearchOrder == XlSearchOrder.xlByColumns) { col = 1; }
                //        if (conditions.SearchOrder == XlSearchOrder.xlByRows) { row = 1; }
                //    }
                //}

                conditions.AfterAddress = selectionActiveCellAddress;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetAfterRange");
                return;
            }
        }

        private void SetAfterRangeWhenEndAddressInFindRange(
            in Application application, string sheetName, ExcelFindSearchConditions conditions,
            string selectionActiveCellAddress)
        {
            try
            {
                // selectionActiveCellAddress Rows.Count,Columns.Count=1 判定済み
                // 一度検索済みで、同じキーワードが含まれている場合は次から検索する
                string buf = Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Value.ToString();
                if (buf.Contains(conditions.KeyWord))
                {
                    string FindBeginAddress = finderAddressUtil.GetCornerAddressFromRangeAddress(
                        (Worksheet)Workbook.Sheets[sheetName], conditions.FindRange, 
                        ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_UP |
                        ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_LEFT);

                    // 検索方向によって Offset 値を切り替える
                    int col = 0, row = 0;
                    if (conditions.SearchDirection == XlSearchDirection.xlNext)
                    {
                        int r = Workbook.Sheets[sheetName].Range[conditions.FindRange].Row + 
                            Workbook.Sheets[sheetName].Range[conditions.FindRange].Rows.Count -1;
                        int c = Workbook.Sheets[sheetName].Range[conditions.FindRange].Column +
                            Workbook.Sheets[sheetName].Range[conditions.FindRange].Columns.Count - 1;
                        // FindRange の最後のセルを取得する
                        string FindEndAddress = Workbook.Sheets[sheetName].Cells[r, c].Address;

                        if (conditions.SearchOrder == XlSearchOrder.xlByColumns)
                        {
                            // 最後だと次のセルをセットできないのでそのままにする
                            if (Workbook.Sheets[sheetName].Range[FindEndAddress].Column >
                                Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Column
                                )
                            {
                                col = 1;
                            }
                        }
                        if (conditions.SearchOrder == XlSearchOrder.xlByRows)
                        {
                            // 最後だと次のセルをセットできないのでそのままにする
                            if (Workbook.Sheets[sheetName].Range[FindEndAddress].Row >
                                Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Row
                                )
                            {
                                row = 1;
                            }
                        }
                    }
                    else if (conditions.SearchDirection == XlSearchDirection.xlPrevious)
                    {
                        // 1 だと前に戻れないのでそのままにする
                        if (conditions.SearchOrder == XlSearchOrder.xlByColumns) { 
                            if (Workbook.Sheets[sheetName].Range[FindBeginAddress].Column != 1)
                            {
                                // 1 だと前に戻れないのでそのままにする
                                col = -1;
                            }
                            if (Workbook.Sheets[sheetName].Range[FindBeginAddress].Row != 1)
                            {
                                // 1 だと前に戻れないのでそのままにする
                                row = -1;
                            }
                        }
                    }
                    selectionActiveCellAddress = Workbook.Sheets[sheetName]
                        .Range[selectionActiveCellAddress].Offset(row, col).Address;
                }
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetAfterRangeWhenEndAddressInFindRange");
                return;
            }
        }

        /// <summary>
        /// 検索開始アドレスAfterAddress を設定する
        /// Selection.Address が単一の時にはその Address を設定し、複数の時にはその最初 (左上) の Address を設定する
        /// FindRev の時は左下 (最後) の Address を設定する
        /// </summary>
        public void SetAfterRangeTooLate(in Application application, string sheetName, ExcelFindSearchConditions conditions)
        {
            try
            {
                bool isMulti = false;
                string selectionActiveCellAddress = application.Selection.Address;
                string setAddress = "";
                if (selectionActiveCellAddress != "")
                {
                    if ((Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Rows.Count > 1) ||
                        (Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Columns.Count > 1))
                    {
                        // 選択中のセルが複数の場合は、左上を設定する
                        isMulti = true;
                    }
                }
                else
                {
                    // Selection が空の場合は ActiveCell もチェックする
                    selectionActiveCellAddress = application.ActiveCell.Address;
                    if ((Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Rows.Count > 1) ||
                        (Workbook.Sheets[sheetName].Range[selectionActiveCellAddress].Columns.Count > 1))
                    {
                        // ActiveCell が複数の場合は、FindRange に設定する
                        isMulti = true;
                    }
                }

                if (isMulti)
                {
                    setAddress = finderAddressUtil.GetCornerAddressFromRangeAddress(
                        Workbook.Sheets[sheetName], selectionActiveCellAddress,
                        ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_LEFT |
                        ExcelFinderAddress.ExcelFinderAddressConstants.DIRECTION_UP);
                    if (_Error.HasException()) { return; }
                } else
                {
                    // Selection,ActiveCell が単一セル (Rows.Count==1 && Columns.Count==1) の場合
                    setAddress = selectionActiveCellAddress;
                }
                conditions.AfterAddress = setAddress;
                return;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetAfterRangeTooLate");
                return;
            }
        }

        public bool SetFindAddressFromSelectionOrActiveCell(in Application application, string sheetName,ExcelFindSearchConditions conditions)
        {
            try
            {
                string selectionAddress = application.Selection.Address;
                if (selectionAddress != "")
                {
                    if ((Workbook.Sheets[sheetName].Range[selectionAddress].Rows.Count > 1) ||
                        (Workbook.Sheets[sheetName].Range[selectionAddress].Columns.Count > 1))
                    {
                        // 選択中のセルが複数の場合は、FindRange に設定する
                        conditions.FindRange = selectionAddress;
                        return true;
                    }
                } else
                {
                    // Selection が空の場合は ActiveCell もチェックする
                    string ActiveCell = application.ActiveCell.Address;
                    if ((Workbook.Sheets[sheetName].Range[ActiveCell].Rows.Count > 1) ||
                        (Workbook.Sheets[sheetName].Range[ActiveCell].Columns.Count > 1))
                    {
                        // ActiveCell が複数の場合は、FindRange に設定する
                        conditions.FindRange = ActiveCell;
                        return true;
                    }
                }
                // Selection,ActiveCell が単一セル (Rows.Count==1 && Columns.Count==1) の場合は FindRange をここで設定しない
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetFindAddressFromSelectionOrActiveCell");
                return false;
            }
        }

        // 検索アドレスを conditions.FindRange から _targetFindRange に セットする
        public void SetFindAddressFromConditions(string sheetName, ExcelFindSearchConditions conditions)
        {
            try
            {
                //string SelectionAddress = Workbook.Worksheets[sheetName].Selection.Address;
                //if ( (Workbook.Sheets[sheetName].Range[SelectionAddress].Rows.Count >= 1) ||
                //    (Workbook.Sheets[sheetName].Range[SelectionAddress].Columns.Count >= 1))
                //{
                //    // 選択されているセルが１つ以上の時は、検索対象アドレスとする
                //    conditions.FindRange = SelectionAddress;
                //    return;
                //}

                if ((conditions.FindRange == "")||(conditions.FindRange == null))
                {
                    // FindRange が空の場合は sheet の UsedRange とする
                    _targetFindRange = Workbook.Sheets[sheetName].UsedRange;
                    conditions.FindRange = Workbook.Sheets[sheetName].UsedRange.Address;
                } 
                //else
                //{
                //    // FindRange 値がすでにある場合アドレス形式かどうかを判定する
                //    if (ValueIsAddressAndExceptionThrowWhenValueIsInvalid((Worksheet)this.Workbook.Worksheets[sheetName], conditions.FindRange,1)){
                //        // 有効
                //        _targetFindRange = Workbook.Sheets[sheetName].Range[conditions.FindRange];
                //    } else
                //    {
                //        // 無効
                //        // ここでは、無効の時はエラーとする
                //        if (_Error.HasException()) { return; }
                //        throw new Exception("FindRange Value Is Invalid.");
                //    }
                //}
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetFindAddressFromConditions");
                return;
            }
        }

        // これはアドレスで値が無効時は例外が発生する
        // This is an address and an exception is thrown when the value is invalid
        public bool ValueIsAddressAndExceptionThrowWhenValueIsInvalid(
            in Worksheet worksheet,string address,int errMode = 1)
        {
            try
            {
                string buf = worksheet.Range[address].Address;
                return true;
            } catch (Exception ex)
            {                
                _Error.AddException(ex, this.ToString() + ".ValueIsAddressAndExceptionThrowWhenValueIsInvalid",errMode);
                return false;
            }
        }

        // conditions が有効な値か判定する
        public bool IsValidConditionsValue(ExcelFindSearchConditions conditions)
        {
            try
            {
                if (conditions.KeyWord == null) { throw new Exception("Conditions Value Is Invalid. Keyword Is Null "); }
                if (conditions.KeyWord == "") { throw new Exception("Conditions Value Is Invalid. Keyword.Length Is Zero "); }
                return true;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ExcelFindSearchCondiions");
                return false;
            }
        }
        // is Search related values are correct
        private bool IsSearchRelatedValuesCorrect(string sheetName)
        {
            try
            {
                if (this.Workbook == null) { throw new Exception("Workbook Is Null"); }
                if (sheetName == null) { throw new Exception("SheetName Is Null"); }
                if (sheetName == "") { throw new Exception("SheetName.Length Is Zero"); }
                return true;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsSearchRelatedValuesCorrect");
                return false;
            }
        }

        // 次を検索
        public string GetAddressByFindNext(string sheetName, ExcelFindSearchConditions conditions)
        {
            try
            {
                _currentFindRange = _targetFindRange.Find(
                    conditions.KeyWord, conditions.After,
                    conditions.LookIn, conditions.LookAt,
                    conditions.SearchOrder, conditions.SearchDirection, conditions.MatchCase,
                    conditions.MatchByete, conditions.SearchFormat);
                if(_currentFindRange == null) { return ""; }
                return _currentFindRange.Address;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetAddressByFindNext");
                return "";
            }
        }

        // -------------------------------------------------------------------------------------------
        // xxxxxx
        // シートの中の有効セルすべてを検索し、マッチしたアドレスリストを取得する
        public List<string> GetAddressListByFindAtSepalatedAddressInSheet(string sheetName, ExcelFindSearchConditions conditions)
        {
            List<string> AddressList = new List<string>();
            Range currentFind = null;
            Range firstFind = null;
            Range FindRange = null;
            Worksheet worksheet = null;
            try
            {
                worksheet = null;
                worksheet = Workbook.Worksheets[sheetName];
                if (!IsSearchRelatedValuesCorrect(sheetName)) { return AddressList; }
                if (!IsValidConditionsValue(conditions)) { return AddressList; }
                FindRange = Workbook.Sheets[sheetName].UsedRange;
                Console.WriteLine("FindRange : " + FindRange.Address);
                string findAddress = FindRange.Address;
                string[] findAddressArray = findAddress.Split(',');
                List<string> findAddressList = new List<string>();
                foreach (string value in findAddressArray)
                {
                    // SearchOrder が XlRows か XlColumns しか選べないので
                    // 検索用に一行ずつ分割したアドレスリストを取得する
                   findAddressList.AddRange( GetRowAddresListInRangeForFind(worksheet, value));
                    if (_Error.HasException()) { return AddressList; }
                }

                if (findAddressList.Count < 1)
                {
                    throw new Exception("Find Address List is Not Exists");
                }

                List<string> tmpList;
                foreach(string address in findAddressList)
                {
                    conditions.FindRange = address;
                    tmpList = GetAddressListByFindInSheet(sheetName, conditions);
                    if (_Error.HasException()) { return AddressList; }
                    if (tmpList.Count > 0) {
                        AddressList.AddRange(tmpList);
                    }
                }
                return AddressList;
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetAddressListByFindInSheet");
                return AddressList;
            } finally
            {
                if (FindRange != null)
                {
                    Marshal.ReleaseComObject(FindRange);
                    //FindRange = null;
                }

                if (firstFind != null)
                {
                    Marshal.ReleaseComObject(firstFind);
                    //firstFind = null;
                }

                if (currentFind != null)
                {
                    Marshal.ReleaseComObject(currentFind);
                    //currentFind = null;
                }
                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(worksheet);
                    //worksheet = null;
                }
                System.GC.Collect();
            }
        }


        // 単にFindを実行して AddressList を取得する
        public List<string> GetAddressListByFindInSheet(
            string sheetName, ExcelFindSearchConditions conditions)
        {
            List<string> AddressList = new List<string>();
            Range currentFind = null;
            Range firstFind = null;
            Range FindRange = null;
            //Worksheet worksheet = null;
            string firstAddress = "";
            try
            {
                if (this.Workbook == null) { throw new Exception("Workbook Is Null"); }
                if (conditions.KeyWord == null) { return AddressList; }
                if (conditions.KeyWord == "") { return AddressList; }
                if (sheetName == null) { return AddressList; }
                if (sheetName == "") { return AddressList; }
                //worksheet = Workbook.Worksheets[sheetName];
                //for (int i = 1; i <= Workbook.Worksheets.Count; i++)
                //{
                //    if (Workbook.Worksheets[i].Name == sheetName)
                //    {
                //        this.Worksheet = Workbook.Worksheets[i];
                //    }
                //}
                //FindRange = Workbook.Sheets[sheetName].UsedRange;
                FindRange = Workbook.Sheets[sheetName].Range[conditions.FindRange];
                //Console.WriteLine("* FindRange : " + FindRange.Address);
                /*
                 * What : String Keyword
                 * After : Range 検索位置を示すセルです。
                 * LookIn : 情報の種類です。値、数式、書式等
                 * LookAt : XlLookAt 値 (xlWhole または xlPart) のどちらかです。
                 * SearchOrder : XlSearchOrder 値 (xlByRows または xlByColumns) のどちらかです。
                 * SearchDirection : 次のいずれかの XlSearchDirection 値です。 xlNext または xlPrevious。
                 * MatchCase : 検索で大文字と小文字を区別する場合は true を指定します。 既定値は false です
                 * MatchByte : 2 バイト言語サポートを選択またはインストールした場合のみ使用します。
                 * 2 バイト文字を 2 バイト文字にのみ一致させる場合は true。
                 * 2 バイト文字を、等価な 1 バイト文字に一致させる場合は false。
                 * SearchFormat : 書式を検索する (True)、検索しない(False)
                 */
                currentFind = FindRange.Find(
                    conditions.KeyWord, conditions.After,
                    conditions.LookIn, conditions.LookAt,
                    conditions.SearchOrder, conditions.SearchDirection, conditions.MatchCase,
                    conditions.MatchByete, conditions.SearchFormat);

                while (currentFind != null)
                {
                    //Console.WriteLine("* address = " + currentFind.Address);
                    //Console.WriteLine(currentFind.get_Address(Type.Missing, Type.Missing,
                    //    XlReferenceStyle.xlA1, Type.Missing, Type.Missing).ToString());
                    SetRangeInfo(currentFind);
                    if (_Error.HasException()) { return AddressList; }
                    if (currentFind == null)
                    {
                        // Find Not Found In Sheet
                        //Console.WriteLine("Find Not Found In Sheet");
                    }
                    else if (currentFind.Address == firstAddress)
                    {
                        // Find End In Sheet
                        //Console.WriteLine("Find End In Sheet");
                        break;
                    } else if (firstAddress == "")
                    {
                        // First Match
                        firstAddress = currentFind.Address;
                        AddressList.Add(firstAddress);
                        firstFind = currentFind;
                    } else
                    {
                        // CurrentFind != null
                        AddressList.Add(currentFind.Address);
                    }
                    currentFind = FindRange.FindNext(currentFind);
                }
                return AddressList;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetAddressListByFindLineInSheet");
                return AddressList;
            }
            finally
            {
                if (FindRange != null)
                {
                    Marshal.ReleaseComObject(FindRange);
                    //FindRange = null;
                }

                if (firstFind != null)
                {
                    Marshal.ReleaseComObject(firstFind);
                    //firstFind = null;
                }

                if (currentFind != null)
                {
                   Marshal.ReleaseComObject(currentFind);
                    //currentFind = null;
                }
                System.GC.Collect();
            }
        }

        private void SetRangeInfo(in Range range)
        {
            try
            {
                ExcelFinderResult ret = new ExcelFinderResult
                {
                    Address = range.Address,
                    CellValue = range.Value2,
                    //Console.WriteLine("range.Parent.Name = "+ (string)range.Parent.Name);
                    SheetName = (string)range.Parent.Name,
                    //Console.WriteLine("range.Parent.Parent.Name = " + (string)range.Parent.Parent.Name);
                    WorkbookName = (string)range.Parent.Parent.Name
                };
                this.ResultList.Add(ret);
            } catch (Exception ex)
            {
                _Error.AddException(ex, ToString() + ".SetRangeInfo");
                return;
            }
        }

        public List<string> GetRowAddresListInRangeForFind(in Worksheet sheet, in string address)
        {
            List<string> retList = new List<string>();
            try
            {
                string buf = address;
                // $A$2
                // $A$2:$AE$137
                // $A$2:$B$3:$C$4
                string[] ary = buf.Split(':');
                int minRow = 1, maxRow = 1, minCol = 1,maxCol = 1;
                if (ary.Length == 1)
                {
                    retList.Add(ary[0]);
                    return retList;
                }
                else if (ary.Length == 2)
                {
                    minRow = sheet.Range[ary[0]].Row;
                    maxRow = sheet.Range[ary[1]].Row;
                    if (maxRow < minRow)
                    {
                        int tmp = maxRow; maxRow = minRow; minRow = tmp;
                    }
                    minCol = sheet.Range[ary[0]].Column;
                    maxCol = sheet.Range[ary[1]].Column;
                    if (maxCol < minCol)
                    {
                        int tmp = maxCol; maxCol = minCol; minCol = tmp;
                    }
                }
                else
                {
                    // length > 2
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
                for(int i = minCol; i<maxCol; i++)
                {
                    beginAddress = sheet.Cells[minRow, i].Address;
                    endAddress = sheet.Cells[maxRow, i].Address;
                    retList.Add(beginAddress + ":" + endAddress);
                }
                return retList;

            } catch (Exception ex)
            {
                _Error.AddException(ex, ToString() + ".GetRowAddresListInRangeForFind");
                return retList;
            }
        }

        public string GetRowAddressInRange(in Worksheet sheet, in Range range)
        {
            try
            {
                string buf = range.Address;
                // $A$2
                // $A$2,$A$2
                // $A$2:$AE$137
                // $A$2,$AE$137
                // $A$2:$AE$137,$A$2
                // $A$2:$AE$137,$A$2:$AE$137

                // $A$2
                // $A$2:$AE$137
                // $A$2:$B$3:$C$4
                string[] ary = buf.Split(',');
                int minRow = 1, maxRow = 1, minCol = 1;
                if (ary.Length == 1)
                {
                    return ary[0];
                }
                else if (ary.Length == 2)
                {
                    minRow = sheet.Range[ary[0]].Row;
                    maxRow = sheet.Range[ary[1]].Row;
                    if (maxRow < minRow)
                    {
                        int tmp = maxRow; maxRow = minRow; minRow = tmp;
                    }
                    minCol = sheet.Range[ary[0]].Row;
                }
                else
                {
                    // length > 2
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
                string beginAddress = sheet.Cells[minRow, minCol].Address;
                string endAddress = sheet.Cells[maxRow, maxRow].Address;
                return beginAddress + ":" + endAddress;

            } catch (Exception ex)
            {
                _Error.AddException(ex, ToString() + ".GetRowAddressInRange");
                return "";
            } finally
            {

            }
        }


        public void Close()
        {
            try
            {

                if (this.Workbook != null)
                {
                    Marshal.ReleaseComObject(Workbook);
                    Workbook = null;
                }
                //if (this.Worksheet != null)
                //{
                //    Marshal.ReleaseComObject(Worksheet);
                //    Worksheet = null;
                //}
                System.GC.Collect();
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Close");
                return;
            }
        }
    }
}
