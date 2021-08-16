using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelIO
{
    public class ExcelFinderForExcelApps
    {
        protected ErrorManager.ErrorManager _Error;
        protected List<ExcelApps> _ExcelAppsList;
        protected List<AppsInfo> _TargetInfoList;
        protected ExcelFinder finder;

        public ExcelFinderForExcelApps(ErrorManager.ErrorManager error,List<ExcelApps> appsList,List<AppsInfo> infoList)
        {
            try
            {
                _Error = error;
                if(appsList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if (appsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                _ExcelAppsList = appsList;
                if (infoList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if (infoList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                _TargetInfoList = infoList;
                finder = new ExcelFinder(_Error);
            } catch (Exception ex)
            {
                if(_Error == null)
                {
                    _Error = new ErrorManager.ErrorManager(1);
                } else
                {
                    _Error.AddException(ex, this.ToString() + ".ExcelFinderForExcelApps Constracta Failed.");
                }
            }
        }

        // ExcelConditions、Workbook のセットがあらかじめ必要
        public void ExcuteFind(ExcelFindSearchConditions conditions)
        {
            try
            {
                // appslist 取得
                // ※ ExcelAppsList が必要
                if (_ExcelAppsList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if (_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                if (_TargetInfoList== null) { throw new Exception("TargetInfoList Is Null"); }
                if (_TargetInfoList.Count < 1) { throw new Exception("TargetInfoList.Count Is Zero"); }

                // 現在 Activate している Wrokbook の AppsInfo を取得する
                AppsInfo nowActivateInfo = GetExcelAppsActiveWorkbookFromExcelAppsList();
                if (_Error.HasException()) { return; }

                // 検索対象のリストに現在のApplication、ActiveWorkbook が含まれいるか
                if (!IsExistsActivateWrokbookInTargetInfoList(nowActivateInfo))
                {
                    if (_Error.HasException()) { return; } // 失敗時は False を返すのでここでエラーチェックする
                    // 含まれていなければ、AppsList の最初の Workbook を ActiveBook に設定する
                    // 現在の Activate している Workbook が TargetInfoList 内にない場合の検索対象は
                    // TargetInfoList の Index=0 にする
                    nowActivateInfo = GetAppsInfoInFirstWorkbookInFirstTargetInfoList();
                    if (_Error.HasException()) { return; }
                }

                // すべて検索する場合
                // FindedFlagで管理する？ or 検索最初のシートインデックスとworkbookNameの一つ前までにする？
                // 保留

                // Workbook
                ExcelApps targetApps = GetExcelAppsByAppsInfoFromExcelAppsList(nowActivateInfo);
                if (_Error.HasException()) { return; }
                // 変数 Workbook SheetName をセットする
                finder.Workbook = targetApps.Application.Workbooks[nowActivateInfo.FileName];
                string sheetName = targetApps.Application.ActiveSheet.Name;

                // Activate する
                targetApps.ActivateWorkbook(nowActivateInfo);
                finder.Workbook.Sheets[sheetName].Activate();

                // Application.Selection Application.ActiveCell が複数の場合は FindRange に設定する
                bool ret = finder.SetFindAddressFromSelectionOrActiveCell(targetApps.Application, sheetName, conditions);
                if (_Error.HasException()) { return; }
                if (!ret)
                {
                    // Selection,ActiveCell が単一でない場合
                }
                // FindRange をチェックする
                finder.SetFindAddressFromConditions(sheetName, conditions);
                if (_Error.HasException()) { return; }
                Console.WriteLine("conditions.FindRange : " + conditions.FindRange);


                // ActiveCell を取得する
                //conditions.BeginAddress = targetApps.Application.ActiveCell.Address;
                // 検索開始アドレスAfterAddress を設定する
                finder.SetAfterRange(targetApps.Application, sheetName, conditions);
                if (_Error.HasException()) { return; }
                Console.WriteLine("conditions.AfterAddress : " + conditions.AfterAddress);

                //finder.SetAfterIfIncludeSearchValue(sheetName, conditions);
                //if (_Error.HasException()) { return; }

                // 次へ検索
                finder.SelectRangeByFindNext(targetApps.Application, sheetName, conditions);
                if (_Error.HasException()) { return; }

                // ブック、アプリケーションフラグがONの場合、検索 Match しなければ次のシートへ
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ExcuteFind");
                return;
            }
        }

        // ExcelAppsList から AppsList と一致する ExcelApps を取得する
        private ExcelApps GetExcelAppsByAppsInfoFromExcelAppsList(AppsInfo info)
        {
            try
            {
                if (_ExcelAppsList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if (_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }

                foreach (ExcelApps apps in _ExcelAppsList)
                {
                    if (apps.IsExistsWorkbookInApplication(info.FileName))
                    {
                        return apps;
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetExcelAppsByAppsInfoFromExcelAppsList");
                return null;
            }
        }

        // 現在の Activate している Workbook が TargetInfoList 内にない場合の検索対象は
        // TargetInfoList の Index=0 の Wrokbooks とする
        // TargetInfoList の Wrokbooks がない場合は、その次の Index とする
        private AppsInfo GetAppsInfoInFirstWorkbookInFirstTargetInfoList()
        {
            AppsInfo retInfo = null;
            try
            {
                // _TargetInfoList Null チェック済み
                // _ExcelAppsList Null チェック済み
                bool isExists = false;
                foreach (AppsInfo info in _TargetInfoList)
                {
                    foreach(ExcelApps apps in _ExcelAppsList)
                    {
                        if (apps.GetWorkBooksCount() > 0)
                        {
                            for (int i=1; i<=apps.Application.Workbooks.Count; i++)
                            {
                                if(info.FileName == apps.Application.Workbooks[i].Name)
                                {
                                    isExists = true;
                                    retInfo = new AppsInfo
                                    {
                                        FileName = apps.Application.Workbooks[i].Name,
                                        Index = i,
                                        ProcessId = apps.ProcessId
                                    };
                                    return retInfo;
                                }
                            }
                        }
                    } // _ExcelAppsList Loop End
                } // _TargetInfoList Loop End


                if (isExists)
                {
                    // _TargetInfoList が _ExcelAppsList にない
                    throw new Exception("TargetInfoList All Value Is Nothing In ExcelAppsList");
                }
                return retInfo;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ExcuteFind");
                return retInfo;
            }
        }


        // 検索対象の AppdInfo リストに現在の Application、ActiveWorkbook が含まれいるか
        private bool IsExistsActivateWrokbookInTargetInfoList(AppsInfo nowActivateInfo)
        {
            try
            {
                // TargetInfoList Null チェック済み
                foreach(AppsInfo info in _TargetInfoList)
                {
                    if ((nowActivateInfo.FileName == info.FileName) && 
                       (nowActivateInfo.Index == info.Index) && 
                       (nowActivateInfo.ProcessId == info.ProcessId))
                    {
                        // FileName,Index,ProcessId すべて合致するものがあればそのまま返す
                        return true;
                    }
                }
                return false;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".IsExistsActivateWrokbookInTargetInfoList");
                return false;
            }
        }

        // 現在 Activate している Wrokbook の AppsInfo を取得する
        private AppsInfo GetExcelAppsActiveWorkbookFromExcelAppsList()
        {
            try
            {
                if(_ExcelAppsList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if(_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                bool isMatch = false;
                AppsInfo retInfo = null;
                // Application Loop
                foreach (ExcelApps apps in _ExcelAppsList)
                {
                    if (apps.IsActivate > 0)
                    {
                        // Workbooks Loop
                        for(int i = 0; i<=apps.Application.Workbooks.Count; i++)
                        {
                            if(apps.IsActivate == i)
                            {
                                if (retInfo == null)
                                {
                                    // Match First
                                    retInfo = new AppsInfo
                                    {
                                        FileName = apps.Application.Workbooks[i].Name,
                                        Index = i,
                                        ProcessId = apps.ProcessId
                                    };
                                    isMatch = true;
                                }
                            }
                        }
                    }
                    // Workbooks Loop End
                }
                // Application Loop End

                if (!isMatch)
                {
                    // ひとつも Match していない
                    // まだ設定されていない or 有効な Wrokbook が一つもない
                    // ないときは、リストの最後の ExcelApps の最初の Wrokbook にセットする
                    retInfo = SetActivateFlagToFirstWrokbookInExcelAppsListOfLastIndex();
                    if (_Error.HasException()) { return null; }
                    if (retInfo == null)
                    {
                        // 有効な Wrokbook が一つもない
                        // 警告を出す
                        _Error.AddException(
                            new Exception("Wroksheet IsActivate Flag Index Is Not Exists In Application"),
                            this.ToString() + ".ActiveWorkbookIsExsist",
                            _Error.Constants.TYPE_WARNING);
                    }
                }
                return retInfo;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".ActiveWorkbookIsExsist");
                return null;
            }
        }

        // IsActivate フラグが一つもないときにセットする
        // 最後の ExcelAppsList の最初の Wrokbook にセットする
        // ない場合は要素をさかのぼってセットする
        private AppsInfo SetActivateFlagToFirstWrokbookInExcelAppsListOfLastIndex()
        {
            AppsInfo retInfo = null;
            try
            {
                if (_ExcelAppsList == null) { throw new Exception("ExcelAppsList Is Null"); }
                if (_ExcelAppsList.Count < 1) { throw new Exception("ExcelAppsList.Count Is Zero"); }
                ExcelApps tmpApps = null;
                bool flag = false;
                for(int i=_ExcelAppsList.Count-1; i>=0; i--)
                {
                    tmpApps = _ExcelAppsList[i];
                    if(tmpApps.GetWorkBooksCount() < 1)
                    {
                        // 有効な Workbook がない→次へ
                        tmpApps.IsActivate = 0;
                        continue;
                    } else
                    {
                        // 有効な Workbook がある
                        flag = true;
                        retInfo = new AppsInfo
                        {
                            FileName = tmpApps.Application.Workbooks[1].Name,
                            Index = 1,
                            ProcessId = tmpApps.ProcessId
                        };
                        return retInfo;
                    }
                }
                if (!flag)
                {
                    // 一つも設定できていない
                    // 警告を出す
                    _Error.AddException(
                        new Exception("Valid Wrokbook Is Nothing In All Of ExcelAppsList "),
                        this.ToString() + ".SetActivateFlagToFirstWrokbookInExcelAppsListOfLastIndex",
                        _Error.Constants.TYPE_WARNING);
                }
                return retInfo;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".SetActivateFlagToFirstWrokbookInExcelAppsListOfLastIndex");
                return null;
            }
        }
        public void Close()
        {
            try
            {
                if(finder != null)
                {
                    finder.Close();
                    finder = null;
                }
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Close");
                return;
            }
        }
    }
}
