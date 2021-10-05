
using CommonUtility.FileListUtility;
using System;
using System.Collections.Generic;

namespace CommonUtility.FileListUtility
{
    public enum FileReadOption
    {
        ALL = 1,
        FIRSTONLY = 2
    }
    public class FileListManager
    {
        protected ErrorManager.ErrorManager _err;
        protected IFiles _files;
        protected List<string> _folderList;
        protected FileListRegister _filesRegister;
        protected RandomListCreater randomListCreater;
        protected DirectoryGetter _directoryGetter;
        public EventHandler UpdateFileListAfterEvent;
        public int ReadOption = 1;

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

        protected bool IsRandom = false;
        
        public FileListManager(ErrorManager.ErrorManager err,IFiles files)
        {
            _err = err;
            _files = files;
            _filesRegister = new FileListRegister(_err);
            _files.ChangedFileListEvent += ChangedFileListEvent;
            _directoryGetter = new DirectoryGetter(_err);
        }

        // SetFileListFromFolderList
        // NextFolder
        // PreviousFolder

        public void ChangedFileListEvent(object sender,EventArgs e)
        {
            try
            {
                _err.AddLog(this, "ChangedFileList");
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString(), "ChangedFileList");
            }
        }

        public string GetFilePathFromShortcut(string path)
        {
            return _filesRegister.GetFilePathFromShortcut(path);
        }

        public void SetFilesFromPath(string path)
        {
            try
            {
                _err.AddLog(this, "SetFilesFromPath");
               int ret = _filesRegister.SetFileListFromPath(path);
               if (_err.hasAlert) { _err.AddLogAlert("  SetFileListFromPath Failed"); }
                _files.DirectoryPath = _filesRegister.DirectoryPath;
                _err.AddLog("  SetDirecoryPath="+ _files.DirectoryPath);
                _files.FileList = _filesRegister.GetList();
                _files.Move(_filesRegister.getFile(path, IsReadSourceOfShotcut));
                UpdateFileListAfterEvent?.Invoke(_files.FileList.ToArray(), EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString(), "SetFilesFromPath");
            }
        }

        public void MoveNextFileWhenLastFileNextDirectory()
        {
            try
            {
                if (_files.IsLastIndex()) { this.MoveNextDirectory(); }
                else { _files.MoveNext(); }                
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "MoveNextFileWhenLastFileNextDirectory");
            }
        }
        public void MoveProviousFileWhenFirstFilePreviousDirectory()
        {
            try
            {
                if (_files.IsFirstIndex()) { this.MovePreviousDirectory(); }
                else { _files.MovePrevious(); }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "MoveNextFileWhenLastFileNextDirectory");
            }
        }

        public void MoveNextDirectory()
        {
            try
            {
                _err.AddLog(this, "MoveNextDirectory");
                string dir = _directoryGetter.GetNextDirectory(_files.DirectoryPath);
                SetFilesFromPath(dir);
                if (_err.hasError) { _err.AddLogAlert(this, "MoveNextDirectory Failed"); _err.ClearError(); }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "MoveNextDirectory");
            }
        }
        public void MovePreviousDirectory()
        {
            try
            {
                _err.AddLog(this, "MovePreviousDirectory");
                string dir = _directoryGetter.GetPreviousDirectory(_files.DirectoryPath);
                SetFilesFromPath(dir);
                if (_err.hasError) { _err.AddLogAlert(this, "MovePreviousDirectory Failed"); _err.ClearError(); }
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "MovePreviousDirectory");
            }
        }

        ///// <summary>
        ///// ファイルリストを DragDrop イベントから登録する
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void RegistFileByDragDrop(object sender, DragEventArgs e)
        //{
        //    try
        //    {
        //        _err.AddLog(this, "RegistFileListByDragDrop");
        //        // DragDrop の e を配列へ
        //        string[] files = GetFilesByDragAndDrop(e);
                
        //        // 配列→Listへ
        //        List<string> list = new List<string>(files);


        //        // リストからFileListへ登録
        //        // 条件に合致したファイルリストを作成する
        //        int ret = this._filesRegister.SetFileList(list);
        //        if (ret < 1)
        //        {
        //            _err.AddLogWarning("  Control_DragDrop.setFileList Failed");
        //            return;
        //        }
        //        _err.AddLog("  List.Count = " + _filesRegister.GetListCount());
        //        // ファイルリストへ登録する
        //        _files.FileList = _filesRegister.GetList();

        //        if (_err.hasAlert) { _err.AddLogAlert("  SetFileList Failed"); }
        //        else
        //        {
        //            _err.AddLog("  SetFileList Success!");
        //        }
        //        UpdateFileListAfterEvent?.Invoke(_files.FileList.ToArray(), EventArgs.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        _err.AddException(ex, this.ToString(), "RegistFileListByDragDrop");
        //    }
        //    finally
        //    {
        //        if (_err.hasError)
        //        {
        //            Debug.WriteLine(_err.GetLastErrorMessagesAsString(3,true));
        //        }
        //    }
        //}
        ///// <summary>
        ///// ドラッグされたオブジェクト内のファイルやディレクトリのみ (DataFormats.FileDrop) を取得する
        ///// </summary>
        ///// <param name="e"></param>
        ///// <returns></returns>
        //private string[] GetFilesByDragAndDrop(DragEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //        {
        //            // ドラッグ中のファイルやディレクトリの取得
        //            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //            return files;
        //        }
        //        else
        //        {
        //            _err.AddLogAlert(this, "e.Data.GetDataPresent(DataFormats.FileDrop)=false");
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _err.AddException(ex, this.ToString(), "GetFilesByDragAndDrop");
        //        return null;
        //    }
        //}
        /// <summary>
        /// リストの順番をランダムまたは通常へ切り替える。実行毎に OFF/ON を切り替える。
        /// </summary>
        /// <returns></returns>
        public int SwitchOrderToRandomOrCorrect()
        {
            try
            {
                int ret = 0;
                List<string> list = _files.FileList;
                if (IsRandom)
                {
                    IsRandom = false;
                    // 名前順にする
                    list.Sort();
                    _files.FileList = list;
                    return 1;
                }
                else
                {
                    // ランダム順位する
                    _files.FileList = randomListCreater.ListOrtderToRandom(list);
                    IsRandom = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "switchOrderToRandomOrCorrect");
                return 0;
            }
        }
    }
}
