using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Common;
using ErrorLog;

namespace FileList
{
    public class FileListRegister
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected List<string> _fileList;
        //protected List<string> _randomList;
        //protected List<int> _indexList;
        //public int NowIndex = 0;
        //protected string NowFolder = "";
        // ファイルリストは存在するが、すべてのファイルが条件に合わない場合true(すべてNotInclude、すべてIncludeと合わない場合)
        public bool IsNotConditionAllOfList = true;
        protected List<string> _includeFileTypeList;
        protected List<string> _notIncludeFileTypeList;
        protected List<string> _includeFileNameList;
        protected List<string> _notIncludeFileNameList;
        // フォルダ読み込み時の階層
        protected int ReadFolderHierarchy = 1;
        // ショートカットの元を読み取る
        public bool IsReadSourceOfShotcut = true;

        public List<string> IncludeFileTypeList {
            get { return _includeFileTypeList; }
            set { _includeFileTypeList = value; }
        }
        public List<string> NotIncludeFileTypeList
        {
            get { return _notIncludeFileTypeList; }
            set { _notIncludeFileTypeList = value; }
        }
        public List<string> IncludeFileNameList
        {
            get { return _includeFileNameList; }
            set { _includeFileNameList = value; }
        }
        public List<string> NotIncludeFileNameList
        {
            get { return _notIncludeFileNameList; }
            set { _notIncludeFileNameList = value; }
        }
        public FileListRegister(IErrorLog errorLog)
        {
            _errorLog = errorLog;

            // エラー抑制
            if (_errorLog == null) { DebugPrintList(this.GetList()); }
        }

        //public int SetErrorLog(ErrorLog.IErrorLog errorLog)
        //{
        //    try
        //    {
        //        if (errorLog is ErrorLog.ErrorLog)
        //        {
        //            _errorLog = errorLog;
        //            return 1;
        //        } else { return -1; }
        //    }
        //    catch (Exception ex) {
        //        Debug.WriteLine(this.ToString() + ".setErrorLog failed");
        //        Debug.WriteLine(ex.Message);  return 0; }
        //}

        public FileListRegister(List<string> list)
        {
            _fileList = list;
        }

        public List<string> GetList()
        {
            return _fileList;
        }

        private void DebugPrintList(List<string> list)
        {
            foreach(var val in list)
            {
                Debug.WriteLine(val);
            }
        }
        public int GetListCount()
        {
            try
            {
                if (_fileList == null) { return -2; }
                return _fileList.Count;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex,this.ToString(), "getListCount Failed");
                return -1;
            }
        }

        // DragAndDrop用
        public int SetFileList(List<string> list)
        {
            try
            {
                // includeListの空要素を削除
                int ret = RemoveBlankValueInIncludeList();
                if (ret < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString(), "setFileList : RemoveBlankValueInIncludeList Failed");
                }

                _fileList = new List<string>();
                
                //DebugPrintList(list);
                // リストがない
                if (list.Count < 1) { return -1; }
                // ファイルを全て読み込み
                ret = MakeFileList(list,_fileList,0);
                if(ret < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString(), "setFileList : MakeFileList Failed");
                }
                // 条件以外を除外
                ret = SetFileListWithApplyConditions(_fileList);
                if (ret < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString(), "setFileList : setFileListWithApplyConditions Failed");
                }
                // 単に追加
                //_fileList = list;
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setFileList");
                return 0;
            }
        }

        private int RemoveBlankValueInIncludeList()
        {
            try
            {
                int ret = RemoveBlankValueInList(_includeFileTypeList);
                if (ret < 1) { _errorLog.AddErrorNotException(this.ToString(), "RemoveBlankValueInList failed : _includeFileTypeList"); }
                 ret = RemoveBlankValueInList(_notIncludeFileTypeList);
                if (ret < 1) { _errorLog.AddErrorNotException(this.ToString(), "RemoveBlankValueInList failed : _notIncludeFileTypeList"); }
                 ret = RemoveBlankValueInList(_includeFileNameList);
                if (ret < 1) { _errorLog.AddErrorNotException(this.ToString(), "RemoveBlankValueInList failed : _includeFileNameList"); }
                 ret = RemoveBlankValueInList(_notIncludeFileNameList);
                if (ret < 1){ _errorLog.AddErrorNotException(this.ToString(), "RemoveBlankValueInList failed : _notIncludeFileNameList"); }
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "RemoveBlankValueInIncludeList");
                return 0;
            }
        }

        private int RemoveBlankValueInList(List<string> list)
        {
            try
            {
                if (list == null) { return 1; }
                for(int i= 0; i < list.Count; i++)
                {
                    if (list[i].Length < 1)
                    {
                        list.RemoveAt(i);
                        // リストがなくなったらbreak
                        if (list.Count < 1) { break; }
                        // インデックスが変わるのでもう一度
                        i--;
                        // 最初の場合
                        if (i < 0) { i = 0; }
                        // 最後の場合
                        if (i >= list.Count) { break; }
                    }
                }
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "RemoveBlankValueInList");
                return 0;
            }
        }

        // ファイル読み込み
        private int MakeFileList(List<string> list,List<string> newList, int NowHierarchy)
        {
            try
            {
                if (list == null) { _errorLog.AddErrorNotException(this.ToString(),"list is null"); return -1; }

                List<string> folderList = new List<string>();

                string tempPath = "";
                // ルートのファイルのみを追加
                foreach (string listValue in list)
                {
                    tempPath = listValue;
                    // ショートカットの場合
                    if (IsReadSourceOfShotcut)
                    {
                        if (Path.GetExtension(listValue).CompareTo(".lnk") == 0)
                        {
                            tempPath = new ReadShortCut().GetSourceFromPath(tempPath);

                        } else { /* ショートカットではない */ tempPath = listValue; }
                    }
                    else { /* ショートカットの元は読み取らない場合 */ tempPath = listValue; }

                    if (File.Exists(tempPath))
                    {
                        newList.Add(tempPath);
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(tempPath))
                        {
                            folderList.Add(tempPath);
                        }
                        else
                        {
                            // ファイルでもフォルダでもない
                        }
                    }
                }
                if (folderList.Count < 1) { return 1; }

                foreach (string listValue in folderList)
                {
                    if (NowHierarchy < ReadFolderHierarchy)
                    {
                        string[] files = System.IO.Directory.GetFiles(listValue,"*", System.IO.SearchOption.AllDirectories);
                        MakeFileList(new List<string>(files),newList,NowHierarchy);
                    }
                }
                _fileList = newList;
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "MakeFileList");
                return 0;
            }
        }

        // List からファイルのみを抽出、フォルダを除外
        //private List<string> removeFolderFromList(List<string> list)
        //{
        //    try
        //    {
        //        for (int i = 0; i < list.Count-1; i++)
        //        {
        //            if (System.IO.Directory.Exists(list[i]))
        //            {
        //                list.RemoveAt(i);
        //                // リストがなくなったらbreak
        //                if (list.Count < 1) { break; }
        //                // インデックスが変わるのでもう一度
        //                i--;
        //                // 最初の場合
        //                if (i < 0) { i = 0; }
        //                // 最後の場合
        //                if (i >= list.Count) { break; }
        //            }
        //        }
        //        return list;
        //    } catch (Exception ex)
        //    {
        //        _errorLog.AddException(ex, this.ToString(), "removeFolderFromList");
        //        return new List<string>();
        //    }
        //}

        // 戻り値 1 : フォルダしかない場合、2 : ファイルしかない、3 :　ファイルフォルダ混在
        // 最初のフォルダを読み込む

        //private int ListIsStateIncludingFolder(List<string> list)
        //{
        //    try
        //    {
        //        bool retFile = false;
        //        bool retFolder = false;
        //        foreach(string value in list)
        //        {
        //            if (System.IO.Directory.Exists(value))
        //            {
        //                retFolder = true;
        //            }else
        //            {
        //                if (System.IO.File.Exists(value))
        //                {
        //                    retFile = true;
        //                }
        //            }
        //            // すでに両方あることがわかっている場合終了
        //            if (retFolder && retFile)
        //            {
        //                return 3;
        //            }
        //        }
        //        // 
        //        if ( retFolder && (!retFile))
        //        {
        //            return 1;
        //        }
        //        if ((!retFolder) && retFile)
        //        {
        //            return 2;
        //        }
        //        return -1;
        //    } catch (Exception ex)
        //    {
        //        _errorLog.AddException(ex, this.ToString(), "ListIsFolderOnly");
        //        return 0;
        //    }
        //}

        // MakeFileの後に実行
        // 条件に合うファイルのみリストを作成
        public int SetFileListWithApplyConditions(List<string> list)
        {
            try
            {
                if (list == null) { _errorLog.AddErrorNotException(this.ToString(), "setFileListWithApplyConditions failed. list is null"); return -1; }
                if (list.Count < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString(), "setFileListWithApplyConditions : "+ "List Count Zero");
                    return -1;
                }
                // ファイル名、拡張子を指定したもののみのListにする
                list = GetListApplyIncludeFiletype(list);

                // 単に追加
                _fileList = list;
                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setFileList");
                return 0;
            }
        }

        // ファイル名、拡張子を指定したもののみのListにする
        private List<string> GetListApplyIncludeFiletype(List<string> list)
        {
            try
            {
                bool isRemove = false;
                for (int i=0; i < list.Count; i++)
                {
                    isRemove = false;
                    // ファイル名長さゼロを全て除外
                    if (list[i].Length < 1) {
                        isRemove = true;
                    }
                    // 拡張子チェック
                    if (!isRemove)
                    {
                        //if (i == 0)
                        //{
                        //    i = i;
                        //}
                        isRemove = IsIncludeFileType(list[i]);
                    }
                    // ファイル名チェック
                    if (!isRemove)
                    {
                        isRemove = IsIncludeFileName(list[i]);
                    }
                    // remove
                    if (isRemove)
                    {
                        list.RemoveAt(i);
                        // リストがなくなったらbreak
                        if (list.Count < 1) { break; }
                        // インデックスが変わるのでもう一度
                        i--;
                        // 最初の場合
                        //if (i < 0) { i = 0; }
                        // 最後の場合
                        if (i >= list.Count) { break; }
                    }
                } // for end
                return list;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setFileList");
                return list;
            }
        }


        // ファイル名が条件に合うか
        //private bool IsMeetConditions(string path,string type)
        //{
        //    try
        //    {
        //        if (type == null) { }
        //        // 拡張子チェック
        //        bool ret = IsIncludeFileType(path);
        //        if (!ret) { return false; }
        //        // ファイル名チェック
        //        ret = IsIncludeFileName(path);
        //        return ret;

        //    } catch (Exception ex)
        //    {
        //        _errorLog.AddException(ex, this.ToString(), "isMeetConditions");
        //        return false;
        //    }
        //}

        private bool IsIncludeFileType(string path)
        {
            try
            {
                bool ret = false;
                string type = "";
                if (!((_includeFileTypeList == null) && (_notIncludeFileTypeList == null)))
                {
                    // get file type
                    type = GetFileTypeFromFilePath(path);
                } else { /* 両方値がない */ return false; }

                if (_includeFileTypeList == null)
                {
                    // Listがない場合はスルー
                    ret = false; 
                } else
                {
                    // TypeListがない場合はすべてtrue
                    if (_includeFileTypeList.Count >= 1)
                    {
                        // Listがある場合
                        // 拡張子がない場合 & IncludeFileTypeList が1つ以上の場合はすべてfalse、_notIncludeあるなしに関わらない
                        if (type.Length < 1) { 
                            ret = false; 
                        } else
                        {
                            // リスト内に一致するものがあるか　IncludeType
                            if (_includeFileTypeList.Count > 0) // リストがある時
                            {
                                if (!ValueIsMatchInList(_includeFileTypeList, type))
                                {
                                    // 1つも一致しない
                                    ret = true;
                                }
                                // 一致する　場合は継続
                            }
                        }

                    }
                }

                // not include を優先する
                if (_notIncludeFileTypeList == null) { return false; }
                if (_notIncludeFileTypeList.Count >= 1)
                {
                    // Listがある場合
                    // 拡張子がない場合 & IncludeFileTypeList が1つ以上の場合はすべてfalse、上記_includeFileTypeListがない場合ここに来る
                    if (type.Length < 1) { return false; }

                    // リスト内に一致するものがあるか　NotIncludeType
                    if (_notIncludeFileTypeList.Count > 0) // リストがある時
                    {
                        if (ValueIsMatchInList(_notIncludeFileTypeList, type))
                        {
                            // 一致するものがある
                            return true;
                        }
                        // 一致する物がない場合　場合は継続
                    }
                }
                // Listがない場合はスルー

                // すべてクリア
                return ret;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "isIncludeFileType");
                return false;
            }
        }

        private bool IsIncludeFileName(string path)
        {
            try
            {
                bool ret = false;
                string filename = "";
                if (!((_includeFileNameList == null) && (_notIncludeFileNameList == null)))
                {
                    // get file type
                    filename = GetFileNameFromFilePath(path);
                }
                else { /* 両方値がない */ return false; }

                if (_includeFileNameList == null)
                {
                    // Listがない場合はスルー
                    ret = false; 
                } else
                {
                    // TypeListがない場合はすべてtrue
                    if (_includeFileNameList.Count >= 1)
                    {
                        // Listがある場合
                        // ファイル名がない場合 & _includeFileNameList が1つ以上の場合はすべてfalse、_notIncludeあるなしに関わらない
                        if (filename.Length < 1) { 
                            ret = false; 
                        } else
                        {
                            // リスト内に一致するものがあるか　IncludeType
                            if (_includeFileNameList.Count > 0) // リストがある時
                            {
                                if (!ValueIsIncludeInList(_includeFileNameList, filename))
                                {
                                    // 1つも一致しない
                                    ret = true;
                                }
                                // 一致する　場合は継続
                            }
                        }
                    }
                }

                // not include を優先
                if (_notIncludeFileNameList == null) { return false; }
                if (_notIncludeFileNameList.Count >= 1)
                {
                    // Listがある場合
                    // ファイル名がない場合 & IncludeFileTypeList が1つ以上の場合はすべてfalse、上記 _includeFileNameList がない場合ここに来る
                    if (filename.Length < 1) { return false; }

                    // リスト内に一致するものがあるか　NotIncludeType
                    if (_notIncludeFileNameList.Count > 0) // リストがある時
                    {
                        if (ValueIsIncludeInList(_notIncludeFileNameList, filename))
                        {
                            // 一致するものがある
                            return true;
                        }
                        // 一致する物がない場合　場合は継続
                    }
                }
                // Listがない場合はスルー

                // すべてクリア
                return ret;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "isIncludeFileName");
                return false;
            }
        }


        // リスト処理、list内にvalueと一致するものがあるか_完全一致
        private bool ValueIsMatchInList(List<string> list,string value)
        {
            try
            {
                foreach (string listValue in list)
                {
                    //　比較
                    if (value.CompareTo(listValue) == 0)
                    {
                        // 一致する
                        return true;
                    }
                }
                // 一度も一致しない
                return false;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ValueIsMatchInList");
                return false;
            }
        }

        //リスト処理、List内のvlaueが含まれるものがあるか_部分一致
        private bool ValueIsIncludeInList(List<string> list,string value)
        {
            try
            {
                foreach (string listValue in list)
                {
                    //　比較
                    if (value.IndexOf(listValue) >= 0)
                    {
                        // 部分一致する
                        return true;
                    }
                }
                // 一度も部分一致しない
                return false;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ValueIsIncludeInList");
                return false;
            }
        }


        // 拡張子を取得
        private string GetFileTypeFromFilePath(string value)
        {
            try
            {
                // 文字列長さが０
                if (value.Length < 1) { return ""; }
                // 最後からのピリオドの位置
                int dotpos = value.LastIndexOf('.');
                // ピリオドがない
                if (dotpos < 1) { return ""; }
                // 拡張子
                return value.Substring(dotpos + 1);
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "getFileTypeFromFilePath");
                return "";
            }
        }
        // 拡張子以前のファイル名を取得
        private string GetFileNameFromFilePath(string value)
        {
            try
            {
                // 文字列長さが０
                if (value.Length < 1) { return ""; }
                // 最後からのピリオドの位置
                int dotpos = value.LastIndexOf('.');
                // ピリオドがない場合はすべて返す
                if (dotpos < 1) { return value; }
                // 拡張子があればそれ以前を返す
                return value.Substring(0,dotpos);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "getFileNameFromFilePath");
                return "";
            }
        }
    }
}
