using CloseHandleUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handles
{
    public class ExcelFileType
    {
        public readonly string[] Types =
            { ".xlsx",".xlsm",".xlsb",".xltx",".xltm",".xls",".xlt","xml",".xlam",".xla",".xlw",".xlr" };
    }
    public class Program
    {
        static void Main()
        {
            try
            {
                List<string> pathList = Test2(23664);
                pathList = RemoveSameValue(pathList);
                pathList = RemoveElementIncludeValue(pathList, "~$");
                if (pathList.Count > 0)
                {
                    foreach (string path in pathList)
                    {
                        Console.WriteLine(path);
                    }
                }
                //test1();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("any pless key.");
                Console.ReadKey();
            }
        }
        static public List<string> RemoveElementIncludeValue(List<string> list,string value)
        {
            List<string> retList = new List<string>();
            try
            {
                if (value == "") { return list; }
                if (list == null) { throw new Exception("List Is Null"); }
                if (list.Count < 1) { throw new Exception("List.Count Is Zero"); }
                foreach (string buf in list) { retList.Add(buf); }

                int max = list.Count;
                for (int i = 0; i < max; i++)
                {
                    if (retList[i].IndexOf(value)>1)
                    {
                        // include
                        retList.RemoveAt(i);
                        max = retList.Count;
                        if (i >= max)
                        {
                            break;
                        }
                        else
                        {
                            i--;
                        }
                    }
                }
            
                return retList;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return retList;
            }
        }
        static public List<string> RemoveSameValue(List<string> list)
        {
            List<string> retList = new List<string>();
            try
            {
                if (list == null) { throw new Exception("List Is Null"); }
                if (list.Count < 1) { throw new Exception("List.Count Is Zero"); }
                foreach(string value in list) { retList.Add(value); }

                foreach(string value in list)
                {
                    int max = retList.Count;
                    bool flag = false;
                    for (int i=0; i<max; i++)
                    {
                        if (value == retList[i])
                        {
                            if (flag)
                            {
                                // 2度目
                                retList.RemoveAt(i);
                                max = retList.Count;
                                if (i >= max)
                                {
                                    break;
                                } else
                                {
                                    i--;
                                }
                            } else
                            {
                                flag = true;
                            }
                        }
                    }
                }
                return retList;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return retList;
            }
        }

        static public List<string> Test2(int pid)
        {
            List<string> pathList = new List<string>();
            try
            {
                ExcelFileType typesForCheck = new ExcelFileType();


                foreach (CloseHandleUtil.HandleInfo hi in CloseHandleUtil.Handles.EnumProcessHandles(pid))
                {
                    foreach(string type in typesForCheck.Types)
                    {
                        if (hi.Name.Contains(type))
                        {
                            pathList.Add(hi.Name);
                        }
                    }
                }
                return pathList;
            } catch (Exception ex)
            {
                Console.WriteLine("test2 failed");
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
                return pathList;
            }
        }

        static public void Test1()
        {
            try
            {

                //HandleInfo handleInfo = new HandleInfo();


                int pid = 12336;
                foreach (CloseHandleUtil.HandleInfo hi in CloseHandleUtil.Handles.EnumProcessHandles(pid))
                {
                    string[] itemStr = new string[9];
                    itemStr[0] = "0x" + hi.Handle.ToString("X");
                    itemStr[1] = hi.Type;
                    itemStr[2] = hi.Name;
                    itemStr[3] = "0x" + hi.GrantedAccess.ToString("X");
                    itemStr[4] = "0x" + hi.HandleAttributes.ToString("X");
                    itemStr[5] = "0x" + hi.CreatorBackTraceIndex.ToString("X");
                    itemStr[6] = "0x" + hi.Object.ToString("X");
                    itemStr[7] = "0x" + hi.ObjectTypeIndex.ToString("X");
                    itemStr[8] = "0x" + hi.Reserved.ToString("X");

                    Console.WriteLine("-------");
                    foreach (string buf in itemStr)
                    {
                        Console.WriteLine(buf);
                    }

                    //ListViewItem i = new ListViewItem(itemStr);
                    //listViewProcessHandle.Items.Add(i);
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
