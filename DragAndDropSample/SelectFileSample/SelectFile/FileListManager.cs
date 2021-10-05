using SelectFileSample.SelectFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlUtility.SelectFiles
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
        protected CreateRandomList createRandomList;
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
            _files.ChangedFileList += ChangedFileList;
        }

        // SetFileListFromFolderList
        // NextFolder
        // PreviousFolder

        public void ChangedFileList(object sender,EventArgs e)
        {
            try
            {
                _err.AddLog(this, "ChangedFileList");
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString(), "ChangedFileList");
            }
        }

        /// <summary>
        /// ファイルリストを DragDrop イベントから登録する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RegistFileByDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                _err.AddLog(this, "RegistFileListByDragDrop");
                // DragDrop の e を配列へ
                string[] files = GetFilesByDragAndDrop(e);
                
                // 配列→Listへ
                List<string> list = new List<string>(files);


                // リストからFileListへ登録
                // 条件に合致したファイルリストを作成する
                int ret = this._filesRegister.SetFileList(list);
                if (ret < 1)
                {
                    _err.AddLogWarning("  Control_DragDrop.setFileList Failed");
                    return;
                }
                _err.AddLog("  List.Count = " + _filesRegister.GetListCount());
                // ファイルリストへ登録する
                _files.FileList = _filesRegister.GetList();

                if (_err.hasAlert) { _err.AddLogAlert("  SetFileList Failed"); }
                else
                {
                    _err.AddLog("  SetFileList Success!");
                }
                UpdateFileListAfterEvent?.Invoke(_files.FileList.ToArray(), EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString(), "RegistFileListByDragDrop");
            }
            finally
            {
                if (_err.hasError)
                {
                    Debug.WriteLine(_err.GetLastErrorMessagesAsString(3,true));
                }
            }
        }
        /// <summary>
        /// ドラッグされたオブジェクト内のファイルやディレクトリのみ (DataFormats.FileDrop) を取得する
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string[] GetFilesByDragAndDrop(DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // ドラッグ中のファイルやディレクトリの取得
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    return files;
                }
                else
                {
                    _err.AddLogAlert(this, "e.Data.GetDataPresent(DataFormats.FileDrop)=false");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString(), "GetFilesByDragAndDrop");
                return null;
            }
        }
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
                    _files.FileList = createRandomList.ListOrtderToRandom(list);
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
