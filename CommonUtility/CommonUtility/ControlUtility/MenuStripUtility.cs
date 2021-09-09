using System;
using System.Windows.Forms;

namespace CommonUtility.ControlUtility
{
    public class MenuStripUtility
    {
        ErrorManager.ErrorManager _err;

        public MenuStripUtility(ErrorManager.ErrorManager err)
        {
            _err = err;
        }


        public void AddMenu(
            ToolStripMenuItem parentMenuItem,
            string text, bool showShortCutKeys, Keys shortcutKeys)
        {
            try
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = text;
                item.ShortcutKeys = shortcutKeys;
                item.ShowShortcutKeys = showShortCutKeys;
                // parentMenuItem に追加する
                parentMenuItem.DropDownItems.Add(item);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "AddMenu");
            }
        }

        /// <summary>
        /// MenuStrip から Text に合致した ToolStripMenuItem を取得する
        /// 引数 textArray によって子コントロールまで判定、取得が可能
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="textArray"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ToolStripMenuItem GetToolStripMenuItemMatchTextFromMenuStripWithMultipleHierarchies(
            MenuStrip menuStrip, string[] textArray)
        {
            ToolStripMenuItem ret = null;
            try
            {
                _err.AddLog(this, "GetToolStripMenuItemMatchTextFromMenuStripWithMultipleHierarchies");
                if (textArray == null) { _err.AddLogWarning(this, "textArray is Null"); return null; }
                if (textArray.Length < 1) { _err.AddLogWarning(this, "textArray.Length<1"); return null; }
                int n = 0;
                ToolStripMenuItem item;
                foreach (object obj in menuStrip.Items)
                {
                    if (obj.GetType().Equals(typeof(ToolStripMenuItem)))
                    {
                        item = (ToolStripMenuItem)obj;
                        if (item.Text.Equals(textArray[n]))
                        {
                            _err.AddLog(" Match:" + textArray[n]);
                            n++;
                            if (n == textArray.Length)
                            {
                                return item;
                            }
                            else if (n < textArray.Length)
                            {
                                ret = GetToolStripMenuItemMatchText(item, textArray, n);
                                if (ret != null) { return ret; }
                            }
                        }
                    }
                }
                if (0 >= n)
                {
                    _err.AddLogWarning(this, "Control is Nothing. text=" + textArray[0] + " , " + textArray[0]);
                }
                return ret;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + ".GetToolStripMenuItemMatchTextFromMenuStripWithMultipleHierarchies");
                return null;
            }
        }

        /// <summary>
        /// toolStripMenuItem.Text と合致したものを取得する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ToolStripMenuItem GetToolStripMenuItemMatchText(
            ToolStripMenuItem toolStripMenuItem, string[] textArray, int index = 0)
        {
            ToolStripMenuItem ret = null;
            try
            {
                _err.AddLog(this, "GetToolStripMenuItemMatchText");
                if (toolStripMenuItem == null) { throw new Exception("control is null"); }

                if (textArray == null) { _err.AddLogWarning(this, "textArray is Null"); return null; }
                if (textArray.Length < 1) { _err.AddLogWarning(this, "textArray.Length<1"); return null; }
                int n = index;
                ToolStripMenuItem item;
                foreach (object obj in toolStripMenuItem.DropDownItems)
                {
                    if (obj.GetType().Equals(typeof(ToolStripMenuItem)))
                    {
                        item = (ToolStripMenuItem)obj;
                        if (item.Text.Equals(textArray[n]))
                        {
                            _err.AddLog(" Match:"+textArray[n]);
                            n++;
                            if (textArray.Length == n)
                            {
                                ret = item;
                                break;
                            } else if(textArray.Length > n)
                            {
                                if (item.HasDropDownItems)
                                {
                                    ret = GetToolStripMenuItemMatchText(item, textArray, n);
                                    break;
                                } else
                                {
                                    string buf = "  ToolStripMenuItem[" + item.Text + "].HasDropDownItems=false";
                                    buf += " , next.Text=" + textArray[n];
                                    _err.AddLogWarning(buf);
                                    break;
                                }
                            } else
                            {
                                // textArray.Length < n
                                _err.AddLogAlert(" Unexpected");
                                return null;
                            }
                        }
                    }
                }
                if(ret == null)
                {
                    _err.AddLogAlert(" Target Item Not Found");
                }
                return ret;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + ".GetToolStripMenuItemMatchText");
                return null;
            }
        }
    }
}
