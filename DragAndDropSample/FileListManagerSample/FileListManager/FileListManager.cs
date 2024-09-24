
using CommonUtility.FileListUtility;
using System;
using System.Collections.Generic;
using AppLoggerModule;
using System.IO;
using System.Diagnostics;
//using CommonUtility.FileListUtility;

namespace CommonUtility.FileListUtility
{
    public enum FileReadOption
    {
        ALL = 1,
        FIRSTONLY = 2
    }
    public class FileListManager
    {
        public AppLogger _logger;
        public IFiles _files;
        protected List<string> _folderList;
        public FileListRegister _filesRegister;
        protected RandomListCreater randomListCreater;
        protected DirectoryGetter _directoryGetter;
        // CurrentFileが更新されたときのイベント
        public EventHandler UpdateFileListAfterEvent;
        public int ReadOption = 1;
        // 複数同時に使用することがあるので名前を追加
        public string Name = "FileListManager";

        public FileListManagerSetting _fileListManagerSetting;
        //public List<string> _fileFilterConditionList = new List<string> { };
        //public List<string> _fileIgnoreConditionList = new List<string> { };

        // ショートカットの元を読み取る
        public bool IsReadSourceOfShotcut { 
            get { return _filesRegister.IsReadSourceOfShotcut; } 
            set { _filesRegister.IsReadSourceOfShotcut = value; }
        }
        // 最初に合致したもののみ読み込む
        public bool IsReadMatchFirstOnly
        {
            get { return _filesRegister.IsReadMatchFirstOnly; }
            set { _filesRegister.IsReadMatchFirstOnly = value; }
        }
        // フォルダ読み込み時の階層
        public int ReadFolderHierarchy
        {
            get { return _filesRegister.ReadFolderHierarchy; }
            set { _filesRegister.ReadFolderHierarchy = value; }
        }

        public bool IsRandom = false;
        
        public FileListManager(AppLogger logger, IFiles files)
        {
            this._logger = logger;
            _files = files;
            _filesRegister = new FileListRegister();
            _filesRegister._logger = logger;
            _files.ChangedFileListEvent += ChangedFileListEvent;
            _directoryGetter = new DirectoryGetter(_logger);
            _directoryGetter._logger = logger;
            this.randomListCreater = new RandomListCreater(this._logger);
            _fileListManagerSetting = new FileListManagerSetting(_logger, this);
            IsRandom = _fileListManagerSetting._isListRandom;
        }

        // SetFileListFromFolderList
        // NextFolder
        // PreviousFolder

        public void ChangedFileListEvent(object sender,EventArgs e)
        {
            try
            {
                _logger.AddLog(this, "ChangedFileList");
            } catch (Exception ex)
            {
                _logger.AddException(ex, this.ToString(), "ChangedFileList");
            }
        }

        public string GetFilePathFromShortcut(string path)
        {
            return _filesRegister.GetFilePathFromShortcut(path);
        }


        /// <summary>
        /// フォルダ・ファイルリスト読み込み・ファイルリスト設定をお行うメインメソッド
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileFilterConditionList"></param>
        /// 対応する拡張子リスト。不要な場合はnullを渡す
        /// nullの場合でも、_fileListManagerのメンバ変数 _supportedImageExtentionList に設定されていたらこれを使用する
        /// 無効にするときは List string {} を渡す
        /// <param name="fileIgnoreConditionList"></param>
        public void SetFilesFromPath(
            string path,
            List<string> fileFilterConditionList = null,
            List<string> fileIgnoreConditionList = null)
        {
            try
            {
                //throw new Exception("supportedImageExtentionList の処理未実装");
                // Nullの時、メンバを使用する。 絞り込み処理をRegisterでする？
                _logger.AddLog(this, "SetFilesFromPath");
                // ファイルフィルターリスト、除外リスト
                if (fileFilterConditionList == null) { fileFilterConditionList = _filesRegister._fileFilterConditionList; }
                if (fileFilterConditionList.Count < 1) { fileFilterConditionList = _filesRegister._fileFilterConditionList; }
                if (fileIgnoreConditionList == null) { fileIgnoreConditionList = _filesRegister._fileIgnoreConditionList; }
                if (fileIgnoreConditionList.Count < 1) { fileIgnoreConditionList = _filesRegister._fileIgnoreConditionList; }
                //
                string buf;
                buf = String.Format("fileFilterConditionList = {0}", string.Join(",", fileFilterConditionList));
                _logger.AddLog(buf);
                buf = String.Format("fileIgnoreConditionList = {0}", string.Join(",", fileIgnoreConditionList));
                _logger.AddLog(buf);
                ////
                // ファイルリスト取得
                int ret = 0;
                _filesRegister.SetConditionList(fileFilterConditionList, fileIgnoreConditionList);
                //#
                //通常時（ディレクトリのファイルのみ）
                // サブフォルダ以下のすべてのファイルが対象
                ret = _filesRegister.SetFileListFromPath(
                   path,
                   fileFilterConditionList,
                   fileIgnoreConditionList,
                   _fileListManagerSetting._isLoadEverythingUnderSubfoldersFiles);
                ////
                if (_logger.hasAlert()) { _logger.AddLogAlert("  SetFileListFromPath Failed"); }
                _files.DirectoryPath = _filesRegister.DirectoryPath;
                _logger.AddLog("  SetDirecoryPath="+ _files.DirectoryPath);
                //#
                // 
                // Directory更新とFile更新のイベントがほぼ同時に走るので
                // それぞれの後続の処理が干渉しあわないように注意
                //
                // ファイルリストを最終決定＞セット
                List<string> bufList = _filesRegister.GetList();
                _files.FileList = bufList;
                //ランダム適用
                //_fileListManagerSetting._isListRandom = true;  //Debug用
                int retRandom = SwitchOrderToRandomOrCorrectByFlag(_fileListManagerSetting._isListRandom);
                //#
                //最初のファイルを設定する
                //string movePath = _filesRegister.getFile(path, IsReadSourceOfShotcut);
                //_logger.AddLog(string.Format("movePath = {0}", movePath));
                //if (Directory.Exists(path))
                //{
                //    // MoveDirectoryでこのmovePathが読み込みルートのディレクトリになる
                //    _files.Move(0);
                //}
                //else
                //{
                //    // ファイルの場合はそのまま選択する
                //    // 引数のpathがファイルがリストにない場合は（index0にする）　未対応 240901
                //    _files.Move(movePath);
                //}
                int index = _files.FileList.IndexOf(path);
                if (0 < index)
                {
                    _files.Move(index);
                } else
                {
                    if (retRandom == 2)
                    {
                        _files.Move(0);
                    }
                    else
                    {
                        _files.Move(0);
                    }
                }
                //string movePath = _filesRegister.getFile(path, IsReadSourceOfShotcut);
                _logger.AddLog(string.Format("first Path = {0}", _files.GetCurrentValue()));
                UpdateFileListAfterEvent?.Invoke(_files.FileList.ToArray(), EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this.ToString(), "SetFilesFromPath");
            }
        }

        /// <summary>
        /// タイマーなどイベント設定用
        /// MoveNextFileWhenLastFileNextDirectoryが呼ばれる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveNextFileWhenLastFileNextDirectoryEvent(object sender, EventArgs e)
        {
            _logger.PrintInfo(this.Name +  " > MoveNextFileWhenLastFileNextDirectoryEvent [SlideShow]");

            // 1つ目のタイマーが止まらない調査用
            ////#
            //// スタックトレースを取得して呼び出し元のメソッド情報を得る
            //StackTrace stackTrace = new StackTrace();
            //int max = 3;
            //for (int i = 1; i<max; i++)
            //{
            //    StackFrame frame = stackTrace.GetFrame(i); // 1つ上のフレームを取得（呼び出し元）

            //    // 呼び出し元のメソッド名とクラス名を取得
            //    var method = frame.GetMethod();
            //    string className = method.DeclaringType.Name; // 呼び出し元のクラス名
            //    string methodName = method.Name; // 呼び出し元のメソッド名

            //    // ログに出力
            //    _logger.PrintInfo(this.Name + $" >  Called from {className}.{methodName}");
            //}
            ////#


            bool isMove = MoveNextFileWhenLastFileNextDirectory();
        }

        public bool MoveNextFileWhenLastFileNextDirectory()
        {
            try
            {
                if (_files.IsLastIndex()) 
                { 
                    return this.MoveNextDirectory(); 
                }
                else {
                    _files.MoveNext();
                } 
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "MoveNextFileWhenLastFileNextDirectory");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 現在のファイル位置がリストの最初なら、前のディレクトリに移動する
        /// そうでなければ、1つ前のファイルに移動する
        /// </summary>
        public bool MoveProviousFileWhenFirstFilePreviousDirectory()
        {
            try
            {
                if (_files.IsFirstIndex()) {
                   return this.MovePreviousDirectory();
                }
                else { _files.MovePrevious(); }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "MoveNextFileWhenLastFileNextDirectory");
                return false;
            }
            return true;
        }

        public bool MoveNextDirectory()
        {
            try
            {
                _logger.AddLog(this, "MoveNextDirectory");
                if (_fileListManagerSetting._isFixedDirectory)
                {
                    _logger.AddLog(this, "Setting._isFixedDirectory = true");
                    return false;
                }
                if (!Directory.Exists(_files.DirectoryPath))
                {
                    _logger.AddLog(this, string.Format("MoveNextDirectory > _files.DirectoryPath Not Exists [path={0}]", _files.DirectoryPath));
                    return false;
                }

                string dir = _directoryGetter.GetNextDirectory(_files.DirectoryPath);
                SetFilesFromPath(dir);
                if (_logger.hasError()) { 
                    _logger.AddLogAlert(this, "MoveNextDirectory Failed"); 
                    _logger.ClearError();
                    return false;
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "MoveNextDirectory");
                return false;
            }
            return true;
        }
        public bool MovePreviousDirectory()
        {
            try
            {
                _logger.AddLog(this, "MovePreviousDirectory");
                if (_fileListManagerSetting._isFixedDirectory)
                {
                    _logger.AddLog(this, "Setting._isFixedDirectory = true");
                    return false;
                }
                string dir = _directoryGetter.GetPreviousDirectory(_files.DirectoryPath);
                SetFilesFromPath(dir);
                if (_logger.hasError()) { 
                    _logger.AddLogAlert(this, "MovePreviousDirectory Failed"); 
                    _logger.ClearError();
                    return false;
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "MovePreviousDirectory");
                return false;
            }
            return true;
        }

        /// <summary>
        /// リストの順番をランダムまたは通常へ切り替える。実行毎に OFF/ON を切り替える。
        /// </summary>
        /// <returns></returns>
        public int SwitchOrderToRandomOrCorrect()
        {
            try
            {
                _logger.AddLog(this, "SwitchOrderToRandomOrCorrect");
                int ret = 0;
                List<string> list = _files.FileList;
                if (IsRandom)
                {
                    IsRandom = false;
                    _fileListManagerSetting._isListRandom = IsRandom;
                    // 名前順にする
                    list.Sort();
                    _files.FileList = list;
                    ret = 1;
                }
                else
                {
                    // ランダム順位する
                    _files.FileList = randomListCreater.ListOrtderToRandom(list);
                    IsRandom = true;
                    _fileListManagerSetting._isListRandom = IsRandom;
                    ret = 2;
                }
                return ret;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "switchOrderToRandomOrCorrect");
                return 0;
            }
        }
        /// <summary>
        /// リストの順番をランダムまたは通常へ切り替える。フラグで指定する。
        /// メンバ変数 IsRandom , _setting^._isListRandom を更新する。
        /// </summary>
        /// <returns></returns>
        public int SwitchOrderToRandomOrCorrectByFlag(bool argIsRandom)
        {
            try
            {
                _logger.AddLog(this, "SwitchOrderToRandomOrCorrectByFlag");
                int ret = 0;
                List<string> list = _files.FileList;
                IsRandom = argIsRandom;
                _fileListManagerSetting._isListRandom = IsRandom;
                if (!argIsRandom)
                {
                    // 名前順にする
                    list.Sort();
                    ////#
                    //List<string> bufList = new List<string> { };
                    //foreach(string buf in list)
                    //{
                    //    bufList.Add(Path.GetFileName(buf));
                    //}
                    ////#
                    //string bufJoin = String.Join(",", bufList);
                    //_logger.PrintInfo("fileNameList = " + bufJoin);
                    _files.FileList = list;
                    ret = 1;
                }
                else
                {
                    // ランダム順位する
                    _files.FileList = randomListCreater.ListOrtderToRandom(list);
                    ret = 2;
                }
                return ret;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "SwitchOrderToRandomOrCorrectByFlag");
                return 0;
            }
        }
    }
}
