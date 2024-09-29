using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AppLoggerModule;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonUtility.FileListUtility
{
    public class FileListRegister
    {
        public AppLogger _logger;
        protected List<string> _fileList;
        protected List<string> _folderList;
        // ファイルリストは存在するが、すべてのファイルが条件に合わない場合true(すべてNotInclude、すべてIncludeと合わない場合)
        public bool IsNotConditionAllOfList = true;
        protected List<string> _includeFileTypeList;
        protected List<string> _notIncludeFileTypeList;
        protected List<string> _includeFileNameList;
        protected List<string> _notIncludeFileNameList;
        // フォルダ読み込み時の階層
        public int ReadFolderHierarchy = 0;
        // ショートカットの元を読み取る
        public bool IsReadSourceOfShotcut = true;
        // 最初に合致したもののみ読み込む
        public bool IsReadMatchFirstOnly = false;
        public EventHandler ChangedFileListEvent;
        public string DirectoryPath = "";
        // ファイル絞り込み条件
        public List<string> _fileFilterConditionList = new List<string> { };
        public List<string> _fileIgnoreConditionList = new List<string> { };

        // ファイルリストをセットしたときの最初のパス
        // D&Dしたものがlnkでないファイルならそのまま
        // lnkならlnk内の最初のファイル
        // dirならDirの最初のファイルとなる
        public string _listStartPath = "";

        //#
        //以下の条件リストは使用していない 240901
        //#
        /// <summary>
        /// ファイル読み込み時、IncludeFileTypeList に含まれる拡張子を指定する
        /// </summary>
        public List<string> IncludeFileTypeList {
            get { return _includeFileTypeList; }
            set { _includeFileTypeList = value; }
        }
        /// <summary>
        /// ファイル読み込み時、NotIncludeFileTypeList に含まれる拡張子を除外する
        /// </summary>
        public List<string> NotIncludeFileTypeList
        {
            get { return _notIncludeFileTypeList; }
            set { _notIncludeFileTypeList = value; }
        }
        /// <summary>
        /// ファイル読み込み時、IncludeFileTypeList に含まれるファイル名を指定する
        /// </summary>
        public List<string> IncludeFileNameList
        {
            get { return _includeFileNameList; }
            set { _includeFileNameList = value; }
        }
        /// <summary>
        /// ファイル読み込み時、NotIncludeFileTypeList に含まれるファイル名を除外する
        /// </summary>
        public List<string> NotIncludeFileNameList
        {
            get { return _notIncludeFileNameList; }
            set { _notIncludeFileNameList = value; }
        }
        public FileListRegister()
        {
        }

        public FileListRegister(AppLogger logger, List<string> list)
        {
            this._logger = logger;
            _fileList = list;
        }
        /// <summary>
        /// ファイルリストを取得する。
        /// </summary>
        /// <returns></returns>
        public List<string> GetList()
        {
            return _fileList;
        }

        /// ファイルリストのカウントを取得する
        /// </summary>
        /// <returns></returns>
        public int GetListCount()
        {
            try
            {
                if (_fileList == null) { return -2; }
                return _fileList.Count;
            } catch (Exception ex)
            {
                _logger.AddException(ex,this, "getListCount Failed");
                return -1;
            }
        }


        public string GetFilePathFromShortcut(string path)
        {
            try
            {
                // ショートカットの場合
                if (Path.GetExtension(path).CompareTo(".lnk") == 0)
                {
                    return new ShortCutReader(this._logger).GetSourceFromPath(path);

                }
                return path;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex,this, "GetFilePathFromShortcut");
                return "";
            }
        }

        public string getFile(string path,bool isReadShortcutContents)
        {
            try
            {
                string ret = "";
                if (isReadShortcutContents)
                {
                    // ショートカットの場合
                    if (Path.GetExtension(path).CompareTo(".lnk") == 0)
                    {
                        ret = new ShortCutReader(_logger).GetSourceFromPath(path);

                    }
                    else { /* ショートカットではない */ ret = path; }
                }
                else { /* ショートカットの元は読み取らない場合 */ ret = path; }
                if (ret.Equals(path))
                {
                    // ショートカットではない場合、or、ショートカットの中身は読み込まない場合
                    if ((System.IO.File.Exists(path)) || (System.IO.Directory.Exists(path)))
                    {
                        return path;
                    }
                    else
                    {
                        return "";
                    }
                }
                return ret;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "getFile Failed");
                return "";
            }
        }

        /// <summary>
        /// 条件リストをセットする
        /// SetFileListFromPathの初回実行前にセットする
        /// 2回目以降はメンバのリストが使用される
        /// </summary>
        /// <param name="fileFilterConditionList"></param>
        /// <param name="fileIgnoreConditionList"></param>
        public void SetConditionList(
            List<string> fileFilterConditionList = null,
            List<string> fileIgnoreConditionList = null)
        {
            //_fileFilterConditionList = fileFilterConditionList;
            //_fileIgnoreConditionList = fileIgnoreConditionList;
            _fileFilterConditionList = new List<string>(fileFilterConditionList);
            _fileIgnoreConditionList = new List<string>(fileIgnoreConditionList);
        }

        /// <summary>
        /// パスを受け取りディレクトリパスを取得、そのファイルリスをを取得して
        /// 条件で絞込、メンバへセットする（FileListRegister._fileList)
        /// 引数pathは this.DirectoryPathにセットされる
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int SetFileListFromPath(
            string path,
            List<string> fileFilterConditionList = null,
            List<string> fileIgnoreConditionList = null,
            bool isIncludeSubDirAll = false)
        {
            try
            {
                if ((fileFilterConditionList == null) || (fileFilterConditionList.Count < 1))
                {
                    fileFilterConditionList = _fileFilterConditionList;
                }
                else
                if ((fileIgnoreConditionList == null) || (fileIgnoreConditionList.Count < 1))
                {
                    fileIgnoreConditionList = _fileIgnoreConditionList;
                }
                _fileList = new List<string>();
                _folderList = new List<string>();
                DirectoryPath = "";

                if (!((System.IO.File.Exists(path)) || (System.IO.Directory.Exists(path))))
                {
                    _logger.AddLogWarning(" Path Not Exists. path=" + path); return -1;
                }
                bool changeStartFile = false;
                if (File.Exists(path))
                {
                    if (Path.GetExtension(path) == ".lnk")
                    {
                        _logger.PrintInfo(string.Format("lnk file true = {0}", path));
                        path = new ShortCutReader(_logger).GetSourceFromPath(path);
                        _logger.PrintInfo(string.Format("lnk file read content path = {0}", path));
                    }
                    if (File.Exists(path))
                    {
                        // path がファイルの時は、DirectoryPath を取得する
                        int pos = path.LastIndexOf("\\");
                        path = path.Substring(0, pos);
                    }
                    //D&Dしたものがディレクトリ以外の場合は、
                    //fileList内のMove(path)とすると、存在せずえらとなるので以下のフラグを契機に置き換える
                    changeStartFile = true;
                }
                this.DirectoryPath = path;
                string[] aryFilePath;
                if (isIncludeSubDirAll)
                {
                    aryFilePath = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                }
                else { 
                    aryFilePath = Directory.GetFiles(path);
                }
                _logger.PrintInfo(String.Format("Dirctory.GetFiles.Length = {0}", aryFilePath.Length));
                //List<string> list = new List<string>(ary);
                //この絞り込みはMkeListの後にしなければならない？
                List<string> retList = FilterAndExcludeFilesWithRegex(
                    aryFilePath, fileFilterConditionList, fileIgnoreConditionList);
                //#
                int ret = SetFileList(retList);
                //#
                //このリスト
                if (changeStartFile)
                {
                    if (0 < _fileList.Count)
                    {
                        _listStartPath = _fileList[0];
                    }
                    else
                    {
                        _listStartPath = "";
                    }
                }
                return ret;

            } catch(System.UnauthorizedAccessException ex)
            {
                //_logger.AddException(ex, this, "SetFileListFromPath Failed");
                _logger.AddLogAlert(this, "System.UnauthorizedAccessException",ex);
                return -2;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "SetFileListFromPath Failed");
                return 0;
            }
        }

        public List<string> RemoveEmptyStrings(List<string> inputList)
        {
            // 空文字を除外する
            return inputList.Where(str => !string.IsNullOrEmpty(str)).ToList();
        }

        private List<string> FilterAndExcludeFilesWithRegex(
            string[] filePathList,
            List<string> fileFilterConditionList,
            List<string> fileIgnoreConditionList)
        {

            try
            {
                fileFilterConditionList = RemoveEmptyStrings(fileFilterConditionList);
                fileIgnoreConditionList = RemoveEmptyStrings(fileIgnoreConditionList);
                ////#
                //int count = 0;
                //foreach (string buf in fileIgnoreConditionList)
                //{
                //    count += 1;
                //    Console.WriteLine(string.Format("[{0}] {1}", count, buf));
                //}
                ////#
                // fileFilterConditionListに合致するファイルパスをリストに追加
                List<string> filteredFiles = new List<string>();
                if (0 < fileFilterConditionList.Count)
                {
                    foreach (string filePath in filePathList)
                    {
                        bool matchesFilter = false;
                        foreach (string condition in fileFilterConditionList)
                        {
                            if (Regex.IsMatch(filePath, condition))
                            {
                                matchesFilter = true;
                                break;
                            }
                        }
                        if (matchesFilter)
                        {
                            filteredFiles.Add(filePath);
                        }
                    }
                }
                else
                {
                    //フィルターリストがない場合は、そのまま
                    filteredFiles = filePathList.ToList();
                }

                // fileIgnoreConditionListに合致するファイルパスをリストから除外
                List<string> resultFiles = new List<string>();
                if (0 < fileIgnoreConditionList.Count)
                {
                    foreach (string filePath in filteredFiles)
                    {
                        bool shouldIgnore = false;
                        foreach (string ignoreCondition in fileIgnoreConditionList)
                        {
                            if (Regex.IsMatch(filePath, ignoreCondition))
                            {
                                shouldIgnore = true;
                                break;
                            }
                        }
                        if (!shouldIgnore)
                        {
                            resultFiles.Add(filePath);
                        }
                    }
                }
                else
                {
                    //ignoreListがない場合はそのまま渡す
                    resultFiles = filteredFiles;
                }

                return resultFiles;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "FilterAndExcludeFilesWithRegex Failed");
            }
            return new List<string>(filePathList);
        }

        // DragAndDrop用
        /// <summary>
        /// ファイルリストを List<string> から取得する。DragDrop イベントで取得した後のリストを引数に渡す。
        /// そして条件を絞り込み、その結果は
        /// FileListRegister._fileList に格納される
        /// リストがないときは ListStringをNewする
        /// </summary>
        /// <param name="list">このリストは読み込み元Dirパス,やlnkファイルパスのリスト</param>
        /// <returns></returns>
        public int SetFileList(List<string> list)
        {
            try
            {
                _logger.AddLog(this, "SetFileList");
                // includeListの空要素を削除
                int ret = RemoveBlankValueInIncludeList();
                if (ret < 1)
                {
                    _logger.AddLogAlert(this, "setFileList : RemoveBlankValueInIncludeList Failed");
                }

                _fileList = new List<string>();
                
                // リストがない
                if (list.Count < 1) {
                    _logger.AddLogWarning("  list.Count < 1");
                    _fileList = new List<string>();
                    return 2; 
                }
                // ファイルを全て読み込み
                ret = MakeFileList(list,_fileList,0);
                if(ret < 1)
                {
                    _logger.AddLogAlert(this, "setFileList : MakeFileList Failed");
                    //return 3;
                }
                // 条件以外を除外
                ret = SetFileListWithApplyConditions(_fileList);
                if (ret < 1)
                {
                    _logger.AddLogAlert(this, "setFileList : setFileListWithApplyConditions Failed");
                    return 4;
                }
                return 1;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "setFileList");
                return 0;
            }
        }
        /// <summary>
        /// IncludeList の空白を除外する
        /// </summary>
        /// <returns></returns>
        private int RemoveBlankValueInIncludeList()
        {
            try
            {
                int ret = RemoveBlankValueInList(_includeFileTypeList);
                if (ret < 1) { _logger.AddLogAlert(this, "RemoveBlankValueInList failed : _includeFileTypeList"); }
                 ret = RemoveBlankValueInList(_notIncludeFileTypeList);
                if (ret < 1) { _logger.AddLogAlert(this, "RemoveBlankValueInList failed : _notIncludeFileTypeList"); }
                 ret = RemoveBlankValueInList(_includeFileNameList);
                if (ret < 1) { _logger.AddLogAlert(this, "RemoveBlankValueInList failed : _includeFileNameList"); }
                 ret = RemoveBlankValueInList(_notIncludeFileNameList);
                if (ret < 1){ _logger.AddLogAlert(this, "RemoveBlankValueInList failed : _notIncludeFileNameList"); }
                return 1;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "RemoveBlankValueInIncludeList");
                return 0;
            }
        }
        /// <summary>
        /// ファイルリストに空白があれば除去する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "RemoveBlankValueInList");
                return 0;
            }
        }
        /// <summary>
        /// ファイルリストを読み込み、追加・作成する
        /// ショートカットはリンク先のファイルとして変換し、フォルダ(指定階層分)内のファイルをすべてを読み込み、リストへ追加する
        /// </summary>
        /// <param name="list"></param>
        /// <param name="newList"></param>
        /// <param name="NowHierarchy"></param>
        /// <returns></returns>
        private int MakeFileList(List<string> list,List<string> newList, int NowHierarchy,bool isSingleFile = false)
        {
            try
            {
                _logger.AddLog(this, "MakeFileList");
                if (list == null) { _logger.AddLogAlert(this,"list is null"); return -1; }

                List<string> folderList = new List<string>();

                string tempPath = "";
                // ルートのファイルのみを追加
                foreach (string listValue in list)
                {
                    tempPath = listValue;
                    if (IsReadSourceOfShotcut)
                    {
                        // ショートカットの場合
                        if (Path.GetExtension(listValue).CompareTo(".lnk") == 0)
                        {
                            tempPath = new ShortCutReader(_logger).GetSourceFromPath(tempPath);

                        } else { /* ショートカットではない */ tempPath = listValue; }
                    }
                    else { /* ショートカットの元は読み取らない場合 */ tempPath = listValue; }

                    // ファイルが存在する
                    if (File.Exists(tempPath))
                    {
                        // 条件が合致するか判定する
                        if (IsMatchConditions(tempPath))
                        {
                            newList.Add(tempPath);
                            _logger.PrintTrace("  add path=" + tempPath);
                            if (IsReadMatchFirstOnly) { _fileList = newList;  return 1; }
                        } else
                        {
                            _logger.AddLog("  not add path=" + tempPath);
                        }
                    }
                    else
                    {
                        // ファイルではない場合
                        if (System.IO.Directory.Exists(tempPath))
                        {
                            // フォルダの場合はフォルダリストへ登録する、
                            // ファイル登録処理を終えた後フォルダ内のファイルを再帰的に読み込む
                            folderList.Add(tempPath);
                        }
                        else
                        {
                            // ファイルでもフォルダでもない
                            _logger.AddLogAlert("File.Exists||Directory.Exists == False");
                        }
                    }
                    if (isSingleFile) { break; }
                }

                if (isSingleFile)
                {
                    _fileList = newList;
                    return 1;
                }

                if (folderList.Count < 1) {
                    //return 1;
                }
                _folderList = folderList;

                // ↑で作成したフォルダーリストからファイルを読みこむ
                // 複数階層は読み込まない
                // ファイル数が多すぎるときに DragDrop から処理が戻ってこないので
                // ファイル読み込み上限を設け、フォルダ内のファイルはは別処理で読み込む
                foreach (string listValue in folderList)
                {
                    if (NowHierarchy < ReadFolderHierarchy)
                    {
                        string[] files = System.IO.Directory.GetFiles(listValue, "*", System.IO.SearchOption.AllDirectories);
                        MakeFileList(new List<string>(files), newList, NowHierarchy);
                        NowHierarchy++;
                    }
                }
                _fileList = newList;
                return 1;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "MakeFileList");
                return 0;
            }
        }
        // 条件に合うファイルのみリストを作成
        /// <summary>
        /// 条件に合うファイルのみリストを作成する。MakeFileの後に実行する。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private int SetFileListWithApplyConditions(List<string> list)
        {
            try
            {
                if (list == null) { _logger.AddLogAlert(this, "setFileListWithApplyConditions failed. list is null"); return -1; }
                if (list.Count < 1)
                {
                    _logger.AddLogAlert(this, "setFileListWithApplyConditions : "+ "List Count Zero");
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
                _logger.AddException(ex, this, "setFileList");
                return 0;
            }
        }

        private bool IsMatchConditions(string path)
        {
            return (!IsIncludeFileType(path)) && (!IsIncludeFileName(path));
        }

        /// <summary>
        /// ファイルリストにフィルタを適用する。
        /// ファイル名、拡張子にフィルタをかけ指定したもの以外は除外する。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "setFileList");
                return list;
            }
        }
        /// <summary>
        /// path が設定された条件に合致するか判定する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "isIncludeFileType");
                return false;
            }
        }
        /// <summary>
        /// path が指定した条件 (FileName) に合致するか判定する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "isIncludeFileName");
                return false;
            }
        }
        /// <summary>
        /// リスト処理、list内にvalueと一致するものがあるか_完全一致
        /// 条件リスト処理用
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "ValueIsMatchInList");
                return false;
            }
        }

        /// <summary>
        /// リスト処理、List内のvlaueが含まれるものがあるか_部分一致
        /// 条件リスト処理用
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "ValueIsIncludeInList");
                return false;
            }
        }


        /// <summary>
        /// 拡張子を取得する
        /// System.IO.Path.GetExtension でも代用できる
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "getFileTypeFromFilePath");
                return "";
            }
        }
        /// <summary>
        /// 拡張子以前のファイル名を取得
        /// System.IO.Path.GetFileName でも代用できる
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
                _logger.AddException(ex, this, "getFileNameFromFilePath");
                return "";
            }
        }
    }
}
