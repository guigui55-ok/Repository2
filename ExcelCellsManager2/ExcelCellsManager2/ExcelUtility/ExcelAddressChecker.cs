using ExcelUtility;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExcelUtility.ExcelObject.ExcelCells;

namespace ExcelUtility
{
    public class ExcelAddressChecker
    {
        protected ErrorManager.ErrorManager _error;
        protected string _address;
        protected string[] _addressArray;
        protected List<IndexList> _indexList;
        protected int _index = 0;
        protected int _maxindex;
        protected ExcelValueFinder _addressUtil;

        public class IndexList
        {
            public IndexList(int r,int c) { Row = r; Col = c; }
            public long Row = 0;
            public long Col = 0;
        }
        public ExcelAddressChecker(ErrorManager.ErrorManager error, string address)
        {
            _error = error;
            _address = address;
            _addressUtil = new ExcelValueFinder(error);
        }
        public string Address { get { return _address; } }
        public long GetMaxCount() { return _maxindex;  }

        // リセットする
        private void SetAddress(in Worksheet worksheet,string address, XlSearchOrder order)
        {
            try
            {
                _address = address;
                AnalyzeAddress(worksheet, order);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetAddress");
            }
        }

        // 
        public void AnalyzeAddress(in Worksheet worksheet,XlSearchOrder order)
        {
            try
            {
                _addressArray = _address.Split(',');
                _index = 0;
                SetIndexList(worksheet, order);
                _maxindex = CellsCount(worksheet,_address);
                if (_error.HasException()) { return; }

            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString() + ".AnalyzeAddress");
            }
        }

        public int CellsCount(in Worksheet worksheet,string address)
        {
            try
            {
                string[] addressArray = address.Split(',');
                int count = 0;
                int tmpRow = 0, tmpCol = 0;
                string nowAddress = "";
                if (addressArray.Length > 0)
                {
                    for(int i = 0; i< addressArray.Length; i++)
                    {
                        nowAddress = addressArray[i];
                        tmpRow = worksheet.Range[nowAddress].Rows.Count;
                        tmpCol = worksheet.Range[nowAddress].Columns.Count;
                        count += tmpRow * tmpCol;
                    }
                } else
                {
                    return 0;
                }
                return count;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".CellsCount");
                return 0;
            }
        }


        public void SetIndexList(in Worksheet worksheet,XlSearchOrder order)
        {
            try
            {
                _indexList = new List<IndexList>();
                int beginRow = 0, endRow = 0, beginCol = 0, endCol = 0;
                string beginCornerAddres = "";
                string nowAddress = "";
                if (_addressArray.Length > 0)
                {
                    for (int i = 0; i < _addressArray.Length; i++)
                    {
                        nowAddress = _addressArray[i];
                        // Ready
                        beginCornerAddres = _addressUtil.GetCornerAddressFromRangeAddress(
                            worksheet, nowAddress,
                            ExcelValueFinder.ExcelValueFinderConstants.DIRECTION_LEFT |
                            ExcelValueFinder.ExcelValueFinderConstants.DIRECTION_UP);

                        beginRow = worksheet.Range[beginCornerAddres].Row;
                        beginCol = worksheet.Range[beginCornerAddres].Column;

                        endRow = beginRow + worksheet.Range[nowAddress].Rows.Count - 1;
                        endCol = beginCol + worksheet.Range[nowAddress].Columns.Count - 1;
                        // Save Index
                        // Column
                        if(order == XlSearchOrder.xlByColumns)
                        {
                            for (int r = beginRow; r <= endRow; r++)
                            {
                                for (int c = beginCol; c <= endCol; c++)
                                {
                                    _indexList.Add(new IndexList(r, c));
                                }
                            }
                        } else if(order == XlSearchOrder.xlByRows)
                        {
                            // For ループのRowとColumnをを逆にしている
                            for (int c = beginCol; c <= endCol; c++)
                            {
                                for (int r = beginRow; r <= endRow; r++)
                                {
                                    _indexList.Add(new IndexList(r, c));
                                }
                            }
                        } else
                        {
                            throw new Exception("SearchOrder Is Invalid");
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetIndexList");
            }
        }


        public string GetAddress(in Worksheet worksheet)
        {
            try
            {
                return worksheet.Cells[_indexList[_index].Row, _indexList[_index].Col].Address;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".getAddress");
                return "";
            }
        }
        public void MoveNext()
        {
            try
            {
                _index++;
                if(_index > _maxindex)
                {
                    _index = _maxindex;
                }
            }catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MoveNext");
            }
        }
        public void MovePrevious()
        {
            try
            {
                _index--;
                if (_index < 0)
                {
                    _index = 0;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MovePrevious");
            }
        }

        public bool IsEOA()
        {
            try
            {
                if (_index >= _maxindex) { return true; }
                return false;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".IsEOA");
                return false;
            }
        }
        public bool IsBOA()
        {
            try
            {
                if (_index <= 0) { return true; }
                return false;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".IsBOA");
                return false;
            }
        }
    }
}
