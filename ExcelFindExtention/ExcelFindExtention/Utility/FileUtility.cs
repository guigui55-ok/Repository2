using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class FileUtility
    {
        protected ErrorManager.ErrorManager _error;
        public FileUtility(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public string MakeNewFileNameNotLock(string fileName,string directoryPath)
        {
            try
            {
                _error.AddLog(this, "MakeNewFileNameNotLock : path=" + directoryPath + "\\" + fileName);
                if (!System.IO.Directory.Exists(directoryPath)) { throw new Exception("Directry Path Is Not Exists"); }


                string retFileName = fileName;
                string filePath = directoryPath + "\\" + retFileName;
                bool isExists = System.IO.File.Exists(filePath);
                bool isExistsAndLock = isExists && IsLock(filePath);
                int count = 2;
                // 既にファイルが存在する And ロックしている
                while (isExistsAndLock)
                {
                    // ファイルがロックしている場合、次のファイル名をセットする
                    retFileName = System.IO.Path.GetFileName(fileName) + " (" + count + ")"
                        + System.IO.Path.GetExtension(filePath);
                    filePath = directoryPath + "\\" + retFileName;
                    // ファイルの存在を判定する
                    isExists = System.IO.File.Exists(filePath);
                    if (isExists) { isExistsAndLock = IsLock(filePath); }
                    else { isExistsAndLock = false; }
                    count++;
                }
                _error.AddLog("  MakeNewFileNameNotLock=" + retFileName);
                return retFileName;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MakeNewFileNameNotLock");
                return "";
            }
        }

        // fileName が directoryPath に存在する場合は fileName を返す
        public string MakeNewFileName(string fileName, string directoryPath)
        {
            try
            {
                _error.AddLog(this, "MakeNewFileName : path=" + directoryPath + "\\" + fileName);
                if (!System.IO.Directory.Exists(directoryPath)) { throw new Exception("Directry Path Is Not Exists"); }


                string retFileName = fileName;
                string filePath = directoryPath + "\\" + retFileName;
                bool isExists = System.IO.File.Exists(filePath);
                int count = 2;
                // 既にファイルが存在する
                while (isExists)
                {
                    // ファイルがある場合、次のファイル名をセットする
                    retFileName = System.IO.Path.GetFileName(fileName) + " (" + count + ")"
                        + System.IO.Path.GetExtension(filePath);
                    filePath = directoryPath + "\\" + retFileName;
                    // ファイルの存在を判定する
                    isExists = System.IO.File.Exists(filePath);
                    count++;
                }
                _error.AddLog("  MakeFileName=" + retFileName);
                return retFileName;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".MakeNewFileName");
                return "";
            }
        }

        public bool IsLock(string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return false;
        }
        public string GetFileNameTypeRemoved(string filePath)
        {
            try
            {
                string fileNameWithExt = System.IO.Path.GetFileName(filePath);
                string fileNameOnly = "";
                int piriodPos = fileNameWithExt.LastIndexOf("."); // 最後から
                
                if (piriodPos > 0)
                {
                    // ピリオドがある場合
                    fileNameOnly = fileNameWithExt.Substring(0, piriodPos);
                    return fileNameOnly;
                } else
                {
                    // ピリオドがない場合
                    return fileNameWithExt;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetFileNameTypeRemoved");
                return "";
            }
        }
        public bool IsMatchFileType(string filePath,string[] typeList)
        {
            try
            {
                if(typeList == null) { throw new Exception("typeList Is Null"); }
                if(typeList.Length < 1) { throw new Exception("typList.Length < 1"); }
                foreach(string typeValue in typeList)
                {
                    if(System.IO.Path.GetExtension(filePath) == typeValue)
                    {
                        return true;
                    }
                }
                return false;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".IsMatchFileType");
                return false;
            }
        }
    }
}
