using CommonUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utility
{
    public class OpenedFile
    {
        protected ErrorManager.ErrorManager _error;
        protected string _filePath;
        protected string _saveFileDialogCurrentDirectory;
        public string SaveFileDialogLastDirectory;
        public string LastWriteDirectory;
        protected FileIO _fileIO;
        public event EventHandler ChangeFilePathEvent;
        public event EventHandler OpenFileEvent;
        public string SaveFileDialogFilter;
        public string SaveDialogTitle;
        public string OpenDialogTitle;
        public string ConformEditedTitle;
        public string ConformEditedMessage;
        public string DefaultNewFileName;
        public bool IsNewFile = true;
        public OpenedFile(ErrorManager.ErrorManager error)
        {
            _error = error;
            _fileIO = new FileIO(_error);
            _saveFileDialogCurrentDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            SaveFileDialogLastDirectory = _saveFileDialogCurrentDirectory;
            LastWriteDirectory = _saveFileDialogCurrentDirectory;
            SaveFileDialogFilter = "すべてのファイル(*.*)|*.*;|すべてのファイル(*.*)|*.*";
            SaveDialogTitle = "保存するファイル名を入力してください";
            OpenDialogTitle = "開くファイル名を入力してください";
            ConformEditedMessage = " の変更内容を保存しますか？";
            ConformEditedTitle = "Message";
            DefaultNewFileName = "NewFile";
        }

        public string GetPath()
        {
            return _filePath;
        }

        public string GetFileName()
        {
            if ((_filePath == "")||(_filePath == null))
            {
                return "";
            } else
            {
                return System.IO.Path.GetFileName(_filePath);
            }
        }

        public void OpenFile(string filePath)
        {
            try
            {
                List<string> ReadValueList = _fileIO.ReadFile(filePath);
                if (_error.HasException()) { return ; }

                OpenFileEvent?.Invoke(ReadValueList,EventArgs.Empty);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ReadFile");
            }
        }

        public List<string> GetListByOpenFile(string filePath)
        {
            return _fileIO.ReadFile(filePath);
        }

        public void SetSaveFileDialogCurrentDirectory(string directoryPath)
        {
            _saveFileDialogCurrentDirectory = directoryPath;
        }

        public void SetFilePath(string filePath)
        {
            try
            {
                _filePath = filePath;

                ChangeFilePathEvent?.Invoke(_filePath, EventArgs.Empty);
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".SetFilePath");
            }
        }

        // 名前を付けて保存する
        // writeFlag は Create 関連のみ,CreateNewFile
        public void SaveAsData(string data,string newFileName,int writeFlag)
        {
            ShowSaveFileDialog(data,newFileName,writeFlag);
        }

        // file dialog
        // flag appendOnly overwriteOnly createonly 
        // appendAndCreateWhenPathNotExists,OverWriteAndCreateWhenPathNotExists
        // AppendAndNotWriteWhenPathNotExists,OverWriteAndNotWriteWhenPathNotExists
        public void SaveData(string data,string newFileName,int writeFlag)
        {
            try
            {
                if ((_filePath == "") || (_filePath == null) ||
                    (new FileUtility(_error).GetFileNameTypeRemoved(_filePath) == "*"))
                {
                    // ファイル名がない(新規作成)のとき
                    // ファイルパスが空の時はダイアログから指定して保存する
                    // dialog
                    ShowSaveFileDialog(data, newFileName, writeFlag);
                    if (_error.hasAlert) { throw new Exception("SaveData ShowSaveFileDialog"); }
                    return;
                } else
                {
                    // ファイルパスがないとき
                    if (!System.IO.File.Exists(_filePath))
                    {
                        _error.AddLog("FilePath Not Exists [" + _filePath + "]");
                    }
                }
                // 保存する
                _fileIO.WriteFile(_filePath,false, data);
                if (_error.hasAlert) { throw new Exception("_fileIO.WriteFile Failed : path="+_filePath); }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SaveData");
            }
        }

        // fileName が directoryPath に存在する場合は fileName を返す
        public string MakeNewFileName(string fileName,string directryPath)
        {
            try
            {
                _error.AddLog(this, "MakeNewFileName : path=" + directryPath + "\\" + fileName);
                if (!System.IO.Directory.Exists(directryPath)) { throw new Exception("Directry Path Is Not Exists"); }


                string retFileName = fileName;
                string filePath = directryPath + "\\" + retFileName;
                bool isExists = System.IO.File.Exists(filePath);
                int count = 2;
                while (isExists)
                {
                    // ファイルがある場合、次のファイル名をセットする
                    retFileName = System.IO.Path.GetFileName(fileName) + " (" + count + ")"
                        + System.IO.Path.GetExtension(filePath);
                    filePath = directryPath + "\\" + retFileName;
                    // ファイルの存在を判定する
                    isExists = System.IO.File.Exists(filePath);
                    count++;
                }
                _error.AddLog("  MakeFileName=" + retFileName);
                return retFileName;

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MakeNewFileName");
                return "";
            }
        }

        // 戻り値
        // 0：保存しない、変更されていない
        // 1：保存する
        // 2：Cancel
        // -1：Exception 発生
        // -2：謎のエラー、DialogResult が Yen,No,Cancel 以外の値のとき
        public int ConformSaveWhenFileEdited(bool isEdited,string nowFileName)
        {
            try
            {
                _error.AddLog(this, "ConformSaveWhenFileEdited");
                // 閉じる
                // 変更されていない場合は、Flag 1 を返す
                if (!isEdited) { _error.AddLog(" NotEdited return 1"); return 1; }
                // 変更されていたら、確認する
                string msg = System.IO.Path.GetFileName(nowFileName) + this.ConformEditedMessage;
                DialogResult ret = MessageBox.Show(msg,this.ConformEditedTitle,MessageBoxButtons.YesNoCancel);
                // キャンセルは Flag 2 を返す
                if (ret == DialogResult.Cancel) { _error.AddLog(" return 2 Result.Cancel"); return 2; }
                // 保存しないは Flag 0 を返す
                if (ret == DialogResult.No) { _error.AddLog(" return 0 Result.No = NotSave"); return 0; }
                // 保存する場合
                if (ret == DialogResult.Yes)
                {
                    _error.AddLog(" return Result.Yes = Save");
                    // 保存する場合は 1 を返す
                    return 1;
                    // 以下の処理は別メソッドで実行する
                    // ファイルパスがない場合は、保存ダイアログを表示する
                    // ファイルパスがある場合は、上書きする
                    // ファイルパスがない場合は、保存ダイアログを表示する
                    // 保存ダイアログでキャンセルした場合は、なにもせず Flag 2 を返す
                    // 保存ダイアログで保存した場合は、Flag 1 を返す
                } else { _error.AddLogAlert(" return -2 DialogResult Unexcepted"); return -2; }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".FileClose : return -1");
                return -1;
            }
        }

        // 保存ダイアログを表示する
        public void ShowSaveFileDialog(string data, string newFileName, int writeFlag)
        {
            
            GetPathFromDialog(data, newFileName, SaveDialogTitle, writeFlag,2);
        }

        // CreateNewFile 存在しない場合は新たに作成
        // AppendWhenExistsFile 存在する場合は置き換える
        // open 1,save 2
        public void GetPathFromDialog(string data,string newFileName,string title,int writeFlag,int dialogMode)
        {
            try
            {
                _error.AddLog(this, "GetPathFromDialog");
                //SaveFileDialogクラスのインスタンスを作成
                SaveFileDialog sfd = new SaveFileDialog();

                //はじめに表示されるフォルダを指定する
                if (_saveFileDialogCurrentDirectory != "")
                {
                    sfd.InitialDirectory = _saveFileDialogCurrentDirectory;
                }
                else
                {
                    sfd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
                //はじめに「ファイル名」で表示される文字列を指定する
                //はじめのファイル名を指定する
                // 新しいファイル名が既に存在している場合、存在していないものにする
                sfd.FileName = MakeNewFileName(newFileName, sfd.InitialDirectory);
                if (_error.hasAlert) { _error.AddLogAlert("MakeNewFileName Failed"); return; }
                //[ファイルの種類]に表示される選択肢を指定する
                //指定しない（空の文字列）の時は、現在のディレクトリが表示される
                sfd.Filter = SaveFileDialogFilter;
                //[ファイルの種類]ではじめに選択されるものを指定する
                //2番目の「すべてのファイル」が選択されているようにする
                sfd.FilterIndex = 1;
                //タイトルを設定する
                sfd.Title = title;
                //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                sfd.RestoreDirectory = true;
                //既に存在するファイル名を指定したとき警告する
                //デフォルトでTrueなので指定する必要はない
                sfd.OverwritePrompt = true;
                //存在しないパスが指定されたとき警告を表示する
                //デフォルトでTrueなので指定する必要はない
                sfd.CheckPathExists = false;

                //ダイアログを表示する
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string path = sfd.FileName;
                    if (dialogMode == 2)
                    {
                        _error.AddLog("  dialogMode == 2 saveFile");
                        _error.AddLog("  Selected DialogResult.OK");
                        //OKボタンがクリックされたとき、ファイルを保存する
                        _fileIO.WriteFile(path, false, data);
                        if (_error.hasAlert) { _error.AddLogAlert("_fileIO.WriteFile Failed"); return; }
                        else { _error.AddLogAlert("WriteFile Success. FilePath="+path); }
                        //_filePath = path;
                        SetFilePath(path);
                        LastWriteDirectory = sfd.InitialDirectory;
                    } else if (dialogMode == 1)
                    {
                        _error.AddLog("  dialogMode == 1 openFile");
                        _error.AddLog("  Selected DialogResult.OK");
                        // 開く用に filePath をセットする
                        SetFilePath(path);
                        LastWriteDirectory = sfd.InitialDirectory;
                        this.IsNewFile = false;
                    }
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ShowSaveFileDialog");
            }
        }

        public void SetPathFromDilog(string defaultFileName = "")
        {
            try
            {
                //OpenFileDialogクラスのインスタンスを作成
                OpenFileDialog ofd = new OpenFileDialog
                {

                    //はじめのファイル名を指定する
                    //はじめに「ファイル名」で表示される文字列を指定する
                    FileName = defaultFileName
                };
                //はじめに表示されるフォルダを指定する
                //指定しない（空の文字列）の時は、現在のディレクトリが表示される
                if (_saveFileDialogCurrentDirectory != "")
                {
                    ofd.InitialDirectory = _saveFileDialogCurrentDirectory;
                }
                else
                {
                    ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
                //[ファイルの種類]に表示される選択肢を指定する
                //指定しないとすべてのファイルが表示される
                ofd.Filter = SaveFileDialogFilter;
                //[ファイルの種類]ではじめに選択されるものを指定する
                //2番目の「すべてのファイル」が選択されているようにする
                ofd.FilterIndex = 1;
                //タイトルを設定する
                ofd.Title = "開くファイルを選択してください";
                //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                ofd.RestoreDirectory = true;
                //存在しないファイルの名前が指定されたとき警告を表示する
                //デフォルトでTrueなので指定する必要はない
                ofd.CheckFileExists = true;
                //存在しないパスが指定されたとき警告を表示する
                //デフォルトでTrueなので指定する必要はない
                ofd.CheckPathExists = true;

                //ダイアログを表示する
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //OKボタンがクリックされたとき
                    // 開く用に filePath をセットする
                    SetFilePath(ofd.FileName);
                    _error.AddLog("SetPathFromDilog Open. FilePath=" + ofd.FileName); 
                    LastWriteDirectory = ofd.InitialDirectory;
                    this.IsNewFile = false;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetPathFromDilog");
            }
        }
    }
}
