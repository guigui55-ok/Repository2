using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

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

    public static class CommonGeneralB
    {
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
            foreach (Control con in control.Controls)
            {
                if (con.GetType() == typeObject)
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
        public static List<ToolStripMenuItem> GetMenuItemListIsMatchNameInMenuItem(ToolStripMenuItem menuItem, string objectName)
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
