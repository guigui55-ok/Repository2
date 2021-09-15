using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelWorkbookList
{
    public class WorkbookListCheckedListBox : IWorkbookListControl
    {
        protected ErrorManager.ErrorManager _err;
        protected CheckedListBox CheckedListBox;
        public EventHandler DoubleClickedEvent;
        public Control Control { get => (Control)CheckedListBox; }
        public WorkbookListCheckedListBox (ErrorManager.ErrorManager err,CheckedListBox checkedListBox)
        {
            _err = err;
            this.CheckedListBox = checkedListBox;
        }

        public List<string> GetValueList()
        {
            List<string> retList = new List<string>();
            try
            {
                _err.AddLog(this, "GetValueList");
                if (this.CheckedListBox == null) { _err.AddLogWarning(" GetIndexListSelecteditem"); return retList; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return retList; }

                for (int i = 0; i < CheckedListBox.Items.Count; i++)
                {
                    retList.Add(CheckedListBox.Items[i].ToString());
                }
                return retList;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetValueList");
                return retList;
            }
        }

        public void ClearItems()
        {
            try
            {
                _err.AddLog(this, "ClearItems");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return; }

                while(CheckedListBox.Items.Count > 0)
                {
                    CheckedListBox.Items.RemoveAt(0);
                }

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "ClearItems");
            }
        }
        public void SetWorkbookList(List<string> list)
        {
            try
            {
                _err.AddLog(this, "SetWorkbookList");
                ClearItems();
                if (_err.hasAlert) { return; }
                if (list == null) { _err.AddLogWarning(" list == null"); return; }
                if(list.Count < 1) { _err.AddLogWarning(" list.Count < 1"); return; }
                this.CheckedListBox.Items.AddRange(list.ToArray());
            } catch(Exception ex)
            {
                _err.AddException(ex,this, "SetWorkbookList");
            }
        }

        public string GetItemValue(int index)
        {
            try
            {
                _err.AddLog(this, "GetIndexListSelecteditem");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return ""; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return ""; }

                return CheckedListBox.Items[index].ToString();

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetItemValue");
                return "";
            }
        }

        public ExcelUtility.AppsInfo ConvertToAppsInfoFromItemValue(string value)
        {
            try
            {
                _err.AddLog(this, "ConvertToAppsInfoFromItemValue");
                ExcelUtility.AppsInfo info = new ExcelUtility.AppsInfo();
                int pos = value.IndexOf(']');
                string pidstr = value.Substring(1, pos-1-1);
                int pid;
                if(!int.TryParse(pidstr,out pid))
                {
                    pid = 0;
                }
                pos = pos + 2;
                string bookname = value.Substring(pos,value.Length - pos);
                info.FileName = bookname;
                info.ProcessId = pid;

                return info;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetIConvertToAppsInfoFromItemValuetemValue");
                return null;
            }
        }

        public string[] GetSelectedItems()
        {
            string[] retAry = new string[] { };
            try
            {
                _err.AddLog(this, "GetSelectedItems");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return retAry; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return retAry; }

                for (int i = 0; i < CheckedListBox.Items.Count; i++)
                {
                    if (CheckedListBox.GetItemChecked(i))
                    {
                        Array.Resize(ref retAry, retAry.Length + 1);
                        retAry[retAry.Length-1] = CheckedListBox.Items[i].ToString();
                    }
                }
                return retAry;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetSelectedItems");
                return retAry;
            }
        }

        public List<int> GetIndexListSelectedItem()
        {
            List<int> retList = new List<int>();
            try
            {
                _err.AddLog(this, "GetIndexListSelecteditem");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return retList; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return retList; }

                for (int i = 0; i < CheckedListBox.Items.Count; i++)
                {
                    if (CheckedListBox.GetItemChecked(i))
                    {
                        retList.Add(i);
                    }
                }
                return retList;
            } catch(Exception ex)
            {
                _err.AddException(ex, this, "GetIndexListSelecteditem");
                return retList;
            }
        }


    }
}
