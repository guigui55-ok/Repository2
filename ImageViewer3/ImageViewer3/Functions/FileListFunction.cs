using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer.Functions
{
    public class FileListFunction
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected FileList.FileListReader _fileList;
        protected FileList.FileListRegister _fileListRegister;
        public FileListFunction(ErrorLog.IErrorLog errorLog,FileList.FileListReader listReader)
        {
            _errorLog = errorLog;
            _fileList = listReader;
            _fileListRegister = new FileList.FileListRegister(_errorLog);
        }

        public void RegistFileListByDragDrop(object sender, DragEventArgs e)
        {
            //Debug.WriteLine("Control_DragDrop");
            try
            {
                // DragDrop の e を配列へ
                string[] files = GetFilesByDragAndDrop(e);
                // 配列→Listへ
                List<string> list = new List<string>(files);


                // リストからFileListへ登録
                int ret = this._fileListRegister.SetFileList(list);
                if (ret < 1)
                {
                    MessageBox.Show("Control_DragDrop.setFileList Failed");
                }
                else
                {
                    Debug.WriteLine("Count = " + _fileListRegister.GetListCount());
                    _fileList.FileList = _fileListRegister.GetList();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _errorLog.AddException(ex, this.ToString(), "RegistFileListByDragDrop");
            }
        }
        public string[] GetFilesByDragAndDrop(DragEventArgs e)
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
                    _errorLog.AddErrorNotException(this.ToString(), "GetFilesByDragAndDrop GetDataPresent Else");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ClickPointIsRightSideOnControl");
                return null;
            }
        }
    }
}
