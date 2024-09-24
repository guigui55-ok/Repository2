using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace CommonModule
{
    public static class Debugger
    {
        public static void DebugPrint(string value)
        {
            Debug.Print(value);
        }
        public static void DebugPrint(params string[] values)
        {
            foreach (var value in values)
            {
                Debug.Print(value);
            }
        }
    }

    public static class CommonGeneral
    {

        /// <summary>
        /// Dictionary(string,object)をすべてConsoleに出力する
        /// （多階層のDictにも対応）
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="num_str"></param>
        /// <param name="withDataType"></param>
        public static void PrintDict(Dictionary<string, object> dict, string num_str = "", bool withDataType = true)
        {
            int count = 0;
            foreach (string key in dict.Keys)
            {
                if (dict[key].GetType().ToString().IndexOf("Dictionary") > 0)
                {
                    Dictionary<string, object> dictB = (Dictionary<string, object>)dict[key];
                    if (num_str != "") { num_str += "-" + count.ToString(); }
                    PrintDict(dictB, num_str, withDataType);
                }
                else
                {
                    string num_strB;
                    if (num_str != "") { num_strB = num_str + "-" + count.ToString(); }
                    else { num_strB = count.ToString(); }
                    string dataTypeStr = "";
                    if (withDataType)
                    {
                        dataTypeStr = dict[key].GetType().ToString();
                        dataTypeStr = string.Format(" {{{0}}} ", dataTypeStr);
                    }
                    Console.WriteLine(String.Format("i={0},{3}{1} : {2}", num_strB, key, dict[key], dataTypeStr));
                }
                count++;
            }
        }



        /// <summary>
        /// 親ディレクトリを取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetParentDirectory(string filePath, int count = 0)
        {
            // DirectoryInfoを使用する場合
            DirectoryInfo dirInfo = new DirectoryInfo(filePath);

            // 1回目の親ディレクトリ取得
            DirectoryInfo parentDirInfo = dirInfo?.Parent;
            //Console.WriteLine("Parent Directory (DirectoryInfo): " + parentDirInfo.FullName);
            if (count <= 0)
            {
                return parentDirInfo.FullName;
            }
            else
            {
                // 2回目以降の親ディレクトリ取得
                for (int i = 0; i < count; i++)
                {
                    parentDirInfo = parentDirInfo?.Parent;
                    //Console.WriteLine("Grandparent Directory (DirectoryInfo): " + parentDirInfo?.FullName);
                }
                return parentDirInfo.FullName;
            }
        }



        /// <summary>
        /// コントロールリストを任意の型に変換する
        /// </summary>
        /// <param name=""></param>
        /// <param name="convertType"></param>
        /// <returns></returns>
        public static List<object> ConvertControlList(List<Control>controlList, Type convertType)
        {
            List<object> retList = new List<object> { };
            foreach(object con in controlList)
            {
                retList.Add(Convert.ChangeType(con, convertType));
            }
            return retList;
        }

        /// <summary>
        /// タイプとマッチした子コントロールをすべて取得する
        /// （子の子以下のコントロール以下すべてが対象）
        /// </summary>
        /// <param name="control"></param>
        /// <param name="typeObject"></param>
        /// <returns></returns>
        public static List<Control> GetControlListIsMatchType(Control control, Type typeObject)
        {
            List<Control> retList = new List<Control> { };
            foreach(Control con in control.Controls)
            {
                if(con.GetType() == typeObject)
                {
                    retList.Add(con);
                    List<Control> bufList = GetControlListIsMatchType(con, typeObject);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }

        /// <summary>
        /// 名前と合致するコントロールをすべて取得する
        /// （子の子以下のコントロール以下すべてが対象）
        /// </summary>
        /// <param name="control"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static List<Control> GetControlListIsMatchName(Control control, string objectName)
        {
            List<Control> retList = new List<Control> { };
            foreach (Control con in control.Controls)
            {
                Console.WriteLine(string.Format("con.Name = {0}", con.Name));
                Console.WriteLine(string.Format("con.GetType.ToString = {0}", con.GetType().ToString()));
                if (con.Name == objectName)
                {
                    retList.Add(con);
                    List<Control> bufList = GetControlListIsMatchName(con, objectName);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }


        /// <summary>
        /// MenuStrip.Items の名前に合致したものすべてをリストで取得
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static List<ToolStripMenuItem> GetMenuItemListIsMatchNameInMenuStrip(ContextMenuStrip menuStrip, string objectName)
        {
            // MenuStrip, ToolStripMenuItemを両方扱いたかったが、Itemsプロパティがないため別関数で扱う
            //if (menuStrip.GetType() == typeof(MenuStrip))
            //{
            //    menuStrip = (MenuStrip)menuStrip;
            //} else if( menuStrip.GetType() == typeof(ToolStripMenuItem))
            //{
            //    menuStrip = (ToolStripMenuItem)menuStrip;
            //    ToolStripMenuItem item = (ToolStripMenuItem)menuStrip;
            //    item.items
            //}
            //else
            //{
            //    throw new Exception("TypeError " + menuStrip.GetType().ToString());
            //}
            List<ToolStripMenuItem> retList = new List<ToolStripMenuItem> { };
            foreach (ToolStripMenuItem con in menuStrip.Items)
            {
                if (con.Name == objectName)
                {
                    retList.Add(con);
                    List<ToolStripMenuItem> bufList = GetMenuItemListIsMatchNameInMenuItem(con, objectName);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }
        /// <summary>
        /// ToolStripMenuItemのDropDownItems の名前に合致したものすべてをリストで取得
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static List<ToolStripMenuItem> GetMenuItemListIsMatchNameInMenuItem(ToolStripMenuItem　menuItem, string objectName)
        {
            List<ToolStripMenuItem> retList = new List<ToolStripMenuItem> { };
            foreach (ToolStripMenuItem con in menuItem.DropDownItems)
            {
                if (con.Name == objectName)
                {
                    retList.Add(con);
                    List<ToolStripMenuItem> bufList = GetMenuItemListIsMatchNameInMenuItem(con, objectName);
                    //retList = (List<Control>)retList.Concat(bufList);
                    retList.AddRange(bufList);
                }
            }
            return retList;
        }


        public static bool AnyToBool(object value)
        {
            if (value.GetType() == typeof(string))
            {
                string _valueStr = (string)value;
                // trueをメインに判定する
                // 基本的にtrueでなければfalseとする
                if (_valueStr == "1") { return true; }
                if (_valueStr.ToLower() == "true") { return true; }
                // 空文字はfalse
                if (_valueStr == "") { return false; }
                return false;
            }
            if ((value.GetType() == typeof(int)))
            {
                int _valueInt = (int)value;
                if (_valueInt >= 1) { return true; }
                return false;
            }
            if (value.GetType() == typeof(bool))
            {
                return (bool)value;
            }
            else
            {
                return (bool)value;
            }
        }
    }
}
