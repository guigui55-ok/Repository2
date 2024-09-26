using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonModulesProject
{
    public class FileChangerSimple
    {
        List<string> _fileList;
        int _currentIndex = 0;
        public bool _isOutputConsole = true;
        public EventHandler ChangeFileEvent;
        string _searchPattern = "*.*";
        string _basePath;
        string _currentValue = "";
        bool _isFileListUpdating = false;

        public FileChangerSimple()
        {

        }

        public string GetCurrentPath()
        {
            if (_fileList.Count < 1)
            {
                return "";
            }
            if (_fileList.Count <= _currentIndex)
            {
                return "MaxError";
            }
            return _fileList[_currentIndex];
        }

        private void OutputConsole(string value)
        {
            if (_isOutputConsole)
            {
                Console.WriteLine(value);
            }
        }

        public void SetFileListByPath(string path, string searchPattern="*.*")
        {
            OutputConsole("SetFileListByPath");
            _fileList = CommonModules.CommonGeneral.GetPathList(path, searchPattern);
            OutputConsole(string.Format("GetFileList List.Count={0}  , path={1}", _fileList.Count, path));
            _searchPattern = searchPattern;
            _basePath = path;
        }

        public void UpdateFileList()
        {
            OutputConsole("UpdateFileList");
            _isFileListUpdating = true;
            string beforeCurrentPath = GetCurrentPath();
            //OutputConsole(string.Format("* beforeCurrentPath = {0}", beforeCurrentPath));
            //string nextPath = _fileList
            _fileList = CommonModules.CommonGeneral.GetPathList(_basePath, _searchPattern);
            OutputConsole(string.Format("GetFileList List.Count={0}  , path={1}", _fileList.Count, _basePath));

            // ファイルが削除されたときは、更新されて、イベントも実行される
            // 以前とファイルが同じときは、更新されず、イベントも実行されない
            _isFileListUpdating = false;
            if (_currentIndex < 0)
            {
                OutputConsole("_currentIndex < 0");
                MoveIndex(0);
                return;
            }
            if (_fileList.Count <= _currentIndex)
            {
                OutputConsole("_fileList.Count >= _currentIndex");
                if (_fileList.Count <= 0)
                {
                    MoveIndex(0);
                    return;
                }
                int buf = _fileList.Count - 1;
                MoveIndex(buf);
                return;
            }
            MoveIndex(_currentIndex);
            string nowCurrentPath = GetCurrentPath();
            // 以前と同じときは1つ進める
            if (beforeCurrentPath == nowCurrentPath)
            {
                OutputConsole("UpdateFileList > Same Before");
                NextIndex();
            }
        }

        public void CopyFilesInDirectory(string srcDirPath, string distDirPath,string searchPattern= "*.*")
        {
            OutputConsole("CopyFilesInDirectory");
            List<string> srcList = CommonModules.CommonGeneral.GetPathList(srcDirPath, searchPattern);
            foreach(string srcPath in srcList)
            {
                string distPath = Path.Combine(distDirPath, Path.GetFileName(srcPath));
                File.Copy(srcPath, distPath, true);
                OutputConsole(string.Format("COPIED src={0}, dist={1}", srcPath, distDirPath));
            }
        }

        public void MoveIndex(int index, bool updateForce=false)
        {
            //CommonModules.CommonGeneral.LogCallerInfo(2);
            //#
            //ファイルリストがないときに、indexがゼロ以外（1、-1）になる
            if (index < 0)
            {
                OutputConsole("index < 0");
                //MoveIndex(0);
                index = 0;
                return;
            }
            if (_fileList.Count <= index)
            {
                OutputConsole("_fileList.Count >= index");
                if (_fileList.Count <= 0)
                {
                    //MoveIndex(0);
                    index = 0;
                    return;
                }
                int buf = _fileList.Count - 1;
                //MoveIndex(buf);
                index = buf;
                return;
            }
            //#
            try
            {
                if (_currentIndex == index)
                {
                    if (_currentValue != _fileList[_currentIndex])
                    {
                        SetValueByMoveMethod(index, updateForce);
                        return;
                    }
                }
                //#
                if ((0 <= index) && (index < _fileList.Count))
                {
                    SetValueByMoveMethod(index, updateForce);
                } else if (_fileList.Count <= index)
                {
                    SetValueByMoveMethod(_fileList.Count-1, updateForce);
                } else if (index < 0)
                {
                    SetValueByMoveMethod(0, updateForce);
                } else if (_fileList.Count < 1)
                {
                    SetValueByMoveMethod(0, updateForce);
                }
                
            } catch(Exception ex)
            {
                OutputConsole("MoveIndex Error");
                OutputConsole(ex.Message);
                if (_currentIndex <= 0)
                {
                    //ArgumentOutOfRangeException
                    _currentIndex = 0;
                    _currentValue = "";
                    OutputConsole(string.Format("MoveIndex[{0}] = {1}", index, _currentValue));
                    ChangeFileEvent(this, EventArgs.Empty);
                }
            }
        }

        private void SetValueByMoveMethod(int index, bool updateForce=false)
        {
            string beforeValue = GetCurrentPath();
            if (0 < _fileList.Count)
            {
                _currentIndex = index;
                _currentValue = _fileList[index];
                OutputConsole(string.Format("MoveIndex[{0}] = {1}", index, _currentValue));
            }
            else
            {
                _currentIndex = 0;
                _currentValue = "";
                OutputConsole(string.Format("*MoveIndex[{0}] = {1}", index, _currentValue));
            }
            if (_isFileListUpdating)
            {

            }
            else
            {
                if (beforeValue != GetCurrentPath())
                {
                    ChangeFileEvent(this, EventArgs.Empty);
                }
                else
                {
                    if (updateForce)
                    {
                        //初回起動時とかは上記の分岐に入らない
                        ChangeFileEvent(this, EventArgs.Empty);
                    }
                }
            }
        }

        public void NextIndex()
        {
            //_currentIndex++;
            MoveIndex(_currentIndex + 1);
        }
        public void PrevIndex()
        {
            //_currentIndex--;
            MoveIndex(_currentIndex -1);
        }

        /// <summary>
        /// 他のコントロールのKeyDownから呼び出す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FileChangeSimple_KeyDown(object sender, KeyEventArgs e)
        {
            //右　Next
            if (e.KeyCode == Keys.Right)
            {
                NextIndex();
            } else if (e.KeyCode == Keys.Left)
            {
                //左　Prev
                PrevIndex();
            }
        }

    }
}
