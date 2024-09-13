using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace CommonUtility.FileListUtility
{
    public class DirectoryGetter
    {
        public AppLogger _logger;
        public DirectoryGetter(AppLogger logger)
        {
            _logger = logger;
        }

        public string GetDirectoryFormPath(string path)
        {
            try
            {
                _logger.AddLog(this, "GetDirectoryFormPath");
                if (System.IO.File.Exists(path))
                {
                    // path がファイルの時は、DirectoryPath を取得する
                    int pos = path.LastIndexOf("\\");
                    path = path.Substring(0, pos);
                }
                if (!System.IO.Directory.Exists(path))
                {
                    _logger.AddLogAlert("  Directory Not Exists path=" + path);
                    return "";
                }
                return path;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetDirectoryFormPath");
                return "";
            }
        }

        public bool IsAbleAccess(string path)
        {
            try
            {
                string[] ary = Directory.GetFiles(path);
                return true;
            } catch (Exception)
            {
                return false;
            }
        }

        public string GetPreviousDirectory(string path)
        {
            try
            {
                _logger.AddLog(this, "GetPreviousDirectory");
                path = GetDirectoryFormPath(path);
                if (path == "") { _logger.AddLog("  path is invalid. path=" + path); return ""; }
                // ルートなら最後のフォルダを取得する
                if (IsRoot(path))
                {
                    _logger.AddLog("  path is root. path="+path);
                    string ret = GetDeepestAndLastDirectory(path);
                    return ret;
                }

                // 親ディレクトリを取得して、親ディレクトリでのディレクトリ一覧を取得、
                // そこから現在の名前と合致した一つ前を返す
                DirectoryInfo info = System.IO.Directory.GetParent(path);
                string[] dirs = System.IO.Directory.GetDirectories(info.FullName);
                if (dirs.Length < 1) { _logger.AddLog(" dirs.Length < 1"); return ""; }
                string dir;
                for (int i = dirs.Length-1; i >= 0; i--)
                {
                    dir = dirs[i];
                    if (dir.Equals(path))
                    {
                        // 合致するものがあるときはindexで一つ前の Directory をかえす
                        if (i <= 0)
                        {
                            // 前の Directory がない場合
                            // 親ディレクトリ Directory(2) をそのまま返す
                            /*
                             * L Directoy1
                             * l   L Directory2 今ここ 変数info
                             * l     L Directoy4 ここから呼び出し 引数path
                             * l     L Directoy5
                             * l   L Directoty3 
                             * L Directory6
                             */
                            string ret = dir;
                            ret = System.IO.Directory.GetParent(ret).FullName;
                            return ret;

                        }
                        else
                        {
                            // 前の Directory がある場合
                            if (!IsFirstDirectory(dir))
                            {
                                // 前の Direcotry の最後を取得する
                                // 前のディレクトリの最下層かつ最後のディレクトリを取得する
                                string ret = GetDeepestAndLastDirectory(dirs[i - 1]);
                                return ret;
                            } else
                            {
                                string ret = System.IO.Directory.GetParent(dir).FullName;
                                int count = 0;
                                while (IsFirstDirectory(ret))
                                {
                                    ret = System.IO.Directory.GetParent(ret).FullName;
                                    count++;
                                    if (count >= 100)
                                    {
                                        break;
                                    }
                                }
                                return ret;
                            }
                        }
                    }
                }
                _logger.AddLogAlert(this, "GetPreviousDirectory : Unexpected Error");
                return "";
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetPreviousDirectory");
                return "";
            }
        }

        public bool IsLastDirectory(string path)
        {
            try
            {
                string parentdir = System.IO.Directory.GetParent(path).FullName;
                string[] dirs = System.IO.Directory.GetDirectories(parentdir);
                if (dirs.Length < 1) { return true; }
                if (path.Equals(dirs[dirs.Length-1]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "IsLastDirectory");
                return false;
            }
        }
        public bool IsFirstDirectory(string path)
        {
            try
            {
                string parentdir = System.IO.Directory.GetParent(path).FullName;
                string[] dirs = System.IO.Directory.GetDirectories(parentdir);
                if(dirs.Length < 1) { return true; }
                if (path.Equals(dirs[0]))
                {
                    return true;
                } else
                {
                    return false;
                }

            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "IsFirstDirectory");
                return false;
            }
        }

        //public bool DirectoryHasChildDirectory(string path)
        //{
        //    try
        //    {

        //    } catch (Exception ex)
        //    {

        //    }
        //}

        // 最下層かつ最後の Directory を取得する
        public string GetDeepestAndLastDirectory(string path)
        {
            try
            {
                _logger.AddLog(this, "GetDeepestAndLastDirectory");
                string before = path;
                string after = GetLastDirectory(before);
                if(after == "") { _logger.AddLog("  Child Directory Nothing");  return path; }
                while (!(after == ""))
                {
                    before = after;
                    after = GetLastDirectory(before);
                }
                return before;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetDeepestAndLastDirectory");
                return "";
            }
        }

        //public string GetParentDirectory(string path)
        //{

        //}

        // 対象の path の最後の Directory を取得する
        public string GetLastDirectory(string path)
        {
            try
            {
                _logger.AddLog(this, "GetLastDirectory");
                path = GetDirectoryFormPath(path);
                if (path == "") { _logger.AddLog("  path is invalid. path=" + path); return ""; }
                // ディレクトリ一覧を取得する
                string[] dirs = System.IO.Directory.GetDirectories(path);
                if (dirs.Length < 1) { _logger.AddLog(" dirs.Length < 1[Child Directory Nothing]"); return ""; }
                return dirs[dirs.Length - 1];
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetLastDirectory");
                return "";
            }
        }

        //public string GetDeepestDirectory(string path)
        //{

        //}

        // 一つ下の階層の最後の Directory を取得する
        //public string GetLastDirectoryOneLevelBelow(string path)
        //{
        //    try
        //    {
        //        _logger.AddLog(this, "GetLastDirectoryOneLevelBelow");
        //        string dir = GetLastDirectory(path);
        //        if(dir == "") { _logger.AddLog(" dir == \"\""); return path; }
        //        // 再帰的に呼び出す
        //        dir = GetLastDirectory(dir);
        //        return dir;
        //    } catch (Exception ex)
        //    {
        //        _logger.AddException(ex, this, "GetLastDirectoryOneLevelBelow");
        //        return "";
        //    }
        //}
        public string GetNextDirectory(string path)
        {
            try {
                _logger.AddLog(this, "GetNextDirectory");
                string newpath = GetDirectoryFormPath(path);
                if(newpath == "") { _logger.AddLog("  path is invalid. path="+path); return""; }

                string ret = NextDirectoryWhenNotAbleAccess(newpath);
                //if (IsAbleAccess(newpath))
                //{
                //    // newpath がアクセス可能おなら、一覧を取得する
                //    string[] dirs = System.IO.Directory.GetDirectories(newpath);
                //} else
                //{
                //    // newpath がアクセス不可能なら、次のディレクトリを取得する
                //    newpath = NextDirectoryWhenNotAbleAccess(newpath);
                //}

                //string ret = "";
                //if (dirs.Length > 0)
                //{
                //    // 子ディレクトリがある場合
                //    ret = dirs[0];
                //} else
                //{
                //    // 子ディレクトリがない場合
                //    ret = GetNextDirecotryNotHasChild(path);
                //}
                //while (!IsAbleAccess(ret))
                //{
                //    //ret = GetNextDirectory(ret);
                //}
                return ret;

            } catch (Exception ex)
            {
                _logger.AddException(ex,this, "GetNextDirectory");
                return "";
            }
        }

        public bool DirectorhHasChildDirectory(string path)
        {
            try
            {                
                if (!IsAbleAccess(path)) { _logger.AddLogAlert(this, "DirectorhHasChildDirectory IsAbleAccess=false, path="+path); return false; }

                string[] dirs = System.IO.Directory.GetDirectories(path);
                if(dirs == null) { return false; }
                if(dirs.Length < 1) { return false; }
                return true;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "DirectorhHasChildDirectory");
                return false;
            }
        }

        /// <summary>
        /// path 親ディレクトリが構成しているディレクトリ一覧から path の次を取得する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isTargetIncludeChild"></param>
        /// <returns></returns>
        public string NextDirectoryWhenNotAbleAccess(string path,bool isTargetIncludeChild = true)
        {
            try
            {
                string[] dirs;
                string targetPath = "";
                _logger.AddLog(this, "NextDirectoryWhenNotAbleAccess");
                string parentdir = path;
                bool isAbleAccess = true;

                // ルートまで来たら、一番最後だったので、最初(Root)に戻る
                if (IsRoot(path)) { _logger.AddLog(" return directory to root."); return path; }

                // アクセス不能な場合は次のディレクトリを対象とする
                if (!IsAbleAccess(path))
                {
                    isAbleAccess = false;
                    _logger.AddLogAlert("  IsAbleAccess , path="+path);
                    // path ディレクトリがアクセス不可能なら、さらに親ディレクトリを取得する
                    //targetPath = System.IO.Directory.GetParent(path).FullName;
                    //targetPath = NextDirectoryWhenNotAbleAccess(targetPath);
                    targetPath = path;
                } else
                {
                    isAbleAccess = true;
                    // path がアクセス可能
                    targetPath = path;
                    // 子ディレクトリが取得対象の場合は実行する
                    // LastDirectory=true で再帰的に実行したとき、子ディレクトリを対象とすると、永遠ループとなるため、フラグで管理する
                    if (isTargetIncludeChild)
                    {
                        // 子ディレクトリを持っている
                        if (DirectorhHasChildDirectory(targetPath))
                        {
                            // path の親ディレクトリの一覧を取得する
                            dirs = System.IO.Directory.GetDirectories(targetPath);
                            // path がアクセス可能で、
                            // path が子ディレクトリを持っている場合は、子ディレクトリの最初を取得する
                            return dirs[0];
                        }
                    }
                }

                // path がアクセス可能で、子ディレクトリを持っていない状態
                if (IsLastDirectory(targetPath))
                {
                    _logger.AddLog("  IsLastDirectory=true , path=" + targetPath);
                    // 対象ディレクトリが最後なら、さらに親ディレクトリを取得する (子ディレクトリを対象としない)
                    targetPath = System.IO.Directory.GetParent(targetPath).FullName;
                    targetPath = NextDirectoryWhenNotAbleAccess(targetPath,false);
                    return targetPath;
                }

                if (isAbleAccess)
                {
                    // targetPath が最後のディレクトリではなく、次のディレクトリがある、
                    // かつ、子ディレクトリを持たない
                    // かつ、アクセス可能な場所を取得した状態
                    // → 親ディレクトリの一覧から targetPath の次のディレクトリを取得する
                    // TargetDirectory の親ディレクトリを取得する
                    targetPath = System.IO.Directory.GetParent(targetPath).FullName;
                    dirs = System.IO.Directory.GetDirectories(targetPath);
                } else
                {
                    // targetPath がアクセス不可能な時は、
                    // TargetDirectory の親ディレクトリを取得する
                    targetPath = System.IO.Directory.GetParent(targetPath).FullName;
                    dirs = System.IO.Directory.GetDirectories(targetPath);
                }

                if (dirs.Length < 1)
                {
                    _logger.AddLog(" dirs.Length < 1 -> Child Directory Is Nothing : Unexpected State");
                    return "";
                    // path が存在しているので Length<1 になることはない
                    // が、ほかのプロセスなどの Directory 操作のタイミングによってはありうるかもしれない
                }

                string ret = "";
                // 対象のディレクトリ一覧から現在のディレクトリを検索
                string dir;
                for (int i = 0; i < dirs.Length; i++)
                {
                    dir = dirs[i];
                    if (dir.Equals(path))
                    {
                        if (!IsLastDirectory(path))
                        {
                            // 次のディレクトリを取得する
                            ret = dirs[i + 1];

                            // このディレクトリがアクセス不能なら、次へ次へとチェックし
                            // すべてアクセス不能なら、さらに親ディレクトリを探す
                            if (!IsAbleAccess(ret))
                            {
                                // アクセス不能なので、子ディレクトリを含む、次のディレクトリを取得する
                                ret = NextDirectoryWhenNotAbleAccess(ret, true);
                            }
                            break;
                        } else
                        {
                            // 親ディレクトリが最後なら、さらに親ディレクトリを取得する
                            ret = NextDirectoryWhenNotAbleAccess(targetPath);
                            _logger.AddLogAlert(this, "NextDirectoryWhenNotAbleAccess Unexpected State");
                            // このメソッドの前方で処理しているのでここには来ないはず
                            break;
                        }
                    }
                }
                // ret=="" の状態はないはず
                if (ret == "") { _logger.AddLogAlert(this, "Get Directory is Nothing :  Unexpected State"); }
                return ret;

            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "NextDirectoryWhenNotAbleAccess");
                return "";
            }
        }

        // 子ディレクトリがない場合
        private string GetNextDirecotryNotHasChild(string path)
        {
            try
            {
                _logger.AddLog(this, "GetNextDirecotryNotChild");
                if (IsRoot(path)) { _logger.AddLog("  IsRoot=true -> Directory Is Root Only");  return path; } // Root しかない状態
                DirectoryInfo info = System.IO.Directory.GetParent(path);
                string[] dirs = System.IO.Directory.GetDirectories(info.FullName);
                if(dirs.Length < 1) { _logger.AddLog(" dirs.Length < 1"); return""; }
                string dir;
                for (int i=0; i<dirs.Length; i++)
                {
                    dir = dirs[i];
                    if (dir.Equals(path))
                    {
                        // 合致するものがあるときは次の Directory をかえす
                        if ( i >= (dirs.Length - 1))
                        {
                            // 次の Directory がない場合
                            // 最後の Directory(3) の時は親 Directory(5) とする
                            /*
                             * L Directoy1
                             * l   L Directory2
                             * l   L Directoty3 今ここ 変数info
                             * l     L Directoy4 ここから呼び出し 引数path
                             * L Directory5
                             */
                            string ret = GetNextDirecotryNotHasChild(info.FullName);
                            return ret;
                        } else
                        {
                            // 次の Directory がある場合
                            return dirs[i + 1];
                        }
                    }
                }
                _logger.AddLogAlert(this, "GetNextDirecotryNotHasChild : Unexpected Error");
                return "";
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetNextDirecotryNotChild");
                return "";
            }
        }

        public bool IsRoot(string path)
        {
            try
            {
                if (path.Length < 3)
                {
                    _logger.AddLogWarning(this, "IsRoot : path.Length < 3"); return false;
                }
                string root = path.Substring(0, 3);
                if (path.Equals(System.IO.Path.GetPathRoot(path)))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "IsRoot");
                return false;
            }
        }

    }
}
