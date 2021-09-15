
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelWorkbookList
{
    public class ErrorCode
    {
        public int GetValueListOfAll = 1; 
        public int GetIndexList = 2;
        public int GetIndexListChecked = 3; 
        public int GetAppsInfoListFromCheckdItemList = 4;
    }
    public class ErrorMessage
    {
        public string ObjectNull = "Object Is Null";
        public string GetValueListOfAll_CountZero = "リストのアイテムがありません。";
        public string GetValueListOfAll = "リストが取得できません。";
        public string NotCheckedItemInList = "リストのアイテムが 1 つもチェックされていません。";
        public string GetIndexListChecked = "";
        public string GetAppsInfoListFromCheckdItemList = "リスト値取得エラー";
    }
    public class CheckedListBoxUtility : IWorkbookListControl
    {
        protected ErrorManager.ErrorManager _error;
        protected CheckedListBox _checkedList;
        protected ErrorCode _errorCode = new ErrorCode();
        protected ErrorMessage _errorMessage = new ErrorMessage();

        public ErrorCode ErrorCode => _errorCode;
        public ErrorMessage ErrorMessage => _errorMessage;

        public CheckedListBoxUtility(ErrorManager.ErrorManager error,CheckedListBox checkedList)
        {
            _error = error;
            _checkedList = checkedList;
        }

        //public void SetEventWhenAddValue()
        //{
        //    try
        //    {
        //        if(_checkedList == null) { throw new Exception("_checkdListBox is null"); }
        //        if (_checkedList.Items.Count < 1) { throw new Exception("_checkdListBox.Count < 1"); }
        //        foreach(CheckedListBox.ObjectCollection val in _checkedList.Items)
        //        {
                    
        //        }
        //    } catch (Exception ex)
        //    {

        //    }
        //}

        public List<string> GetValueListOfAll()
        {
            List<string> retList = new List<string>();
            string errmsg = "";
            try
            {
                if (_checkedList == null) {
                    errmsg = ErrorMessage.ObjectNull;
                    throw new Exception("CheckedList Is Null"); 
                }
                if (_checkedList.Items.Count < 1)
                {
                    errmsg = ErrorMessage.GetValueListOfAll_CountZero;
                    throw new Exception("CheckedList Item Count Is Zero"); 
                }

                for (int i = 0; i < _checkedList.Items.Count; i++)
                {
                    retList.Add(_checkedList.Items[i].ToString());
                }
                return retList;
            }
            catch (Exception ex)
            {
                if (errmsg == "") { errmsg = "不明なエラー"; }
                _error.AddException(ex, this.ToString() + ".GetValueListOfAll",errmsg);
                return retList;
            }
        }

        public List<string> GetListAll()
        {
            List<string> retList = new List<string>();
            try
            {
                if (_checkedList == null) { throw new Exception("CheckedList Is Null"); }
                if (_checkedList.Items.Count < 1) { throw new Exception("CheckedList Item Count Is Zero"); }

                for (int i = 0; i < _checkedList.Items.Count; i++)
                {
                    retList.Add(_checkedList.Items[i].ToString());
                }
                return retList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetValueListOfAll");
                return retList;
            }
        }

        /// <summary>
        /// チェックされている項目のインデックスのリストを取得する
        /// </summary>
        /// <returns></returns>
        public List<int> GetIndexListChecked()
        {
            List<int> checkedIndexList = new List<int>();
            try
            {
                if (_checkedList == null) { throw new Exception("CheckedList Is Null"); }
                if (_checkedList.Items.Count < 1) { throw new Exception("CheckedList Item Count Is Zero"); }

                for (int i = 0; i < _checkedList.Items.Count; i++)
                {
                    if (_checkedList.GetItemChecked(i))
                    {
                        checkedIndexList.Add(i);
                    }
                }
                return checkedIndexList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetIndexListChecked");
                return checkedIndexList;
            }
        }

        /// <summary>
        /// チェックされている項目の値をリストで取得する
        /// </summary>
        /// <returns></returns>
        public List<string> GetValueListOfCheckedItem()
        {
            List<string> retList = new List<string>();
            try
            {
                if (_checkedList == null) { throw new Exception("CheckedList Is Null"); }
                if (_checkedList.Items.Count < 1) { throw new Exception("CheckedList Item Count Is Zero"); }

                for (int i = 0; i < _checkedList.Items.Count; i++)
                {
                    if (_checkedList.GetItemChecked(i))
                    {
                        retList.Add(_checkedList.Items[i].ToString());
                    }
                }
                return retList;
                } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetValueListOfCheckedItem");
                return retList;
            }
        }

        // checkedBoxList から [ファイル名 or プロセス名] とプロセス ID を取得する
        // pid processName, pid FileName
        // CheckedBoxList からチェックされている FileName、pid、index を取得する
        public List<string[]> GetAppsInfoListFromCheckdItemList()
        {
            List<string[]> retList = new List<string[]>();
            int errorCode = 0;
            string errmsg = "";
            try
            {
                // リスト内の値をすべて取得する
                List<string> allValueList = GetValueListOfAll();
                if (_error.hasError) {
                    // CheckedListBox 取得エラー
                    throw new Exception("GetValueListOfAll Failed");
                    //_error.AddAlert(new Exception("GetValueListOfAll Failed"),
                    //    this.ToString() + ".GetAppsInfoListFromCheckdItemList", ErrorCode.GetValueListOfAll);
                }
                // リスト内の値から indexList を作成する
                List<int> allIndexList = GetIndexList(allValueList);
                if (_error.hasError)
                {
                    errorCode = ErrorCode.GetIndexList;
                    throw new Exception("GetIndexList Failed");
                    // CheckedListBox 取得エラー
                    //_error.AddAlert(new Exception("GetIndexList Failed"),
                    //    this.ToString() + ".GetAppsInfoListFromCheckdItemList", ErrorCode.GetValueListOfAll);
                }

                // チェックされている位置のリストを取得する
                List<int> checkedIndexList = GetIndexListChecked();
                if (_error.hasError)
                {
                    errorCode = ErrorCode.GetIndexListChecked;
                    throw new Exception("GetIndexListChecked Failed");
                }
                if (checkedIndexList.Count < 1)
                {
                    errmsg = ErrorMessage.NotCheckedItemInList;
                    throw new Exception("checkedIndexList.Count < 1");
                }

                    // checkedBoxList から [ファイル名 or プロセス名] とプロセス ID を取得する
                    // pid processName, pid FileName
                    // CheckedBoxList からチェックされている FileName、pid、index を取得する
                    retList = GetAppsInfoListFromCheckdItemList(
                    allValueList, allIndexList,checkedIndexList);
                if (_error.hasError)
                {
                    errmsg = ErrorMessage.GetAppsInfoListFromCheckdItemList;
                    errorCode = ErrorCode.GetAppsInfoListFromCheckdItemList;
                    throw new Exception("GetAppsInfoListFromCheckdItemList Failed");
                }

                return retList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetValueListOfCheckedItem", errmsg);
                return retList;
            }
        }

        // filename,pid,index の ArrayString を返す
        public string[] GetSelectedItem()
        {
            try
            {
                if(_checkedList == null) { throw new Exception("CheckedBoxList is null"); }
                if (_checkedList.Items.Count < 1) { throw new Exception("CheckedBoxList.Count < 1"); }

                List<string> allValues = GetValueListOfAll();
                List<int> indexList = GetIndexList(allValues);

                // selectedIndexList
                List<int> selectedindexList = new List<int>();
                for (int i=0; i<_checkedList.Items.Count; i++)
                {
                    if (_checkedList.GetSelected(i) == true)
                    {
                        selectedindexList.Add(i);
                    }
                }
                if (selectedindexList.Count < 1)
                {
                    _error.AddLog("CheckedListBox.Items.GetSelected Is Nothing");
                    return null;
                }
                //make info
                List<string[]> list = GetAppsInfoListFromCheckdItemList(allValues, indexList, selectedindexList);
                return list[0];
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetSelectedItem");
                return null;
            }
        }

        //private string ConvertToStringArray(string value)
        //{
        //    try
        //    {
        //        return "";
        //    } catch (Exception ex)
        //    {
        //        _error.AddException(ex, this.ToString() + ".ConvertToStringArray");
        //        return "";
        //    }
        //}

        // pid が異なる場合があるので比較しながら Appliction の indexList を作成する
        private List<int> GetIndexList(List<string> allValueList)
        {
            List<int> indexList = new List<int>();
            try
            {
                string value;
                int beforePid = 0;
                int count = 0;
                for (int i = 0; i < allValueList.Count; i++)
                {
                    value = allValueList[i];
                    // [0000] FileName.xls 形式から ProcessId を取得する
                    value = value.Substring(1, value.IndexOf("] ") - 1);
                    bool canConvert = int.TryParse(value, out int nowPid);
                    if (!canConvert)
                    {
                        nowPid = 0;
                    }
                    // index For ExcelApplist 
                    if (beforePid == nowPid)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }
                    beforePid = nowPid;
                    indexList.Add(count);
                }
                return indexList;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetIndexList");
                return indexList;
            }
        }

        // List<AppsInfo>
        private List<string[]> GetAppsInfoListFromCheckdItemList(
            List<string> allValueList, List<int> allIndexList, List<int> checkedIndexList)
        {
            try
            {
                List<string[]> retList = new List<string[]>();
                string[] tempInfo;
                int count = 0;
                // valueList.Count チェック済み
                for (int i = 0; i < checkedIndexList.Count; i++)
                {
                    count = checkedIndexList[i];
                    tempInfo = GetAppsInfoFromCheckdItemValue(allValueList[count], allIndexList[count]);
                    retList.Add(tempInfo);
                    if (_error.hasAlert) { _error.AddLogAlert("GetAppsInfoListFromCheckdItemList Failed"); return retList; }
                }
                return retList;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetAppsInfoListFromCheckdItemList");
                return null;
            }
        }

        private class info
        {
            public string FileName;
            public int ProcessId;
            public string Index;
        }

        private string[] GetAppsInfoFromCheckdItemValue(string value, int index)
        {
            //string[] ret = null;
            info info=new info();
            try
            {
                
                if (value != "")
                {
                    info = new info();
                    string str = "] ";
                    // [0000] FileName.xls 形式からファイル名を取得・設定する
                    info.FileName = value.Substring(value.IndexOf(str) + str.Length);

                    // [0000] FileName.xls 形式から ProcessId を取得・設定する
                    value = value.Substring(1, value.IndexOf("] ") - 1);
                    bool canConvert = int.TryParse(value, out int pid);
                    if (!canConvert)
                    {
                        info.ProcessId = 0;
                    }
                    else
                    {
                        info.ProcessId = pid;
                    }
                    // index For ExcelApplist 
                    info.Index = index.ToString();
                }
                return new string[] { info.FileName,info.ProcessId.ToString(),info.Index};
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetAppsInfoFromCheckdItemValue");
                return null;
            }
        }


        public int GetIndexForExcelAppsFromCheckedListBox(string bookName)
        {
            try
            {
                //_error.ClearError();
                _error.AddLog(this.ToString() + "GetIndexForExcelAppsFromCheckedListBox");
                // すべての値を取得
                List<string> allValueList = GetValueListOfAll();
                if (_error.hasError)
                { _error.AddLogAlert(this.ToString()+".GetValueListOfAll Failed(1)"); return -1; }
                if (allValueList.Count < 1) { _error.AddLogAlert(this.ToString() + ".ItemList.Count Is Zero"); return -1; }

                // indexList を作成する、Workbook 指定用
                List<int> allIndexList = GetIndexList(allValueList);
                if (_error.hasAlert)
                { _error.AddLogAlert(this.ToString() + ".GetIndexList Failed(1)"); return -1; }
                if (allIndexList.Count < 1) { _error.AddLogAlert(this.ToString() + ".ItemList.Count Is Zero"); return -1; }

                string value;
                string nowBook = "";
                string str = "] ";
                for (int i = 0; i < allValueList.Count; i++)
                {
                    value = allValueList[i];
                    // [0000] FileName.xls 形式からファイル名を取得・設定する
                    nowBook = value.Substring(value.IndexOf(str) + str.Length);
                    if (nowBook == bookName)
                    {
                        return allIndexList[i];
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetIndexForExcelAppsFromCheckedListBox");
                return -2;
            }
        }

        public void UpdateItemListAfterClearList(List<string> addList)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".UpdateItemListAfterClearList");
                if (addList == null) { _error.AddLogAlert("AddList is Null"); return; }
                ClearAllCheckBox();
                if (addList.Count < 1) { _error.AddLog("addList.Count < 1"); return; }
                if (_error.hasError) { _error.AddLogAlert("ClearAllCheckBox Failed"); _error.ReleaseErrorState(); }
                _checkedList.Items.AddRange(addList.ToArray());

            }
            catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".UpdateItemListAfterClearList");
            }
        }

        /// <summary>
        /// CheckedBoxList の Item をすべてクリアする
        /// </summary>
        private void ClearAllCheckBox()
        {
            try
            {
                if (_checkedList.Items.Count < 1) { return; }
                int max = _checkedList.Items.Count;
                for (int i = 0; i <= max; i++)
                {
                    _checkedList.Items.RemoveAt(0);
                    max = _checkedList.Items.Count;
                    if (max < 1) { break; }
                    i = 0;
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ClearAllCheckBox");
            }
        }
    }
}
