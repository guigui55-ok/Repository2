using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace CommonUtility.FileListUtility
{
    /// <summary>
    /// FileListManagerクラスの設定用クラス
    /// （FileListManagerに移譲している）
    /// <para></para>
    /// FileListRegister にも少し個別の設定があるので注意
    /// </summary>
    public class FileListManagerSetting
    {
        AppLogger _logger;
        // 呼び出し元クラス（移譲元クラス）連携用
        public FileListManager _fileListManager;
        // サブフォルダ以下をすべて読み込む
        public bool _isLoadEverythingUnderSubfoldersFiles = false;
        // ファイルリストを固定する
        // （ボタンでの変更をしない、テキストボックス空のみ変更をする）
        // （NextDir,PrevDirでの変更をしない、SetDirノミの変更を可能とする）
        public bool _isFixedFileList = false;
        // フォルダを固定する
        public bool _isFixedDirectory = false;
        // 
        public bool _isListRandom = false;

        public FileListManagerSetting(AppLogger logger, FileListManager fileListManager)
        {
            _logger = logger;
            _fileListManager = fileListManager;
        }

        public void ChangeFixedFileListFlag(bool value)
        {
            if (value != _isFixedFileList)
            {
                _logger.PrintInfo(String.Format("Setting.FlagChange > ChangeFixedFileListFlag = {0}", value));
                _isFixedFileList = value;
                _isFixedDirectory = value;
                _logger.PrintInfo("[Caution] _isFixedFileList, _isFixedDirectory 両方変更している");
                //他の処理は特になし、_filesクラスで、_settingのフラグを参照している
            }
        }
        public void ChangeFixedDirectoryFlag(bool value)
        {
            if (value != _isFixedDirectory)
            {
                _logger.PrintInfo(String.Format("Setting.FlagChange > _isFixedDirectory = {0}", value));
                _isFixedFileList = value;
                _isFixedDirectory = value;
                _logger.PrintInfo("[Caution] _isFixedFileList, _isFixedDirectory 両方変更している");
                //他の処理は特になし、_filesクラスで、_settingのフラグを参照している
            }
        }

        public void ChangeListRandom(bool value)
        {
            if (_isListRandom == value)
            {
                if (!value)
                {
                    //設定が変更されていない、かつ、ランダムOFFの場合はそのまま
                    return;

                }
            }
            // ランダムON＞ON、OFF＞ONの時は実行する
            _logger.PrintInfo(String.Format("Setting.FlagChange > ChangeListRandom = {0}", value));
            //ランダムは強制的に実行する
            //すでにランダムなら、再度ランダムを実行する
            //_isListRandom = value;
            _fileListManager.SwitchOrderToRandomOrCorrectByFlag(value);
            _fileListManager.UpdateFileListAfterEvent?.Invoke(
                _fileListManager._files.FileList.ToArray(), EventArgs.Empty);
        }

        public void ChangeIncludeSubFolder(bool value)
        {
            //以前と値が異なる
            if (value != _isLoadEverythingUnderSubfoldersFiles)
            {
                _logger.PrintInfo(String.Format("Setting.FlagChange > ChangeIncludeSubFolder = {0}", value));
                _isLoadEverythingUnderSubfoldersFiles = value;
                //以下メソッド内で、SettingFlagによって動作が変わる
                //変更されたときは再度セットする。
                _fileListManager.SetFilesFromPath(
                    _fileListManager._files.GetCurrentValue());
            }
        }

    }
}
